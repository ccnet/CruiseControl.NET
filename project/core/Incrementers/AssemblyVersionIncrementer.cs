using System;
using System.Reflection;

namespace tw.ccnet.core
{
	public class AssemblyVersionIncrementer : ILabelIncrementer
	{
		public enum VersionPart 
		{
			major, minor, build, revision
		}

		private VersionPart partToIncrement;
		private string assemblyPath;

		public AssemblyVersionIncrementer(string assemblyPath, VersionPart partToIncrement)
		{
			this.assemblyPath = assemblyPath;
			this.partToIncrement = partToIncrement;
		}

		public AssemblyVersionIncrementer(string assemblyPath) : this(assemblyPath, VersionPart.revision) {}

		public string GetNextLabel() 
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
