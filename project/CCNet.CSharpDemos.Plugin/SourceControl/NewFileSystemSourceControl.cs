namespace CCNet.CSharpDemos.Plugin.SourceControl
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core;

    [ReflectorType("fileSystemSource")]
    public class NewFileSystemSourceControl
        : ISourceControl
    {
        private Dictionary<string, DateTime> files = new Dictionary<string, DateTime>();

        [ReflectorProperty("directory")]
        public string DirectoryName { get; set; }

        public Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
        {
            var newList = new Dictionary<string, DateTime>();
            var modifications = new List<Modification>();
            var directory = new DirectoryInfo(this.DirectoryName);
            var newFiles = directory.GetFiles();
            foreach (var file in newFiles)
            {
                var inList = this.files.ContainsKey(file.FullName);
                if (!inList ||
                    (this.files[file.FullName] < file.LastWriteTime))
                {
                    newList.Add(file.FullName, file.LastWriteTime);
                    var modification = new Modification
                    {
                        FileName = file.Name,
                        ModifiedTime = file.LastWriteTime,
                        FolderName = file.DirectoryName,
                        Type = inList ? "Added" : "Modified"
                    };
                    modifications.Add(modification);
                }

                if (inList)
                {
                    this.files.Remove(file.Name);
                }
            }

            foreach (var file in this.files.Keys)
            {
                var modification = new Modification
                {
                    FileName = Path.GetFileName(file),
                    ModifiedTime = DateTime.Now,
                    FolderName = Path.GetDirectoryName(file),
                    Type = "Deleted"
                };
                modifications.Add(modification);
            }

            this.files = newList;
            return modifications.ToArray();
        }

        public void LabelSourceControl(IIntegrationResult result)
        {
            var fileName = Path.Combine(
                this.DirectoryName,
                DateTime.Now.ToString("yyyyMMddHHmmss") + ".label");
            File.WriteAllText(fileName, result.Label);
            this.files.Add(fileName, DateTime.Now);
        }

        public void GetSource(IIntegrationResult result)
        {
            foreach (var modification in result.Modifications)
            {
                var source = Path.Combine(
                    modification.FolderName,
                    modification.FileName);
                var destination = result.BaseFromWorkingDirectory(
                    modification.FileName);
                if (File.Exists(source))
                {
                    File.Copy(source, destination, true);
                }
                else
                {
                    File.Delete(destination);
                }
            }
        }

        public void Initialize(IProject project)
        {
            // Not needed
        }

        public void Purge(IProject project)
        {
            // Not needed
        }
    }
}
