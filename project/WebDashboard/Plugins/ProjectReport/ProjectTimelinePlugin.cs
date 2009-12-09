namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport
{
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

    /// <summary>
    /// A plugin to show the project build history in a timeline.
    /// </summary>
    [ReflectorType("projectTimelinePlugin")]
    public class ProjectTimelinePlugin
        : IPlugin
    {
        #region Private fields
        private readonly IActionInstantiator actionInstantiator;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <see cref="ProjectTimelinePlugin"/>.
        /// </summary>
        /// <param name="actionInstantiator"></param>
        public ProjectTimelinePlugin(IActionInstantiator actionInstantiator)
        {
            this.actionInstantiator = actionInstantiator;
        }
        #endregion

        #region Public properties
        #region LinkDescription
        /// <summary>
        /// The description of the link.
        /// </summary>
        public string LinkDescription
        {
            get { return "Project Timeline"; }
        }
        #endregion

        #region NamedActions
        /// <summary>
        /// The actions that are exposed.
        /// </summary>
        public INamedAction[] NamedActions
        {
            get
            {
                var action = actionInstantiator.InstantiateAction(typeof(ProjectTimelineAction));
                return new INamedAction[]
					{
						new ImmutableNamedAction(ProjectTimelineAction.TimelineActionName, action),
						new ImmutableNamedAction(ProjectTimelineAction.DataActionName, action)
					};
            }
        }
        #endregion
        #endregion
    }
}
