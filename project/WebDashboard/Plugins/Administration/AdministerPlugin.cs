using System;
using System.Collections.Generic;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.Administration
{
    /// <summary>
    /// A plugin for administering the dashboard. This allows to install and uninstall the various plugins without modifying the Dashboard.config by hand.
    /// This plugin is enabled by default. The admin password must be entered into the Dashboard.config once, there is no default password, and this plugin does not work with a blank password.
    /// <para>
    /// The plugin also allows for adding and removing build servers, and maintainting their properties : see <link>remoteServices</link> of the <link>Dashboard Configuration Block</link>.
    /// </para>
    /// <para>Like the ability to enable or disable the start/stop buttons and the force build button.</para>
    /// <heading>Attention</heading>
    ///  Be sure that CCNet server (service) can write to the following files : 
    ///  <para> ° webdashboard\dashboard.config</para>
    ///  <para> ° webdashboard\Packages\packages.xml</para>
    ///    
    /// Adjust the security of these files if needed.
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
