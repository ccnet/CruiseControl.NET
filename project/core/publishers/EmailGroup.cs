using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	[ReflectorType("group")]
	public class EmailGroup
	{
		public enum NotificationType
		{
			Always,
			Change,
			Failed,
			Success
		}

		public EmailGroup()
		{
		}

		public EmailGroup(string name, NotificationType notification)
		{
			Name = name;
			Notification = notification;
		}

		[ReflectorProperty("name")]
		public string Name;

		[ReflectorProperty("notification")]
		public NotificationType Notification;

		public override bool Equals(Object o)
		{
			if (o == null || o.GetType() != GetType())
			{
				return false;
			}
			EmailGroup g = (EmailGroup) o;
			return Name == g.Name && Notification == g.Notification;
		}

		public override int GetHashCode()
		{
			return StringUtil.GenerateHashCode(Name, Notification.ToString());
		}

		public override string ToString()
		{
			return string.Format("EmailGroup: [name: {0}, notification: {1}]", Name, Notification);
		}
	}
}