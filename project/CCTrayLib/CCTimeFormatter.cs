using System;
using System.Text;

namespace ThoughtWorks.CruiseControl.CCTrayLib
{
	public class CCTimeFormatter
	{
		private readonly TimeSpan timeSpan;

		public CCTimeFormatter(TimeSpan timeSpan)
		{
			this.timeSpan = timeSpan;
		}

		public override string ToString()
		{
			if (timeSpan < TimeSpan.FromMinutes(1.0)) return "Now Building";
			StringBuilder sb = new StringBuilder();
			AddIfNeeded(sb, timeSpan.Days, "Day(s)");
			AddIfNeeded(sb, timeSpan.Hours, "Hour(s)");
			AddIfNeeded(sb, timeSpan.Minutes, "Minute(s)");
			return sb.ToString().Trim();
		}

		private void AddIfNeeded(StringBuilder sb, int value, String type)
		{
			if (value != 0)
				sb.AppendFormat("{0} {1} ", value, type);
		}
	}
}