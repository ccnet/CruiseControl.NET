#pragma warning disable 1591
using System.IO;
using System.Xml;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core
{
	public class NetReflectorProjectSerializer : IProjectSerializer
	{
		// ToDo - componentize NetReflector.Write, use that, and test (using a mock)
		public string Serialize(IProject project)
		{
			StringWriter buffer = new StringWriter();
			new ReflectorTypeAttribute("project").Write(new XmlTextWriter(buffer), project);
			return buffer.ToString();
		}

		public IProject Deserialize(string serializedProject)
		{
			return NetReflector.Read(serializedProject) as IProject;
		}
	}
}