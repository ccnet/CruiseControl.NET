using System;
using System.Collections.Generic;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.Administration
{
    /// <summary>
    /// A plugin for administering the dashboard.
    /// </summary>
    [ReflectorType("administrationPlugin")]
    public class AdministerPlugin
        : IPlugin
    {
        #region Private fields
        private readonly IActionInstantiator actionInstantiator;
        private string password;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <see cref="AdministerPlugin"/>.
        /// </summary>
        /// <param name="actionInstantiator"></param>
        public AdministerPlugin(IActionInstantiator actionInstantiator)
		{
			this.actionInstantiator = actionInstantiator;
        }
        #endregion

        #region Public properties
        #region Password
        /// <summary>
        /// The password to lock down the administration plugin.
        /// </summary>
        [ReflectorProperty("password", Required = false)]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        #endregion

        #region LinkDescription
        /// <summary>
        /// The description of the link.
        /// </summary>
        public string LinkDescription
		{
			get { return "Administer Dashboard"; }
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
                ICruiseAction action = actionInstantiator.InstantiateAction(typeof(AdministerAction));
                if (action is AdministerAction)
                {
                    (action as AdministerAction).Password = password;
                }
				return new INamedAction[]
					{
						new ImmutableNamedAction(AdministerAction.ActionName, action)
					};
			}
		}
        #endregion
        #endregion
    }
}
