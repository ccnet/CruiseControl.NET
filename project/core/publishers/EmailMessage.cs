using System.Collections;
using ThoughtWorks.CruiseControl.Core.Util;
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
				string[] always = CreateNotifyList(EmailGroup.NotificationType.Always);
				if (BuildStateChanged(result))
				{
					string[] change = CreateNotifyList(EmailGroup.NotificationType.Change);
					return StringUtil.JoinUnique(", ", always, change);
				}
				else
				{
					return StringUtil.JoinUnique(", ", always, Modifiers);
				}
			}
		}

		public string[] Modifiers
		{
			get
			{
				ArrayList modifiers = new ArrayList();
				foreach (Modification modification in result.Modifications)
				{
					EmailUser user = GetEmailUser(modification.UserName);
					if (user != null)
					{
						modifiers.Add(user.Address);
					}
				}
				return (string[]) modifiers.ToArray(typeof (string));
			}
		}

		public string Subject
		{
			get
			{
				if (result.Status == IntegrationStatus.Success)
				{
					if (BuildStateChanged(result))
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
			return (EmailUser) emailPublisher.EmailUsers[username];
		}

		private EmailGroup GetEmailGroup(string groupname)
		{
			return (EmailGroup) emailPublisher.EmailGroups[groupname];
		}

		private string[] CreateNotifyList(EmailGroup.NotificationType notification)
		{
			ArrayList userList = new ArrayList();
			foreach (EmailUser user in emailPublisher.EmailUsers.Values)
			{
				EmailGroup group = GetEmailGroup(user.Group);
				if (group != null && group.Notification.Equals(notification))
				{
					userList.Add(user.Address);
				}
			}
			return (string[]) userList.ToArray(typeof (string));
		}

		private bool BuildStateChanged(IIntegrationResult result)
		{
			return result.LastIntegrationStatus != result.Status;
		}
	}
}