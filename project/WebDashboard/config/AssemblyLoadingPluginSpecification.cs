using System;
using System.Reflection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Config
{
	public class AssemblyLoadingPluginSpecification : IPluginSpecification
	{
		private readonly string assemblyFileName;
		private readonly string typeName;

		public string TypeName
		{
			get { return typeName; }
		}

		public Type Type
		{
			get
			{
				return Assembly.LoadFrom(assemblyFileName).GetType(typeName);
			}
		}

		public AssemblyLoadingPluginSpecification(string typeName, string assemblyFileName)
		{
			this.typeName = typeName;
			this.assemblyFileName = assemblyFileName;
		}
	}
}
