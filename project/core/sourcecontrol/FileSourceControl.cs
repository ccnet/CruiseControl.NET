using System;
using System.Collections.Generic;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// Use the 'Filesystem' Source Control plugin to check for modifications on a directory accessible by the build server. A file is
    /// considered modified if the file's modified time stamp is more recent than the last time CruiseControl.Net checked for modifications.
    /// You can use either directories on 'mapped' drives (local or remote), or UNC paths (remote).
    /// </summary>
    /// <title>Filesystem Source Control Block</title>
    /// <version>1.0</version>
    /// <key name="type">
    /// <description>The type of source control block.</description>
    /// <value>filesystem</value>
    /// </key>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;sourcecontrol type="filesystem"&gt;
    /// &lt;repositoryRoot&gt;c:\mycode&lt;/repositoryRoot&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;sourcecontrol type="filesystem"&gt;
    /// &lt;repositoryRoot&gt;c:\mycode&lt;/repositoryRoot&gt;
    /// &lt;autoGetSource&gt;true&lt;/autoGetSource&gt;
    /// &lt;ignoreMissingRoot&gt;false&lt;/ignoreMissingRoot&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// </example>
    [ReflectorType("filesystem")]
    public class FileSourceControl 
        : SourceControlBase
    {
        private readonly IFileSystem fileSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSourceControl" /> class.	
        /// </summary>
        /// <remarks></remarks>
        public FileSourceControl()
            : this(new SystemIoFileSystem())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSourceControl" /> class.	
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <remarks></remarks>
        public FileSourceControl(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
            CheckRecursively = true;
        }

        /// <summary>
        /// The directory to check for changes. This directory will be checked recursively. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("repositoryRoot")]
        public string RepositoryRoot { get; set; }

        /// <summary>
        /// Whether to automatically (recursively) copy the contents of the repositoryRoot directory to the Project Working Directory.
        /// </summary>
        /// <version>1.0</version>
        /// <default>false</default>
        [ReflectorProperty("ignoreMissingRoot", Required = false)]
        public bool IgnoreMissingRoot { get; set; }

        /// <summary>
        /// Whether to not fail if the repository doesn't exist.
        /// </summary>
        /// <version>1.0</version>
        /// <default>false</default>
        [ReflectorProperty("autoGetSource", Required = false)]
        public bool AutoGetSource { get; set; }

        /// <summary>
        /// Whether or not to check recursively for changes inside repositoryRoot.
        /// </summary>
        /// <version>1.6</version>
        /// <default>true</default>
        [ReflectorProperty("checkRecursively", Required = false)]
        public bool CheckRecursively { get; set; }

        /// <summary>
        /// Gets the modifications.	
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
        {
            DirectoryInfo root = new DirectoryInfo(from.BaseFromWorkingDirectory(RepositoryRoot));
            var modifications = GetMods(root, from.StartTime);
            return modifications.ToArray();
        }

        private List<Modification> GetMods(DirectoryInfo dir, DateTime from)
        {
            var mods = new List<Modification>();
            try
            {
                foreach (FileInfo file in dir.GetFiles())
                {
                    if (IsLocalFileChanged(file, from))
                    {
                        mods.Add(CreateModification(file));
                    }
                }

                foreach (DirectoryInfo sub in dir.GetDirectories())
                {
                    if (IsLocalFileChanged(sub, from))
                    {
                        mods.Add(CreateModification(sub));
                    }

                    if (CheckRecursively)
                    {
                        mods.AddRange(GetMods(sub, from));
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
                if (!IgnoreMissingRoot)
                {
                    throw;
                }
            }

            return mods;
        }

        private Modification CreateModification(FileInfo info)
        {
            Modification result = CreateModificationBase(info);
            result.FolderName = info.DirectoryName;
            return result;
        }

        private Modification CreateModification(DirectoryInfo info)
        {
            Modification result = CreateModificationBase(info);
            result.FolderName = info.Name;
            return result;
        }

        private Modification CreateModificationBase(FileSystemInfo info)
        {
            Modification modification = new Modification();
            modification.FileName = info.Name;

            if (info.CreationTime > info.LastWriteTime)
            {
                modification.ModifiedTime = info.CreationTime;
            }
            else
            {
                modification.ModifiedTime = info.LastWriteTime;
            }

            return modification;
        }

        private bool IsLocalFileChanged(FileSystemInfo reposFile, DateTime date)
        {
            bool result = false;

            if (reposFile.LastWriteTime > date) result = true;
            if (reposFile.CreationTime > date) result = true;

            return result;
        }

        /// <summary>
        /// Labels the source control.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
        public override void LabelSourceControl(IIntegrationResult result)
        { }

        /// <summary>
        /// Gets the source.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
        public override void GetSource(IIntegrationResult result)
        {
            result.BuildProgressInformation.SignalStartRunTask("Getting source from FileSourceControl");

            if (AutoGetSource)
                fileSystem.Copy(RepositoryRoot, result.WorkingDirectory);
        }

        /// <summary>
        /// Initializes the specified project.	
        /// </summary>
        /// <param name="project">The project.</param>
        /// <remarks></remarks>
        public override void Initialize(IProject project)
        { }

        /// <summary>
        /// Purges the specified project.	
        /// </summary>
        /// <param name="project">The project.</param>
        /// <remarks></remarks>
        public override void Purge(IProject project)
        { }
    }
}