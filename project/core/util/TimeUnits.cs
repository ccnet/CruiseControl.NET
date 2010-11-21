
using System;
using System.Collections;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class TimeUnits : IComparable
	{
		private static Hashtable values = new Hashtable();
		
		public static TimeUnits MILLIS = new TimeUnits("millis", 1);
		public static TimeUnits SECONDS = new TimeUnits("seconds", 1000);
		public static TimeUnits MINUTES = new TimeUnits("minutes", 60*1000);
		public static TimeUnits HOURS = new TimeUnits("hours", 60*60*1000);
		
		private readonly int factor;
		private readonly string name;

		public TimeUnits(string name, int numberOfMillis)
		{
			this.factor = numberOfMillis;
			this.name = name;
			values[name.ToLower()] = this;
		}

		public int ToMillis(int timeout)
		{
			return timeout*factor;
		}


		public override string ToString()
		{
			return name;
		}

		public static TimeUnits Parse(string input)
		{
			string key = input.ToLower();
			if (!values.ContainsKey(key))
			{
				throw new CruiseControlException(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Invalid time units specified [{0}]. I understand {1}", input, MakeString(values.Keys)));
			}
			return (TimeUnits) values[key];
			
		}

		public int CompareTo(object obj)
		{
			TimeUnits other = (TimeUnits) obj;
			return this.factor.CompareTo(other.factor);
		}		
		
		private static string MakeString(ICollection c)
		{
			StringBuilder sb = new StringBuilder();
			ArrayList list = new ArrayList(c);
			list.Sort();
			list.Reverse();
			for (int i = 0; i < list.Count; i++)
			{
				if (i>0) sb.Append(", ");
				object o = list[i];
				sb.Append(o.ToString());
			}
			return sb.ToString();
		}

	}
}