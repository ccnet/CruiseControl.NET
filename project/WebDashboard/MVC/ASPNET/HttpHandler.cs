using System.IO;
using System.Reflection;
using System.Web;
using Objection;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.ASPNET
{
	// No need for session state yet, but if we do later then we should also add IRequiresSessionState to list of interfaces
	public class HttpHandler : IHttpHandler
	{
		private const string RESOLVED_TYPE_MAP = "ResolvedTypeMap";
		
		public void ProcessRequest(HttpContext context)
		{
            // The following check has been added to handle conversion from 1.4.4 to 1.5.0
            // In 1.5.0, the configuration file has been moved from the program files folder to program data
            var configPath = ProgramDataFolder.MapPath("dashboard.config");
            if (!File.Exists(configPath))
            {
                var file = context.Server.MapPath("~/DataMigration.htm");
                if ((context.Request.HttpMethod == "POST") &&
                    (context.Request.Form["Upgrade"] == "Yes"))
                {
                    if (!Directory.Exists(ProgramDataFolder.Location)) Directory.CreateDirectory(ProgramDataFolder.Location);
                    File.Copy(context.Server.MapPath("dashboard.config"), configPath);
                    file = context.Server.MapPath("~/DataMigrationDone.htm");
                }
                context.Response.WriteFile(file);
            }
            else
            {
                ObjectionStore objectionStore = new ObjectionStore(
                    new CachingImplementationResolver(new NMockAwareImplementationResolver(), new CachedTypeMap(context.Cache, RESOLVED_TYPE_MAP)),
                    new MaxLengthConstructorSelectionStrategy());
                ObjectSource objectSource = new CruiseObjectSourceInitializer(objectionStore).SetupObjectSourceForRequest(context);

                context.Response.AppendHeader("X-CCNet-Version",
                    string.Format("CruiseControl.NET/{0}", Assembly.GetExecutingAssembly().GetName().Version));
                Assembly.GetExecutingAssembly().GetName().Version.ToString();

                IResponse response = ((RequestController)objectSource.GetByType(typeof(RequestController))).Do();
                response.Process(context.Response);
            }
            context.Response.Flush();
        }

		public bool IsReusable
		{
			get { return true; }
		}
	}
}
