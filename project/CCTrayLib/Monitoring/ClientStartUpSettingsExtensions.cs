using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
    /// <summary>
    /// Extension methods for working with <see cref="ClientStartUpSettings"/>.
    /// </summary>
    public static class ClientStartUpSettingsExtensions
    {
        #region Public methods
        #region GenerateStartupSettings()
        /// <summary>
        /// Generates a set of <see cref="ClientStartUpSettings"/>.
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public static ClientStartUpSettings GenerateStartupSettings(BuildServer server)
        {
            var extSettings = server.ExtensionSettings ?? string.Empty;
            if (extSettings.Length < 6) extSettings += new string(' ', 6 - extSettings.Length);
            var settings = new ClientStartUpSettings
            {
                BackwardsCompatable = (extSettings.Substring(0, 3) == "OLD"),
                UseEncryption = (extSettings.Substring(3, 3) == "SEC")
            };
            return settings;
        }
        #endregion

        #region GenerateExtensionSettings()
        /// <summary>
        /// Generates the extension settings to store in a <see cref="BuildServer"/>.
        /// </summary>
        /// <param name="useOld"></param>
        /// <param name="useEncryption"></param>
        /// <returns></returns>
        public static string GenerateExtensionSettings(bool useOld, bool useEncryption)
        {
            var settings = (useOld ? "OLD" : "NEW") +
                    (useEncryption ? "SEC" : "OPN");
            return settings;
        }
        #endregion
        #endregion
    }
}
