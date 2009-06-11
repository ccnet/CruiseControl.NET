using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// Reads modifications from file back into the current integration result
    /// </summary>
    [ReflectorType("modificationReader")]
    public class ModificationReaderTask
        : TaskBase, ITask
    {
        private readonly IFileSystem fileSystem;
        private bool deleteAfterRead = false;

        public ModificationReaderTask()
            : this(new SystemIoFileSystem()){ }

        public ModificationReaderTask(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        [ReflectorProperty("deleteAfterRead", Required = false)]
        public bool DeleteAfterRead
        {
            get { return deleteAfterRead; }
            set { deleteAfterRead = value; }
        }

        public void Run(IIntegrationResult result)
        {
            List<string> filesToDelete = new List<string>();
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description : "Reading Modifications");                


			List<object> stuff = new List<object>();
        	System.Collections.ArrayList AllModifications = new System.Collections.ArrayList();
            

            foreach (string file in GetModificationFiles(result))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Modification[]));
                StringReader reader = new StringReader(fileSystem.Load(file).ReadToEnd());
                object dummy = serializer.Deserialize(reader);
                reader.Close();
                System.Collections.ArrayList currentModification = new System.Collections.ArrayList((Modification[])dummy);

                AllModifications.AddRange(currentModification);

                if (deleteAfterRead) filesToDelete.Add(file);
            }
            
            Modification[] newMods = new Modification[result.Modifications.Length + AllModifications.Count];

            //copy existing modifications
            result.Modifications.CopyTo(newMods, 0);

            // copy modifications read from the file(s)
            int modificationCounter = result.Modifications.Length;
            foreach (Modification mod in AllModifications)
            {
                newMods[modificationCounter] = mod;
                modificationCounter++;
            }

            result.Modifications = newMods;

            // Delete all the files
            foreach (string file in filesToDelete)
            {
                try
                {
                    File.Delete(file);
                }
                catch (IOException error)
                {
                    Log.Warning(
                        string.Format(
                            "Unable to delete file '{0}' - {1}",
                            file,
                            error.Message));
                }
            }
        }

        private string[] GetModificationFiles(IIntegrationResult result)
        {
            FileInfo fi = new FileInfo(Path.Combine(result.BaseFromArtifactsDirectory(OutputPath), Filename));
            string filespec = fi.Name.Remove(fi.Name.Length - fi.Extension.Length) + "*" + fi.Extension;
            return Directory.GetFiles(fi.DirectoryName,filespec);        
        }

        /// <summary>
        /// The fileName to use to store the modifications
        /// </summary>
        [ReflectorProperty("filename", Required = false)]
        public string Filename = "modifications.xml";

        /// <summary>
        /// Path of the file
        /// </summary>
        [ReflectorProperty("path", Required = false)]
        public string OutputPath;
    }
}
