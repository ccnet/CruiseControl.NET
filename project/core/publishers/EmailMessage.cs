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


        private Hashtable SetSubjects;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="result"></param>
        /// <param name="emailPublisher"></param>
        public EmailMessage(IIntegrationResult result, EmailPublisher emailPublisher)
        {
            this.result = result;
            this.emailPublisher = emailPublisher;


            // copy into own lookuptable for easier processing
            SetSubjects = new Hashtable();
            string mySubject;
            foreach (EmailSubject setValue in emailPublisher.SubjectSettings)
            {
                SetSubjects.Add(setValue.BuildResult, setValue.Value);
            }

            //add missing defaults for each notificationtype
            foreach (EmailSubject.BuildResultType item in System.Enum.GetValues(typeof(EmailSubject.BuildResultType)))
            {
                if (!SetSubjects.ContainsKey(item))
                {
                    switch (item)
                    {
                        case EmailSubject.BuildResultType.Broken:
                            mySubject =  "${CCNetProject} Build Failed";
                            break;

                        case EmailSubject.BuildResultType.Exception:
                            mySubject = "${CCNetProject} Exception in Build !";
                            break;

                        case EmailSubject.BuildResultType.Fixed:
                            mySubject = "${CCNetProject} Build Fixed: Build ${CCNetLabel}";
                            break;

                        case EmailSubject.BuildResultType.StillBroken:
                            mySubject = "${CCNetProject} is still broken";
                            break;

                        case EmailSubject.BuildResultType.Success:
                            mySubject = "${CCNetProject} Build Successful: Build ${CCNetLabel}";
                            break;

                        default:
                            throw new CruiseControlException("Unknown BuildResult : " + item);
                    }

                    SetSubjects.Add(item, mySubject);
                }
            }

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
                
                if (result.Status == IntegrationStatus.Failure) 
                    AddRecipients(recipients, EmailGroup.NotificationType.Failed);
                
                if (result.Status == IntegrationStatus.Success)
                    AddRecipients(recipients, EmailGroup.NotificationType.Success);
                
                if (result.Fixed)
                    AddRecipients(recipients, EmailGroup.NotificationType.Fixed);
                
                if (result.Status == IntegrationStatus.Exception)
                    AddRecipients(recipients, EmailGroup.NotificationType.Exception);


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
                            if (result.Status == IntegrationStatus.Failure)
                            {
                                AddModifiers(recipients);
                                AddFailureUsers(recipients);
                            }
                            break;
                        case EmailGroup.NotificationType.Exception:
                            if (result.Status == IntegrationStatus.Exception)
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
                            throw new CruiseControlException("Unknown notification type: '" + notificationType + "'");
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
            foreach (EmailUser user in emailPublisher.EmailUsers)
            {
                EmailGroup group = GetEmailGroup(user.Group);
                if (group != null && group.HasNotification(notificationType))
                {
                    recipients[user.Address] = user;
                }
            }
        }


        /// <summary>
        /// Gets the subject.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string Subject
        {
            get
            {
                string prefix = string.Empty;
                string message = string.Empty;
                string subject = string.Empty;

                if (emailPublisher.SubjectPrefix != null)
                    prefix = emailPublisher.SubjectPrefix + " ";


                if (result.Status == IntegrationStatus.Exception)
                {
                    message = SetSubjects[EmailSubject.BuildResultType.Exception].ToString();
                }

                if (result.Status == IntegrationStatus.Success)
                {
                    if (BuildStateChanged())
                    {
                        message = SetSubjects[EmailSubject.BuildResultType.Fixed].ToString();
                    }
                    else
                    {
                        message = SetSubjects[EmailSubject.BuildResultType.Success].ToString();
                    }
                }

                if (result.Status == IntegrationStatus.Failure)
                {
                    if (BuildStateChanged())
                    {
                        message = SetSubjects[EmailSubject.BuildResultType.Broken].ToString();
                    }
                    else
                    {
                        message = SetSubjects[EmailSubject.BuildResultType.StillBroken].ToString();
                    }
                }

                subject = message;

                IDictionary properties = result.IntegrationProperties;
                foreach (string key in properties.Keys)
                {
                    string search = "${" + key + "}";
                    subject = subject.Replace(search, Util.StringUtil.IntegrationPropertyToString(properties[key]));
                }

                return string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0}{1}", prefix, subject);
            }
        }


        private EmailUser GetEmailUser(string username)
        {
            if (username == null) return null;
            var user = emailPublisher.IndexedEmailUsers.ContainsKey(username) ?
                emailPublisher.IndexedEmailUsers[username] :
                null;

            // if user is not specified in the project config, 
            // use the converters to create the email address from the sourcecontrol ID           
            if (user == null && emailPublisher.Converters.Length > 0)
            {
                string email = username;
                string tempEmail = null;

                foreach (IEmailConverter converter in emailPublisher.Converters)
                {
                    tempEmail = converter.Convert(email);
                    if(tempEmail != null)
                    {
                        email = tempEmail;
                    }
                }

                if (email != null)
                {
                    user = new EmailUser(username, null, email);
                }
            }
            return user;
        }

        private EmailGroup GetEmailGroup(string groupname)
        {
            EmailGroup group = null;
            if (groupname != null)
            {
                emailPublisher.IndexedEmailGroups.TryGetValue(groupname, out group);
            }

            return group;
        }

        private bool BuildStateChanged()
        {
            return result.LastIntegrationStatus != result.Status;
        }
    }
}
