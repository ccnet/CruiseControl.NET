using System;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.WebDashboard.Config
{
	public class SimplePluginSpecification : IPluginSpecification
	{
		private readonly string typeName;

		public SimplePluginSpecification (string typeName)
		{
			this.typeName = typeName;
			if (Type.GetType(typeName) == null)
			{
				throw new CruiseControlException("Unable to get Type for type name [" + typeName + "]");
			}
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
