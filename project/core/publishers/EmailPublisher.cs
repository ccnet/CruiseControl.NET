using System;
using System.Collections;
using Exortech.NetReflector;
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

        public override void PublishIntegrationResults(IIntegrationResult result)
        {
            if (result.Status == IntegrationStatus.Unknown)
                return;

        	EmailMessage  emailMessage = new EmailMessage(result, this); 
            string to = emailMessage.Recipients;
            string subject = emailMessage.Subject;
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

        public virtual void SendMessage(string from, string to, string subject, string message)
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

        internal string CreateMessage(IIntegrationResult result)
        {
            // TODO Add culprit to message text -- especially if modifier is not an email user
            //      This information is included, when using Html email (all mods are shown)
            return _messageBuilder.BuildMessage(result, ProjectUrl);
        }
    }
}