using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

using NUnit.Framework;
using Exortech.NetReflector;

using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Label;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.ControlPanel
{
	public class ConfigurationModel
	{
		private ProjectItem [] _projects;
		private IConfiguration _configuration;

		public void Load(string filename) 
		{
			using (ConfigurationLoader loader = new ConfigurationLoader(filename)) 
			{
				Load(loader.Load());
			}
		}

		public void Save(string filename) 
		{
			using (StreamWriter streamWriter = new StreamWriter(filename)) {
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
				projects.Add(new ProjectItem(project));
			}
			_projects = (ProjectItem []) projects.ToArray(typeof(ProjectItem));
		}

		public ProjectItem [] Projects
		{
			get { return _projects; }
		}
	}

	public class ProjectItem 
	{
		private string _name;
		private ConfigurationItemCollection _items = new ConfigurationItemCollection();

		public ProjectItem(IProject project) 
		{
			_name = project.Name;
			_items.LoadPropertiesOf(project);
		}

		public string Name 
		{
			get { return _name; }
		}

		public ConfigurationItemCollection Items
		{
			get { return _items; }
		}
	}
}
