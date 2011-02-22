namespace CruiseControl.Core.Utilities
{
    using System;

    /// <summary>
    /// Helper methods for working with URNs.
    /// </summary>
    public static class UrnHelpers
    {
        #region Public methods
        #region IsCCNetUrn()
        /// <summary>
        /// Determines whether a URN is a valid CC.Net URN.
        /// </summary>
        /// <param name="urn">The URN to check.</param>
        /// <returns>
        ///   <c>true</c> if the URN is a valid CC.Net URN; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCCNetUrn(string urn)
        {
            if (string.IsNullOrEmpty(urn))
            {
                return false;
            }

            var isValid = urn.StartsWith("urn:ccnet:", StringComparison.InvariantCultureIgnoreCase);
            return isValid;
        }
        #endregion

        #region GenerateProjectUrn()
        /// <summary>
        /// Generates a project URN.
        /// </summary>
        /// <param name="server">The server URN or name.</param>
        /// <param name="project">The project name.</param>
        /// <returns>
        /// The URN for the project.
        /// </returns>
        public static string GenerateProjectUrn(string server, string project)
        {
            if (string.IsNullOrEmpty(server))
            {
                throw new ArgumentNullException("server");
            }

            if (string.IsNullOrEmpty(project))
            {
                throw new ArgumentNullException("project");
            }

            if (IsCCNetUrn(project))
            {
                return project;
            }

            if (IsCCNetUrn(server))
            {
                return ExtractServerUrn(server) + ":" + project.ToLowerInvariant();
            }

            var urn = "urn:ccnet:" + server + ":" + project;
            return urn;
        }

        /// <summary>
        /// Generates a project URN.
        /// </summary>
        /// <param name="server">The server details.</param>
        /// <param name="project">The project name.</param>
        /// <returns>
        /// The URN for the project.
        /// </returns>
        public static string GenerateProjectUrn(Server server, string project)
        {
            if (server == null)
            {
                throw new ArgumentNullException("server");
            }

            return GenerateProjectUrn(server.UniversalName, project);
        }
        #endregion

        #region ExtractServerUrn()
        /// <summary>
        /// Extracts the server URN.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <returns>
        /// The URN for the server.
        /// </returns>
        public static string ExtractServerUrn(string server)
        {
            if (string.IsNullOrEmpty(server))
            {
                throw new ArgumentNullException("server");
            }

            if (!IsCCNetUrn(server) || (server.Length == 10))
            {
                throw new ArgumentException("Not a valid CC.Net URN");
            }

            var nextColon = server.IndexOf(':', 11);
            if (nextColon < 0)
            {
                return server;
            }

            var urn = server.Substring(0, nextColon);
            return urn;
        }
        #endregion
        #endregion
    }
}
