using System.IO;
using System.Text;
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
			StringWriter writer = new Utf8StringWriter();
			serializer.Serialize(writer, result.Modifications);
			fileSystem.Save(ModificationFile(result), writer.ToString());
		}

		private class Utf8StringWriter : StringWriter
		{
			public override Encoding Encoding
			{
				get { return Encoding.UTF8; }
			}
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