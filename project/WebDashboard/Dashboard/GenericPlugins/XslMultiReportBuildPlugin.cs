using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.GenericPlugins
{
	/// <summary>
	/// A Generic XSL report template that can accept multiple transforms.
    /// It works the same as <link>BuildReportBuildPlugin</link>. 
    /// But because the hyperlink in the <link>BuildReportBuildPlugin</link> is fixed, so there can be only one in the config.
    /// <para>
    /// This plugin allows you to group xsl files in a logical manner. For example all code analysis in one xslMultiReportBuildPlugin,
    /// and test and coverage in another.
    /// </para>
    /// <para>
    /// The benefit is that you can keep the <link>BuildReportBuildPlugin</link> as an overview, which you want to load fast, 
    /// and this plugin as a more dedicated overview, focussing on certain areas, and still having all the info on 1 screen.
    /// </para>
    /// <para>
    /// LinkDescription : Must be set by the user.
    /// </para>
    /// </summary>
    /// <title>xsl Multi Report Build Plugin</title>
    /// <example>
    /// <code>
    /// &lt;xslMultiReportBuildPlugin description="Build and Test Details" actionName="NUnitDetailsBuildReport"&gt;
    /// &lt;xslFileNames&gt;
    /// &lt;xslFile&gt;xsl\header.xsl&lt;/xslFile&gt;
    /// &lt;xslFile&gt;xsl\compile.xsl&lt;/xslFile&gt;
    /// &lt;xslFile&gt;xsl\unittests.xsl&lt;/xslFile&gt;
    /// &lt;/xslFileNames&gt;
    /// &lt;/xslMultiReportBuildPlugin&gt;
    /// </code>
    /// </example>
	[ReflectorType("xslMultiReportBuildPlugin")]
	public class XslMultiReportBuildPlugin : ProjectConfigurableBuildPlugin
	{
		public XslMultiReportBuildPlugin(IActionInstantiator actionInstantiator)
		{
			this.actionInstantiator = actionInstantiator;
		}

		private readonly IActionInstantiator actionInstantiator;
		private string description = "no description set";
		private string actionName = "NoActionSet";

		// These 2 are separate due to inheritence / property monkey-ness
        /// <summary>
        /// The description for the hyperlink you would like to see in the build log panel.
        /// The <link>BuildReportBuildPlugin</link> has description : Build Report
        /// </summary>
        /// <version>1.3</version>
        /// <default>no description set</default>
		[ReflectorProperty("description")]
		public string ConfiguredLinkDescription
		{
			get { return description; }
			set { description = value; }
		}

		// See note on ConfiguredLinkDescription
		public override string LinkDescription
		{
			get { return description; }
		}


        /// <summary>
        /// The suffix for the hyperlink. For the <link>BuildReportBuildPlugin</link> this is : ViewBuildReport
        /// A name for the action for this instance of the plugin. This must be strictly alpha Numerical with no spaces and unique.
        /// </summary>
        /// <version>1.0</version>
        /// <default>NoActionSet</default>
		[ReflectorProperty("actionName")]
		public string ActionName
		{
			get { return actionName; }
			set { actionName = value; }
		}

        /// <summary>
        /// The xsl files to use to transform the build log. Location is relative to the web.config file.
        /// Standard installation places the xsl files in the xsl folder.
        /// </summary>
        /// <version>1.0</version>
        /// <default></default>
        [ReflectorProperty("xslFileNames", typeof(BuildReportXslFilenameSerialiserFactory))]
        public BuildReportXslFilename[] XslFileNames { get; set; }

		public override INamedAction[] NamedActions
		{
			get
			{
				MultipleXslReportBuildAction buildAction = (MultipleXslReportBuildAction) actionInstantiator.InstantiateAction(typeof (MultipleXslReportBuildAction));
				buildAction.XslFileNames = XslFileNames;
				return new INamedAction[] {new ImmutableNamedAction(ActionName, buildAction)};
			}
		}
	}
}