using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// <para>
    /// This tasks makes it possible to read back modifications made by the <link>Modification Writer Task</link>.
    /// </para>
    /// </summary>
    /// <title>Modification Reader Task</title>
    /// <version>1.4</version>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;rss /&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;rss items="30" /&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>
    /// 2 projects in CCNet
    /// </para>
    /// <para>
    /// 1) is a project that does the compile, test, ... stuff, and also writes the modifications using the ModificationWriterTask be sure to
    /// set the appendTimeStamp of the modificationWriter to true
    /// </para>
    /// <para>
    /// 2) is a project that deploys the result of project 1
    /// </para>
    /// <para>
    /// --&gt; copies it to other servers, updates source control (binary references like a framework), ...
    /// </para>
    /// <para>
    /// The reason for a second project is that this can be done on releases of milestones of project 1
    /// </para>
    /// <para>
    /// The ModificationReaderTask can now easily read the modification file(s) made by project one, into it's own integration, making it
    /// possible that these can be used by the existing tasks/publishers of ccnet for project 2
    /// </para>
    /// <para>
    /// It is best to place the modificationreader in the prebuild section, so all the other tasks / publisers know the read modifications
    /// also. 
    /// </para>
    /// <para>
    /// It is adivisable to keep these configuration elements of the modificationWriter and the modificationReader the same.  
    /// </para>
    /// </remarks>
    [ReflectorType("modificationReader")]
    public class ModificationReaderTask
        : TaskBase
    {
        private readonly IFileSystem fileSystem;
        private bool deleteAfterRead = false;

        public ModificationReaderTask()
            : this(new SystemIoFileSystem()){ }

        public ModificationReaderTask(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        /// <summary>
        /// Delete the files after they have been read. 
        /// </summary>
        /// <version>1.4</version>
        /// <default>false</default>
        [ReflectorProperty("deleteAfterRead", Required = false)]
        public bool DeleteAfterRead
        {
            get { return deleteAfterRead; }
            set { deleteAfterRead = value; }
        }

        protected override bool Execute(IIntegrationResult result)
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

            return true;
        }

        private string[] GetModificationFiles(IIntegrationResult result)
        {
            FileInfo fi = new FileInfo(Path.Combine(result.BaseFromArtifactsDirectory(OutputPath), Filename));
            string filespec = fi.Name.Remove(fi.Name.Length - fi.Extension.Length) + "*" + fi.Extension;
            return Directory.GetFiles(fi.DirectoryName,filespec);        
        }

        /// <summary>
        /// The filename pattern for the file containing the modifications. CCnet with search in the path for files starting with this
        /// filename, and having the same extention. For example when filename is set to modifications.xml, ccnet will search for files
        /// like so: modifications*.xml 
        /// </summary>
        /// <version>1.4</version>
        /// <default>modifications.xml</default>
        [ReflectorProperty("filename", Required = false)]
        public string Filename = "modifications.xml";

        /// <summary>
        /// The directory to search the xml file(s) in. 
        /// </summary>
        /// <version>1.4</version>
        /// <default>Project Artefact Directory</default>
        [ReflectorProperty("path", Required = false)]
        public string OutputPath;
    }
}
