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
    /// Publisher to generate a ZIP package.
    /// </summary>
    /// <remarks>
    /// This will generate a "package" of files in a compressed format. The files must be
    /// specified, plus an optional manifest can be included.
    /// </remarks>
    [ReflectorType("package")]
    public class PackagePublisher
        : TaskBase, ITask
    {
        #region Private fields
        private int compressionLevel = 5;
        private bool singleInstance;
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
        /// This will be the filename of the package. If the extension zip is omitted, it
        /// will be added automatically.
        /// </remarks>
        [ReflectorProperty("name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        #endregion

        #region CompressionLevel
        /// <summary>
        /// The level of compression to use.
        /// </summary>
        /// <remarks>
        /// The default compression level is 5. This is an optional setting.
        /// </remarks>
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
        [ReflectorProperty("flatten", Required = false)]
        public bool Flatten
        {
            get { return flatten; }
            set { flatten = value; }
        }
        #endregion

        #region SingleInstance
        /// <summary>
        /// Whether there should only be one instance of this package.
        /// </summary>
        /// <remarks>
        /// By default this publisher will generate the package and store a back-up copy of it in a
        /// sub-folder. This means there will be a history of all the packages produced. If there is
        /// no need for a history this property can be set to true to stop this behaviour.
        /// </remarks>
        [ReflectorProperty("single", Required = false)]
        public bool SingleInstance
        {
            get { return singleInstance; }
            set { singleInstance = value; }
        }
        #endregion

        #region BaseDirectory
        /// <summary>
        /// The directory to base all the file locations from.
        /// </summary>
        /// <remarks>
        /// If this property is not set, the base location will default to the working directory.
        /// </remarks>
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
        /// All relative files will be relative to the <see cref="BaseDirectory"/>.
        /// </remarks>
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
        [ReflectorProperty("outputDir", Required = false)]
        public string OutputDirectory { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region Run()
        /// <summary>
        /// Run this publisher.
        /// </summary>
        /// <param name="result">The result of the build.</param>
        public void Run(IIntegrationResult result)
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
                    string listFile = result.ProjectName + "-packages.xml";
                    AddToPackageList(result, listFile, actualFile, fileList.Count);
                    if (!singleInstance)
                    {
                        listFile = Path.Combine(result.Label, listFile);
                        var buildCopy = Path.Combine(
                            Path.Combine(
                                Path.GetDirectoryName(actualFile),
                                result.Label),
                                Path.GetFileName(actualFile));
                        File.Copy(actualFile, buildCopy);
                        AddToPackageList(result, listFile, buildCopy, fileList.Count);
                    }
                }
                finally
                {
                    if (File.Exists(tempFile)) File.Delete(tempFile);
                }
            }

            // Set the status of the result
            if (result.Status == IntegrationStatus.Unknown) result.Status = IntegrationStatus.Success;
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
            FileInfo packageFile = new FileInfo(fileName);

            // Add the properties for the package
            packageElement.SetAttribute("file", fileName);
            packageElement.SetAttribute("label", result.Label);
            packageElement.SetAttribute("time", DateTime.Now.ToString("s"));
            packageElement.SetAttribute("files", numberOfFiles.ToString());
            packageElement.SetAttribute("size", packageFile.Length.ToString());

            // Make sure the folder name exists
            string folderName = Path.GetDirectoryName(listFile);
            if (!Directory.Exists(folderName)) Directory.CreateDirectory(folderName);

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
            string actualFile = name;
            if (!actualFile.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase)) actualFile += ".zip";
            if (!singleInstance) actualFile = Path.Combine(result.Label, actualFile);
            actualFile = result.BaseFromArtifactsDirectory(actualFile);
            if (File.Exists(actualFile))
            {
                // Add a retry loop for deleting the file
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
            string baseFolder = string.IsNullOrEmpty(baseDirectory) ? result.WorkingDirectory : baseDirectory;
            string fullName = Path.IsPathRooted(file) ? file : Path.Combine(baseFolder, file);
            FileInfo fileInfo = new FileInfo(fullName);
            if (fileInfo.Exists)
            {
                // Generate the name of the file to store in the package
                string fileName = file;
                if (flatten)
                {
                    // For flattened packages, just store the name of the file
                    fileName = fileInfo.Name;
                }
                else
                {
                    // For non-flattened packages, store the full path, except when it is relative to the base directory, 
                    if (fileName.StartsWith(baseFolder, StringComparison.InvariantCultureIgnoreCase))
                    {
                        fileName = fileName.Substring(baseFolder.Length);
                    }
                    if (fileName.StartsWith(Path.DirectorySeparatorChar + string.Empty)) fileName = fileName.Substring(1);
                }

                // Add the entry to the file file
                ZipEntry entry = new ZipEntry(fileName);
                zipStream.PutNextEntry(entry);
                byte[] buffer = new byte[8182];

                // Add the actual file - just tranfer the data from one stream to another
                FileStream inputStream = fileInfo.OpenRead();
                try
                {
                    int dataLength = 1;
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
