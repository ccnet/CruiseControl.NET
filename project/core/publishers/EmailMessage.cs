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

        /// <summary>
        /// Determine the recipients list for the email.
        /// </summary>
        /// <remarks>Note: This can be a mildly-heavyweight property to read.</remarks>
        public string Recipients
        {
            get
            {
                IDictionary recipients = new SortedList();

                // Add users who are explicity intended to get the message we're going to send.
                AddRecipients(recipients, EmailGroup.NotificationType.Always);
                if (BuildStateChanged())
                    AddRecipients(recipients, EmailGroup.NotificationType.Change);
                if ((result.Status == IntegrationStatus.Failure) || (result.Status == IntegrationStatus.Exception))
                    AddRecipients(recipients, EmailGroup.NotificationType.Failed);
                if (result.Status == IntegrationStatus.Success)
                    AddRecipients(recipients, EmailGroup.NotificationType.Success);
                if (result.Fixed)
                    AddRecipients(recipients, EmailGroup.NotificationType.Fixed);

                // Add users who contributed modifications to this or possibly previous builds.
                foreach (EmailGroup.NotificationType notificationType in emailPublisher.ModifierNotificationTypes)
                {
                    switch (notificationType)
                    {
                        case EmailGroup.NotificationType.Always:
                            AddModifiers(recipients);
                            AddFailureUsers(recipients);
                            break;
                        case EmailGroup.NotificationType.Change:
                            if (BuildStateChanged())
                            {
                                AddModifiers(recipients);
                                AddFailureUsers(recipients);
                            }
                            break;
                        case EmailGroup.NotificationType.Failed:
                            if ((result.Status == IntegrationStatus.Failure) || (result.Status == IntegrationStatus.Exception))
                            {
                                AddModifiers(recipients);
                                AddFailureUsers(recipients);
                            }
                            break;
                        case EmailGroup.NotificationType.Success:
                            if (result.Status == IntegrationStatus.Success)
                            {
                                AddModifiers(recipients);
                                AddFailureUsers(recipients);
                            }
                            break;
                        case EmailGroup.NotificationType.Fixed:
                            if (result.Fixed)
                            {
                                AddModifiers(recipients);
                                AddFailureUsers(recipients);
                            }
                            break;
                        default:
                            throw new CruiseControlException("Unknown notification type" + notificationType);
                    }
                }

                StringBuilder buffer = new StringBuilder();
                foreach (string key in recipients.Keys)
                {
                    if (buffer.Length > 0)
                        buffer.Append(", ");
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

        private void AddFailureUsers(IDictionary recipients)
        {
            foreach (string username in result.FailureUsers)
            {
                EmailUser user = GetEmailUser(username);
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
                string prefix = "";
                string message = "";
                string label = "";

                if (emailPublisher.SubjectPrefix != null)
                    prefix = emailPublisher.SubjectPrefix + " ";

                if (result.Status == IntegrationStatus.Success)
                {
                    label = " " + result.Label;
                    if (BuildStateChanged())
                    {
                        message = "Build Fixed: Build";
                    }
                    else
                    {
                        message = "Build Successful: Build";
                    }
                }
                else
                {
                    message = "Build Failed";
                }

                return string.Format("{0}{1} {2}{3}", prefix, result.ProjectName, message, label);
            }
        }

        private EmailUser GetEmailUser(string username)
        {
            if (username == null) return null;
            EmailUser user = (EmailUser)emailPublisher.EmailUsers[username];

            // if user is not specified in the project config, 
            // use the converters to create the email address from the sourcecontrol ID           
            if (user == null && emailPublisher.Converters.Length > 0)
            {
                string email = username;
                foreach (IEmailConverter converter in emailPublisher.Converters)
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
            return (EmailGroup)emailPublisher.EmailGroups[groupname];
        }

        private bool BuildStateChanged()
        {
            return result.LastIntegrationStatus != result.Status;
        }
    }
}
