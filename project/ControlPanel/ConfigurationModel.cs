using System.Collections;
using System.IO;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;

namespace ThoughtWorks.CruiseControl.ControlPanel
{
	public class ConfigurationModel
	{
		private string _filename;
		private ProjectModel [] _projects;
		private IConfiguration _configuration;

		public void Load(string filename) 
		{
			DefaultConfigurationFileLoader fileLoader = new DefaultConfigurationFileLoader(); 
			Load(fileLoader.Load(new FileInfo(filename)));
			_filename = filename;
		}

		public void Save() 
		{
			Save(_filename);
		}

		public void Save(string filename) 
		{
			string tmpFilename = filename + ".tmp";

			DoSave(tmpFilename);
			CheckIfFileIsLoadable(tmpFilename);
			
			if (File.Exists(filename)) File.Delete(filename);
			File.Move(tmpFilename, filename);			
		}

		private void CheckIfFileIsLoadable(string filename)
		{
			try 
			{
				new DefaultConfigurationFileLoader().Load(new FileInfo(filename));
			} 
			catch (ConfigurationException e) 
			{
				throw new ConfigurationException("couldn't save because state is invalid :", e);
			}
		}

		private void DoSave(string filename)
		{
			using (StreamWriter streamWriter = new StreamWriter(filename)) 
			{
				XmlTextWriter writer = new XmlTextWriter(streamWriter);
			
				writer.Formatting = Formatting.Indented;
				writer.Indentation = 4;
				writer.IndentChar = ' ';
				writer.QuoteChar = '\'';
				
				writer.WriteStartElement("cruisecontrol");
				foreach (IProject project in _configuration.Projects)
				{
					NetReflector.Write(writer, project);
				}
				writer.WriteEndElement();
			}
		}

		public void Load(IConfiguration configuration) 
		{
			_configuration = configuration;
			ArrayList projects = new ArrayList();
			foreach (IProject project in configuration.Projects) 
			{
				projects.Add(new ProjectModel(this, project));
			}
			_projects = (ProjectModel []) projects.ToArray(typeof(ProjectModel));
		}

		public ConfigurationTreeNode GetNavigationTreeNodes()
		{
			ConfigurationTreeNode baseNode = new ConfigurationTreeNode(_filename, this);

			foreach (ProjectModel project in Projects) 
			{
				ConfigurationItemTreeNode projectNode = new ConfigurationItemTreeNode(project, project, project.Items.ThatCanNotHaveChildren());
				baseNode.Nodes.Add(projectNode);

				foreach (ConfigurationItem item in project.Items.ThatCanHaveChildren())
				{
					ConfigurationItemTreeNode itemNode = new ConfigurationItemTreeNode(project, item, new ConfigurationItem [] {item});
					projectNode.Nodes.Add(itemNode);
				}
			}

			return baseNode;
		}

		public ProjectModel [] Projects
		{
			get { return _projects; }
		}
	}
}
