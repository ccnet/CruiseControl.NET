using System;
using Exortech.NetReflector;

namespace tw.ccnet.core.publishers
{
	[ReflectorType("yahoouser")]
	public class YahooUser
	{
		private string _name;
		private string _id;
		private string _group;

		public YahooUser() { }

		public YahooUser(string name, string group, string id)
		{
			_name = name;
			_id = id;
			_group = group;
		}

		[ReflectorProperty("name")]
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		[ReflectorProperty("id")]
		public string ID
		{
			get { return _id; }
			set { _id = value; }
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
			YahooUser user = (YahooUser)obj;
			return (user.Name == Name && user.ID == ID && user.Group == Group);
		}

		public override int GetHashCode()
		{
			return String.Concat(Name, ID, Group).GetHashCode();
		}

		public override string ToString()
		{
			return String.Format("Yahoo User: {0} {1} {2}", Name, ID, Group);
		}
	}
}
