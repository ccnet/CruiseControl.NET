using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

using Exortech.NetReflector;
using tw.ccnet.core.util;
using tw.ccnet.remote;


namespace tw.ccnet.core.publishers
{
	[ReflectorType("email")]
	public class EmailPublisher : PublisherBase
	{
		private EmailGateway _emailGateway = new EmailGateway();
		private string _projectUrl;
		private string _fromAddress;
		private Hashtable _users = new Hashtable();
		private Hashtable _groups = new Hashtable();
		private bool _includeDetails = false;		

		private XmlLogPublisher logPublisher;

		public EmailGateway EmailGateway
		{
			get { return _emailGateway; }
			set { _emailGateway = value; }
		}

		#region Configuration Properties
		[ReflectorProperty("mailhost")]
		public string MailHost
		{
			get { return EmailGateway.MailHost; }
			set { EmailGateway.MailHost = value; }
		}

		[ReflectorProperty("from")]
		public string FromAddress
		{
			get { return _fromAddress; }
			set { _fromAddress = value; }
		}
		
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

		public override void Publish(object source, IntegrationResult result)
		{
			if (result.Status == IntegrationStatus.Unknown)
			{
				return;
			}

			Project p = (Project) source;
			if (p != null) 
			{
				foreach (PublisherBase publisher in p.Publishers) 
				{
					if (publisher is XmlLogPublisher) 
					{
						logPublisher = (XmlLogPublisher) publisher;
						break;
					}
				}
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

		internal string CreateSubject(IntegrationResult result)
		{
			if (result.Status == IntegrationStatus.Success)
			{
				if (BuildStateChanged(result))
				{
					return String.Format("{0} {1} {2}", 
						result.ProjectName, "Build Fixed: Build", result.Label);
				}
				else
				{
					return String.Format("{0} {1} {2}", 
						result.ProjectName, "Build Successful: Build", result.Label);
				}
			}
			else
			{
				return String.Format("{0} {1}", 
					result.ProjectName, "Build Failed");
			}
		}

		internal string CreateMessage(IntegrationResult result) 
		{
			// TODO Add culprit to message text -- especially if modifier is not an email user
			if(_includeDetails) 
			{
				return CreateHtmlMessage(result);
			}
			else
			{
				return CreateLinkMessage(result);
			}
		}
		
		private string CreateLinkMessage(IntegrationResult result)
		{
			return String.Format(@"CC.NET Build Results for {0}: {1}", 
				result.ProjectName, LogFile.CreateUrl(ProjectUrl, result)) ;
		}

		private string CreateHtmlMessage(IntegrationResult result)
		{
			StringBuilder message = new StringBuilder(10000);
			message.Append("<html><head></head><body>");
			message.Append(CreateLinkMessage(result));
			AppendHtmlMessageDetails(result, message);
			message.Append("</body></html>");
			return message.ToString();
		}

		private void AppendHtmlMessageDetails(IntegrationResult result, StringBuilder message)
		{
			StringWriter buffer = new StringWriter();
			XmlTextWriter writer = new XmlTextWriter(buffer);
			if (logPublisher != null)
				logPublisher.Write(result, writer);
			else
				new XmlLogPublisher().Write(result, writer);
			writer.Close();
			
			XmlDocument xml = new XmlDocument();
			xml.LoadXml(buffer.ToString());
			message.Append(BuildLogTransformer.Transform(xml));
		}
				
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

		private bool BuildStateChanged(IntegrationResult result)
		{
			return result.LastIntegrationStatus != result.Status;
		}

	}
}
