using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.GenericPlugins
{
    /// <title>HTML Report Plugin</title>
    /// <version>1.5</version>
    /// <summary>
    /// A generic plug-in to display a report from an HTML file.
    /// </summary>
    /// <example>
    /// <code title="Minimalist">
    /// &lt;htmlReportPlugin description="Document Report" actionName="viewReport" htmlFileName="reports\document.html"/&gt;
    /// </code>
    /// <code title="In Context">
    /// &lt;buildPlugins&gt;
    /// &lt;buildReportBuildPlugin&gt;
    /// &lt;xslFileNames&gt;
    /// &lt;xslFile&gt;xsl\header.xsl&lt;/xslFile&gt;
    /// &lt;xslFile&gt;xsl\modifications.xsl&lt;/xslFile&gt;
    /// &lt;/xslFileNames&gt;
    /// &lt;/buildReportBuildPlugin&gt;
    /// &lt;buildLogBuildPlugin /&gt;
    /// &lt;htmlReportPlugin description="Document Report" actionName="viewReport" htmlFileName="reports\document.html"/&gt;
    /// &lt;/buildPlugins&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <heading>HTML Source Location</heading>
    /// <para>
    /// This plug-in can display any file that is in the build folder under artefacts folder for the 
    /// project. It cannot display files from any other location (for security reasons).
    /// </para>
    /// <para>
    /// Files can be published to a build folder using the <link>File Merge Task</link>. This will
    /// automatically generate the correct folder structure for the HTML reports.
    /// </para>
    /// <heading>File Names</heading>
    /// <para>
    /// All file names are relative to the build folder. Files directly in the folder can be specified,
    /// as well as folders in sub-folders. For example both report.html and documents\report.html are
    /// valid file names.
    /// </para>
    /// <para>
    /// Absolute filepaths are not allowed (e.g. c:\somewhere\report.html).
    /// </para>
    /// <para>
    /// If the project's artefact folder was d:\data\ and the build label was 1.0.1, then report.html
    /// would come from d:\data\1.0.1\report.html.
    /// </para>
    /// </remarks>
    [ReflectorType("htmlReportPlugin")]
    public class HtmlReportPlugin
        : ProjectConfigurableBuildPlugin
    {
        #region Private fields
        private readonly IActionInstantiator actionInstantiator;
        private string description = "no description set";
        private string actionName = "NoActionSet";
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="HtmlReportPlugin"/>.
        /// </summary>
        /// <param name="actionInstantiator"></param>
        public HtmlReportPlugin(IActionInstantiator actionInstantiator)
		{
			this.actionInstantiator = actionInstantiator;
        }
        #endregion

        #region Public properties
        #region ConfiguredLinkDescription
        /// <summary>
        /// The description of the plug-in.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        /// <remarks>
        /// This will be displayed as the title of the link.
        /// </remarks>
        [ReflectorProperty("description")]
        public string ConfiguredLinkDescription
        {
            get { return description; }
            set { description = value; }
        }
        #endregion

        #region LinkDescription
        /// <summary>
        /// The description of the plug-in.
        /// </summary>
        /// <remarks>
        /// These 2 are separate due to inheritence / property monkey-ness.
        /// </remarks>
        public override string LinkDescription
        {
            get { return description; }
        }
        #endregion

        #region ActionName
        /// <summary>
        /// The name of the action - this will be used in the URL for the plug-in.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        /// <remarks>
        /// This must be unique for the dashboard (e.g. there cannot be another action or plug-in with the
        /// same name.
        /// </remarks>
        [ReflectorProperty("actionName")]
        public string ActionName
        {
            get { return actionName; }
            set { actionName = value; }
        }
        #endregion

        #region HtmlFileName
        /// <summary>
        /// The name of the file to display.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        /// <remarks>
        /// See the notes below on what are valid names.
        /// </remarks>
        [ReflectorProperty("htmlFileName")]
        public string HtmlFileName { get; set; }
        #endregion

        #region NamedActions
        /// <summary>
        /// The named actions that are exposed.
        /// </summary>
        public override INamedAction[] NamedActions
        {
            get
            {
                HtmlReportAction buildAction = (HtmlReportAction)actionInstantiator.InstantiateAction(typeof(HtmlReportAction));
                buildAction.HtmlFileName = HtmlFileName;
                return new INamedAction[] { new ImmutableNamedAction(ActionName, buildAction) };
            }
        }
        #endregion
        #endregion
    }
}
