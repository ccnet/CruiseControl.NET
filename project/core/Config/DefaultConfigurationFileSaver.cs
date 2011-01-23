using System.IO;
using System.Text;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Config
{
    /// <summary>
    /// 	
    /// </summary>
	public class DefaultConfigurationFileSaver : IConfigurationFileSaver
	{
		private readonly IProjectSerializer projectSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultConfigurationFileSaver" /> class.	
        /// </summary>
        /// <param name="projectSerializer">The project serializer.</param>
        /// <remarks></remarks>
		public DefaultConfigurationFileSaver(IProjectSerializer projectSerializer)
		{
			this.projectSerializer = projectSerializer;
		}

		// ToDo - overwrites? Exceptions? Schema?
        /// <summary>
        /// Saves the specified config.	
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="configFile">The config file.</param>
        /// <remarks></remarks>
		public void Save(IConfiguration config, FileInfo configFile)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<cruisecontrol/>");

			StringBuilder concatenatedProjects = new StringBuilder();
			foreach (IProject project in config.Projects)
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
