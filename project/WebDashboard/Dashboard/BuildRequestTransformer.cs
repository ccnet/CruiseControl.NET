using System.Collections;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
    public class BuildRequestTransformer 
        : IBuildLogTransformer
	{
		private readonly IMultiTransformer transformer;
		private readonly IBuildRetriever buildRetriever;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildRequestTransformer"/> class.
        /// </summary>
        /// <param name="buildRetriever">The build retriever.</param>
        /// <param name="transformer">The transformer.</param>
		public BuildRequestTransformer(IBuildRetriever buildRetriever, IMultiTransformer transformer)
		{
			this.buildRetriever = buildRetriever;
			this.transformer = transformer;
        }
        #endregion

        #region Public methods
        #region Transform()
        /// <summary>
        /// Transforms the specified build specifier.
        /// </summary>
        /// <param name="buildSpecifier">The build specifier.</param>
        /// <param name="transformerFileNames">The transformer file names.</param>
        /// <param name="xsltArgs">The XSLT args.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns>The transformed content.</returns>
        public string Transform(IBuildSpecifier buildSpecifier, string[] transformerFileNames, Hashtable xsltArgs, string sessionToken)
		{
            var buildLog = buildRetriever.GetBuild(buildSpecifier, sessionToken).Log;
            return transformer.Transform(buildLog, transformerFileNames, xsltArgs);
        }
        #endregion
        #endregion
    }
}
