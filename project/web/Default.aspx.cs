using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.XPath;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Web
{
	public class Default : Page
	{
		protected HtmlGenericControl Contents;
		protected PluginLinks PluginLinks;
		protected HtmlGenericControl BodyLabel;

		private void Page_Load(object sender, EventArgs e)
		{
			try
			{
				ApplyStylesheetsToRenderContent();
			}
			catch (CruiseControlException ex)
			{
				// This fixes a problem where the BodyLabel control isn't initialised, causing
				// a NullReferenceException.  The original exception (ex) was being lost.
				// Why is BodyLabel null?  (drewnoakes: I saw this problem while working with
				// invalid Xsl in file modifications.xsl)
				if (BodyLabel == null)
					throw new CruiseControlException("Unable to render page.", ex);

				if (BodyLabel.InnerText == null)
					BodyLabel.InnerText = string.Empty;

				BodyLabel.InnerText += new HtmlExceptionFormatter(ex).ToString();
			}
		}

		private void ApplyStylesheetsToRenderContent()
		{
			StringBuilder builder = new StringBuilder();
			try
			{
				string logfile = WebUtil.ResolveLogFile(Context);
				XPathDocument document = new XPathDocument(logfile);

				IList list = GetSummaryXslFiles();
				foreach (string xslFile in list)
				{
					if (xslFile.ToLower().IndexOf("header") > -1) // header content goes first
					{
						builder.Insert(0, Transform(xslFile, document));
					}
					else
					{
						builder.Append(Transform(xslFile, document)).Append("<br>");
					}
				}
			}
			catch (XmlException ex)
			{
				throw new CruiseControlException(String.Format("Bad XML in logfile: " + ex.Message));
			}

			Contents.InnerHtml = builder.ToString();
		}

		private IList GetSummaryXslFiles()
		{
			return (IList) ConfigurationSettings.GetConfig("CCNet/xslFiles");
		}

		private string Transform(string xslfile, XPathDocument logFileDocument)
		{
			string directory = Path.GetDirectoryName(xslfile);
			string file = Path.GetFileName(xslfile);
			string transformFile = Path.Combine(Request.MapPath(directory), file);
			return new BuildLogTransformer().Transform(logFileDocument, transformFile);
		}

		#region Web Form Designer generated code

		protected override void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}

		private void InitializeComponent()
		{
			this.Load += new EventHandler(this.Page_Load);
		}

		#endregion
	}
}