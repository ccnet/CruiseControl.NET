using System;
using System.Collections;
using System.Configuration;

namespace ThoughtWorks.CruiseControl.Core.Config
{
	public class Configuration : IConfiguration
	{
		private ProjectList _projects = new ProjectList();
		private ProjectIntegratorList _integrators = new ProjectIntegratorList();

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
			_projects.Add(project);
			_integrators.Add(new ProjectIntegrator(project));
		}

		public IProjectList Projects
		{
			get { return _projects; }
		}

		public IProjectIntegratorList Integrators
		{
			get { return _integrators; }
		}
	}
}
