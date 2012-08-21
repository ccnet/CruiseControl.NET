using System;
using System.Collections.Generic;
using System.IO;
using Exortech.NetReflector;
using ICSharpCode.SharpZipLib.Zip;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    /// <summary>
    /// A folder to include in the package	
    /// </summary>
    [ReflectorType("packageFolder")]
    public class PackageFolder
        : IPackageItem
    {
        /// <summary>
        /// The name of the folder to store into the package
        /// </summary>
        /// <remarks>
        /// This is the path to the folder that you wish to store in the package
        /// </remarks>
        [ReflectorProperty("sourceFolder", Required = true)]
        public string SourceFolder
        {
            get;
            set;
        }

        /// <summary>
        /// The filename filter to apply
        /// </summary>
        /// <remarks>
        /// Use this attribute to filter files that will be stored into the package. For example <code type="None">*.*</code> 
        /// will select all files (default), <code type="None">*.xml</code> will only select xml files, <code type="None">test*.xml</code> will select 
        /// only files that start with the word test and have an extension of xml.
        /// </remarks>
        [ReflectorProperty("fileFilter", Required = true)]
        public string FileFilter
        {
            get;
            set;
        }

        /// <summary>
        /// The name of the folder in the package that the file will be saved under
        /// </summary>
        /// <remarks>
        /// Use this attribute if you wish to override the location of the files being saved in 
        /// the package.
        /// </remarks>
        [ReflectorProperty("targetFolder", Required = false)]
        public string TargetFolder
        {
            get;
            set;
        }

        /// <summary>
        /// Recursively save files
        /// </summary>
        /// <remarks>
        /// Use this attribute if you wish to recursively add files to the package
        /// </remarks>
        [ReflectorProperty("includeSubFolders", Required = false)]
        public bool IncludeSubFolders
        {
            get;
            set;
        }

        /// <summary>
        /// Flatten the hierachy
        /// </summary>
        /// <remarks>
        /// Use this attribute if you wish to save the files without folder information
        /// </remarks>
        [ReflectorProperty("flatten", Required = false)]
        public bool Flatten
        {
            get;
            set;
        }

        /// <summary>
        /// Base folder to strip out of the file names (if flatten is false)
        /// </summary>
        /// <default>Working Directory for the project</default>
        [ReflectorProperty("baseFolder", Required = false)]
        public string BaseFolder
        {
            get;
            set;
        }

        /// <summary>
        /// Packages the specified items.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="zipStream">The zip stream.</param>
        /// <returns>The name of the files that were packaged.</returns>
        public IEnumerable<string> Package(IIntegrationResult result, ZipOutputStream zipStream)
        {
            var filesAdded = new List<string>();

            string effectiveBaseFolder;
            if (BaseFolder == null)
            {
                effectiveBaseFolder = result.WorkingDirectory;
            }
            else
            {
                // If base is relative, make it relative to working directory and ensure we convert to absolute path
                effectiveBaseFolder = Path.IsPathRooted(this.BaseFolder) ? this.BaseFolder : Path.GetFullPath(result.BaseFromWorkingDirectory(this.BaseFolder));
            }

            var fullName = Path.IsPathRooted(this.SourceFolder) ? this.SourceFolder : result.BaseFromWorkingDirectory(this.SourceFolder);
            var folderInfo = new DirectoryInfo(fullName);
            if (folderInfo.Exists)
            {
                // Process files in the directory
                ProcessFolder(effectiveBaseFolder, this.TargetFolder, folderInfo, ref filesAdded, ref zipStream);
            }
            return filesAdded;
        }

        /// <summary>
        /// Rursive function to process a directory tree, adding found files to an output zip stream.
        /// </summary>
        /// <param name="effectiveBaseFolder">Base folder in which package files are built from.</param>
        /// <param name="folder">Folder in which files will be added to zip.</param>
        /// <param name="filesAdded">List of files added to zip.</param>
        /// <param name="zipStream">The zip stream.</param>
        /// <returns>The name of the files that were packaged.</returns>
        void ProcessFolder(string effectiveBaseFolder, string effectiveTargetFolder, DirectoryInfo folder, ref List<string> filesAdded, ref ZipOutputStream zipStream)
        {
            // Recursively process sub folders if requested
            if (this.IncludeSubFolders)
            {
                foreach (DirectoryInfo currentFolder in folder.GetDirectories())
                {
                    string subTargetFolder = string.Empty;
                    if (!string.IsNullOrEmpty(effectiveTargetFolder))
                    {
                        subTargetFolder = Path.Combine(effectiveTargetFolder, currentFolder.Name);
                    }
                    ProcessFolder(effectiveBaseFolder, subTargetFolder, currentFolder, ref filesAdded, ref zipStream);
                }
            }

            foreach (FileInfo file in folder.GetFiles(string.IsNullOrEmpty(this.FileFilter) ? "*.*" : this.FileFilter, SearchOption.TopDirectoryOnly))
            {
                // Generate the name of the file to store in the package
                string fileName = file.FullName;
                if (this.Flatten)
                {
                    // store the file at the root of the package
                    fileName = file.Name;
                }
                else if (!string.IsNullOrEmpty(effectiveTargetFolder))
                {
                    // store the file with the new path
                    fileName = Path.Combine(effectiveTargetFolder, file.Name);
                }
                else
                {
                    // store the file with the path minus the baseFolder
                    if (fileName.StartsWith(effectiveBaseFolder, StringComparison.OrdinalIgnoreCase))
                    {
                        fileName = fileName.Substring(effectiveBaseFolder.Length);
                    }
                }
                if (fileName.StartsWith(Path.DirectorySeparatorChar + string.Empty))
                {
                    fileName = fileName.Substring(1);
                }

                // Add the entry to the file file
                var entry = new ZipEntry(ZipEntry.CleanName(fileName));
                entry.Size = file.Length;
                // zipentry date set to last changedate, other it contains not the right filedate.
                // added 10.11.2010 by rolf eisenhut
                entry.DateTime = file.LastWriteTime;
                zipStream.PutNextEntry(entry);
                var buffer = new byte[8182];

                // Add the actual file - just tranfer the data from one stream to another
                var inputStream = file.OpenRead();
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
                filesAdded.Add(fileName);
            }
        }
    }
}

