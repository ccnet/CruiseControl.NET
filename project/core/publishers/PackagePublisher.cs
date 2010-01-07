using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Exortech.NetReflector;
using ICSharpCode.SharpZipLib.Zip;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Tasks;
using System.Threading;

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
    /// &lt;files&gt;
    /// &lt;file&gt;Results.txt&lt;/file&gt;
    /// &lt;/files&gt;
    /// &lt;/package&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;package&gt;
    /// &lt;name&gt;Example&lt;/name&gt;
    /// &lt;compression&gt;9&lt;/compression&gt;
    /// &lt;always&gt;true&lt;/always&gt;
    /// &lt;flatten&gt;true&lt;/flatten&gt;
    /// &lt;baseDirectory&gt;C:\Builds\CC.Net&lt;/baseDirectory&gt;
    /// &lt;manifest type="defaultManifestGenerator" /&gt;
    /// &lt;files&gt;
    /// &lt;file&gt;Results.txt&lt;/file&gt;
    /// &lt;/files&gt;
    /// &lt;/package&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <heading>Sub-folders</heading>
    /// <para>
    /// Sub-folders can be added using a combination of relative paths in the filename and a base directory. When a file
    /// is added, the publisher checks if the filename is relative to the base directory and strips the base directory
    /// from the file name.
    /// </para>
    /// <para>
    /// For example, the following configuration will add two files, logo.gif in the root and AdminIcon.gif in a 
    /// sub-folder.
    /// </para>
    /// <code>
    /// &lt;package&gt;
    /// &lt;baseDirectory&gt;temp&lt;/baseDirectory&gt;
    /// &lt;files&gt;
    /// &lt;file&gt;logo.gif&lt;/file&gt;
    /// &lt;file&gt;images\AdminIcon.gif&lt;/file&gt;
    /// &lt;/files&gt;
    /// &lt;name&gt;output&lt;/name&gt;
    /// &lt;/package&gt;
    /// </code>
    /// </remarks>
    [ReflectorType("package")]
    public class PackagePublisher
        : TaskBase
    {
        #region Private fields
        private int compressionLevel = 5;
        private bool alwaysPackage;
        private bool flatten;
        private IManifestGenerator manifestGenerator;
        private string baseDirectory;
        private string[] files = {};
        private string name;
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
        [ReflectorProperty("name")]
        public string PackageName
        {
            get { return name; }
            set { name = value; }
        }
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
            get { return compressionLevel; }
            set
            {
                if ((value < 0) || (value > 9)) throw new ArgumentOutOfRangeException("CompressionLevel");
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
        public bool AlwaysPackage
        {
            get { return alwaysPackage; }
            set { alwaysPackage = value; }
        }
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
        public bool Flatten
        {
            get { return flatten; }
            set { flatten = value; }
        }
        #endregion

        #region BaseDirectory
        /// <summary>
        /// The directory to base all the file locations from.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>Project working directory</default>
        [ReflectorProperty("baseDirectory", Required = false)]
        public string BaseDirectory
        {
            get { return baseDirectory; }
            set { baseDirectory = value; }
        }
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
        public IManifestGenerator ManifestGenerator
        {
            get { return manifestGenerator; }
            set { manifestGenerator = value; }
        }
        #endregion

        #region Files
        /// <summary>
        /// The files to include in the package.
        /// </summary>
        /// <remarks>
        /// All relative files will be relative to the baseDirectory.
        /// </remarks>
        /// <version>1.4.4</version>
        /// <default>n/a</default>
        [ReflectorArray("files")]
        public string[] Files
        {
            get { return files; }
            set { files = value; }
        }
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
            if (alwaysPackage || (result.Status == IntegrationStatus.Success))
            {
                string logMessage = string.Format("Building package '{0}'", name);
                result.BuildProgressInformation.SignalStartRunTask(logMessage);
                Log.Info(logMessage);

                // Work from a temporary file, if the package is successful generated, then it will
                // be moved to the correct location.
                string tempFile = Path.GetTempFileName();
                try
                {
                    List<string> fileList = new List<string>();
                    ZipOutputStream zipStream = new ZipOutputStream(File.Create(tempFile));
                    zipStream.IsStreamOwner = true;
                    zipStream.UseZip64 = UseZip64.Off;
                    try
                    {
                        zipStream.SetLevel(compressionLevel);

                        // Add all the files
                        List<string> packagedFiles = new List<string>();
                        fileList = GenerateFileList(result);
                        Log.Debug(string.Format("Compressing {0} file(s)", fileList.Count));
                        foreach (string file in fileList)
                        {
                            string fileInfo = PackageFile(result, file, zipStream);
                            if (fileInfo != null) packagedFiles.Add(fileInfo);
                        }

                        // Generate and add the manifest
                        if (manifestGenerator != null)
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
                    if (File.Exists(tempFile)) File.Delete(tempFile);
                }
            }

            // Set the status of the result
            if (result.Status == IntegrationStatus.Unknown) result.Status = IntegrationStatus.Success;
            return true;
        }
        #endregion
        #endregion

        #region Private methods
        #region GenerateFileList()
        /// <summary>
        /// Generate the list of files to include in the package.
        /// </summary>
        /// <param name="result">The build result.</param>
        /// <returns>A list of all the files to be included.</returns>
        /// <remarks>
        /// This method uses custom logic for handling "**"
        /// </remarks>
        private List<string> GenerateFileList(IIntegrationResult result)
        {
            List<string> fileList = new List<string>();
            string allDirsWildcard = Path.DirectorySeparatorChar + "**" + Path.DirectorySeparatorChar;

            foreach (string fileName in files)
            {
                // Handle any wildcard characters
                if (fileName.Contains("*") || fileName.Contains("?"))
                {
                    List<string> possibilities = new List<string>();

                    string actualPath = EnsureFileIsRooted(result, fileName);

                    // Check for **, if it exists, then split the search pattern and use the second half to find all
                    // matching files
                    if (actualPath.Contains(allDirsWildcard))
                    {
                        int position = actualPath.IndexOf(allDirsWildcard);
                        string path = actualPath.Substring(0, position);
                        string pattern = actualPath.Substring(position + 4);
                        possibilities.AddRange(Directory.GetFiles(path, pattern, SearchOption.AllDirectories));
                    }
                    else
                    {
                        // Otherwise, just use the plain ordinary search pattern
                        int position = actualPath.IndexOfAny(new char[] { '*', '?' });
                        position = actualPath.LastIndexOf(Path.DirectorySeparatorChar, position);
                        string path = actualPath.Substring(0, position);
                        string pattern = actualPath.Substring(position + 1);
                        possibilities.AddRange(Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly));
                    }

                    // The current list of files is just a set of possibilities, now need to check that they completely
                    // match the search criteria and they have not already been added.
                    foreach (string possibility in possibilities)
                    {
                        if (!fileList.Contains(possibility))
                        {
                            if (PathUtils.MatchPath(actualPath, possibility, false))
                            {
                                fileList.Add(possibility);
                            }
                        }
                    }
                }
                else
                {
                    // Make sure the file is rooted
                    string actualPath = EnsureFileIsRooted(result, fileName);

                    // Only add it to the list if it doesn't already exist
                    if (!fileList.Contains(actualPath))
                    {
                        fileList.Add(actualPath);
                    }
                }
            }

            return fileList;
        }
        #endregion

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
                string.Format("/packages/package[@name='{0}']", name)) as XmlElement;
            if (packageElement == null)
            {
                packageElement = listXml.CreateElement("package");
                listXml.DocumentElement.AppendChild(packageElement);
                packageElement.SetAttribute("name", name);
            }

            // Ensure the folder exists
            var packageFile = new FileInfo(fileName);
            if (!packageFile.Directory.Exists)
            {
                packageFile.Directory.Create();
            }

            // Add the properties for the package
            packageElement.SetAttribute("file", fileName);
            packageElement.SetAttribute("label", result.Label);
            packageElement.SetAttribute("time", DateTime.Now.ToString("s"));
            packageElement.SetAttribute("files", numberOfFiles.ToString());
            packageElement.SetAttribute("size", packageFile.Length.ToString());

            // Save the updated list
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
            string actualFile = Path.Combine(result.Label, name);
            if (!actualFile.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase)) actualFile += ".zip";
            actualFile = result.BaseFromArtifactsDirectory(actualFile);
            if (File.Exists(actualFile)) DeleteFileWithRetry(actualFile);
            string actualFolder = Path.GetDirectoryName(actualFile);
            if (!Directory.Exists(actualFolder)) Directory.CreateDirectory(actualFolder);
            File.Move(tempFile, actualFile);

            if (!string.IsNullOrEmpty(OutputDirectory))
            {
                // Copy the file to the output directory (so it can be used by other tasks)
                var basePath = OutputDirectory;
                if (!Path.IsPathRooted(basePath)) basePath = Path.Combine(result.ArtifactDirectory, basePath);
                Log.Info(string.Format("Copying file to '{0}'", basePath));
                File.Copy(actualFile, Path.Combine(basePath, Name), true);
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
                                "Unable to delete file '{0}', delaying before retry",
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
            XmlDocument manifest = manifestGenerator.Generate(result, packagedFiles.ToArray());

            // And add the manifest to the package
            ZipEntry entry = new ZipEntry("manifest.xml");
            zipStream.PutNextEntry(entry);
            manifest.Save(zipStream);
            zipStream.CloseEntry();
        }
        #endregion

        #region PackageFile()
        /// <summary>
        /// Adds a file to the current package.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="file"></param>
        /// <param name="zipStream"></param>
        /// <returns></returns>
        private string PackageFile(IIntegrationResult result, string file, ZipOutputStream zipStream)
        {
            // Generate the full path to the file and make sure it exists
            var baseFolder = string.IsNullOrEmpty(baseDirectory) ? result.WorkingDirectory : baseDirectory;
            if (!Path.IsPathRooted(baseFolder))
            {
                baseFolder = result.BaseFromWorkingDirectory(baseFolder);
            }

            // Generate the full path to the file
            var fullName = Path.IsPathRooted(file) ? file : result.BaseFromWorkingDirectory(file);
            var fileInfo = new FileInfo(fullName);
            if (fileInfo.Exists)
            {
                // Generate the name of the file to store in the package
                var fileName = file;
                if (flatten)
                {
                    // For flattened packages, just store the name of the file
                    fileName = fileInfo.Name;
                }
                else
                {
                    // For non-flattened packages, store the full path, except when it is relative to the base directory, 
                    if (fullName.StartsWith(baseFolder, StringComparison.InvariantCultureIgnoreCase))
                    {
                        fileName = fullName.Substring(baseFolder.Length);
                    }
                    if (fileName.StartsWith(Path.DirectorySeparatorChar + string.Empty)) fileName = fileName.Substring(1);
                }

                // Add the entry to the file file
                var entry = new ZipEntry(ZipEntry.CleanName(fileName));
                entry.Size = fileInfo.Length;
                zipStream.PutNextEntry(entry);
                var buffer = new byte[8182];

                // Add the actual file - just tranfer the data from one stream to another
                var inputStream = fileInfo.OpenRead();
                try
                {
                    var dataLength = 1;
                    while (dataLength > 0)
                    {
                        dataLength = inputStream.Read(buffer, 0, buffer.Length);
                        zipStream.Write(buffer, 0, dataLength);
                    }
                }
                finally
                {
                    inputStream.Close();
                }

                // Finish up and return the file name so it can be used in the manifest
                zipStream.CloseEntry();
                return fileName;
            }
            else
            {
                // Still need to return something, but since no file was found, just return null
                return null;
            }
        }
        #endregion

        #region EnsureFileIsRooted()
        /// <summary>
        /// Ensure the file is rooted to the base directory.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string EnsureFileIsRooted(IIntegrationResult result, string fileName)
        {
            string actualPath = fileName;
            string baseFolder = string.IsNullOrEmpty(baseDirectory) ? result.WorkingDirectory : baseDirectory;
            if (!Path.IsPathRooted(actualPath)) actualPath = Path.Combine(baseFolder, actualPath);
            return actualPath;
        }
        #endregion
        #endregion
    }
}
