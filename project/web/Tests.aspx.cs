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
	public class Tests : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label BodyLabel;

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				InitDisplayLogFile();
			}
			catch(CruiseControlException ex)
			{
				BodyLabel.Text += WebUtil.FormatException(ex);
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
				
				string xslFile = "xsl/tests.xsl";
				string directory = Path.GetDirectoryName(xslFile);
				string file = Path.GetFileName(xslFile);
				string transformFile = Path.Combine(Request.MapPath(directory), file);
				builder.Append(ThoughtWorks.CruiseControl.Core.Publishers.BuildLogTransformer.Transform(document, transformFile)).Append("<br>");
			}
			catch(XmlException ex)
			{
				throw new CruiseControlException(String.Format("Bad XML in logfile: {0}\n{1}", logfile, ex));
			}

			BodyLabel.Text = builder.ToString();
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
