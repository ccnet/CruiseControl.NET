using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    /// <summary>
    /// Defines a group of users to receive e-mails.
    /// </summary>
    /// <title>Email Group</title>
    /// <version>1.3</version>
    /// <example>
    /// <code>
    /// &lt;group name="developers"&gt;
    /// &lt;notifications&gt;
    /// &lt;notificationType&gt;Failed&lt;/notificationType&gt;
    /// &lt;notificationType&gt;Fixed&lt;/notificationType&gt;
    /// &lt;/notifications&gt;
    /// &lt;/group&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <para type="warning">
    /// Up to CC.NET version 1.4.4, notification is a single attribute on the group. Starting with CC.NET 1.5.0, 
    /// this has been changed to an array of notification types. From 1.5.0 onwards, the Failed notification type, 
    /// is just failed, it does not include the Exception anymore. Making it possible to mail Exception to the
    /// buildmaster, and Failed to the developpers. Developers will not get Exception mails, unless configured so.
    /// </para>
    /// </remarks>
    [ReflectorType("group")]
	public class EmailGroup
	{

        private NotificationType[] notifications = { EmailGroup.NotificationType.Always };

		public enum NotificationType
		{
			Always,
			Change,
			Failed,
			Success,
            Fixed,
            Exception
		}

		public EmailGroup()
		{
		}

		public EmailGroup(string name, NotificationType[] notifications)
		{
			Name = name;
			Notifications = notifications;
		}

        /// <summary>
        /// The name of the group, which corresponds to the "group" values used in the &lt;user&gt; elements. 
        /// </summary>
        /// <version>1.3</version>
        /// <default>n.a</default>
		[ReflectorProperty("name")]
		public string Name;

        /// <summary>
        /// A list of notification types, determining when to send email to this group.
        /// </summary>
        /// <version>1.3</version>
        /// <default>n.a</default>
        [ReflectorProperty("notifications", Required = false)]
        public NotificationType[] Notifications
        {
            get { return notifications; }
            set { notifications = value; }
        }


		public override bool Equals(Object o)
		{
			if (o == null || o.GetType() != GetType())
			{
				return false;
			}
			EmailGroup g = (EmailGroup) o;
			return Name == g.Name ;
		}

		public override int GetHashCode()
		{
            return Name.GetHashCode() & StringUtil.GetArrayContents(Notifications).GetHashCode() ; 
		}

		public override string ToString()
		{
			return string.Format("EmailGroup: [name: {0}, notifications: {1}]", Name, StringUtil.GetArrayContents( Notifications) );
		}


        public bool HasNotification(NotificationType toSearch)
        {
            bool found = false;

            foreach (NotificationType nt in Notifications)
            {
                if (nt == toSearch)
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

	}
}