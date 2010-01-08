using Exortech.NetReflector;
using Objection;
using System;
using System.Collections;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.Security
{
    /// <title>Simple Security Plugin</title>
    /// <version>1.5</version>
    /// <summary>
    /// Allows the user to send a user name and password to the server for authentication.
    /// </summary>
    /// <example>
    /// <code title="Minimalist Example">
    /// &lt;simpleSecurity /&gt;
    /// </code>
    /// <code title="Full Example">
    /// &lt;simpleSecurity hidePassword="true" /&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// </remarks>
    [ReflectorType("simpleSecurity")]
    public class UserNameSecurityPlugin
        : ISecurityPlugin
    {
        #region Private constants
        private const string actionName = "SimpleUserLogin";
        #endregion

        #region Private fields
        private readonly IFarmService farmService;
        private readonly IVelocityViewGenerator viewGenerator;
        private ISessionStorer storer;
        private bool hidePassword;
        #endregion

        #region Constructors
        public UserNameSecurityPlugin(IFarmService farmService, IVelocityViewGenerator viewGenerator,
            ISessionStorer storer)
        {
            this.farmService = farmService;
            this.viewGenerator = viewGenerator;
            this.storer = storer;
        }
        #endregion

        #region Public properties
        public INamedAction[] NamedActions
        {
            get { return new INamedAction[] { new ImmutableNamedAction(actionName, new UserNameSecurityAction(farmService, viewGenerator, storer, hidePassword)) }; }
        }

        public string LinkDescription
        {
            get { return "Simple Login"; }
        }

        public ISessionStorer SessionStorer
        {
            get { return storer; }
            set { storer = value; }
        }

        /// <summary>
        /// Whether to hide the password field or not.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        [ReflectorProperty("hidePassword", Required = false)]
        public bool HidePassword
        {
            get { return hidePassword; }
            set { hidePassword = value; }
        }
        #endregion

        #region Public methods
        public bool IsAllowedForServer(IServerSpecifier serviceSpecifier)
        {
            return true;
        }
        #endregion
    }
}
