using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	/// <summary>
	/// Publishes results of integrations via email.  This implementation supports
	/// plain-text, and Html email formats.  Rules regarding who receives email
	/// are configurable.
	/// </summary>
	[ReflectorType("email")]
	public class EmailPublisher : PublisherBase
	{
		EmailGateway _emailGateway = new EmailGateway();
		string _projectUrl;
		string _fromAddress;
		Hashtable _users = new Hashtable();
		Hashtable _groups = new Hashtable();
		bool _includeDetails = false;

		public EmailGateway EmailGateway
		{
			get { return _emailGateway; }
			set { _emailGateway = value; }
		}

		/// <summary>
		/// The host name of the mail server.  This field is required to send email notifications.
		/// </summary>
		[ReflectorProperty("mailhost")] 
		public string MailHost
		{
			get { return EmailGateway.MailHost; }
			set { EmailGateway.MailHost = value; }
		}

		/// <summary>
		/// The email address from which build results appear to have originated from.  This
		/// value seems to be required for most mail servers.
		/// </summary>
		[ReflectorProperty("from")] 
		public string FromAddress
		{
			get { return _fromAddress; }
			set { _fromAddress = value; }
		}

		/// <summary>
		/// Set this property (in configuration) to enable HTML emails containing build details.
		/// </summary>
		[ReflectorProperty("includeDetails", Required = false)] 
		public bool IncludeDetails
		{
			get { return _includeDetails; }
			set { _includeDetails = value; }
		}

		[ReflectorProperty("projectUrl")] 
		public string ProjectUrl
		{
			get { return _projectUrl; }
			set { _projectUrl = value; }
		}

		[ReflectorHash("users", "name")] 
		public Hashtable EmailUsers
		{
			get { return _users; }
			set { _users = value; }
		}

		[ReflectorHash("groups", "name")] 
		public Hashtable EmailGroups
		{
			get { return _groups; }
			set { _groups = value; }
		}

		public EmailUser GetEmailUser(string username)
		{
			return (EmailUser) _users[username];
		}

		public EmailGroup GetEmailGroup(string groupname)
		{
			return (EmailGroup) _groups[groupname];
		}

		public override void PublishIntegrationResults(IProject project, IntegrationResult result)
		{
			if (result.Status == IntegrationStatus.Unknown)
				return;

			string to = CreateRecipientList(result);
			string subject = CreateSubject(result);
			string message = CreateMessage(result);
			if (IsRecipientSpecified(to))
			{
				SendMessage(_fromAddress, to, subject, message);
			}
		}

		private bool IsRecipientSpecified(string to)
		{
			return to != null && to.Trim() != string.Empty;
		}

		internal void SendMessage(string from, string to, string subject, string message)
		{
			try
			{
				_emailGateway.Send(from, to, subject, message);
			}
			catch (Exception e)
			{
				throw new CruiseControlException("EmailPublisher exception: " + e.ToString(), e);
			}
		}

		internal string CreateSubject(IntegrationResult result)
		{
			if (result.Status == IntegrationStatus.Success)
			{
				if (BuildStateChanged(result))
				{
					return string.Format("{0} {1} {2}", result.ProjectName, "Build Fixed: Build", result.Label);
				}
				else
				{
					return string.Format("{0} {1} {2}", result.ProjectName, "Build Successful: Build", result.Label);
				}
			}
			else
			{
				return string.Format("{0} {1}", result.ProjectName, "Build Failed");
			}
		}

		internal string CreateMessage(IntegrationResult result)
		{
			// TODO Add culprit to message text -- especially if modifier is not an email user
			//      This information is included, when using Html email (all mods are shown)
			if (_includeDetails)
				return CreateHtmlMessage(result);
			else
				return CreateLinkMessage(result, false);
		}

		private string CreateLinkMessage(IntegrationResult result, bool makeHyperlink)
		{
			string link = LogFileUtil.CreateUrl(ProjectUrl, result);

			if (makeHyperlink)
				link = string.Format("<a href='{0}'>web page</a>", link);

			return string.Format("CruiseControl.NET Build Results for project {0} ({1})", result.ProjectName, link);
		}

		/// <summary>
		/// Creates an HTML representation of the build result as a string, intended
		/// to be included in the email message whenever 'IncludeDetails' is set to
		/// true.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		private string CreateHtmlMessage(IntegrationResult result)
		{
			StringBuilder message = new StringBuilder(10000);

			// open HTML tags
			message.Append(string.Format("<html><head>{0}</head><body>", HtmlEmailCss));

			// include a link to the build results page
			message.Append(CreateLinkMessage(result, true));

			// insert a dividing line
			message.Append(@"<p></p><hr size=""1"" width=""98%"" align=""left"" color=""#888888""/>");

			// append html details of the build
			AppendHtmlMessageDetails(result, message);

			// close HTML tags
			message.Append("</body></html>");

			return message.ToString();
		}

		private void AppendHtmlMessageDetails(IntegrationResult result, StringBuilder message)
		{
			StringWriter buffer = new StringWriter();
			XmlTextWriter writer = new XmlTextWriter(buffer);
			new XmlLogPublisher().Write(result, writer);
			writer.Close();

			XmlDocument xml = new XmlDocument();
			xml.LoadXml(buffer.ToString());
			message.Append(BuildLogTransformer.TransformResultsWithAllStyleSheets(xml));
		}

		// for now, this is simply a copy of the contents of web/cruisecontrol.css
		// TODO read this from the actual file (need a way to access the file from this publisher)
		private const string HtmlEmailCss = @"<style>
body, table, form, input, td, th, p, textarea, select
{
	font-family: verdana, helvetica, arial;
	font-size: 11px;
}

a:hover { color:#FC0; }

.main-panel { color:#FC0; }

.link { color:#FFF; text-decoration:none; }
.link-failed { color:#F30; text-decoration:none; }
.buildresults-header { color: #FFF; font-weight: bold; }
.buildresults-data { color: #9F3; }
.buildresults-data-failed { color: #F30; }

.stylesection { margin-left: 4px; }
.header-title { font-size:12px; color:#000; font-weight:bold; padding-bottom:10pt; }
.header-label { font-weight:bold; }
.header-data { color:#000; }
.header-data-error { font-family:courier, monospaced; color:#000; white-space:pre; }

.modifications-data { font-size:9px; color:#000; }
.modifications-sectionheader { background-color:#006; color:#FFF; }
.modifications-oddrow { background-color:#F0F7FF; }
.modifications-evenrow { background-color:#FFF; }

.changelists-oddrow { background-color:#F0F7FF; }
.changelists-evenrow { background-color:#FFF; }
.changelists-file-spacer { background-color:#FFF; }
.changelists-file-evenrow { background-color:#FFF; }
.changelists-file-oddrow { background-color:#F0F7FF; }
.changelists-file-header { font-size:9px; background-color:#666; color:#FFF; }

.compile-data { font-size:9px; color:#000; }
.compile-error-data { font-size:9px; color:#F30; white-space:pre; }
.compile-warn-data { font-size:9px; color:#C90; white-space:pre; }
.compile-sectionheader { background-color:#006; color:#FFF; }

.distributables-data { font-size:9px; color:#000; }
.distributables-sectionheader { background-color:#006; color:#FFF; }
.distributables-oddrow { background-color:#F0F7FF; }

.unittests-sectionheader { background-color:#006; color:#FFF; }
.unittests-oddrow { background-color:#F0F7FF; }
.unittests-data { font-size:9px; color:#000; }
.unittests-error { font-size:9px; color:#F30; white-space:pre; }

.javadoc-sectionheader { background-color:#006; color:#FFF; }
.javadoc-oddrow { background-color:#CCC; }
.javadoc-data { font-size:9px; color:#000; }
.javadoc-error { font-size:9px; color:#F30; white-space:pre; }
.javadoc-warning { font-size:9px; color:#000; white-space:pre; }

.section-table { margin-top:10px; }
</style>";

		internal string CreateRecipientList(IntegrationResult result)
		{
			string[] always = CreateNotifyList(EmailGroup.NotificationType.Always);
			if (BuildStateChanged(result))
			{
				string[] change = CreateNotifyList(EmailGroup.NotificationType.Change);
				return StringUtil.JoinUnique(", ", always, change);
			}
			else
			{
				string[] modifiers = CreateModifiersList(result.Modifications);
				return StringUtil.JoinUnique(", ", always, modifiers);
			}
		}

		internal string[] CreateModifiersList(Modification[] modifications)
		{
			ArrayList modifiers = new ArrayList(modifications.Length);
			foreach (Modification modification in modifications)
			{
				EmailUser user = GetEmailUser(modification.UserName);
				if (user != null)
				{
					modifiers.Add(user.Address);
				}
			}
			return (string[]) modifiers.ToArray(typeof(string));
		}

		internal string[] CreateNotifyList(EmailGroup.NotificationType notification)
		{
			ArrayList userList = new ArrayList();
			foreach (EmailUser user in EmailUsers.Values)
			{
				EmailGroup group = GetEmailGroup(user.Group);
				if (group != null && group.Notification.Equals(notification))
				{
					userList.Add(user.Address);
				}
			}
			return (string[]) userList.ToArray(typeof(string));
		}

		private bool BuildStateChanged(IntegrationResult result)
		{
			return result.LastIntegrationStatus != result.Status;
		}
	}
}