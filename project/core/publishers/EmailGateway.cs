using System;
using System.Web.Mail;

namespace tw.ccnet.core.publishers
{
	public class EmailGateway
	{
		public virtual string MailHost
		{
			get { return SmtpMail.SmtpServer; }
			set { SmtpMail.SmtpServer = value; }
		}

		public virtual void Send(string from, string to, string subject, string messageText)
		{
			MailMessage mailMessage = new MailMessage();
			mailMessage.From = from;
			mailMessage.To = to;
			mailMessage.Subject = subject;
			mailMessage.BodyFormat = MailFormat.Html;
			mailMessage.Body = messageText;
			SmtpMail.Send(mailMessage);
		}
	}
}
