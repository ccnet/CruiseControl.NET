using System;
using System.Reflection;
using Exortech.NetReflector;

namespace tw.ccnet.core
{
	[ReflectorType("assemblylabel")]
	public class AssemblyVersionLabeller : ILabeller
	{
		public enum VersionPart 
		{
			major, minor, build, revision
		}

		private VersionPart partToIncrement = VersionPart.revision;
		private string assemblyPath;

		[ReflectorProperty("assemblypath")]
		public string AssemblyPath
		{
			get { return assemblyPath; }
			set { assemblyPath = value; }
		}

		public VersionPart VersionPartToIncrement
		{
			get { return partToIncrement; }
			set { partToIncrement = value; }
		}

		public string Generate(IntegrationResult previousLabel) 
		{
			AssemblyName name = AssemblyName.GetAssemblyName(assemblyPath);
			Version version = name.Version;
			int major = partToIncrement == VersionPart.major ? version.Major + 1 : version.Major;
			int minor = partToIncrement == VersionPart.minor ? version.Minor + 1 : version.Minor;
			int build = partToIncrement == VersionPart.build ? version.Build + 1 : version.Build;
			int revision = partToIncrement == VersionPart.revision ? version.Revision + 1 : version.Revision;
			Version newVersion = new Version(major, minor, build, revision);
			return newVersion.ToString();
		}
	}
}
