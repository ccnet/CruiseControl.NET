using System;
using System.Collections;
using System.Xml;
using System.Xml.Xsl;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using System.IO;
using System.Text;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport
{
	public class HtmlBuildLogAction : ICruiseAction, IConditionalGetFingerprintProvider
	{
		public static readonly string ACTION_NAME = "ViewBuildLog";
        private const string TEMPLATE_NAME = @"BuildLog.vm";

		private readonly IBuildRetriever buildRetriever;
		private readonly IVelocityViewGenerator viewGenerator;
		private readonly ICruiseUrlBuilder urlBuilder;
	    private readonly IFingerprintFactory fingerprintFactory;
        private readonly ISessionRetriever retriever;

	    public HtmlBuildLogAction(IBuildRetriever buildRetriever, IVelocityViewGenerator viewGenerator,
		                          ICruiseUrlBuilder urlBuilder, IFingerprintFactory fingerprintFactory,
            ISessionRetriever retriever)
		{
			this.buildRetriever = buildRetriever;
			this.viewGenerator = viewGenerator;
			this.urlBuilder = urlBuilder;
		    this.fingerprintFactory = fingerprintFactory;
            this.retriever = retriever;
		}

		public IResponse Execute(ICruiseRequest cruiseRequest)
		{
            // Transform the document into a dynamic document
            var buildSpecifier = cruiseRequest.BuildSpecifier;
            var build = buildRetriever.GetBuild(buildSpecifier, cruiseRequest.RetrieveSessionToken());
            var transform = new XslCompiledTransform();
            using (var reader = XmlReader.Create(this.GetType().Assembly.GetManifestResourceStream("ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport.BuildReportTransform.xslt")))
            {
                transform.Load(reader);
            }
            var builder = new StringBuilder();
            using (var stringReader = new StringReader(build.Log))
            {
                using (var stringWriter = new StringWriter(builder))
                {
                    using (var reader = XmlReader.Create(stringReader))
                    {
                        using (var writer = XmlWriter.Create(stringWriter))
                        {
                            transform.Transform(reader, writer);
                        }
                    }
                }
            }

			Hashtable velocityContext = new Hashtable();
            velocityContext["log"] = builder.ToString();

			// TODO - urk, this is a hack, need a better way of setting extensions
			string oldExtension = urlBuilder.Extension;
			urlBuilder.Extension = "xml";
			velocityContext["logUrl"] = urlBuilder.BuildBuildUrl(XmlBuildLogAction.ACTION_NAME, buildSpecifier);
			urlBuilder.Extension = oldExtension;

		    return viewGenerator.GenerateView(TEMPLATE_NAME, velocityContext);
		}

	    public ConditionalGetFingerprint GetFingerprint(IRequest request)
	    {
	        // TODO - Maybe should get date from Build type rather than LogFile?
	        ICruiseRequest cruiseRequest = new NameValueCruiseRequestFactory().CreateCruiseRequest(request, urlBuilder, retriever);
            LogFile logFile = new LogFile(cruiseRequest.BuildSpecifier.BuildName);
	        DateTime buildDate = logFile.Date;
	        ConditionalGetFingerprint logFingerprint = fingerprintFactory.BuildFromDate(buildDate);
	        ConditionalGetFingerprint templateFingerprint = fingerprintFactory.BuildFromFileNames(TEMPLATE_NAME);
	        return logFingerprint.Combine(templateFingerprint);
	    }
	}

}