using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.config
{
	public class SimplePluginSpecification : IPluginSpecification
	{
		private readonly string typeName;

		public SimplePluginSpecification (string typeName)
		{
			this.typeName = typeName;
		}

		public string TypeName
		{
			get { return typeName; }
		}

		public Type Type
		{
			get { return Type.GetType(typeName); }
		}
	}
}
