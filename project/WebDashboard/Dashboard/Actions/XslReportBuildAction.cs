using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions
{
	[ReflectorType("xslReportBuildAction")]
	public class XslReportBuildAction : ICruiseAction
	{
		private readonly IBuildLogTransformer buildLogTransformer;
		private string xslFileName;

		public XslReportBuildAction(IBuildLogTransformer buildLogTransformer)
		{
			this.buildLogTransformer = buildLogTransformer;
		}

		public IView Execute(ICruiseRequest cruiseRequest)
		{
			if (xslFileName == null)
			{
				throw new ApplicationException("XSL File Name has not been set for XSL Report Action");
			}
			return new StringView(buildLogTransformer.Transform(cruiseRequest.BuildSpecifier, xslFileName));
		}

		[ReflectorProperty("xslFileName")]
		public string XslFileName
		{
			get
			{
				return xslFileName;
			}
			set
			{
				xslFileName = value;
			}
		}
	}
}
