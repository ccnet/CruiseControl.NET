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
		protected HtmlTableCell HeaderCell;
		protected HtmlTableCell DetailsCell;
		protected HtmlGenericControl PluginLinks;
		protected System.Web.UI.WebControls.HyperLink TestDetailsLink;
		protected System.Web.UI.WebControls.HyperLink LogLink;
		protected System.Web.UI.WebControls.Label BodyLabel;

		private string logfile;

		private void Page_Load(object sender, System.EventArgs e)
		{
			resolveLogFile();
			generatePluginLinks();

			try
			{
				InitDisplayLogFile();
			}
			catch (CruiseControlException ex)
			{
				// This fixes a problem where the BodyLabel control isn't initialised, causing
				// a NullReferenceException.  The original exception (ex) was being lost.
				// Why is BodyLabel null?  (drewnoakes: I saw this problem while working with
				// invalid Xsl in file modifications.xsl)
				if (BodyLabel==null)
					throw ex;

				if (BodyLabel.Text==null)
					BodyLabel.Text = string.Empty;

				BodyLabel.Text += WebUtil.FormatException(ex);
			}
		}

		private void resolveLogFile()
		{
			logfile = WebUtil.GetLogFilename(Context, Request);
			if (logfile == null)
			{
				throw new CruiseControlException("Internal Error - couldn't resolve logfile to use");
			}
			if (!File.Exists(logfile))
			{
				throw new CruiseControlException(String.Format("Logfile not found: {0}", logfile));
			}
		}

		private void generatePluginLinks()
		{
			if (ConfigurationSettings.GetConfig("CCNet/buildPlugins") == null)
			{
				return;
			}

			string pluginLinksHtml = "";
			bool firstLink = true;
			foreach (PluginSpecification spec in (IEnumerable) ConfigurationSettings.GetConfig("CCNet/buildPlugins"))
			{
				if (!firstLink)
				{
					pluginLinksHtml += String.Format("|&nbsp; ");
				}
				pluginLinksHtml += String.Format(@"<a class=""link"" href=""{0}"">{1}</a> ", genLogUrl(spec.LinkUrl), spec.LinkText);
				firstLink = false;
			}
			PluginLinks.InnerHtml = pluginLinksHtml;
		}

		private string genLogUrl(string urlPrefix)
		{
			return ResolveUrl(String.Format("{0}{1}", urlPrefix, new FileInfo(logfile).Name));
		}
		
		private void InitDisplayLogFile()
		{
			StringBuilder builder = new StringBuilder();
			try
			{		
				XmlDocument document = new XmlDocument();
				document.Load(logfile);
				
				IList list = (IList) ConfigurationSettings.GetConfig("CCNet/xslFiles");
				foreach (string xslFile in list) 
				{
					if (xslFile.ToLower().IndexOf("header") > -1)
					{
						generateHeader(xslFile, document);
					}
					else
					{
						builder.Append(transform(xslFile, document)).Append("<br>");
					}
				}
			}
			catch(XmlException ex)
			{
				throw new CruiseControlException(String.Format("Bad XML in logfile: " + ex.Message));
			}

			DetailsCell.InnerHtml = builder.ToString();
		}

		private void generateHeader(string headerXslfile, XmlDocument logFileDocument)
		{
			HeaderCell.InnerHtml = "<br/>" + transform(headerXslfile, logFileDocument);
		}

		private string transform(string xslfile, XmlDocument logFileDocument)
		{
			string directory = Path.GetDirectoryName(xslfile);
			string file = Path.GetFileName(xslfile);
			string transformFile = Path.Combine(Request.MapPath(directory), file);
			return tw.ccnet.core.publishers.BuildLogTransformer.Transform(logFileDocument, transformFile);
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
