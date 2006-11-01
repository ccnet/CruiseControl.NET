using System.Web.Mail;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	public class EmailGateway
	{
		// taken from http://www.codeproject.com/dotnet/SystemWeb_Mail_SMTP_AUTH.asp
		private const string DoAuthenticationUrl = "http://schemas.microsoft.com/cdo/configuration/smtpauthenticate";
		public const string AuthenticationUsernameUrl = "http://schemas.microsoft.com/cdo/configuration/sendusername";
		public const string AuthenticationPasswordUrl = "http://schemas.microsoft.com/cdo/configuration/sendpassword";

		private string mailhostUsername = null;
		private string mailhostPassword = null;
		
		public virtual string MailHost
		{
			get { return SmtpMail.SmtpServer; }
			set { SmtpMail.SmtpServer = value; }
		}

		public string MailHostUsername
		{
			get { return mailhostUsername; }
			set { mailhostUsername = value; }
		}

		public string MailHostPassword
		{
			get { return mailhostPassword; }
			set { mailhostPassword = value; }
		}

		public virtual void Send(string from, string to, string replyto, string subject, string messageText)
		{
			MailMessage mailMessage = GetMailMessage(from, to, replyto, subject, messageText);
			SmtpMail.Send(mailMessage);
		}

		protected MailMessage GetMailMessage(string from, string to, string replyto, string subject, string messageText)
		{
			MailMessage mailMessage = new MailMessage();
			mailMessage.From = from;
			mailMessage.To = to;
			mailMessage.Headers.Add("Reply-To", replyto);
			mailMessage.Subject = subject;
			mailMessage.BodyFormat = MailFormat.Html;
			mailMessage.Body = messageText;
			if (MailHostUsername != null && MailHostPassword != null)
			{
				mailMessage.Fields[DoAuthenticationUrl] = 1;
				mailMessage.Fields[AuthenticationUsernameUrl] = MailHostUsername;
				mailMessage.Fields[AuthenticationPasswordUrl] = MailHostPassword;
			}
			return mailMessage;
		}
	}
}
