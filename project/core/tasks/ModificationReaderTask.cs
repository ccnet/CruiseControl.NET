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
    public class ModificationReaderTask : ITask
    {
        private readonly IFileSystem fileSystem;

        public ModificationReaderTask()
            : this(new SystemIoFileSystem()){ }

        public ModificationReaderTask(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        /// <summary>
        /// Description used for the visualisation of the buildstage, if left empty the process name will be shown
        /// </summary>
        [ReflectorProperty("description", Required = false)]
        public string Description = string.Empty;


        public void Run(IIntegrationResult result)
        {
            result.BuildProgressInformation.SignalStartRunTask(Description != string.Empty ? Description : "Reading Modifications");                


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
