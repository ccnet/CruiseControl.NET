using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote.Events;

namespace ThoughtWorks.CruiseControl.Core.Extensions
{
    /// <summary>
    /// An extension to prevent integrations from happening if the available disk space is
    /// insufficient.
    /// </summary>
    public class DiskSpaceMonitorExtension
        : ICruiseServerExtension
    {
        #region Private fields
        private Dictionary<string, long> driveSpaces = new Dictionary<string, long>();
        #endregion

        #region Public methods
        #region Initialise()
        /// <summary>
        /// Initialises the extension.
        /// </summary>
        /// <param name="server">The server that this extension is for.</param>
        /// <param name="extensionConfig"></param>
        public virtual void Initialise(ICruiseServer server, ExtensionConfiguration extensionConfig)
        {
            if (server == null)
            {
                throw new ArgumentNullException("server");
            }
            else
            {
                server.IntegrationStarted += (o, e) =>
                {
                    // Check all the drives that they have sufficient space
                    var fileSystem = server.RetrieveService(typeof(IFileSystem)) as IFileSystem ?? new SystemIoFileSystem();
                    bool hasSpace = true;
                    foreach (var drive in driveSpaces.Keys)
                    {
                        var freeSpace = fileSystem.GetFreeDiskSpace(drive);
                        hasSpace &= (freeSpace >= driveSpaces[drive]);
                    }
                    e.Result = hasSpace ? IntegrationStartedEventArgs.EventResult.Continue 
                        : IntegrationStartedEventArgs.EventResult.Cancel;
                    if (!hasSpace)
                    {
                        Log.Warning(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Integration for '{0}' cancelled due to a lack of space.", e.ProjectName));
                    }
                };
            }

            foreach (var element in extensionConfig.Items)
            {
                if (element.Name == "drive")
                {
                    if (element.SelectNodes("*").Count > 0) throw new ArgumentException("Drive definitions cannot contain child elements");
                    AddDriveSpace(element.GetAttribute("name"), element.GetAttribute("unit"), element.InnerText);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Unknown configuration option: " + element.Name);
                }
            }

            if (driveSpaces.Count == 0)
            {
                throw new ArgumentOutOfRangeException("At least one drive must be defined to monitor");
            }
        }
        #endregion

        #region Start()
        /// <summary>
        /// Starts the extension.
        /// </summary>
        public virtual void Start()
        {
        }
        #endregion

        #region Stop()
        /// <summary>
        /// Stops the extension.
        /// </summary>
        public virtual void Stop()
        {
        }
        #endregion

        #region Abort()
        /// <summary>
        /// Terminates the extension immediately.
        /// </summary>
        public virtual void Abort()
        {
        }
        #endregion

        #region RetrieveMinimumSpaceRequired
        /// <summary>
        /// Retrieve the minimum amount of free space required (in bytes) for a drive.
        /// </summary>
        public long RetrieveMinimumSpaceRequired(string drive)
        {
            drive=drive.ToLowerInvariant();
            if (driveSpaces.ContainsKey(drive))
            {
                return driveSpaces[drive];
            }
            else
            {
                return -1;
            }
        }
        #endregion
        #endregion

        #region Private methods
        #region AddDriveSpace()
        /// <summary>
        /// Adds a new drive space.
        /// </summary>
        /// <param name="driveName"></param>
        /// <param name="unit"></param>
        /// <param name="amount"></param>
        private void AddDriveSpace(string driveName, string unit, string amount)
        {
            // Validate the input arguments
            if (string.IsNullOrEmpty(driveName)) throw new ArgumentNullException("name is a required attribute");
            if (string.IsNullOrEmpty(unit)) unit = "Mb";
            if (string.IsNullOrEmpty(amount)) throw new ArgumentNullException("The amount of free space has not been specified");
            double value;
            if (!double.TryParse(amount, out value)) throw new ArgumentException("Amount must be a valid number");

            // Convert to bytes
            unit = unit.ToLowerInvariant();
            switch (unit)
            {
                case "b":
                    break;
                case "kb":
                    value *= 1024;
                    break;
                case "mb":
                    value *= 1048576;
                    break;
                case "gb":
                    value *= 1073741824;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unknown unit: " + unit);
            }
            long numberOfBytes = Convert.ToInt64(value);

            // Add to the dictionary
            driveSpaces.Add(driveName.ToLowerInvariant(), numberOfBytes);
        }
        #endregion
        #endregion
    }
}
