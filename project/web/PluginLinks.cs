using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace ThoughtWorks.CruiseControl.Web
{
	public class PluginLinks : HtmlGenericControl
	{
		public PluginLinks() : base("div")
		{
			base.Attributes.Add("class", "pluginLinks");
		}

		public PluginLinks(string tag) : this()
		{
		}

		protected override void Render(HtmlTextWriter writer)
		{
			base.RenderBeginTag(writer);
			string logfile = WebUtil.ResolveLogFile(Context);
			if (ConfigurationSettings.GetConfig("CCNet/buildPlugins") == null)
			{
				return;
			}

			bool isFirstSpec = true;
			foreach (PluginSpecification spec in (IEnumerable) ConfigurationSettings.GetConfig("CCNet/buildPlugins"))
			{
				if (! isFirstSpec)
				{
					writer.Write("|&nbsp; ");
				}
				writer.Write(@"<a class=""link"" href=""{0}"">{1}</a> ", GenerateLogUrl(spec.LinkUrl, logfile), spec.LinkText);
				isFirstSpec = false;
			}
			base.RenderEndTag(writer);
		}

		private string GenerateLogUrl(string urlPrefix, string logfile)
		{
			return ResolveUrl(String.Format("{0}{1}", urlPrefix, new FileInfo(logfile).Name));
		}
	}
}
