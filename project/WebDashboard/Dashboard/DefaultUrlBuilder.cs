using System;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class DefaultUrlBuilder : IUrlBuilder
	{
		private readonly IPathMapper pathMapper;
		public static readonly string CONTROLLER_RELATIVE_URL = "default.aspx";

		public DefaultUrlBuilder(IPathMapper pathMapper)
		{
			this.pathMapper = pathMapper;
		}

		private string BuildUrl(string relativeUrl)
		{
			return pathMapper.GetAbsoluteURLForRelativePath(relativeUrl);
		}

		public string BuildUrl(IActionSpecifier action)
		{
			return BuildUrl(action, "");
		}

		public string BuildUrl(IActionSpecifier action, string partialQueryString)
		{
			return BuildUrl(CONTROLLER_RELATIVE_URL, action, partialQueryString);
		}

		private string BuildUrl(string relativeUrl, IActionSpecifier action, string partialQueryString)
		{
			string queryString = "?" + action.ToPartialQueryString();
			
			if (partialQueryString != null && partialQueryString != string.Empty)
			{
				if (queryString.Length > 1)
				{
					queryString += "&";	
				}
				queryString += partialQueryString;
			}
			return BuildUrl(relativeUrl) + queryString;
		}

		public string BuildServerUrl(IActionSpecifier action, IServerSpecifier serverSpecifier)
		{
			return BuildServerUrl(action, serverSpecifier, "");
		}

		public string BuildServerUrl(IActionSpecifier action, IServerSpecifier serverSpecifier, string queryString)
		{
			string fullQueryString = BuildServerQueryString(serverSpecifier);
			if (queryString != null && queryString != string.Empty)
			{
				fullQueryString += ("&" + queryString);
			}
			return BuildUrl(action, fullQueryString);
		}

		public string BuildProjectUrl(IActionSpecifier action, IProjectSpecifier projectSpecifier)
		{
			return BuildUrl(action, BuildProjectQueryString(projectSpecifier));
		}

		public string BuildBuildUrl(IActionSpecifier action, IBuildSpecifier buildSpecifier)
		{
			return BuildUrl(action, BuildBuildQueryString(buildSpecifier));
		}

		public string BuildBuildUrl(IActionSpecifier action, IBuildSpecifier buildSpecifier, string fileName)
		{
			return BuildUrl(fileName, action, BuildBuildQueryString(buildSpecifier));
		}

		public string BuildFormName(IActionSpecifier action, params string[] args)
		{
			string baseName = CruiseActionFactory.ACTION_PARAMETER_PREFIX + action.ActionName;
			foreach (string arg in args)
			{
				baseName += CruiseActionFactory.ACTION_ARG_SEPARATOR;
				baseName += arg;
			}
			return baseName;
		}

		private string BuildServerQueryString(IServerSpecifier serverSpecifier)
		{
			return string.Format("{0}={1}", RequestWrappingCruiseRequest.ServerQueryStringParameter, serverSpecifier.ServerName);
		}

		private string BuildProjectQueryString(IProjectSpecifier projectSpecifier)
		{
			return string.Format("{0}&{1}={2}",BuildServerQueryString(projectSpecifier.ServerSpecifier), RequestWrappingCruiseRequest.ProjectQueryStringParameter, projectSpecifier.ProjectName);
		}

		private string BuildBuildQueryString(IBuildSpecifier buildSpecifier)
		{
			return string.Format("{0}&{1}={2}",BuildProjectQueryString(buildSpecifier.ProjectSpecifier), RequestWrappingCruiseRequest.BuildQueryStringParameter, buildSpecifier.BuildName);
		}
	}
}
