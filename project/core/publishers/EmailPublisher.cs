using System;
using System.Collections;
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
        private EmailGateway _emailGateway = new EmailGateway();
        private string _projectUrl;
        private string _fromAddress;
        private Hashtable _users = new Hashtable();
        private Hashtable _groups = new Hashtable();
        private IMessageBuilder _messageBuilder = new HtmlLinkMessageBuilder(false);

        public EmailGateway EmailGateway
        {
            get { return _emailGateway; }
            set { _emailGateway = value; }
        }

        public IMessageBuilder MessageBuilder
        {
            get { return _messageBuilder; }
            set { _messageBuilder = value; }
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
        /// This property is deprecated and should be removed.  It should be replaced by the MessageBuilder property.
        /// </summary>
        [ReflectorProperty("includeDetails", Required = false)] 
		public bool IncludeDetails
        { 
            get 
			{
				return _messageBuilder is HtmlDetailsMessageBuilder; 
			}
            set
            {
                if (value)
                {
                    _messageBuilder = new HtmlDetailsMessageBuilder();
                }
                else
                {
                    _messageBuilder = new HtmlLinkMessageBuilder(false);
                }
            }
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
			if (username == null) return null;
            return (EmailUser) _users[username];
        }

        public EmailGroup GetEmailGroup(string groupname)
        {
            return (EmailGroup) _groups[groupname];
        }

        public override void PublishIntegrationResults(IProject project, IIntegrationResult result)
        {
            if (result.Status == IntegrationStatus.Unknown)
                return;

            string to = CreateRecipientList(result);
            string subject = CreateSubject(result);
            string message = _messageBuilder.BuildMessage(result, ProjectUrl);
            if (IsRecipientSpecified(to))
            {
                SendMessage(_fromAddress, to, subject, message);
            }
        }

        private bool IsRecipientSpecified(string to)
        {
            return to != null && to.Trim() != string.Empty;
        }

        internal virtual void SendMessage(string from, string to, string subject, string message)
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

        internal string CreateSubject(IIntegrationResult result)
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
            return _messageBuilder.BuildMessage(result, ProjectUrl);
        } 

        internal string CreateRecipientList(IIntegrationResult result)
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

        private bool BuildStateChanged(IIntegrationResult result)
        {
            return result.LastIntegrationStatus != result.Status;
        }
    }
}