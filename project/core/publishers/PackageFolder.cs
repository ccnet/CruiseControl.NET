using System;
using System.Collections.Generic;
using System.IO;
using Exortech.NetReflector;
using ICSharpCode.SharpZipLib.Zip;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    /// <summary>
    /// 	
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
        /// Use this attribute to filter files that will be stored into the package. For example <code>*.*</code> 
        /// will select all files (default), <code>*.xml</code> will only select xml files, <code>test*.xml</code> will select 
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
        /// Packages the specified items.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="zipStream">The zip stream.</param>
        /// <returns>The name of the files that were packaged.</returns>
        public IEnumerable<string> Package(IIntegrationResult result, ZipOutputStream zipStream)
        {
            var filesAdded = new List<string>();
            var baseFolder = result.WorkingDirectory;
            var fullName = Path.IsPathRooted(this.SourceFolder) ? this.SourceFolder : result.BaseFromWorkingDirectory(this.SourceFolder);
            var folderInfo = new DirectoryInfo(fullName);
            if (folderInfo.Exists)
            {
                foreach (FileInfo file in folderInfo.GetFiles(string.IsNullOrEmpty(this.FileFilter) ? "*.*" : this.FileFilter, this.IncludeSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                {
                    // Generate the name of the file to store in the package
                    string fileName = file.FullName;
                    if (this.Flatten)
                    {
                        // store the file at the root of the package
                        fileName = file.Name;
                    }
                    else if (!string.IsNullOrEmpty(this.TargetFolder))
                    {
                        // store the file with the new path
                        fileName = Path.Combine(this.TargetFolder, file.Name);
                    }
                    else
                    {
                        // store the file with the path minus the baseFolder
                        if (fileName.StartsWith(baseFolder, StringComparison.OrdinalIgnoreCase))
                            fileName = fileName.Substring(baseFolder.Length);
                    }
                    if (fileName.StartsWith(Path.DirectorySeparatorChar + string.Empty)) fileName = fileName.Substring(1);

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
            return filesAdded;
        }
    }
}
