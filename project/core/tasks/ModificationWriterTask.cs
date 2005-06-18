using System.IO;
using System.Xml.Serialization;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.tasks
{
	[ReflectorType("modificationWriter")]
	public class ModificationWriterTask : ITask
	{
		private readonly IFileSystem fileSystem;

		public ModificationWriterTask() : this(new SystemIoFileSystem())
		{}

		public ModificationWriterTask(IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;
		}

		public void Run(IIntegrationResult result)
		{
			XmlSerializer serializer = new XmlSerializer(typeof (Modification[]));
			StringWriter writer = new StringWriter();
			serializer.Serialize(writer, result.Modifications);
			fileSystem.Save(ModificationFile(result), writer.ToString());
		}

		private string ModificationFile(IIntegrationResult result)
		{
			return Path.Combine(result.BaseFromArtifactsDirectory(OutputPath), Filename);
		}

		[ReflectorProperty("filename", Required=false)]
		public string Filename = "modifications.xml";

		[ReflectorProperty("path", Required=false)]
		public string OutputPath;
	}
}