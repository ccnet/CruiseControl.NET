using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.GenericPlugins
{
    /// <title>HTML Report Plugin</title>
    /// <version>1.5</version>
    /// <summary>
    /// A generic plug-in to display a report from an HTML file.
    /// </summary>
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
