using System;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
    /// <summary>
    /// An immutable action that doesn't use the site template.
    /// </summary>
    public class ImmutableNamedActionWithoutSiteTemplate
        : ImmutableNamedAction, INoSiteTemplateAction
    {
        public ImmutableNamedActionWithoutSiteTemplate(string actionName, ICruiseAction action)
            : base(actionName, action)
        { }
    }
}
