using System;
using System.Web;

namespace WebApplication1 
{
	public class Global : System.Web.HttpApplication
	{
		public Global()
		{
		}	
		
		protected void Application_Start(Object sender, EventArgs e)
		{

		}
 
		protected void Session_Start(Object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(Object sender, EventArgs e)
		{
			// Security Fix - see http://www.microsoft.com/security/incident/aspnet.mspx
			if (Request.Path.IndexOf('\\') >= 0 || System.IO.Path.GetFullPath(Request.PhysicalPath) != Request.PhysicalPath)
			{
				throw new HttpException(404, "not found");
			}
		}

		protected void Application_EndRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_Error(Object sender, EventArgs e)
		{

		}

		protected void Session_End(Object sender, EventArgs e)
		{

		}

		protected void Application_End(Object sender, EventArgs e)
		{

		}
	}
}

