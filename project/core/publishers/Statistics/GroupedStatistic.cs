using System.Collections;
using System.Xml.XPath;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
	public class GroupedStatistic : Statistic
	{
		private readonly string resultFormat;
		private readonly Statistic[] statistics;

		public GroupedStatistic(string name, string resultFormat, params Statistic[] statistic) : base(name, "")
		{
			this.resultFormat = resultFormat;
			this.statistics = statistic;
		}

		protected override object Evaluate(XPathNavigator nav)
		{
			ArrayList results = new ArrayList();
			for (int i = 0; i < statistics.Length; i++)
			{
				Statistic statistic = this.statistics[i];
				object value = statistic.Apply(nav);
				results.Add(value == null ? "" : value);
			}
			return string.Format(resultFormat, (object[]) results.ToArray(typeof (object)));
		}
	}
}
