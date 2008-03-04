using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
    class RSSLinkBuilder
    {

        public static GeneralAbsoluteLink CreateRSSLink(ILinkFactory linkFactory, IProjectSpecifier projectSpecifier)
        {
            string MachineName;

            if (System.Web.HttpContext.Current == null ) 
            {
                MachineName = "localhost";
            }
            else
            {
                MachineName = System.Web.HttpContext.Current.Request.Url.Host;
                if (!System.Web.HttpContext.Current.Request.Url.IsDefaultPort)
                    MachineName = MachineName + ":" + System.Web.HttpContext.Current.Request.Url.Port;
            }

            return  new GeneralAbsoluteLink("RSS",string.Format("http://{0}/{1}",
                         MachineName,  
                         linkFactory.CreateProjectLink(projectSpecifier, WebDashboard.Plugins.RSS.RSSFeed.ACTION_NAME).Url));
       }

    }
}
