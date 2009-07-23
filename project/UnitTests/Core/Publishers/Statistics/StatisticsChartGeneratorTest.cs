using System;
using System.Collections;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Publishers.Statistics;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers.Statistics
{
	[TestFixture]
	public class StatisticsChartGeneratorTest
	{
		private XmlDocument statistics;
		private StatisticsChartGenerator chartGenerator;
		private IMock mockPlotter;

		#region Report.xml

		private string xml = 
			"<statistics>" + 
			"	<integration build-label='1'>" + 
			"		<statistic name='TestCount'>320</statistic>" + 
			"		<statistic name='TestIgnored'>2</statistic>" + 
			"		<statistic name='ProjectName'>CCNet</statistic>" + 
			"		<statistic name='Duration'>00:00:33</statistic>" + 
			"		<statistic name='FxCop Warnings'>0</statistic>" + 
			"		<statistic name='StartTime'>4/7/2006 9:20:26 PM</statistic>" + 
			"		<statistic name='TestFailures'>0</statistic>" + 
			"		<statistic name='FxCop Errors'>0</statistic>" + 
			"	</integration>" + 
			"	<integration build-label='2'>" + 
			"		<statistic name='TestCount'>434</statistic>" + 
			"		<statistic name='TestIgnored'>9</statistic>" + 
			"		<statistic name='ProjectName'>CCNet</statistic>" + 
			"		<statistic name='Duration'>00:00:45</statistic>" + 
			"		<statistic name='FxCop Warnings'>0</statistic>" + 
			"		<statistic name='StartTime'>4/7/2006 9:27:28 PM</statistic>" + 
			"		<statistic name='TestFailures'>0</statistic>" + 
			"		<statistic name='FxCop Errors'>0</statistic>" + 
			"	</integration>" + 
			"	<integration build-label='3'>" + 
			"		<statistic name='TestCount'>504</statistic>" + 
			"		<statistic name='TestIgnored'>9</statistic>" + 
			"		<statistic name='ProjectName'>CCNet</statistic>" + 
			"		<statistic name='Duration'>00:00:35</statistic>" + 
			"		<statistic name='FxCop Warnings'>0</statistic>" + 
			"		<statistic name='StartTime'>4/7/2006 9:27:28 PM</statistic>" + 
			"		<statistic name='TestFailures'>0</statistic>" + 
			"		<statistic name='FxCop Errors'>0</statistic>" + 
			"	</integration>" + 
			"	<integration build-label='4'>" + 
			"		<statistic name='TestCount'>504</statistic>" + 
			"		<statistic name='TestIgnored'>9</statistic>" + 
			"		<statistic name='ProjectName'>CCNet</statistic>" + 
			"		<statistic name='Duration'>00:00:43</statistic>" + 
			"		<statistic name='FxCop Warnings'>0</statistic>" + 
			"		<statistic name='StartTime'>4/7/2006 9:27:28 PM</statistic>" + 
			"		<statistic name='TestFailures'>0</statistic>" + 
			"		<statistic name='FxCop Errors'>0</statistic>" + 
			"	</integration>" + 
			"	<integration build-label='5'>" + 
			"		<statistic name='TestCount'>532</statistic>" + 
			"		<statistic name='TestIgnored'>9</statistic>" + 
			"		<statistic name='ProjectName'>CCNet</statistic>" + 
			"		<statistic name='Duration'>00:00:45</statistic>" + 
			"		<statistic name='FxCop Warnings'>0</statistic>" + 
			"		<statistic name='StartTime'>4/7/2006 9:27:28 PM</statistic>" + 
			"		<statistic name='TestFailures'>0</statistic>" + 
			"		<statistic name='FxCop Errors'>0</statistic>" + 
			"	</integration>" + 
			"	<integration build-label='6'>" + 
			"		<statistic name='TestCount'>504</statistic>" + 
			"		<statistic name='TestIgnored'>9</statistic>" + 
			"		<statistic name='ProjectName'>CCNet</statistic>" + 
			"		<statistic name='Duration'>00:00:36</statistic>" + 
			"		<statistic name='FxCop Warnings'>0</statistic>" + 
			"		<statistic name='StartTime'>4/7/2006 9:27:28 PM</statistic>" + 
			"		<statistic name='TestFailures'>0</statistic>" + 
			"		<statistic name='FxCop Errors'>0</statistic>" + 
			"	</integration>" + 
			"	<integration build-label='7'>" + 
			"		<statistic name='TestCount'>703</statistic>" + 
			"		<statistic name='TestIgnored'>9</statistic>" + 
			"		<statistic name='ProjectName'>CCNet</statistic>" + 
			"		<statistic name='Duration'>00:00:55</statistic>" + 
			"		<statistic name='FxCop Warnings'>0</statistic>" + 
			"		<statistic name='StartTime'>4/7/2006 9:27:28 PM</statistic>" + 
			"		<statistic name='TestFailures'>0</statistic>" + 
			"		<statistic name='FxCop Errors'>0</statistic>" + 
			"	</integration>" + 
			"	<integration build-label='8'>" + 
			"		<statistic name='TestCount'>804</statistic>" + 
			"		<statistic name='TestIgnored'>9</statistic>" + 
			"		<statistic name='ProjectName'>CCNet</statistic>" + 
			"		<statistic name='Duration'>00:00:35</statistic>" + 
			"		<statistic name='FxCop Warnings'>0</statistic>" + 
			"		<statistic name='StartTime'>4/7/2006 9:27:28 PM</statistic>" + 
			"		<statistic name='TestFailures'>0</statistic>" + 
			"		<statistic name='FxCop Errors'>0</statistic>" + 
			"	</integration>" + 
			"	<integration build-label='9'>" + 
			"		<statistic name='TestCount'>734</statistic>" + 
			"		<statistic name='TestIgnored'>9</statistic>" + 
			"		<statistic name='ProjectName'>CCNet</statistic>" + 
			"		<statistic name='Duration'>00:00:35</statistic>" + 
			"		<statistic name='FxCop Warnings'>0</statistic>" + 
			"		<statistic name='StartTime'>4/7/2006 9:27:28 PM</statistic>" + 
			"		<statistic name='TestFailures'>0</statistic>" + 
			"		<statistic name='FxCop Errors'>0</statistic>" + 
			"	</integration>" + 
			"	<integration build-label='10'>" + 
			"		<statistic name='TestCount'>704</statistic>" + 
			"		<statistic name='TestIgnored'>9</statistic>" + 
			"		<statistic name='ProjectName'>CCNet</statistic>" + 
			"		<statistic name='Duration'>00:00:35</statistic>" + 
			"		<statistic name='FxCop Warnings'>0</statistic>" + 
			"		<statistic name='StartTime'>4/7/2006 9:27:28 PM</statistic>" + 
			"		<statistic name='TestFailures'>0</statistic>" + 
			"		<statistic name='FxCop Errors'>0</statistic>" + 
			"	</integration>" + 
			"	<integration build-label='11'>" + 
			"		<statistic name='TestCount'>814</statistic>" + 
			"		<statistic name='TestIgnored'>9</statistic>" + 
			"		<statistic name='ProjectName'>CCNet</statistic>" + 
			"		<statistic name='Duration'>00:00:35</statistic>" + 
			"		<statistic name='FxCop Warnings'>0</statistic>" + 
			"		<statistic name='StartTime'>4/7/2006 9:27:28 PM</statistic>" + 
			"		<statistic name='TestFailures'>0</statistic>" + 
			"		<statistic name='FxCop Errors'>0</statistic>" + 
			"	</integration>" + 
			"	<integration build-label='12'>" + 
			"		<statistic name='TestCount'>904</statistic>" + 
			"		<statistic name='TestIgnored'>9</statistic>" + 
			"		<statistic name='ProjectName'>CCNet</statistic>" + 
			"		<statistic name='Duration'>00:00:35</statistic>" + 
			"		<statistic name='FxCop Warnings'>0</statistic>" + 
			"		<statistic name='StartTime'>4/7/2006 9:27:28 PM</statistic>" + 
			"		<statistic name='TestFailures'>0</statistic>" + 
			"		<statistic name='FxCop Errors'>0</statistic>" + 
			"	</integration>" + 
			"	<integration build-label='13'>" + 
			"		<statistic name='TestCount'>644</statistic>" + 
			"		<statistic name='TestIgnored'>9</statistic>" + 
			"		<statistic name='ProjectName'>CCNet</statistic>" + 
			"		<statistic name='Duration'>00:00:35</statistic>" + 
			"		<statistic name='FxCop Warnings'>0</statistic>" + 
			"		<statistic name='StartTime'>4/7/2006 9:27:28 PM</statistic>" + 
			"		<statistic name='TestFailures'>0</statistic>" + 
			"		<statistic name='FxCop Errors'>0</statistic>" + 
			"	</integration>" + 
			"	<integration build-label='14'>" + 
			"		<statistic name='TestCount'>634</statistic>" + 
			"		<statistic name='TestIgnored'>9</statistic>" + 
			"		<statistic name='ProjectName'>CCNet</statistic>" + 
			"		<statistic name='Duration'>00:00:35</statistic>" + 
			"		<statistic name='FxCop Warnings'>0</statistic>" + 
			"		<statistic name='StartTime'>4/7/2006 9:27:28 PM</statistic>" + 
			"		<statistic name='TestFailures'>0</statistic>" + 
			"		<statistic name='FxCop Errors'>0</statistic>" + 
			"	</integration>" + 
			"	<integration build-label='15'>" + 
			"		<statistic name='TestCount'>834</statistic>" + 
			"		<statistic name='TestIgnored'>9</statistic>" + 
			"		<statistic name='ProjectName'>CCNet</statistic>" + 
			"		<statistic name='Duration'>00:00:35</statistic>" + 
			"		<statistic name='FxCop Warnings'>0</statistic>" + 
			"		<statistic name='StartTime'>4/7/2006 9:27:28 PM</statistic>" + 
			"		<statistic name='TestFailures'>0</statistic>" + 
			"		<statistic name='FxCop Errors'>0</statistic>" + 
			"	</integration>" + 
			"</statistics>";

		#endregion

		[SetUp]
		public void SetUp()
		{
			statistics = new XmlDocument();
			statistics.LoadXml(xml);

			mockPlotter= new DynamicMock(typeof(IPlotter));
			chartGenerator = new StatisticsChartGenerator((IPlotter)mockPlotter.MockInstance);
		}

		[Test]
		public void ShouldThrowExceptionIfAskedToPlotUnavailableStatistics()
		{
			mockPlotter.ExpectNoCall("DrawGraph", typeof(IList), typeof(IList), typeof(string));
			mockPlotter.ExpectNoCall("WriteToStream", typeof(IList), typeof(IList), typeof(Stream));
			chartGenerator.RelevantStats = new string[]{"Unavailable"};
            Assert.That(delegate { chartGenerator.Process(statistics, "dummy"); }, Throws.TypeOf<UnavailableStatisticsException>());
		}

		[Test]
		public void ShouldPlotChartForAvailableStatistics()
		{
			mockPlotter.Expect("DrawGraph", new IsAnything(), new IsAnything(), new IsAnything());
			mockPlotter.ExpectNoCall("WriteToStream", typeof(IList), typeof(IList), typeof(Stream));
			chartGenerator.RelevantStats = new string[]{"TestCount"};
			chartGenerator.Process(statistics, "dummy");
		}

		[Test]
		[Explicit]
		public void ShouldGenerateChart()
		{
			StatisticsChartGenerator generator = new StatisticsChartGenerator(new Plotter("c:/", "png", ImageFormat.Png));
			generator.RelevantStats = new string[]{"TestCount"};
			generator.Process(statistics, "dummy");
		}

	    [Test]
        [Explicit]
        public void ShouldSetBuildLabelInAbscissa()
	    {
            string xpath = "/statistics/integration";
	        XPathNavigator navigator = statistics.CreateNavigator();
            XPathNodeIterator dataList = navigator.Select(xpath);
	        int count = dataList.Count;
	        Console.Out.WriteLine("count = {0}", count);
	        while(dataList.MoveNext())
            {
                XPathNavigator xPathNavigator = dataList.Current;
                Console.Out.WriteLine("build-label = {0}", xPathNavigator.GetAttribute("build-label",string.Empty));
                XPathNavigator duration = xPathNavigator.SelectSingleNode(string.Format("statistic[@name='{0}']", "Duration"));
                Console.Out.WriteLine("duration = {0}", duration);
            }
	    }

	    [Test]
        [Explicit]
	    public void ShouldFormatStatisticAsSpecified()
	    {
	        string value = "03:00:02";
	        Assert.IsTrue(Regex.IsMatch(value, "[0-9]+:[0-9]+:[0-9]+"));
	    }
	}
}
