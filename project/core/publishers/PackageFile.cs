namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    using System.Collections.Generic;
    using System.IO;
    using Exortech.NetReflector;
    using ICSharpCode.SharpZipLib.Zip;
    using ThoughtWorks.CruiseControl.Core.Util;

    /// <summary>
    /// A file to include in the package.
    /// </summary>
    [ReflectorType("packageFile")]
    public class PackageFile 
        : IPackageItem
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PackageFile"/> class.
        /// </summary>
        public PackageFile()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageFile"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public PackageFile(string fileName)
        {
            this.SourceFile = fileName;
        }
        #endregion

        /// <summary>
        /// The name and path of the file to store into the package
        /// </summary>
        /// <remarks>
        /// This is the path to the file that you wish to store in the package
        /// </remarks>
        [ReflectorProperty("sourceFile", Required = true)]
        public string SourceFile { get; set; }

        /// <summary>
        /// The name of the file that is to be saved. 
        /// </summary>
        /// <remarks>
        /// Use this attribute only if you wish to rename the file
        /// being saved to a different name.
        /// </remarks>
        [ReflectorProperty("targetFileName", Required = false)]
        public string TargetFileName { get; set; }

        /// <summary>
        /// The name of the folder in the package that the file will be saved under
        /// </summary>
        /// <remarks>
        /// Use this attribute if you wish to override the location of the file being saved in 
        /// the package.
        /// </remarks>
        [ReflectorProperty("targetFolder", Required = false)]
        public string TargetFolder { get; set; }

        /// <summary>
        /// Packages the specified items.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="zipStream">The zip stream.</param>
        /// <returns>The name of the files that were packaged.</returns>
        public IEnumerable<string> Package(IIntegrationResult result, ZipOutputStream zipStream)
        {
            var baseFolder = result.WorkingDirectory;
            var files = this.GenerateFileList(result);
            var actualFiles = new List<string>();
            foreach (var fullName in files)
            {
                var fileInfo = new FileInfo(fullName);
                if (fileInfo.Exists)
                {
                    // Generate the name of the file to store in the package
                    string targetFileName = string.IsNullOrEmpty(this.TargetFileName) ?
                        fileInfo.Name :
                        this.TargetFileName;
                    string targetPath = string.IsNullOrEmpty(this.TargetFolder) ?
                        fileInfo.DirectoryName.Substring(baseFolder.Length) :
                        this.TargetFolder;

                    if (targetPath.StartsWith(Path.DirectorySeparatorChar + string.Empty)) targetPath = targetPath.Substring(1);

                    // Add the entry to the file file
                    var entry = new ZipEntry(ZipEntry.CleanName(Path.Combine(targetPath, targetFileName)));
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
                    actualFiles.Add(fullName);
                }
            }

            return actualFiles;
        }

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
            var fileList = new List<string>();
            var allDirsWildcard = Path.DirectorySeparatorChar + "**" + Path.DirectorySeparatorChar;

                // Handle any wildcard characters
            if (this.SourceFile.Contains("*") || this.SourceFile.Contains("?"))
            {
                var possibilities = new List<string>();
                var actualPath = Path.IsPathRooted(this.SourceFile) ? this.SourceFile : result.BaseFromWorkingDirectory(this.SourceFile);

                // Check for **, if it exists, then split the search pattern and use the second half to find all
                // matching files
                if (actualPath.Contains(allDirsWildcard))
                {
                    var position = actualPath.IndexOf(allDirsWildcard);
                    var path = actualPath.Substring(0, position);
                    var pattern = actualPath.Substring(position + 4);
                    possibilities.AddRange(Directory.GetFiles(path, pattern, SearchOption.AllDirectories));
                }
                else
                {
                    // Otherwise, just use the plain ordinary search pattern
                    var position = actualPath.IndexOfAny(new char[] { '*', '?' });
                    position = actualPath.LastIndexOf(Path.DirectorySeparatorChar, position);
                    var path = actualPath.Substring(0, position);
                    var pattern = actualPath.Substring(position + 1);
                    possibilities.AddRange(Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly));
                }

                // The current list of files is just a set of possibilities, now need to check that they completely
                // match the search criteria and they have not already been added.
                foreach (var possibility in possibilities)
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
                string actualPath = Path.IsPathRooted(this.SourceFile) ? this.SourceFile : result.BaseFromWorkingDirectory(this.SourceFile);

                // Only add it to the list if it doesn't already exist
                if (!fileList.Contains(actualPath))
                {
                    fileList.Add(actualPath);
                }
            }

            return fileList;
        }
        #endregion
    }
}
