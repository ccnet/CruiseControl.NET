
using System.IO;
using System.Xml;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// 	
    /// </summary>
	public class NetReflectorProjectSerializer : IProjectSerializer
	{
		// ToDo - componentize NetReflector.Write, use that, and test (using a mock)
        /// <summary>
        /// Serializes the specified project.	
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public string Serialize(IProject project)
		{
			StringWriter buffer = new StringWriter();
			new ReflectorTypeAttribute("project").Write(new XmlTextWriter(buffer), project);
			return buffer.ToString();
		}

        /// <summary>
        /// Deserializes the specified serialized project.	
        /// </summary>
        /// <param name="serializedProject">The serialized project.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public IProject Deserialize(string serializedProject)
		{
			return NetReflector.Read(serializedProject) as IProject;
		}
	}
}