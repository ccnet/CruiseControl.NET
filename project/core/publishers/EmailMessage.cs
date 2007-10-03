using System.Collections;
using System.Text;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	/// <summary>
	/// This class encloses all the details related to a typical message needed by a 
	/// Email Publisher
	/// </summary>
	public class EmailMessage
	{
		private readonly IIntegrationResult result;
		private readonly EmailPublisher emailPublisher;

		public EmailMessage(IIntegrationResult result, EmailPublisher emailPublisher)
		{
			this.result = result;
			this.emailPublisher = emailPublisher;
		}

		public string Recipients
		{
			get
			{
				IDictionary recipients = new SortedList();
				AddRecipients(recipients, EmailGroup.NotificationType.Always);

                if (emailPublisher.ModifierNotificationType == EmailGroup.NotificationType.Always )
                {
                    AddModifiers(recipients);
                }


				if (BuildStateChanged())
				{
					AddRecipients(recipients, EmailGroup.NotificationType.Change);

                    if (emailPublisher.ModifierNotificationType == EmailGroup.NotificationType.Change )
                    {
                        AddModifiers(recipients);
                    }
				}

				if (result.Status == IntegrationStatus.Failure)
				{
                    AddRecipients(recipients, EmailGroup.NotificationType.Failed);

                    if (emailPublisher.ModifierNotificationType == EmailGroup.NotificationType.Failed )
                    {
                        AddModifiers(recipients);
                    }
                }
				
                if (result.Status == IntegrationStatus.Success)
				{
                    AddRecipients(recipients, EmailGroup.NotificationType.Success);

                    if (emailPublisher.ModifierNotificationType == EmailGroup.NotificationType.Success )
                    {
                        AddModifiers(recipients);
                    }
                }
                
                if (result.Fixed )
                {
                    AddRecipients(recipients, EmailGroup.NotificationType.Fixed);
                    if (emailPublisher.ModifierNotificationType == EmailGroup.NotificationType.Fixed )
                    {
                        AddModifiers(recipients);
                    }
                }

				StringBuilder buffer = new StringBuilder();
				foreach (string key in recipients.Keys)
				{
					if (buffer.Length > 0) buffer.Append(", ");
					buffer.Append(key);
				}
				return buffer.ToString();
			}
		}

		private void AddModifiers(IDictionary recipients)
		{
			foreach (Modification modification in result.Modifications)
			{
				EmailUser user = GetEmailUser(modification.UserName);
				if (user != null)
				{
					recipients[user.Address] = user;
				}
			}
		}

		private void AddRecipients(IDictionary recipients, EmailGroup.NotificationType notificationType)
		{
			foreach (EmailUser user in emailPublisher.EmailUsers.Values)
			{
				EmailGroup group = GetEmailGroup(user.Group);
				if (group != null && group.Notification == notificationType)
				{
                    recipients[user.Address] = user;
				}
			}
		}

		public string Subject
		{
			get
			{
				if (result.Status == IntegrationStatus.Success)
				{
					if (BuildStateChanged())
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
		}

		private EmailUser GetEmailUser(string username)
		{
			if (username == null) return null;
            EmailUser user = (EmailUser) emailPublisher.EmailUsers[username];

            // if user is not specified in the project config, 
            // use the converters to create the email address from the sourcecontrol ID           
            if (user == null && emailPublisher.Converters.Length > 0)
            {
                string email = username;
                foreach (EmailConverter converter in emailPublisher.Converters)
                {
                    email = converter.Convert(email);
                }
                user = new EmailUser(username, null, email);
            }
            return user;
		}

		private EmailGroup GetEmailGroup(string groupname)
		{
			if (groupname == null) return null;
			return (EmailGroup) emailPublisher.EmailGroups[groupname];
		}

		private bool BuildStateChanged()
		{
			return result.LastIntegrationStatus != result.Status;
		}
	}
}