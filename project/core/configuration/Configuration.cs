using System;
using System.Collections;
using System.Configuration;

namespace ThoughtWorks.CruiseControl.Core.Config
{
	public class Configuration : IConfiguration
	{
		private IDictionary _projects = new Hashtable();

		// these values have been moved to this class to co-locate all app settings string keys...
		// implemented as property getters, rather than static-readonly, so that they're retrieved for each call
		// and can change during runtime (rather than being set when the type is loaded)... this may not
		// be required, but is safer
		public static string NAntLogger     { get { return ConfigurationSettings.AppSettings["NAnt.Logger"]; } }
		public static string ConfigFileName { get { return ConfigurationSettings.AppSettings["ccnet.config"]; } }
		public static string Remoting       { get { return ConfigurationSettings.AppSettings["remoting"]; } }
		public static string LogDir	        { get { return ConfigurationSettings.AppSettings["logDir"]; } }

		public void AddProject(IProject project)
		{
			_projects[project.Name] = project;
		}

		public IProject GetProject(string name)
		{
			return _projects[name] as IProject;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _projects.Values.GetEnumerator();
		}
	}
}
