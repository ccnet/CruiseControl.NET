using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
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

		[ReflectorProperty("name")]
		public string Name;


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
			return Name == g.Name && Notifications.Equals(g.Notifications);
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