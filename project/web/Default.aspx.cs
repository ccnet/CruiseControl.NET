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
using tw.ccnet.core;

namespace tw.ccnet.web
{
	public class Default : System.Web.UI.Page
	{
		protected Template template;
		protected HtmlGenericControl contentXsl;

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				InitDisplayLogFile();
			}
			catch(CruiseControlException ex)
			{
				contentXsl.InnerHtml += WebUtil.FormatException(ex);
			}
		}

		private void InitDisplayLogFile()
		{
			string logfile = WebUtil.GetLogFilename(Context, Request);
			if (logfile == null)
			{
				return;
			}
			if (!File.Exists(logfile))
			{
				throw new CruiseControlException(String.Format("Logfile not found: {0}", logfile));
			}
			StringBuilder builder = new StringBuilder();
			try
			{		
				XmlDocument document = new XmlDocument();
				document.Load(logfile);
				
				IList list = (IList) ConfigurationSettings.GetConfig("xslFiles");
				foreach (string xslFile in list) 
				{
					string directory = Path.GetDirectoryName(xslFile);
					string file = Path.GetFileName(xslFile);
					string transformFile = Path.Combine(Request.MapPath(directory), file);
					builder.Append(tw.ccnet.core.publishers.BuildLogTransformer.Transform(document, transformFile)).Append("<br>");
				}
			}
			catch(XmlException ex)
			{
				throw new CruiseControlException(String.Format("Bad XML in logfile: " + ex.Message));
			}

			contentXsl.InnerHtml = builder.ToString();
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
