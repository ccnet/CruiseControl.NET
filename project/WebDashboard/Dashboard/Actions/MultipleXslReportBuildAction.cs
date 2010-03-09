using System;
using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions
{
	[ReflectorType("multipleXslReportAction")]
	public class MultipleXslReportBuildAction : ICruiseAction, IConditionalGetFingerprintProvider
	{
		private readonly IBuildLogTransformer buildLogTransformer;
	    private readonly IFingerprintFactory fingerprintFactory;

        public MultipleXslReportBuildAction(IBuildLogTransformer buildLogTransformer, IFingerprintFactory fingerprintFactory)
        {
            this.buildLogTransformer = buildLogTransformer;
            this.fingerprintFactory = fingerprintFactory;
        }

	    public IResponse Execute(ICruiseRequest cruiseRequest)
		{
			if (this.XslFileNames == null)
			{
				throw new ApplicationException("XSL File Name has not been set for XSL Report Action");
			}
			Hashtable xsltArgs = new Hashtable();
			xsltArgs["applicationPath"] = cruiseRequest.Request.ApplicationPath;
            var fileNames = this.GenerateFilenameList(cruiseRequest);
			return new HtmlFragmentResponse(buildLogTransformer.Transform(cruiseRequest.BuildSpecifier, fileNames.ToArray(), xsltArgs, cruiseRequest.RetrieveSessionToken()));
		}

        /// <summary>
        /// Gets or sets the XSL file names.
        /// </summary>
        /// <value>The XSL file names.</value>
        [ReflectorArray("xslFileNames")]
        public BuildReportXslFilename[] XslFileNames { get; set; }

	    public ConditionalGetFingerprint GetFingerprint(IRequest request)
	    {
            var fileNames = this.GenerateFilenameList(null);
            return fingerprintFactory.BuildFromFileNames(fileNames.ToArray());
	    }

        /// <summary>
        /// Generates the filename list.
        /// </summary>
        /// <param name="cruiseRequest">The cruise request.</param>
        /// <returns></returns>
        private List<string> GenerateFilenameList(ICruiseRequest cruiseRequest)
        {
            var fileNames = new List<string>();
            var projectName = cruiseRequest != null ? cruiseRequest.ProjectName : null;
            foreach (var xslFileName in this.XslFileNames)
            {
                if (xslFileName.CheckProject(projectName))
                {
                    fileNames.Add(xslFileName.Filename);
                }
            }

            return fileNames;
        }
    }
}