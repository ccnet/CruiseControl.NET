using Objection;
using System;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
    public interface ISecurityPlugin
        : IPlugin
    {
        bool IsAllowedForServer(IServerSpecifier serviceSpecifier);
        ISessionStorer SessionStorer { get; set; }
    }
}
