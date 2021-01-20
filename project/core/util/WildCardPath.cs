using System.IO;
using System.Collections.Generic;
using System;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// Folder wildcards like \**\ by Juan Pablo Garcia
    /// http://blogs.southworks.net/jpgarcia/2008/05/25/folder-wildcards-like-in-cruisecontrolnet/
    /// </summary>
    public class WildCardPath
    {
        private const string WildCardPattern = "**";
        private string PathPattern;

        /// <summary>
        /// Initializes a new instance of the <see cref="WildCardPath" /> class.	
        /// </summary>
        /// <param name="pathPattern">The path pattern.</param>
        /// <remarks></remarks>
        public WildCardPath(string pathPattern)
        {
            this.PathPattern = pathPattern;
        }

        /// <summary>
        /// Gets the files.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public FileInfo[] GetFiles()
        {
            FileInfo[] files = new FileInfo[0];
            if (this.HasWildCards(this.PathPattern))
            {
                if (this.HasFolderWildcards())
                {
                    string baseDirectory = this.PathPattern.Substring(0, this.PathPattern.IndexOf(WildCardPattern, StringComparison.OrdinalIgnoreCase));

                    List<string> baseDirectories = new List<string>();
                    List<string> tempBaseDirectories = new List<string>();
                    List<FileInfo> filesFound = new List<FileInfo>();

                    bool folderWildcardsMode = true;

                    baseDirectories.Add(baseDirectory);

                    int position = baseDirectory.Length + (Path.DirectorySeparatorChar + "**" + Path.DirectorySeparatorChar).Length - 1;

                    if (!this.IsNextLevelAFile(position))
                    {
                        while (!this.IsNextLevelAFile(position) || folderWildcardsMode)
                        {
                            string nextDirectory = string.Empty;

                            if (!this.IsNextLevelAFile(position))
                            {
                                nextDirectory = this.GetNextDirectoryName(position);
                            }
                            else
                            {
                                nextDirectory = "*";
                            }

                            foreach (string baseDir in baseDirectories)
                            {
                                tempBaseDirectories.Add(baseDir);
                            }

                            if (!this.IsNextLevelAFile(position))
                            {
                                baseDirectories.Clear();
                            }

                            foreach (string directory in tempBaseDirectories)
                            {
                                string[] directories = new List<string>().ToArray();

                                if (folderWildcardsMode)
                                {
                                    directories = Directory.GetDirectories(directory, nextDirectory, SearchOption.AllDirectories);
                                }
                                else
                                {
                                    directories = Directory.GetDirectories(directory, nextDirectory, SearchOption.TopDirectoryOnly);
                                }

                                foreach (string dir in directories)
                                {
                                    baseDirectories.Add(dir);
                                }
                            }

                            tempBaseDirectories.Clear();

                            position = position + nextDirectory.Length + 1;

                            if (this.IsNextLevelAFolderWildcard(position))
                            {
                                folderWildcardsMode = true;
                                position = position + (Path.DirectorySeparatorChar + "**" + Path.DirectorySeparatorChar).Length - 1;
                            }
                            else
                            {
                                folderWildcardsMode = false;
                            }
                        }
                    }
                    else
                    {
                        string[] directories = Directory.GetDirectories(baseDirectory, "*", SearchOption.AllDirectories);

                        foreach (string dir in directories)
                        {
                            baseDirectories.Add(dir);
                        }
                    }

                    foreach (string directory in baseDirectories)
                    {
                        string filePattern = Path.Combine(directory, this.PathPattern.Substring(position));
                        string dir = Path.GetDirectoryName(filePattern);
                        DirectoryInfo info = new DirectoryInfo(directory);
                        string pattern = Path.GetFileName(this.PathPattern);

                        if (info.Exists)
                        {
                            FileInfo[] tempFilesFound = info.GetFiles(pattern);
                            foreach (FileInfo file in tempFilesFound)
                            {
                                filesFound.Add(file);
                            }
                        }
                    }

                    return filesFound.ToArray();
                }
                else
                {
                    string dir = Path.GetDirectoryName(this.PathPattern);
                    DirectoryInfo info = new DirectoryInfo(dir);
                    string pattern = Path.GetFileName(this.PathPattern);

                    if (info.Exists)
                    {
                        files = info.GetFiles(pattern);
                    }

                    return files;
                }
            }
            else
            {
                files = new FileInfo[] { new FileInfo(this.PathPattern.Trim()) };
                return files;
            }
        }

        public bool IsNextLevelAFile(int position)
        {
            string[] dashs = this.PathPattern.Substring(position).Split(Path.DirectorySeparatorChar);

            if (dashs.Length == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetNextDirectoryName(int position)
        {
            return this.PathPattern.Substring(position, this.PathPattern.IndexOf(Path.DirectorySeparatorChar.ToString(), position, StringComparison.OrdinalIgnoreCase) - position);
        }

        public bool HasFolderWildcards()
        {
            return this.HasFolderWildcards(0);
        }

        public bool HasFolderWildcards(int position)
        {
            if (this.PathPattern.IndexOf(WildCardPattern, position, StringComparison.OrdinalIgnoreCase) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsNextLevelAFolderWildcard(int position)
        {
            if (this.PathPattern.Substring(position, 2) == WildCardPattern)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool HasWildCards(string file)
        {
            return file.IndexOf("*") > -1;
        }
    }
}