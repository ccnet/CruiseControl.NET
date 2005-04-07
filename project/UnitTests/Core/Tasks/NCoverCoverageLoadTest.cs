using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
	[TestFixture]
	public class NCoverCoverageLoadTest : CustomAssertion
	{
		private NCoverCoverage _ncoverTask;

		[Test]
		public void LoadWithAllMandatoryParams()
		{
			string xml=@"
						<ncover>
							<ncoverBin>d:\temp\ncover-console.exe</ncoverBin>
							<ncoverReportBin>d:\temp\ncover.exe</ncoverReportBin>
							<nrecoverBin>d:\temp\nrecover.exe</nrecoverBin>
							<source>src\*.cs</source>
						</ncover>";
			_ncoverTask= NetReflector.Read(xml) as NCoverCoverage;
			Assert.IsNotNull(_ncoverTask);

		}
	}
}

