namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Mail;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Config;
    using ThoughtWorks.CruiseControl.Core.Tasks;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// <para>
    /// Publishes results of integrations via email.  This implementation supports plain-text, and Html email formats.
    /// Rules regarding who receives email are configurable.
    /// </para>
    /// <para>
    /// The email publisher can be used to send email to any number of users. It is common to include one user who gets
    /// an email for every build and then also send email to every developer who checked code in for this build.
    /// </para>
    /// <para type="tip">
    /// People tend to prefer to use <link>CCTray</link> rather than email for instant notification these days.
    /// </para>
    /// <para type="warning">
    /// Make sure that all of the Merge Publishers, along with the <link>Xml Log Publisher</link> task are done before
    /// the &lt;email&gt; publisher, or else you won't be able to include output from the build in the email. A common
    /// mistake is to put the email task in the &lt;tasks&gt; section instead of the &lt;publishers&gt; section. If an
    /// error occurs in the &lt;tasks&gt; section, the remaining tasks in that section are skipped, and CC.Net goes
    /// right to the &lt;publishers&gt; section. So if you put the &lt;email&gt; tasks in the &lt;tasks&gt; section, 
    /// you'll never get any failure messages.
    /// </para>
    /// </summary>
    /// <title>Email Publisher</title>
    /// <version>1.0</version>
    /// <example>
    /// <code>
    /// &lt;email mailport="25" includeDetails="TRUE" mailhostUsername="smtpuser" mailhostPassword="smtppassword" useSSL="FALSE"&gt;
    /// &lt;from&gt;buildmaster@mycompany.com&lt;/from&gt;
    /// &lt;mailhost&gt;smtp.mycompany.com&lt;/mailhost&gt;
    /// &lt;users&gt;
    /// &lt;user name="BuildGuru" group="buildmaster" address="buildguru@mycompany.com"/&gt;
    /// &lt;user name="JoeDeveloper" group="developers" address="joedeveloper@thoughtworks.com"/&gt;
    /// &lt;/users&gt;
    /// &lt;groups&gt;
    /// &lt;group name="developers"&gt;
    /// &lt;notifications&gt;
    /// &lt;notificationType&gt;Failed&lt;/notificationType&gt;
    /// &lt;notificationType&gt;Fixed&lt;/notificationType&gt;
    /// &lt;/notifications&gt;
    /// &lt;/group&gt;
    /// &lt;group name="buildmaster" &gt;
    /// &lt;notifications&gt;
    /// &lt;notificationType&gt;Always&lt;/notificationType&gt;
    /// &lt;/notifications&gt;
    /// &lt;/group&gt;
    /// &lt;/groups&gt;
    /// &lt;converters&gt;
    /// &lt;regexConverter find="$" replace="@TheCompany.com" /&gt;
    /// &lt;/converters&gt;
    /// &lt;modifierNotificationTypes&gt;
    /// &lt;NotificationType&gt;Failed&lt;/NotificationType&gt;
    /// &lt;NotificationType&gt;Fixed&lt;/NotificationType&gt;
    /// &lt;/modifierNotificationTypes&gt;
    /// &lt;subjectSettings&gt;
    /// &lt;subject buildResult="StillBroken" value="Build is still broken for {CCNetProject}" /&gt;
    /// &lt;/subjectSettings&gt;
    /// &lt;xslFiles&gt;
    /// &lt;file&gt;xsl\header.xsl&lt;/file&gt;
    /// &lt;file&gt;xsl\compile.xsl&lt;/file&gt;
    /// &lt;file&gt;xsl\unittests.xsl&lt;/file&gt;
    /// &lt;file&gt;xsl\modifications.xsl&lt;/file&gt;
    /// &lt;/xslFiles&gt;
    /// &lt;attachments&gt;
    /// &lt;file&gt;C:\Data\AFile.txt&lt;/file&gt;
    /// &lt;file&gt;Relative.txt&lt;/file&gt;
    /// &lt;/attachments&gt;
    /// &lt;/email&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <heading>HTML E-mails</heading>
    /// <para>
    /// When includedDetails = True, the message body will contain more information. This detailed information is
    /// constructed from xsl transformations on the build log. If the xslFiles section is filled these files will be
    /// used, if not defined the xls files are defined in the ccnet.exe.config in the xslFiles section. (see 
    /// also: <link>Server Application Config File</link>). When adjusting one of these, restart the console/service.
    /// </para>
    /// <para>
    /// The benefits of defining the xsl files in the email publisher: 
    /// </para>
    /// <list type="1">
    /// <item>
    /// the xsl files are automatically the same for the console as for the service (no more keeping these 2 in sync)
    /// </item>
    /// <item>
    /// it is possible to give projects different xsl transformations
    /// </item>
    /// </list>
    /// <para/>
    /// <para type="warning">
    /// The groups node may be empty, but the group section must exist.
    /// </para>
    /// <heading>GMail</heading>
    /// <para>
    /// For sending mail via gmail :
    /// <list type="1">
    /// <item>mailhost="smtp.gmail.com"</item>
    /// <item>mailport="587"</item>
    /// <item>mailhostUsername="xxx.yyy@gmail.com"</item>
    /// <item>mailhostPassword="yourpassword"</item>
    /// <item>useSSL="TRUE"</item>
    /// </list>
    /// </para>
    /// </remarks>
    [ReflectorType("email")]
    public class EmailPublisher 
        : TaskBase, IConfigurationValidation
    {
        private EmailGateway emailGateway = new EmailGateway();
        private string fromAddress;
        private string replytoAddress;
        private string subjectPrefix;
        private IMessageBuilder messageBuilder;
        private EmailGroup.NotificationType[] modifierNotificationTypes = { EmailGroup.NotificationType.Always };
        private IEmailConverter[] converters = new IEmailConverter[0];

        private EmailSubject[] subjectSettings = new EmailSubject[0];

        private string[] xslFiles;

        public EmailPublisher()
            : this(new HtmlLinkMessageBuilder(false))
        { }

        public EmailPublisher(IMessageBuilder messageBuilder)
        {
            this.messageBuilder = messageBuilder;
            this.IndexedEmailUsers = new Dictionary<string, EmailUser>();
            this.IndexedEmailGroups = new Dictionary<string, EmailGroup>();
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
        /// The SMTP server that CruiseControl.NET will connect to to send email.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("mailhost")]
        public string MailHost
        {
            get { return EmailGateway.MailHost; }
            set { EmailGateway.MailHost = value; }
        }

        /// <summary>
        /// The SMTP server port number.
        /// </summary>
        /// <version>1.0</version>
        /// <default>25</default>
        [ReflectorProperty("mailport", Required = false)]
        public int MailPort
        {
            get { return EmailGateway.MailPort; }
            set { EmailGateway.MailPort = value; }
        }

        /// <summary>
        /// The user name to provide to the SMTP server.
        /// </summary>
        /// <version>1.2</version>
        /// <default>None</default>
        [ReflectorProperty("mailhostUsername", Required = false)]
        public string MailhostUsername
        {
            get { return EmailGateway.MailHostUsername; }
            set { EmailGateway.MailHostUsername = value; }
        }

        /// <summary>
        /// The password to provide to the SMTP server. 
        /// </summary>
        /// <version>1.2</version>
        /// <default>None</default>
        [ReflectorProperty("mailhostPassword", typeof(PrivateStringSerialiserFactory), Required = false)]
        public PrivateString MailhostPassword
        {
            get { return EmailGateway.MailHostPassword; }
            set { EmailGateway.MailHostPassword = value; }
        }

        /// <summary>
        /// The e-mail address that email will be marked as coming from. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("from")]
        public string FromAddress
        {
            get { return fromAddress; }
            set { fromAddress = value; }
        }

        /// <summary>
        /// Whether to use SSL or not for sending the e-mail.
        /// </summary>
        /// <version>1.0</version>
        /// <default>false</default>
        [ReflectorProperty("useSSL", Required = false)]
        public bool UseSSL
        {
            get { return EmailGateway.UseSSL; }
            set { EmailGateway.UseSSL = value; }
        }

        /// <summary>
        /// The e-mail address to use for replies. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("replyto", Required = false)]
        public string ReplyToAddress
        {
            get { return replytoAddress; }
            set { replytoAddress = value; }
        }

        /// <summary>
        /// A list of xsl files that will be used to fill up the message body, if left blank the list will be taken
        /// from ccnet.exe.config or ccservice.exe.config.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("xslFiles", Required = false)]
        public string[] XslFiles
        {
            get { return xslFiles; }
            set { xslFiles = value; }
        }

        /// <summary>
        /// A list of files to attach to the e-mail. If the full path is not specified, then it will be relative to the
        /// project working directory.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("attachments", Required = false)]
        public string[] Attachments { get; set; }

        /// <summary>
        /// Whether to send a full report or not. If not, just sends a simple status message with a link to the build 
        /// report.
        /// </summary>
        /// <version>1.0</version>
        /// <default>false</default>
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

        /// <summary>
        /// A set of &lt;NotificationType&gt; elements, specifying build states for which CruiseControl.Net should
        /// send an email to the comitters of the build.
        /// </summary>
        /// <version>1.0</version>
        /// <default>Always</default>
        [ReflectorProperty("modifierNotificationTypes", Required = false)]
        public EmailGroup.NotificationType[] ModifierNotificationTypes
        {
            get { return modifierNotificationTypes; }
            set { modifierNotificationTypes = value; }
        }

        /// <summary>
        /// A set of &lt;user&gt; elements that define who to send emails to. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        /// <dataType>ThoughtWorks.CruiseControl.Core.Publishers.EmailUser</dataType>
        [ReflectorProperty("users")]
        public EmailUser[] EmailUsers
        {
            get
            {
                var values = new EmailUser[this.IndexedEmailUsers.Count];
                this.IndexedEmailUsers.Values.CopyTo(values, 0);
                return values;
            }
            set
            {
                this.IndexedEmailUsers.Clear();
                foreach (var user in value)
                {
                    this.IndexedEmailUsers.Add(user.Name, user);
                }
            }
        }

        /// <summary>
        /// Gets the email users via an index.
        /// </summary>
        /// <value>The indexed email users.</value>
        public Dictionary<string, EmailUser> IndexedEmailUsers { get; private set; }

        /// <summary>
        /// A set of &lt;group&gt; elements that identify which the notification policy for a set of users. 
        /// </summary>
        /// <version>1.3</version>
        /// <default>n/a</default>
        /// <dataType>ThoughtWorks.CruiseControl.Core.Publishers.EmailGroup</dataType>
        [ReflectorProperty("groups")]
        public EmailGroup[] EmailGroups
        {
            get
            {
                var values = new EmailGroup[this.IndexedEmailGroups.Count];
                this.IndexedEmailGroups.Values.CopyTo(values, 0);
                return values;
            }
            set
            {
                this.IndexedEmailGroups.Clear();
                foreach (var group in value)
                {
                    this.IndexedEmailGroups.Add(group.Name, group);
                }
            }
        }

        /// <summary>
        /// Gets the email groups via an index.
        /// </summary>
        /// <value>The indexed email groups.</value>
        public Dictionary<string, EmailGroup> IndexedEmailGroups { get; private set; }

        /// <summary>
        /// A set of &lt;subject&gt; elements that define the subject of the email, according to the state of the build 
        /// (broken, fixed, ...)
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        /// <dataType>ThoughtWorks.CruiseControl.Core.Publishers.EmailSubject</dataType>
        [ReflectorProperty("subjectSettings", Required = false)]
        public EmailSubject[] SubjectSettings
        {
            get { return subjectSettings; }
            set { subjectSettings = value; }
        }

        /// <summary>
        /// A set of elements containing rules for creating email adresses based on the modifiers name. The converters 
        /// will be used when the name of the modifier is not set in the users section. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("converters", Required = false)]
        public IEmailConverter[] Converters
        {
            get { return converters; }
            set { converters = value; }
        }

        /// <summary>
        /// A string that will be the first string of the subject. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("subjectPrefix", Required = false)]
        public string SubjectPrefix
        {
            get { return subjectPrefix; }
            set { subjectPrefix = value; }
        }

        protected override bool Execute(IIntegrationResult result)
        {
            if (result.Status == IntegrationStatus.Unknown) return false;

            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description : "Emailing ...");

            EmailMessage emailMessage = new EmailMessage(result, this);
            string to = emailMessage.Recipients;
            string subject = emailMessage.Subject;
            string message = CreateMessage(result);
            if (IsRecipientSpecified(to))
            {
                Log.Info(string.Format("Emailing \"{0}\" to {1}", subject, to));
                SendMessage(fromAddress, to, replytoAddress, subject, message, result.WorkingDirectory);
            }

            return true;
        }

        private static bool IsRecipientSpecified(string to)
        {
            return to != null && to.Trim() != string.Empty;
        }

        public virtual void SendMessage(string from, string to, string replyto, string subject, string message, string workingFolder)
        {
            try
            {
                using (var actualMessage = GetMailMessage(from, to, replyto, subject, message, workingFolder, Attachments))
                {
                    emailGateway.Send(actualMessage);
                }
            }
            catch (Exception e)
            {
                throw new CruiseControlException("EmailPublisher exception: " + e, e);
            }
        }

        protected static MailMessage GetMailMessage(string from, string to, string replyto, string subject, string messageText, string workingFolder, string[] attachments)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(to);
            mailMessage.From = new MailAddress(from);
            if (!String.IsNullOrEmpty(replyto)) mailMessage.ReplyTo = new MailAddress(replyto);
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = messageText;

            // Add any attachments
            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    var fullPath = attachment;
                    if (!Path.IsPathRooted(fullPath)) fullPath = Path.Combine(workingFolder, fullPath);
                    if (File.Exists(fullPath))
                    {
                        var mailAttachment = new Attachment(fullPath);
                        mailMessage.Attachments.Add(mailAttachment);
                    }
                }
            }

            return mailMessage;
        }

        public string CreateMessage(IIntegrationResult result)
        {
            // TODO Add culprit to message text -- especially if modifier is not an email user
            //      This information is included, when using Html email (all mods are shown)
            try
            {
                messageBuilder.xslFiles = this.XslFiles;
                return messageBuilder.BuildMessage(result);
            }
            catch (Exception e)
            {
                string message = "Unable to build email message: " + e;
                Log.Error(message);
                return message;
            }
        }

        #region Validate()
        /// <summary>
        /// Checks the internal validation of the item.
        /// </summary>
        /// <param name="configuration">The entire configuration.</param>
        /// <param name="parent">The parent item for the item being validated.</param>
        /// <param name="errorProcesser">The error processer to use.</param>
        public virtual void Validate(IConfiguration configuration, ConfigurationTrace parent, IConfigurationErrorProcesser errorProcesser)
        {
            var parentProject = parent.GetAncestorValue<Project>();
            if (parentProject != null)
            {
                // Attempt to find this publisher in the publishers section
                var isPublisher = false;
                foreach (var task in parentProject.Publishers)
                {
                    if (task == this)
                    {
                        isPublisher = true;
                        break;
                    }
                }

                // If not found then throw a validation exception
                if (!isPublisher)
                {
                    errorProcesser.ProcessWarning("Email publishers are best placed in the publishers section of the configuration");
                }
            }
            else
            {
                errorProcesser.ProcessError(
                    new CruiseControlException("This publisher can only belong to a project"));
            }
        }
        #endregion
    }
}
