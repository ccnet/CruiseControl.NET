using System;
using tw.ccnet.core.util;
using Exortech.NetReflector;

namespace tw.ccnet.core.publishers
{
	[ReflectorType("group")]
	public class EmailGroup
	{
		public enum NotificationType
		{
			Always, Change
		}

		private string _name;
		private NotificationType _notification;

		public EmailGroup() {}

		public EmailGroup(string name, NotificationType notification)
		{
			_name = name;
			_notification = notification;
		}

		[ReflectorProperty("name")]
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public NotificationType Notification
		{
			get { return _notification; }
			set { _notification = value; }
		}

		[ReflectorProperty("notification")]
		public string NotificationString
		{
			set { SetNotification(value); }
		}

		public void SetNotification(string notification)
		{
			_notification = (NotificationType)
				Enum.Parse(typeof(NotificationType), notification, true);
		}

		public override bool Equals(Object o)
		{
			if (o == null || o.GetType() != this.GetType())
			{
				return false;
			}
			EmailGroup g = (EmailGroup)o;
			return Name == g.Name && Notification == g.Notification;
		}

		public override int GetHashCode()
		{
			return StringUtil.GenerateHashCode(Name, Notification.ToString());
		}

		public override string ToString()
		{
			return String.Format("EmailGroup: [name: {0}, notification: {1}]", _name, _notification);
		}
	}
}
