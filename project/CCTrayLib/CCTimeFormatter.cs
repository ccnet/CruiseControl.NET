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
			if (timeSpan < TimeSpan.FromSeconds(2.0))
			{
				return "Now Building";
			}
			else if (timeSpan < TimeSpan.FromMinutes(1.0))
			{
				return string.Format("{0} seconds", timeSpan.Seconds);
			}
			StringBuilder sb = new StringBuilder();
			AddIfNeeded(sb, timeSpan.Days, "Day");
			AddIfNeeded(sb, timeSpan.Hours, "Hour");
			AddIfNeeded(sb, timeSpan.Minutes, "Minute");
			return sb.ToString().Trim();
		}

		private void AddIfNeeded(StringBuilder sb, int value, String type)
		{
			if (value != 0)
				sb.AppendFormat("{0} {1}{2} ", value, type, (value > 1 ? "s" : ""));
		}
	}
}