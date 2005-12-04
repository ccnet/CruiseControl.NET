using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.State
{
	[ReflectorType("state")]
	public class FileStateManager : IStateManager
	{
		private readonly IFileSystem fileSystem;
		private string directory = Directory.GetCurrentDirectory();

		public FileStateManager() : this(new SystemIoFileSystem())
		{}

		public FileStateManager(IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;
		}

		[ReflectorProperty("directory", Required=false)]
		public string StateFileDirectory
		{
			get { return directory; }
			set
			{
				if (! Directory.Exists(value))
					throw new CruiseControlException(string.Format("State file directory: {0} does not exist.", value));
				directory = value;
			}
		}

		public IIntegrationResult LoadState(string project)
		{
			string stateFilePath = GetFilePath(project);
			if (! fileSystem.FileExists(stateFilePath)) return IntegrationResult.CreateInitialIntegrationResult(project, null);

			XmlSerializer serializer = new XmlSerializer(typeof (IntegrationResult));
			try
			{
				return (IntegrationResult) serializer.Deserialize(fileSystem.Load(stateFilePath));
			}
			catch (Exception e)
			{
				throw new CruiseControlException(
					string.Format("Unable to read the specified state file: {0}.  The path may be invalid.", stateFilePath), e);
			}
		}

		public void SaveState(IIntegrationResult result)
		{
			XmlSerializer serializer = new XmlSerializer(typeof (IntegrationResult));
			StringWriter buffer = new Utf8StringWriter();
			serializer.Serialize(buffer, result);

			string path = GetFilePath(result.ProjectName);
			try
			{
				fileSystem.Save(path, buffer.ToString());
			}
			catch (SystemException e)
			{
				throw new CruiseControlException(
					string.Format("Unable to save the IntegrationResult to the specified directory: {0}{1}{2}", path, Environment.NewLine, buffer.ToString()), e);
			}
		}

		private string GetFilePath(string project)
		{
			return Path.Combine(StateFileDirectory, StateFilename(project));
		}

		private string StateFilename(string project)
		{
			StringBuilder strBuilder = new StringBuilder();
			foreach (string token in project.Split(' '))
			{
				strBuilder.Append(token.Substring(0, 1).ToUpper());
				if (token.Length > 1)
				{
					strBuilder.Append(token.Substring(1));
				}
			}
			return strBuilder.Append(".state").ToString();
		}
	}
}