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
	public class Default : System.Web.UI.Page
	{
		protected HtmlTableCell HeaderCell;
		protected HtmlTableCell DetailsCell;
		protected HtmlGenericControl PluginLinks;
		protected System.Web.UI.WebControls.HyperLink TestDetailsLink;
		protected System.Web.UI.WebControls.HyperLink LogLink;
		protected HtmlGenericControl BodyLabel;

		private string logfile;

		private void Page_Load(object sender, System.EventArgs e)
		{
			logfile = WebUtil.ResolveLogFile(Context);
			GeneratePluginLinks();

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
					throw new CruiseControlException("Unable to render page.", ex);

				if (BodyLabel.InnerText==null)
					BodyLabel.InnerText = string.Empty;

				BodyLabel.InnerText += WebUtil.FormatException(ex);
			}
		}

		private void GeneratePluginLinks()
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
				pluginLinksHtml += String.Format(@"<a class=""link"" href=""{0}"">{1}</a> ", GenerateLogUrl(spec.LinkUrl), spec.LinkText);
				firstLink = false;
			}
			PluginLinks.InnerHtml = pluginLinksHtml;
		}

		private string GenerateLogUrl(string urlPrefix)
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
						GenerateHeader(xslFile, document);
					}
					else
					{
						builder.Append(Transform(xslFile, document)).Append("<br>");
					}
				}
			}
			catch(XmlException ex)
			{
				throw new CruiseControlException(String.Format("Bad XML in logfile: " + ex.Message));
			}

			DetailsCell.InnerHtml = builder.ToString();
		}

		private void GenerateHeader(string headerXslfile, XmlDocument logFileDocument)
		{
			HeaderCell.InnerHtml = "<br/>" + Transform(headerXslfile, logFileDocument);
		}

		private string Transform(string xslfile, XmlDocument logFileDocument)
		{
			string directory = Path.GetDirectoryName(xslfile);
			string file = Path.GetFileName(xslfile);
			string transformFile = Path.Combine(Request.MapPath(directory), file);
			return ThoughtWorks.CruiseControl.Core.Publishers.BuildLogTransformer.Transform(logFileDocument, transformFile);
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
