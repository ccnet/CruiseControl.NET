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
	public class ConfigurationModel
	{
		private ConfigurationItemCollection _items = new ConfigurationItemCollection();

		public void Load(IProject project) 
		{
			Items.LoadPropertiesOf(project);
		}

		public ConfigurationItemCollection Items
		{
			get { return _items; }
		}
	}
}
