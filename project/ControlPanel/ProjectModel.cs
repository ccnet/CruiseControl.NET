using System;
using System.Collections;
using System.Reflection;

using NUnit.Framework;
using Exortech.NetReflector;

using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Label;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.ControlPanel
{
	public class ProjectModel 
	{
		private ConfigurationModel _configuration;
		private string _name;
		private ConfigurationItemCollection _items = new ConfigurationItemCollection();

		public ProjectModel(ConfigurationModel configuration, IProject project) 
		{
			_configuration = configuration;
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

		public void Save()
		{
			_configuration.Save();
		}
	}
}
