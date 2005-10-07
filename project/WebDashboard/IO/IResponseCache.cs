using System;
using System.Web;
using System.Web.Caching;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
	public interface IResponseCache
	{
		IResponse Get(IRequest request);
		void Insert(IRequest request, IResponse response);
	}

}
using System;
using System.Web;
using System.Web.Caching;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
	public interface IResponseCache
	{
		IResponse Get(IRequest request);
		void Insert(IRequest request, IResponse response);
	}

}