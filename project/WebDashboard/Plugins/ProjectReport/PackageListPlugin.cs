using System;
using System.Collections.Generic;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport
{
    /// <summary>
    /// A plug-in for generating a list of packages.
    /// </summary>
    [ReflectorType("packageListPlugin")]
    public class PackageListPlugin
        : IPlugin
    {
        #region Private fields
        private readonly IActionInstantiator actionInstantiator;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <see cref="PackageListPlugin"/>.
        /// </summary>
        /// <param name="actionInstantiator"></param>
        public PackageListPlugin(IActionInstantiator actionInstantiator)
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
            get { return "Package List"; }
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
                var action = actionInstantiator.InstantiateAction(typeof(PackageListAction));
                return new INamedAction[]
					{
						new ImmutableNamedAction(PackageListAction.ActionName, action)
					};
            }
        }
        #endregion
        #endregion
    }
}
