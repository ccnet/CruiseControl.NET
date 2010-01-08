using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerReport
{
    /// <title>Server Report Server Plugin</title>
    /// <version>1.0</version>
    /// <summary>
    /// The Server Report Server Plugin shows you status information for all projects on a specific server. If the Dashboard cannot connect to
    /// the server then an errors table is shown detailing the problem.. 
    /// </summary>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;serverReportServerPlugin /&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;serverReportServerPlugin defaultSort="Name" /&gt;
    /// </code>
    /// </example>
    [ReflectorType("serverReportServerPlugin")]
	public class ServerReportServerPlugin : ICruiseAction, IPlugin
	{
		public static readonly string ACTION_NAME = "ViewServerReport";

		private readonly IProjectGridAction projectGridAction;
        private ProjectGridSortColumn sortColumn = ProjectGridSortColumn.Name;

        #region Public properties
        #region DefaultSortColumn
        /// <summary>
        /// The default column to sort by.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>Name</default>
        [ReflectorProperty("defaultSort", Required = false)]
        public ProjectGridSortColumn DefaultSortColumn
        {
            get { return this.sortColumn; }
            set { this.sortColumn = value; }
        }
        #endregion
        #endregion

		public ServerReportServerPlugin(IProjectGridAction projectGridAction)
		{
			this.projectGridAction = projectGridAction;
		}

		public IResponse Execute(ICruiseRequest request)
		{
            projectGridAction.DefaultSortColumn = sortColumn;
            return projectGridAction.Execute(ACTION_NAME, request.ServerSpecifier, request);
		}

		public string LinkDescription
		{
			get { return "Server Report"; }
		}

		public INamedAction[] NamedActions
		{
			get {  return new INamedAction[] { new ImmutableNamedAction(ACTION_NAME, this) }; }
		}
	}
}
