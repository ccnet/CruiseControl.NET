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
                    Directory.CreateDirectory(value);
				directory = value;
			}
		}

		public bool HasPreviousState(string project)
		{
			return fileSystem.FileExists(GetFilePath(project));
		}
		
		public IIntegrationResult LoadState(string project)
		{
			TextReader stateFileReader = GetStateFileReader(project);
			return LoadState(stateFileReader);
		}

		public IIntegrationResult LoadState(TextReader stateFileReader)
		{
			XmlSerializer serializer = new XmlSerializer(typeof (IntegrationResult));
			try
			{
				return (IntegrationResult) serializer.Deserialize(stateFileReader);
			}
			catch (Exception e)
			{
				throw new CruiseControlException(
					string.Format("Unable to read the specified saved state.  The configuration data may be invalid."), e);
			}
		}

		private TextReader GetStateFileReader(string project)
		{
			string stateFilePath = GetFilePath(project);
			try
			{
				return fileSystem.Load(stateFilePath);
			}
			catch (Exception e)
			{
				throw new CruiseControlException(string.Format("Unable to read the specified state file: {0}.  The path may be invalid.", stateFilePath), e);				
			}
		}

        /// <summary>
        /// Write the state to disk, ensuring that it gets there in its entirety.
        /// </summary>
        /// <param name="result">The integration to save the state for.</param>
		public void SaveState(IIntegrationResult result)
		{
			XmlSerializer serializer = new XmlSerializer(typeof (IntegrationResult));
			StringWriter buffer = new Utf8StringWriter();
			serializer.Serialize(buffer, result);

			string path = GetFilePath(result.ProjectName);
			try
			{
				fileSystem.AtomicSave(path, buffer.ToString());
			}
			catch (SystemException e)
			{
				throw new CruiseControlException(
					string.Format("Unable to save the IntegrationResult to the specified directory: {0}{1}{2}",
                        path, Environment.NewLine, buffer),
                    e);
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