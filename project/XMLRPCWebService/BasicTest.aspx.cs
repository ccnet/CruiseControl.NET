using System;
using CookComputing.XmlRpc;
using ThoughtWorks.CruiseControl.XMLRPCWebService.Client;
using System.Configuration;

namespace ThoughtWorks.CruiseControl.XMLRPCWebService
{
	public class XMLRPCWebServiceBasicTest : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.DataGrid DataGrid1;
		protected System.Web.UI.WebControls.Label Label1;
		public static readonly string FORCE_BUILD_COMMAND = "forcebuild";
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			Label1.Text = "The following are the results for executing [server.get_project_names()] at [" + ConfigurationSettings.AppSettings["TestURL"] + "]";
			DataGrid1.DataSource = RemoteServer.GetProjectNames();
			DataGrid1.DataBind();
		}

		XMLRPC.Server RemoteServer
		{
			get
			{
				ServerProxy server = new ServerProxy();
				server.Url = ConfigurationSettings.AppSettings["TestURL"];
				return server;
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}
		
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

	}
}
