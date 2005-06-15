using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.State
{
	[ReflectorType("state")]
	public class FileStateManager : IStateManager
	{
		private string directory = Directory.GetCurrentDirectory();

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
			if (! File.Exists(stateFilePath)) return IntegrationResult.CreateInitialIntegrationResult(project, null);

			XmlSerializer serializer = new XmlSerializer(typeof (IntegrationResult));
			using (TextReader reader = CreateTextReader(stateFilePath))
			{
				return (IntegrationResult) serializer.Deserialize(reader);
			}
		}

		public void SaveState(IIntegrationResult result)
		{
			XmlSerializer serializer = new XmlSerializer(typeof (IntegrationResult));
			StringWriter buffer = new StringWriter();
			serializer.Serialize(buffer, result);

			using (TextWriter writer = CreateTextWriter(GetFilePath(result.ProjectName)))
			{
				writer.Write(buffer.ToString());
				writer.Flush();
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

		private TextReader CreateTextReader(string path)
		{
			try
			{
				return new StreamReader(path);
			}
			catch (IOException ex)
			{
				throw new CruiseControlException(string.Format("Unable to read the specified state file: {0}.  The path may be invalid.", path), ex);
			}
		}

		private TextWriter CreateTextWriter(string path)
		{
			try
			{
				return new StreamWriter(path, false, Encoding.Unicode);
			}
			catch (SystemException ex)
			{
				throw new CruiseControlException(string.Format("Unable to save the IntegrationResult to the specified directory: {0}", path), ex);
			}
		}
	}
}