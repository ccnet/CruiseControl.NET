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
using tw.ccnet.core;

namespace tw.ccnet.web
{
	public class Default : System.Web.UI.Page
	{
		protected Template template;
		protected HtmlGenericControl headerXsl;
		protected HtmlGenericControl compileXsl;
		protected HtmlGenericControl javadocXsl;
		protected HtmlGenericControl unittestsXsl;
		protected HtmlGenericControl modificationsXsl;
		protected HtmlGenericControl distributablesXsl;

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				string path = ConfigurationSettings.AppSettings["logDir"];
				InitDisplayLogFile(path);
			}
			catch(CruiseControlException ex)
			{
				headerXsl.InnerHtml += WebUtil.FormatException(ex);
			}
		}

		private void InitDisplayLogFile(string path)
		{
			string logfile = WebUtil.GetLogFilename(path, Request);
			if (logfile == null)
			{
				return;
			}

			headerXsl.InnerHtml = LogFileLister.Transform(logfile, WebUtil.GetXslFilename("header.xsl", Request));
			compileXsl.InnerHtml = LogFileLister.Transform(logfile, WebUtil.GetXslFilename("compile.xsl", Request));
			javadocXsl.InnerHtml = LogFileLister.Transform(logfile, WebUtil.GetXslFilename("javadoc.xsl", Request));
			unittestsXsl.InnerHtml = LogFileLister.Transform(logfile, WebUtil.GetXslFilename("unittests.xsl", Request));
			modificationsXsl.InnerHtml = LogFileLister.Transform(logfile, WebUtil.GetXslFilename("modifications.xsl", Request));
			distributablesXsl.InnerHtml = LogFileLister.Transform(logfile, WebUtil.GetXslFilename("distributables.xsl", Request));
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
