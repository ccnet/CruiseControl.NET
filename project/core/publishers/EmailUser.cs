using System;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	[ReflectorType("user")]
	public class EmailUser
	{
		public EmailUser() { }

		public EmailUser(string name, string group, string address)
		{
			Name = name;
			Address = address;
			Group = group;
		}

		[ReflectorProperty("name")]
		public string Name;

		[ReflectorProperty("address")]
		public string Address;

		[ReflectorProperty("group")]
		public string Group;

		public override bool Equals(Object obj)
		{
			if (obj == null || obj.GetType() != this.GetType())
			{
				return false;
			}
			EmailUser user = (EmailUser)obj;
			return (user.Name == Name && user.Address == Address && user.Group == Group);
		}

		public override int GetHashCode()
		{
			return String.Concat(Name, Address, Group).GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("Email User: {0} {1} {2}", Name, Address, Group);
		}
	}
}