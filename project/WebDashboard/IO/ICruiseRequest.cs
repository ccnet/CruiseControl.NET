using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
	public interface ICruiseRequest
	{
		string ServerName { get; }
		string ProjectName { get; }
		string BuildName { get; }

		IServerSpecifier ServerSpecifier { get;  }
		IProjectSpecifier ProjectSpecifier { get;  }
		IBuildSpecifier BuildSpecifier { get;  }

		IRequest Request { get; }
	}
}
