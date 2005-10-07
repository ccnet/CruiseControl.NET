using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
	public interface IResponseCache
	{
		IResponse Get(IRequest request);
		void Insert(IRequest request, IResponse response);
	}

}