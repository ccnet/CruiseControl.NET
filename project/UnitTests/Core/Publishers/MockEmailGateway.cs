using System.Collections;
using System.Web.Mail;
using ThoughtWorks.CruiseControl.Core.Publishers;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	public class MockEmailGateway : EmailGateway
	{
		private ArrayList _sentMessages = new ArrayList();
		private ArrayList _recipients = new ArrayList();

		private string _mailhost = "mock.gateway.org";

		public override string MailHost
		{
			get { return _mailhost; }
			set { _mailhost = value; }
		}

		public override void Send(string from, string to, string replyto, string subject, string message)
		{
			MailMessage email = GetMailMessage(from, to, replyto, subject, message);

			_sentMessages.Add(email);

			AddRecipients(to);
		}

		public IList SentMessages
		{
			get { return _sentMessages; }
		}

		public IList Recipients
		{
			get { return _recipients; }
		}

		public static MockEmailGateway Create()
		{
			return new MockEmailGateway();
		}

		private void AddRecipients(string to)
		{
			string[] recipients;
			recipients = to.Split(","[0]);

			foreach (string recipient in recipients)
			{
				if (! _recipients.Contains(recipient) )
				{
					_recipients.Add(recipient);
				}
			}
		}

	}
}
