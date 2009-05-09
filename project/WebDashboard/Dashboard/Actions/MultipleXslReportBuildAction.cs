using System;
using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions
{
	[ReflectorType("multipleXslReportAction")]
	public class MultipleXslReportBuildAction : ICruiseAction, IConditionalGetFingerprintProvider
	{
		private readonly IBuildLogTransformer buildLogTransformer;
	    private readonly IFingerprintFactory fingerprintFactory;
	    private string[] xslFileNames;

        public MultipleXslReportBuildAction(IBuildLogTransformer buildLogTransformer, IFingerprintFactory fingerprintFactory)
        {
            this.buildLogTransformer = buildLogTransformer;
            this.fingerprintFactory = fingerprintFactory;
        }

	    public IResponse Execute(ICruiseRequest cruiseRequest)
		{
			if (xslFileNames == null)
			{
				throw new ApplicationException("XSL File Name has not been set for XSL Report Action");
			}
			Hashtable xsltArgs = new Hashtable();
			xsltArgs["applicationPath"] = cruiseRequest.Request.ApplicationPath;
			return new HtmlFragmentResponse(buildLogTransformer.Transform(cruiseRequest.BuildSpecifier, xslFileNames, xsltArgs, cruiseRequest.RetrieveSessionToken()));
		}

		[ReflectorArray("xslFileNames")]
		public string[] XslFileNames
		{
			get { return xslFileNames; }
			set { xslFileNames = value; }
		}

	    public ConditionalGetFingerprint GetFingerprint(IRequest request)
	    {
            return fingerprintFactory.BuildFromFileNames(XslFileNames);
	    }
	}
}