using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Web;
using System.Web.Services;

using tw.ccnet.remote;

namespace CCNet.WebService
{
	public class CCNet : System.Web.Services.WebService
	{
		public CCNet()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}

		public virtual ICruiseManager BackingCruiseManager
		{
			get
			{
				return (ICruiseManager) RemotingServices.Connect(typeof(ICruiseManager), ConfigurationSettings.AppSettings["BackingURL"]);
			}
		}

		[WebMethod]
		public void StopCruiseControl()
		{
			BackingCruiseManager.StopCruiseControl();
		}

		[WebMethod]
		public string GetConfiguration()
		{
			return BackingCruiseManager.Configuration;
		}

		[WebMethod]
		public void SetConfiguration(string configuration)
		{
			BackingCruiseManager.Configuration = configuration;
		}

		[WebMethod]
		public ProjectStatus GetProjectStatus()
		{
			return BackingCruiseManager.GetProjectStatus();
		}

		[WebMethod]
		public void ForceBuild(string projectName)
		{
			BackingCruiseManager.ForceBuild(projectName);
		}

		[WebMethod]
		public void StartCruiseControl()
		{
			BackingCruiseManager.StartCruiseControl();
		}

		[WebMethod]
		public tw.ccnet.remote.CruiseControlStatus GetStatus()
		{
			return BackingCruiseManager.GetStatus();
		}

		[WebMethod]
		public void StopCruiseControlNow()
		{
			BackingCruiseManager.StopCruiseControl();
		}

		#region Component Designer generated code
		
		//Required by the Web Services Designer 
		private IContainer components = null;
				
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion
	}
}
