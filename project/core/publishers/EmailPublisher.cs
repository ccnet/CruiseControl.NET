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
	/// TODO document email recipient rules a little here...
	[ReflectorType("email")]
	public class EmailPublisher : PublisherBase
	{
		#region Field declarations

		EmailGateway _emailGateway = new EmailGateway();
		string _projectUrl;
		string _fromAddress;
		Hashtable _users = new Hashtable();
		Hashtable _groups = new Hashtable();
		bool _includeDetails = false;		

		#endregion

		public EmailGateway EmailGateway
		{
			get { return _emailGateway; }
			set { _emailGateway = value; }
		}

		#region Configuration Properties

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
		[ReflectorProperty("includeDetails", Required=false)]
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

		#endregion

		public EmailUser GetEmailUser(string username)
		{
			return (EmailUser)_users[username];
		}

		public EmailGroup GetEmailGroup(string groupname)
		{
			return (EmailGroup)_groups[groupname];
		}

		public override void PublishIntegrationResults(IProject project, IntegrationResult result)
		{
			if (result.Status==IntegrationStatus.Unknown)
			{
				return;
			}

			string to = CreateRecipientList(result);
			string subject = CreateSubject(result);
			string message = CreateMessage(result);
			SendMessage(_fromAddress, to, subject, message);
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

		#region Creating email content

		internal string CreateSubject(IntegrationResult result)
		{
			if (result.Status == IntegrationStatus.Success)
			{
				if (BuildStateChanged(result))
				{
					return string.Format("{0} {1} {2}", 
						result.ProjectName, "Build Fixed: Build", result.Label);
				}
				else
				{
					return string.Format("{0} {1} {2}", 
						result.ProjectName, "Build Successful: Build", result.Label);
				}
			}
			else
			{
				return string.Format("{0} {1}", 
					result.ProjectName, "Build Failed");
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
		
		string CreateLinkMessage(IntegrationResult result, bool makeHyperlink)
		{
			string link = LogFileUtil.CreateUrl(ProjectUrl, result);
			
			if (makeHyperlink)
				link = string.Format("<a href='{0}'>view results</a>", link);

			return string.Format("CruiseControl.NET Build Results for project {0}: {1}", 
				result.ProjectName, link);
		}

		#region HTML email stuff

		/// <summary>
		/// Creates an HTML representation of the build result as a string, intended
		/// to be included in the email message whenever 'IncludeDetails' is set to
		/// true.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		string CreateHtmlMessage(IntegrationResult result)
		{
			StringBuilder message = new StringBuilder(10000);
			
			// open HTML tags
			message.Append(string.Format("<html><head>{0}</head><body bgcolor='#DEDEF7'>", HtmlEmailCss));

			// include a link to the build results page
			message.Append(CreateLinkMessage(result, true));
			
			// append html details of the build
			AppendHtmlMessageDetails(result, message);
			
			// close HTML tags
			message.Append("</body></html>");

			return message.ToString();
		}

		void AppendHtmlMessageDetails(IntegrationResult result, StringBuilder message)
		{
			StringWriter buffer = new StringWriter();
			XmlTextWriter writer = new XmlTextWriter(buffer);
			new XmlLogPublisher().Write(result, writer);
			writer.Close();
			
			XmlDocument xml = new XmlDocument();
			xml.LoadXml(buffer.ToString());
			message.Append(BuildLogTransformer.TransformResultsWithAllStyleSheets(xml));
		}

		const string HtmlEmailCss = @"<style>
BODY { font-family: verdana, arial, helvetica, sans-serif; font-size:9pt; }
</style>";

		#endregion

		#endregion

		#region Collating email recipients
				
		internal string CreateRecipientList(IntegrationResult result)
		{
			string[] always = CreateNotifyList(EmailGroup.NotificationType.Always);
			string[] modifiers = CreateModifiersList(result.Modifications);

			if (BuildStateChanged(result))
			{
				string[] change = CreateNotifyList(EmailGroup.NotificationType.Change);
				return StringUtil.JoinUnique(", ", always, change, modifiers);
			}
			else
			{
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
			return (string[])modifiers.ToArray(typeof(string));
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
			return (string[])userList.ToArray(typeof(string));
		}

		#endregion

		private bool BuildStateChanged(IntegrationResult result)
		{
			return result.LastIntegrationStatus != result.Status;
		}
	}
}
