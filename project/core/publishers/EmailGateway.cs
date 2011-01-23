using System.Net;
using System.Net.Mail;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    /// <summary>
    /// 	
    /// </summary>
    public class EmailGateway
    {
        private string mailhostUsername = null;
        private PrivateString mailhostPassword = null;
        private SmtpClient smtpServer;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailGateway" /> class.	
        /// </summary>
        /// <remarks></remarks>
        public EmailGateway()
        {
            smtpServer = new SmtpClient();
        }

        /// <summary>
        /// Gets or sets the mail host.	
        /// </summary>
        /// <value>The mail host.</value>
        /// <remarks></remarks>
        public virtual string MailHost
        {
            get { return smtpServer.Host; }
            set { smtpServer.Host = value; }
        }

        /// <summary>
        /// Gets or sets the mail port.	
        /// </summary>
        /// <value>The mail port.</value>
        /// <remarks></remarks>
        public virtual int MailPort
        {
            get { return smtpServer.Port; }
            set { smtpServer.Port = value; }
        }

        /// <summary>
        /// Gets or sets the use SSL.	
        /// </summary>
        /// <value>The use SSL.</value>
        /// <remarks></remarks>
        public bool UseSSL
        {
            get { return smtpServer.EnableSsl ; }
            set { smtpServer.EnableSsl = value; }
        }

        /// <summary>
        /// Gets or sets the mail host username.	
        /// </summary>
        /// <value>The mail host username.</value>
        /// <remarks></remarks>
        public string MailHostUsername
        {
            get { return mailhostUsername; }
            set { mailhostUsername = value; }
        }

        /// <summary>
        /// Gets or sets the mail host password.	
        /// </summary>
        /// <value>The mail host password.</value>
        /// <remarks></remarks>
        public PrivateString MailHostPassword
        {
            get { return mailhostPassword; }
            set { mailhostPassword = value; }
        }

        /// <summary>
        /// Sends the specified mail message.	
        /// </summary>
        /// <param name="mailMessage">The mail message.</param>
        /// <remarks></remarks>
        public virtual void Send(MailMessage mailMessage)
        {
            if (MailHostUsername != null && MailHostPassword != null)
            {
                smtpServer.Credentials = new NetworkCredential(mailhostUsername, mailhostPassword.PrivateValue);
            }
            smtpServer.Send(mailMessage);
        }
    }
}