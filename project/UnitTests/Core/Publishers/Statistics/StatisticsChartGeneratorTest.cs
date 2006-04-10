using System.Collections;
using System.IO;
using System.Xml;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.publishers.Statistics;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers.Statistics
{
	[TestFixture]
	public class StatisticsChartGeneratorTest
	{
		private XmlDocument statistics;
		private StatisticsChartGenerator chartGenerator;
		private IMock mockPlotter;

		#region XML

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
		[ExpectedException(typeof(UnavailableStatisticsException))]
		public void ShouldThrowExceptionIfAskedToPlotUnavailableStatistics()
		{
			mockPlotter.ExpectNoCall("DrawGraph", typeof(IList), typeof(IList), typeof(double));
			mockPlotter.ExpectNoCall("WriteToStream", typeof(IList), typeof(IList), typeof(double), typeof(Stream));
			chartGenerator.RelevantStats = new string[]{"Unavailable"};
			chartGenerator.Process(statistics);
		}

		[Test]
		public void ShouldPlotChartForAvailableStatistics()
		{
			mockPlotter.Expect("DrawGraph", new IsAnything(), new IsAnything(), new IsAnything());
			mockPlotter.ExpectNoCall("WriteToStream", typeof(IList), typeof(IList), typeof(double), typeof(Stream));
			chartGenerator.RelevantStats = new string[]{"TestCount"};
			chartGenerator.Process(statistics);
		}

		[Test]
		[Ignore("Just if you really want to see a chart")]
		public void ShouldGenerateChart()
		{
			StatisticsChartGenerator generator = new StatisticsChartGenerator(new Plotter("c:/", "temp.bmp"));
			generator.RelevantStats = new string[]{"TestCount"};
			generator.Process(statistics);
		}
	}
}
