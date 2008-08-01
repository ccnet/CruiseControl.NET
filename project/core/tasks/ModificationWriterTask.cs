using System.IO;
using System.Xml.Serialization;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    [ReflectorType("modificationWriter")]
    public class ModificationWriterTask : ITask
    {
        private readonly IFileSystem fileSystem;

        public ModificationWriterTask()
            : this(new SystemIoFileSystem())
        { }

        public ModificationWriterTask(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public void Run(IIntegrationResult result)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Modification[]));
            StringWriter writer = new Utf8StringWriter();
            serializer.Serialize(writer, result.Modifications);
            fileSystem.Save(ModificationFile(result), writer.ToString());
        }

        private string ModificationFile(IIntegrationResult result)
        {
            if (AppendTimeStamp)
            {
                System.IO.FileInfo fi = new FileInfo(Filename);
                string dummy = Filename.Remove(Filename.Length - fi.Extension.Length , fi.Extension.Length);
                string newFileName = string.Format("{0}_{1}{2}",dummy,result.StartTime.ToString("yyyyMMddHHmmssfff"),fi.Extension);



                return Path.Combine(result.BaseFromArtifactsDirectory(OutputPath), newFileName);                
            }
            else
            {
                return Path.Combine(result.BaseFromArtifactsDirectory(OutputPath), Filename);
            }
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

        /// <summary>
        /// Append the buildStartdate to the filename, and so prevent overwriting the file with other builds
        /// To be used in conjunction with the ModificationReaderTask <see cref="ModificationReaderTask"/> if set to true
        /// </summary>
        [ReflectorProperty("appendTimeStamp", Required = false)]
        public bool AppendTimeStamp = false;
    }
}