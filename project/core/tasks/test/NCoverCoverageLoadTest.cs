using System;
using ThoughtWorks.CruiseControl.Core.Util;
using NUnit.Framework;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Tasks.Test
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
			AssertNotNull(_ncoverTask);

		}
	}
}

