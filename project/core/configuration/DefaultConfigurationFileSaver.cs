using System.IO;
using System.Text;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Config
{
	public class DefaultConfigurationFileSaver : IConfigurationFileSaver
	{
		private readonly IProjectSerializer projectSerializer;

		public DefaultConfigurationFileSaver(IProjectSerializer projectSerializer)
		{
			this.projectSerializer = projectSerializer;
		}

		// ToDo - overwrites? Exceptions? Schema?
		public void Save(IConfiguration config, FileInfo configFile)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<cruisecontrol/>");

			StringBuilder concatenatedProjects = new StringBuilder();
			foreach (Project project in config.Projects)
			{
				concatenatedProjects.Append(projectSerializer.Serialize(project));
			}
			doc.DocumentElement.InnerXml = concatenatedProjects.ToString();

			using (StreamWriter fileWriter = new StreamWriter(configFile.FullName))
			{
				XmlTextWriter xmlTextWriter = new XmlTextWriter(fileWriter);
				doc.WriteTo(xmlTextWriter);
				xmlTextWriter.Flush();
			}
		}
	}
}
