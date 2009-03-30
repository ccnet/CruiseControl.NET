using System.Collections.Generic;
using System.Xml;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    /// <summary>
    /// Generate a default manifest for a package.
    /// </summary>
    [ReflectorType("defaultManifestGenerator")]
    public class ManifestGenerator
        : IManifestGenerator
    {
        #region Methods
        #region Generate()
        /// <summary>
        /// Generate a manifest for a package.
        /// </summary>
        /// <param name="result">The result of the build.</param>
        /// <param name="packagedFiles">The files that were packaged.</param>
        /// <returns>An <see cref="XmlDocument"/> containing the manifest.</returns>
        public XmlDocument Generate(IIntegrationResult result, string[] packagedFiles)
        {
            XmlDocument manifest = new XmlDocument();
            XmlElement rootElement = manifest.CreateElement("manifest");
            manifest.AppendChild(rootElement);

            // Generate the header information
            AddManifestHeader(result, rootElement);

            // Add the files
            foreach (string file in packagedFiles)
            {
                XmlElement fileElement = manifest.CreateElement("file");
                fileElement.SetAttribute("name", file);
                rootElement.AppendChild(fileElement);
            }

            return manifest;
        }
        #endregion
        #endregion

        #region Private methods
        #region AddManifestHeader()
        /// <summary>
        /// Generates the manifest header.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="rootElement"></param>
        private void AddManifestHeader(IIntegrationResult result, XmlElement rootElement)
        {
            // Initialise the header
            XmlDocument manifest = rootElement.OwnerDocument;
            XmlElement headerElement = manifest.CreateElement("header");
            rootElement.AppendChild(headerElement);

            // Add some details about the build
            headerElement.SetAttribute("project", result.ProjectName);
            headerElement.SetAttribute("label", result.Label);
            headerElement.SetAttribute("build", result.BuildCondition.ToString());
            headerElement.SetAttribute("status", result.Status.ToString());

            // Add the list of modifications
            Dictionary<int, XmlElement> changes = new Dictionary<int, XmlElement>();
            foreach (Modification modification in result.Modifications)
            {
                // Add each change header only once
                if (!changes.ContainsKey(modification.ChangeNumber))
                {
                    // Add a change header with some details about the change
                    XmlElement modificationElement = manifest.CreateElement("modification");
                    headerElement.AppendChild(modificationElement);
                    modificationElement.SetAttribute("user", modification.UserName);
                    modificationElement.SetAttribute("changeNumber", modification.ChangeNumber.ToString());
                    if (!string.IsNullOrEmpty(modification.Comment))
                    {
                        XmlElement commentElement = manifest.CreateElement("comment");
                        commentElement.InnerText = modification.Comment;
                        modificationElement.AppendChild(commentElement);
                    }
                    modificationElement.SetAttribute("time", modification.ModifiedTime.ToString("s"));
                    changes.Add(modification.ChangeNumber, modificationElement);
                }

                // Add each file that was included in the change
                XmlElement fileElement = manifest.CreateElement("file");
                fileElement.SetAttribute("name", modification.FileName);
                fileElement.SetAttribute("type", modification.Type);
                changes[modification.ChangeNumber].AppendChild(fileElement);
            }
        }
        #endregion
        #endregion
    }
}
