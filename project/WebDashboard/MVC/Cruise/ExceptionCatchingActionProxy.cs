using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class ExceptionCatchingActionProxy : IAction
	{
		private readonly IAction proxiedAction;

		public ExceptionCatchingActionProxy(IAction proxiedAction)
		{
			this.proxiedAction = proxiedAction;
		}

		public IView Execute(IRequest request)
		{
			try
			{
				return proxiedAction.Execute(request);	
			}
			catch (Exception e)
			{
				return new DefaultView(string.Format("<p>There was an exception trying to perform request. Details of the Exception are:</p><p>{0}</p><p>{1}</p>", e.Message, e.StackTrace));
			}
		}
	}
}
