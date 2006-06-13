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
    public class EmailPublisher : ITask
    {
        private EmailGateway emailGateway = new EmailGateway();
        private string fromAddress;
		private string replytoAddress;
        private Hashtable users = new Hashtable();
        private Hashtable groups = new Hashtable();
        private IMessageBuilder messageBuilder;

    	public EmailPublisher() : this(new HtmlLinkMessageBuilder(false))
    	{}

    	public EmailPublisher(IMessageBuilder messageBuilder)
    	{
    		this.messageBuilder = messageBuilder;
    	}

    	public EmailGateway EmailGateway
        {
            get { return emailGateway; }
            set { emailGateway = value; }
        }

        public IMessageBuilder MessageBuilder
        {
            get { return messageBuilder; }
            set { messageBuilder = value; }
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
            get { return fromAddress; }
            set { fromAddress = value; }
        }

		[ReflectorProperty("replyto", Required = false)] 
		public string ReplyToAddress
		{
			get { return replytoAddress; }
			set { replytoAddress = value; }
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
				return messageBuilder is HtmlDetailsMessageBuilder; 
			}
            set
            {
                if (value)
                {
                	messageBuilder = new HtmlDetailsMessageBuilder();
                }
                else
                {
                	messageBuilder = new HtmlLinkMessageBuilder(false);
                }
            }
        }

        [ReflectorHash("users", "name")] 
		public Hashtable EmailUsers
        {
            get { return users; }
            set { users = value; }
        }

        [ReflectorHash("groups", "name")] 
		public Hashtable EmailGroups
        {
            get { return groups; }
            set { groups = value; }
        }

        public void Run(IIntegrationResult result)
        {
            if (result.Status == IntegrationStatus.Unknown)
                return;

        	EmailMessage  emailMessage = new EmailMessage(result, this); 
            string to = emailMessage.Recipients;
			string subject = emailMessage.Subject;
            string message = CreateMessage(result);
            if (IsRecipientSpecified(to))
            {
                SendMessage(fromAddress, to, replytoAddress, subject, message);
            }
	    }

        private bool IsRecipientSpecified(string to)
        {
            return to != null && to.Trim() != string.Empty;
        }

        public virtual void SendMessage(string from, string to, string replyto, string subject, string message)
        {
            try
            {
            	emailGateway.Send(from, to, replyto, subject, message);
            }
            catch (Exception e)
            {
                throw new CruiseControlException("EmailPublisher exception: " + e.ToString(), e);
            }
        }

        public string CreateMessage(IIntegrationResult result)
        {
            // TODO Add culprit to message text -- especially if modifier is not an email user
            //      This information is included, when using Html email (all mods are shown)
        	try
        	{
        		return messageBuilder.BuildMessage(result);
        	}
        	catch (Exception e)
        	{
        		string message = "Unable to build email message: " + e;
        		Log.Error(message);
				return message;
        	}
        }
    }
}