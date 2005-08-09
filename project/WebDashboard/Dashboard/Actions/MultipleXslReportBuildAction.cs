using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions
{
	[ReflectorType("multipleXslReportAction")]
	public class MultipleXslReportBuildAction : ICruiseAction
	{
		private readonly IBuildLogTransformer buildLogTransformer;
		private string[] xslFileNames;

		public MultipleXslReportBuildAction(IBuildLogTransformer buildLogTransformer)
		{
			this.buildLogTransformer = buildLogTransformer;
		}

		public IResponse Execute(ICruiseRequest cruiseRequest)
		{
			if (xslFileNames == null)
			{
				throw new ApplicationException("XSL File Name has not been set for XSL Report Action");
			}
			return new HtmlFragmentResponse(buildLogTransformer.Transform(cruiseRequest.BuildSpecifier, xslFileNames));
		}

		[ReflectorArrayAttribute("xslFileNames")]
		public string[] XslFileNames
		{
			get
			{
				return xslFileNames;
			}
			set
			{
				xslFileNames = value;
			}
		}
	}
}
