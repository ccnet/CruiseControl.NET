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
using System.Xml;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.Web
{
	public class ServerLog : System.Web.UI.Page
	{
		protected HtmlTableCell LogData;

		private void Page_Load(object sender, System.EventArgs e)
		{
//				BodyLabel.InnerText += WebUtil.FormatException(ex);

			string serverLogFilename = GetServerLogFilenameFromConfig();
			string logData = ReadLinesFromLog(serverLogFilename, 20);
			LogData.InnerText += logData;
		}

		private string GetServerLogFilenameFromConfig()
		{
			return ConfigurationSettings.AppSettings["ServerLogFilePath"];
		}

		private string ReadLinesFromLog(string filename, int lines)
		{
			return new ServerLogFileReader(filename, lines).Read();
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
