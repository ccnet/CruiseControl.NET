using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using System.Xml;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.Administration
{
    /// <summary>
    /// Manages the packages for CruiseControl.Net.
    /// </summary>
    public class PackageManager
    {
        #region Private constants
        private const int blockSize = 16384;
        #endregion

        #region Private fields
        private readonly IPhysicalApplicationPathProvider physicalApplicationPathProvider;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new <see cref="PackageManager"/>.
        /// </summary>
        /// <param name="physicalApplicationPathProvider"></param>
        public PackageManager(IPhysicalApplicationPathProvider physicalApplicationPathProvider)
		{
			this.physicalApplicationPathProvider = physicalApplicationPathProvider;
        }
        #endregion

        #region Public methods
        #region StorePackage()
        /// <summary>
        /// Stores a package locally on the server
        /// </summary>
        /// <param name="fileName">The file name of the package.</param>
        /// <param name="stream">The stream containing the package.</param>
        public PackageManifest StorePackage(string fileName, Stream stream)
        {
            // Initialise the path and make sure the folder is there
            FileInfo packageDetails = new FileInfo(fileName);
            string packagePath = Path.Combine(physicalApplicationPathProvider.GetFullPathFor("Packages"),
                packageDetails.Name);
            packageDetails = new FileInfo(packagePath);
            if (!packageDetails.Directory.Exists) packageDetails.Directory.Create();

            // We will always overwrite an existing packages, maybe we should do a back-up first?
            if (packageDetails.Exists) packageDetails.Delete();
            SaveToFile(stream, packageDetails.FullName);
            bool delete = false;

            // Load the package and extract the manifest details
            PackageManifest manifest = null;
            using (Stream inputStream = File.OpenRead(packagePath))
            {
                using (Package newPackage = new Package(inputStream))
                {
                    if (!newPackage.IsValid)
                    {
                        delete = true;
                    }
                    else
                    {
                        manifest = newPackage.Manifest;
                        manifest.FileName = packageDetails.Name;
                    }
                }
            }

            if (manifest != null)
            {
                // Update the package list
                XmlDocument packageList = LoadPackageList();
                UpdatePackagesList(manifest, packageList, true);
                SavePackageList(packageList);
            }

            // Don't forget to clean-up, otherwise the packages directory will have invalid packages
            if (delete && packageDetails.Exists) packageDetails.Delete();
            return manifest;
        }
        #endregion

        #region InstallPackage()
        /// <summary>
        /// Installs a package.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="velocityContext"></param>
        public List<PackageImportEventArgs> InstallPackage(string fileName)
        {
            // Validate that the package is still valid, just in case somebody else deleted it
            string packagePath = Path.Combine(physicalApplicationPathProvider.GetFullPathFor("Packages"),
                fileName);
            FileInfo packageDetails = new FileInfo(packagePath);
            List<PackageImportEventArgs> events = null;
            if (packageDetails.Exists)
            {
                // Load the package and install it
                events = new List<PackageImportEventArgs>();
                using (Stream packageStream = File.OpenRead(packagePath))
                {
                    using (Package package = new Package(packageStream))
                    {
                        package.Message += delegate(object source, PackageImportEventArgs args)
                        {
                            events.Add(args);
                        };
                        package.Install(physicalApplicationPathProvider);

                        // Update the list
                        XmlDocument packageList = LoadPackageList();
                        package.Manifest.IsInstalled = true;
                        UpdatePackagesList(package.Manifest, packageList, true);
                        SavePackageList(packageList);
                    }
                }
            }
            return events;
        }
        #endregion

        #region RemovePackage()
        /// <summary>
        /// Removes a package stored locally on the server
        /// </summary>
        /// <param name="fileName">The file name of the package.</param>
        /// <returns>The name of the package.</returns>
        public string RemovePackage(string fileName)
        {
            // Initialise the path and make sure the folder is there
            FileInfo packageDetails = new FileInfo(fileName);
            string packagePath = Path.Combine(physicalApplicationPathProvider.GetFullPathFor("Packages"),
                packageDetails.Name);
            packageDetails = new FileInfo(packagePath);
            string packageName = null;
            if (packageDetails.Directory.Exists)
            {
                // Load the package and extract the manifest details
                PackageManifest manifest = null;
                using (Stream inputStream = File.OpenRead(packagePath))
                {
                    using (Package newPackage = new Package(inputStream))
                    {
                        if (newPackage.IsValid)
                        {
                            manifest = newPackage.Manifest;
                            manifest.FileName = packageDetails.Name;
                        }
                    }
                }

                if (manifest != null)
                {
                    // Update the package list
                    XmlDocument packageList = LoadPackageList();
                    UpdatePackagesList(manifest, packageList, false);
                    SavePackageList(packageList);
                    packageName = manifest.Name;
                }

                // Finally, remove the package
                packageDetails.Delete();
            }

            return packageName;
        }
        #endregion

        #region UninstallPackage()
        /// <summary>
        /// Uninstalls a package.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="velocityContext"></param>
        public List<PackageImportEventArgs> UninstallPackage(string fileName)
        {
            // Validate that the package is still valid, just in case somebody else deleted it
            string packagePath = Path.Combine(physicalApplicationPathProvider.GetFullPathFor("Packages"),
                fileName);
            FileInfo packageDetails = new FileInfo(packagePath);
            List<PackageImportEventArgs> events = null;
            if (packageDetails.Exists)
            {
                // Load the package and uninstall it
                events = new List<PackageImportEventArgs>();
                using (Stream packageStream = File.OpenRead(packagePath))
                {
                    using (Package package = new Package(packageStream))
                    {
                        package.Message += delegate(object source, PackageImportEventArgs args)
                        {
                            events.Add(args);
                        };
                        package.Uninstall(physicalApplicationPathProvider);

                        // Update the list
                        XmlDocument packageList = LoadPackageList();
                        package.Manifest.IsInstalled = false;
                        UpdatePackagesList(package.Manifest, packageList, true);
                        SavePackageList(packageList);
                    }
                }
            }
            return events;
        }
        #endregion

        #region ListPackages()
        /// <summary>
        /// Lists all the available packages.
        /// </summary>
        /// <returns></returns>
        public virtual List<PackageManifest> ListPackages()
        {
            List<PackageManifest> packages = new List<PackageManifest>();

            // Load the document
            XmlDocument document = LoadPackageList();

            // Parse each item and add it in
            foreach (XmlElement packageElement in document.SelectNodes("/packages/package"))
            {
                PackageManifest manifest = new PackageManifest();
                manifest.Name = packageElement.GetAttribute("name");
                manifest.Description = packageElement.GetAttribute("description");
                manifest.Type = (PackageType)Enum.Parse(typeof(PackageType), packageElement.GetAttribute("type"));
                manifest.FileName = packageElement.GetAttribute("file");
                manifest.IsInstalled = (packageElement.GetAttribute("installed") == "yes");
                packages.Add(manifest);
            }

            return packages;
        }
        #endregion
        #endregion

        #region Private methods
        #region SaveToFile()
        /// <summary>
        /// Saves a stream to a file.
        /// </summary>
        /// <param name="stream">The stream to save.</param>
        /// <param name="fileName">The name of the file.</param>
        private void SaveToFile(Stream stream, string fileName)
        {
            using (FileStream outputWriter = File.Create(fileName))
            {
                byte[] data = new byte[blockSize];
                int dataLength = stream.Read(data, 0, blockSize);
                while (dataLength > 0)
                {
                    outputWriter.Write(data, 0, dataLength);
                    dataLength = stream.Read(data, 0, blockSize);
                }
                outputWriter.Close();
            }
        }
        #endregion

        #region LoadPackageList()
        /// <summary>
        /// Load the package list.
        /// </summary>
        /// <returns></returns>
        private XmlDocument LoadPackageList()
        {
            string packagePath = Path.Combine(physicalApplicationPathProvider.GetFullPathFor("packages"),
                "packages.xml");
            FileInfo listDetails = new FileInfo(packagePath);
            XmlDocument document = new XmlDocument();
            if (listDetails.Exists)
            {
                document.Load(listDetails.FullName);
            }
            else
            {
                document.AppendChild(
                    document.CreateElement("packages"));
            }
            return document;
        }
        #endregion

        #region UpdatePackagesList()
        /// <summary>
        /// Updates a package in the list of packages.
        /// </summary>
        /// <param name="manifest">The manifest of the package.</param>
        /// <param name="packageList">The XML document containing the list.</param>
        /// <param name="addPackage">Whether to add or remove the package</param>
        private void UpdatePackagesList(PackageManifest manifest, XmlDocument packageList, bool addPackage)
        {
            XmlElement packageElement = packageList.SelectSingleNode(
                string.Format("/packages/package[@name='{0}']",
                manifest.Name)) as XmlElement;
            if (packageElement == null)
            {
                if (addPackage)
                {
                    // Generate a new package element
                    packageElement = packageList.CreateElement("package");
                    packageElement.SetAttribute("name", manifest.Name);
                    packageElement.SetAttribute("description", manifest.Description);
                    packageElement.SetAttribute("type", manifest.Type.ToString());
                    packageElement.SetAttribute("file", manifest.FileName);
                    packageList.DocumentElement.AppendChild(packageElement);
                }
            }
            else
            {
                if (!addPackage)
                {
                    packageElement.ParentNode.RemoveChild(packageElement);
                }
            }

            if (addPackage)
            {
                // Update the installed status - this is the only thing that should change
                packageElement.SetAttribute("installed", manifest.IsInstalled ? "yes" : "no");
            }
        }
        #endregion

        #region SavePackageList()
        /// <summary>
        /// Save the package list.
        /// </summary>
        private void SavePackageList(XmlDocument packageList)
        {
            string packagePath = Path.Combine(physicalApplicationPathProvider.GetFullPathFor("packages"),
                "packages.xml");

            // Configure the document options
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            // Save the document
            using (XmlWriter writer = XmlWriter.Create(packagePath, settings))
            {
                packageList.Save(writer);
                writer.Close();
                writer.Close();
            }
        }
        #endregion
        #endregion
    }
}
