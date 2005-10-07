using System;
using System.Collections;
using System.Diagnostics;
using System.Web;
using System.Web.Caching;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class ExceptionCatchingActionProxy : IAction
	{
		private readonly IAction proxiedAction;
		private readonly IVelocityViewGenerator velocityViewGenerator;

		public ExceptionCatchingActionProxy(IAction proxiedAction, IVelocityViewGenerator velocityViewGenerator)
		{
			this.proxiedAction = proxiedAction;
			this.velocityViewGenerator = velocityViewGenerator;
		}

		public IResponse Execute(IRequest request)
		{
			try
			{
				return proxiedAction.Execute(request);	
			}
			catch (Exception e)
			{
				Hashtable velocityContext = new Hashtable();
				velocityContext["exception"] = e;
				return velocityViewGenerator.GenerateView(@"ActionException.vm", velocityContext);
			}
		}
	}

}
