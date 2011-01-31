using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using Exortech.NetReflector;
using ICSharpCode.SharpZipLib.Zip;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using System.Globalization;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    /// <summary>
    /// <para>
    /// Generates a ZIP file package containing the specified files.
    /// </para>
    /// <para>
    /// This will generate a "package" of files in a compressed format. The files must be specified, plus an optional manifest can be included.
    /// </para>
    /// <para>
    /// This publisher also allows the generation of a "manifest" to include in the package. A manifest contains additional details on the
    /// package, both at a general level and at a file level.
    /// </para>
    /// </summary>
    /// <title>Package Publisher</title>
    /// <version>1.4.4</version>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;package&gt;
    /// &lt;name&gt;Example&lt;/name&gt;
    /// &lt;packageList&gt;
    /// &lt;packageFile&gt;
    /// &lt;sourceFile&gt;results.txt&lt;/sourceFile&gt;
    /// &lt;/packageFile&gt;
    /// &lt;/packageList&gt;
    /// &lt;/package&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;package&gt;
    /// &lt;name&gt;Example&lt;/name&gt;
    /// &lt;compression&gt;9&lt;/compression&gt;
    /// &lt;always&gt;true&lt;/always&gt;
    /// &lt;flatten&gt;true&lt;/flatten&gt;
    /// &lt;manifest type="defaultManifestGenerator"/&gt;
    /// &lt;packageList&gt;
    /// &lt;packageFile&gt;
    /// &lt;sourceFile&gt;results.txt&lt;/sourceFile&gt;
    /// &lt;/packageFile&gt;
    /// &lt;/packageList&gt;
    /// &lt;/package&gt;
    /// </code>
    /// </example>
    [ReflectorType("package")]
    public class PackagePublisher
        : TaskBase
    {
        #region Private fields
        private int compressionLevel = 5;
        #endregion

        #region Public properties
        #region Name
        /// <summary>
        /// The name of the package file.
        /// </summary>
        /// <remarks>
        /// This will be the filename of the package. If the extension zip is omitted, it will be added automatically.
        /// </remarks>
        /// <version>1.4.4</version>
        /// <default>n/a</default>
        [ReflectorProperty("name", Required = true)]
        public string PackageName { get; set; }
        #endregion
        #region CompressionLevel

        /// <summary>
        /// The level of compression to use. The valid range is from zero to nine, zero is no compression and nine is maximum compression.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>5</default>
        [ReflectorProperty("compression", Required = false)]
        public int CompressionLevel
        {
            get
            {
                return compressionLevel;
            }
            set
            {
                if ((value < 0) || (value > 9))
                {
                    throw new ArgumentOutOfRangeException("CompressionLevel");
                }

                compressionLevel = value;
            }
        }
        #endregion

        #region AlwaysPackage
        /// <summary>
        /// Whether the package should always be generated or not.
        /// </summary>
        /// <remarks>
        /// By default a package will only be generated for a successful build. Setting this property
        /// to true, and including it in the publishers section means the package will always be 
        /// generated, irrespective of the outcome of the build.
        /// </remarks>
        /// <version>1.4.4</version>
        /// <default>false</default>
        [ReflectorProperty("always", Required = false)]
        public bool AlwaysPackage { get; set; }
        #endregion

        #region Flatten
        /// <summary>
        /// Should the file structure be flattened or not.
        /// </summary>
        /// <remarks>
        /// By default, the folder structure will also be included in the package. Setting this property
        /// to true will flatten (omit) the folder information.
        /// </remarks>
        /// <version>1.4.4</version>
        /// <default>false</default>
        [ReflectorProperty("flatten", Required = false)]
        public bool Flatten { get; set; }
        #endregion

        #region ManifestGenerator
        /// <summary>
        ///The manifest generator to be used.
        /// </summary>
        /// <remarks>
        /// If this property is not set no manifest will be generated.
        /// </remarks>
        /// <version>1.4.4</version>
        /// <default>None</default>
        [ReflectorProperty("manifest", Required = false, InstanceTypeKey = "type")]
        public IManifestGenerator ManifestGenerator { get; set; }
        #endregion

        #region PackageList
        /// <summary>
        /// The list of files and folders to include in the package.
        /// </summary>
        /// <remarks>
        /// All relative files will be relative to the baseDirectory.
        /// </remarks>
        /// <version>1.6</version>
        /// <default>n/a</default>
        [ReflectorProperty("packageList", Required = true)]
        public IPackageItem[] PackageList { get; set; }
        #endregion

        #region OutputDirectory
        /// <summary>
        /// The location to output the package to.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>Project Artifact Directory</default>
        [ReflectorProperty("outputDir", Required = false)]
        public string OutputDirectory { get; set; }
        #endregion
        #endregion

        #region Protected methods
        #region Execute()
        /// <summary>
        /// Run this publisher.
        /// </summary>
        /// <param name="result">The result of the build.</param>
        protected override bool Execute(IIntegrationResult result)
        {
            // Check whether the package should be generated
            if (AlwaysPackage || (result.Status == IntegrationStatus.Success))
            {
                var logMessage = string.Format(CultureInfo.CurrentCulture, "Building package '{0}'", PackageName);
                result.BuildProgressInformation.SignalStartRunTask(logMessage);
                Log.Info(logMessage);

                // Work from a temporary file, if the package is successful generated, then it will
                // be moved to the correct location.
                string tempFile = Path.GetTempFileName();
                try
                {
                    var fileList = new List<string>();
                    var zipStream = new ZipOutputStream(File.Create(tempFile));
                    zipStream.IsStreamOwner = true;
                    zipStream.UseZip64 = UseZip64.Off;
                    try
                    {
                        var packagedFiles = new List<string>();
                        zipStream.SetLevel(CompressionLevel == 0 ? 5 : CompressionLevel);
                        foreach (IPackageItem packageEntry in PackageList)
                        {
                            var packagedFileList = packageEntry.Package(result, zipStream);
                            if (packagedFileList != null)
                            {
                                packagedFiles.AddRange(packagedFileList);
                            }
                        }

                        // Generate and add the manifest
                        if (ManifestGenerator != null)
                        {
                            Log.Debug("Generating manifest");
                            AddManifest(result, packagedFiles, zipStream);
                        }
                    }
                    finally
                    {
                        zipStream.Finish();
                        zipStream.Close();
                    }

                    // Move the file over
                    string actualFile = MoveFile(result, tempFile);

                    // Add a list entry
                    Log.Debug("Adding to package list(s)");
                    string listFile = Path.Combine(result.Label, result.ProjectName + "-packages.xml");
                    AddToPackageList(result, listFile, actualFile, fileList.Count);
                }
                finally
                {
                    if (File.Exists(tempFile))
                    {
                        try
                        {
                            File.Delete(tempFile);
                        }
                        catch (IOException)
                        {
                            // Can't delete the temp. file - not a major, just means the file is left on the system!
                        }
                    }
                }
            }

            // Set the status of the result
            if (result.Status == IntegrationStatus.Unknown) result.Status = IntegrationStatus.Success;
            return true;
        }
        #endregion

        #region UpgradeConfiguration()
        /// <summary>
        /// Upgrades the configuration for the node.
        /// </summary>
        /// <param name="configVersion">The version of the configuration.</param>
        /// <param name="node">The input node.</param>
        /// <returns>The upgraded node</returns>
        protected override XmlNode UpgradeConfiguration(Version configVersion, XmlNode node)
        {
            node = base.UpgradeConfiguration(configVersion, node);
            if (configVersion < new Version(1, 6))
            {
                var listNode = node.OwnerDocument.CreateElement("packageList");
                XmlNode filesNode = null;
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (childNode.Name == "files")
                    {
                        filesNode = childNode;
                        break;
                    }
                }

                if (filesNode != null)
                {
                    filesNode.ParentNode.ReplaceChild(listNode, filesNode);
                    foreach (XmlElement childNode in filesNode.ChildNodes)
                    {
                        var fileNode = node.OwnerDocument.CreateElement("packageFile");
                        fileNode.SetAttribute("sourceFile", childNode.InnerText);
                        listNode.AppendChild(fileNode);
                    }
                }
            }

            return node;
        }
        #endregion
        #endregion

        #region Private methods

        #region AddToPackageList()
        /// <summary>
        /// Add the package to the list of packages.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="listFile"></param>
        /// <param name="fileName"></param>
        /// <param name="numberOfFiles"></param>
        /// <remarks>
        /// This is to allow discoverability of packages. In future CruiseControl.Net will allow clients to download this
        /// list to see which packages are available.
        /// </remarks>
        private void AddToPackageList(IIntegrationResult result, string listFile, string fileName, int numberOfFiles)
        {
            XmlDocument listXml = new XmlDocument();

            // See if there is an existing list
            listFile = result.BaseFromArtifactsDirectory(listFile);
            if (File.Exists(listFile))
            {
                listXml.Load(listFile);
            }
            else
            {
                XmlElement rootElement = listXml.CreateElement("packages");
                listXml.AppendChild(rootElement);
            }

            // See if the entry already exists
            XmlElement packageElement = listXml.SelectSingleNode(
                string.Format(CultureInfo.CurrentCulture, "/packages/package[@name='{0}']", PackageName)) as XmlElement;
            if (packageElement == null)
            {
                packageElement = listXml.CreateElement("package");
                listXml.DocumentElement.AppendChild(packageElement);
                packageElement.SetAttribute("name", PackageName);
            }


            // Add the properties for the package
            var packageFile = new FileInfo(fileName);
            packageElement.SetAttribute("file", fileName);
            packageElement.SetAttribute("label", result.Label);
            packageElement.SetAttribute("time", DateTime.Now.ToString("s", CultureInfo.CurrentCulture));
            packageElement.SetAttribute("files", numberOfFiles.ToString(CultureInfo.CurrentCulture));
            packageElement.SetAttribute("size", packageFile.Length.ToString(CultureInfo.CurrentCulture));

            // Save the updated list
            var listDir = Path.GetDirectoryName(listFile);
            if (!Directory.Exists(listDir))
            {
                Directory.CreateDirectory(listDir);
            }

            listXml.Save(listFile);
        }
        #endregion

        #region MoveFile()
        /// <summary>
        /// Moves the file from its temporary (working) location to its final location.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="tempFile"></param>
        /// <returns></returns>
        /// <remarks>
        /// This method will also generate the correct name of the file.
        /// </remarks>
        private string MoveFile(IIntegrationResult result, string tempFile)
        {
            string actualFile = EnsureFileExtension(Path.Combine(result.Label, PackageName), ".zip");
            actualFile = result.BaseFromArtifactsDirectory(actualFile);
            if (File.Exists(actualFile))
            {
                DeleteFileWithRetry(actualFile);
            }

            string actualFolder = Path.GetDirectoryName(actualFile);
            if (!Directory.Exists(actualFolder))
            {
                Directory.CreateDirectory(actualFolder);
            }

            File.Move(tempFile, actualFile);

            if (!string.IsNullOrEmpty(OutputDirectory))
            {
                // Copy the file to the output directory (so it can be used by other tasks)
                var basePath = OutputDirectory;
                if (!Path.IsPathRooted(basePath))
                {
                    basePath = Path.Combine(result.ArtifactDirectory, basePath);
                }

                Log.Info(string.Format(
                    CultureInfo.CurrentCulture,"Copying file to '{0}'", basePath));
                File.Copy(
                    actualFile, 
                    EnsureFileExtension(Path.Combine(basePath, PackageName), ".zip"), 
                    true);
            }

            return actualFile;
        }
        #endregion

        #region DeleteFileWithRetry()
        /// <summary>
        /// Attempts to delete a file within a retry loop.
        /// </summary>
        /// <param name="actualFile"></param>
        private void DeleteFileWithRetry(string actualFile)
        {
            var retryLoop = 3;
            while (retryLoop > 0)
            {
                try
                {
                    File.Delete(actualFile);
                    retryLoop = 0;
                }
                catch (IOException)
                {
                    if (retryLoop-- > 0)
                    {
                        Log.Warning(
                            string.Format(
                                CultureInfo.CurrentCulture, "Unable to delete file '{0}', delaying before retry",
                                actualFile));
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
        #endregion

        #region AddManifest()
        /// <summary>
        /// Generate the manifest and add it to the package.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="packagedFiles"></param>
        /// <param name="zipStream"></param>
        private void AddManifest(IIntegrationResult result, List<string> packagedFiles, ZipOutputStream zipStream)
        {
            // Generate the manifest
            XmlDocument manifest = ManifestGenerator.Generate(result, packagedFiles.ToArray());

            // And add the manifest to the package
            ZipEntry entry = new ZipEntry("manifest.xml");
            zipStream.PutNextEntry(entry);
            manifest.Save(zipStream);
            zipStream.CloseEntry();
        }
        #endregion

        #region EnsureFileExtension()
        /// <summary>
        /// Ensures the file extension.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="extension">The extension.</param>
        /// <returns>The filename with the extension.</returns>
        private static string EnsureFileExtension(string fileName, string extension)
        {
            var actualFile = fileName;
            if (!actualFile.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase))
            {
                actualFile += extension;
            }

            return actualFile;
        }
        #endregion
        #endregion
    }
}
