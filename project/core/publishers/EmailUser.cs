using System;
using Exortech.NetReflector;

namespace tw.ccnet.core.publishers
{
	[ReflectorType("user")]
	public class EmailUser
	{
		private string _name;
		private string _address;
		private string _group;

		public EmailUser() { }

		public EmailUser(string name, string group, string address)
		{
			_name = name;
			_address = address;
			_group = group;
		}

		[ReflectorProperty("name")]
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		[ReflectorProperty("address")]
		public string Address
		{
			get { return _address; }
			set { _address = value; }
		}

		[ReflectorProperty("group")]
		public string Group
		{
			get { return _group; }
			set { _group = value; }
		}

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
