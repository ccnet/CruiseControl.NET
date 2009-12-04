using System;
using System.IO;
using System.Xml;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    /// <summary>
    /// Imports a manifest from an existing file.
    /// </summary>
    /// <title>Manifest Importer</title>
    /// <version>1.4.4</version>
    /// <remarks>
    /// This "generator" is not a true generator, instead it will import an existing file to use as the package
    /// manifest.
    /// </remarks>
    /// <example>
    /// <code>
    /// &lt;manifest type="importManifest" file="ExistingManifest.xml" /&gt;
    /// </code>
    /// </example>
    /// <key name="type">
    /// <description>The type of generator.</description>
    /// <value>importManifest</value>
    /// </key>
    [ReflectorType("importManifest")]
    public class ManifestImporter
        : IManifestGenerator
    {
        #region Private fields
        private string fileName;
        #endregion

        #region Public properties
        #region FileName
        /// <summary>
        /// The name of the file to import.
        /// </summary>
        /// <remarks>
        /// If this is a relative file, it will be based relative to the working directory of the project.
        /// </remarks>
        /// <version>1.4.4</version>
        /// <default>n/a</default>
        [ReflectorProperty("filename")]
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }
        #endregion
        #endregion

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
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentOutOfRangeException("FileName");

            XmlDocument manifest = new XmlDocument();
            string actualFile = fileName;
            if (!Path.IsPathRooted(actualFile)) actualFile = result.BaseFromWorkingDirectory(actualFile);
            manifest.Load(actualFile);
            return manifest;
        }
        #endregion
        #endregion
    }
}
