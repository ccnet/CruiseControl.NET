using System;
using System.Collections;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;

using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;


namespace ThoughtWorks.CruiseControl.Web 
{
	public class Admin : System.Web.UI.Page 
	{
		protected System.Web.UI.HtmlControls.HtmlTableCell contentCell;
		protected System.Web.UI.WebControls.Button startServer;
		protected System.Web.UI.WebControls.Button stopServer;
		protected System.Web.UI.WebControls.Button stopNow;
		protected CruiseControlStatus _status;
		protected System.Web.UI.WebControls.Literal statusLiteral;
		protected System.Web.UI.HtmlControls.HtmlForm Form1;
		protected CruiseManager _manager;
		
		override protected void OnInit(EventArgs e) 
		{
			InitializeComponent();
			base.OnInit(e);
		}

		private void InitializeComponent() 
		{    
			this.startServer.Click += new System.EventHandler(this.StartServer);
			this.stopServer.Click += new System.EventHandler(this.StopServer);
			this.stopNow.Click += new System.EventHandler(this.StopNow);
			this.Load += new System.EventHandler(this.Page_Load);
			this.Unload += new System.EventHandler(this.Page_Unload);

		}

		private void Page_Unload(object sender, System.EventArgs e) 
		{
		}
				
		private void Page_Load(object sender, System.EventArgs e) 
		{
			try{
				_manager = (CruiseManager) RemotingServices.Connect(typeof(CruiseManager), "tcp://localhost:1234/CruiseManager.rem");
				_status = _manager.GetStatus();
				RefreshButtons();
			}catch(SocketException ex){
				statusLiteral.Text = "Remoting Administration is not available on this server. " + ex.Message;			
				startServer.Visible = false;
				stopServer.Visible = false;
				stopNow.Visible = false;
			}
		}
		
		private void StartServer(object sender, System.EventArgs e)
		{
			_manager.StartCruiseControl();
			RefreshButtons();
		}
		
		private void StopServer(object sender, System.EventArgs e)
		{
			_manager.StopCruiseControl();
			RefreshButtons();
		}
		
		private void StopNow(object sender, System.EventArgs e)
		{
			_manager.StopCruiseControlNow();
			RefreshButtons();
		}
		
		private void RefreshButtons()
		{
			_status = _manager.GetStatus();
			statusLiteral.Text = "Build Server Status :"+ _manager.GetStatus();			
			if(_status == CruiseControlStatus.Stopped)
			{
				startServer.Enabled = true;
				stopServer.Enabled = false;
				stopNow.Enabled = false;
			}
			else if(_status == CruiseControlStatus.Running)
			{
				startServer.Enabled = false;
				stopServer.Enabled = true;
				stopNow.Enabled = true;
			}
			else if(_status == CruiseControlStatus.WillBeStopped)
			{
				startServer.Enabled = false;
				stopServer.Enabled = false;
				stopNow.Enabled = true;
			}
		
		}
	}
}
