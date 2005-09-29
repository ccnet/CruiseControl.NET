using NUnit.Framework;

namespace ThoughtWorks.CruiseControl.UnitTests.Xsl
{
	[TestFixture]
	public class MsTestSummaryStylesheetTest : StylesheetTestFixture
	{
		protected override string Stylesheet
		{
			get { return @"xsl\MsTestSummary.xsl"; }
		}

		[Test]
		public void ShouldNotRenderAnyOutputIfRootNodeIsMissing()
		{
			string xml = WrapInBuildResultsElement("<foo>bar</foo>");
			string actualXml = LoadStylesheetAndTransformInput(xml);
			Assert.AreEqual("", actualXml);
		}

		[Test]
		public void ShouldRenderTotalTestsWhenAllTestsPass()
		{
			string xml = WrapInTestsElement(@"<TestRun type=""Microsoft.VisualStudio.TestTools.Common.TestRun"">
<result type=""Microsoft.VisualStudio.TestTools.Common.RunResultAndStatistics"">
  <totalTestCount type=""System.Int32"">29</totalTestCount> 
  <executedTestCount type=""System.Int32"">29</executedTestCount> 
  <passedTestCount type=""System.Int32"">29</passedTestCount> 
</result></TestRun>");
			string actualXml = LoadStylesheetAndTransformInput(xml);
			CustomAssertion.AssertContains("Tests run: 29", actualXml);
			CustomAssertion.AssertContains("Failures: 0", actualXml);
			CustomAssertion.AssertContains("Not run: 0", actualXml);
			CustomAssertion.AssertContains("All tests passed.", actualXml);
			CustomAssertion.AssertNotContains("FooTest", actualXml);
		}

		[Test]
		public void ShouldRenderFailedTests()
		{
			string xml = WrapInTestsElement(@"<TestRun type=""Microsoft.VisualStudio.TestTools.Common.TestRun"">
<result type=""Microsoft.VisualStudio.TestTools.Common.RunResultAndStatistics"">
  <totalTestCount type=""System.Int32"">29</totalTestCount> 
  <executedTestCount type=""System.Int32"">28</executedTestCount> 
  <passedTestCount type=""System.Int32"">27</passedTestCount> 
</result></TestRun>
<UnitTestResult type=""Microsoft.VisualStudio.TestTools.TestTypes.Unit.UnitTestResult, Microsoft.VisualStudio.QualityTools.Tips.UnitTest.ObjectModel, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"">
	<outcome type=""Microsoft.VisualStudio.TestTools.Common.TestOutcome"">
		<value__ type=""System.Int32"">1</value__>
	</outcome>
	<errorInfo type=""Microsoft.VisualStudio.TestTools.Common.TestResultErrorInfo"">
		<message type=""System.String"">
		Test method Com.Suncor.Olt.Common.DataAccess.DatabaseGatewayTest.ShouldNotAcceptWrongNumberOfParameters threw exception:  System.ApplicationException: Received 7 parameters for InsertFoo, expected 1.
		</message>
		<stackTrace type=""System.String"">
    at Com.Suncor.Olt.Common.DataAccess.DatabaseGateway.CheckNumberOfInputParameters(IDbCommand command, String procedureName, Object[] parameters) in C:\Projects\OLT\src\Common\App\DataAccess\DatabaseGateway.cs:line 148
   at Com.Suncor.Olt.Common.DataAccess.DatabaseGateway.ExecuteNonQuery(String procedureName, Object[] parameters) in C:\Projects\OLT\src\Common\App\DataAccess\DatabaseGateway.cs:line 78
   at Com.Suncor.Olt.Common.DataAccess.DatabaseGatewayTest.ShouldNotAcceptWrongNumberOfParameters() in C:\Projects\OLT\src\Common\Test.UnitTest\DataAccess\DatabaseGatewayTest.cs:line 90
		</stackTrace>
	</errorInfo>
	<testName type=""System.String"">ShouldNotAcceptWrongNumberOfParameters</testName>
</UnitTestResult>
<UnitTestResult type=""Microsoft.VisualStudio.TestTools.TestTypes.Unit.UnitTestResult, Microsoft.VisualStudio.QualityTools.Tips.UnitTest.ObjectModel, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"">
	<outcome type=""Microsoft.VisualStudio.TestTools.Common.TestOutcome"">
		<value__ type=""System.Int32"">10</value__>
	</outcome>
	<testName type=""System.String"">FooTest</testName>
</UnitTestResult>");
			string actualXml = LoadStylesheetAndTransformInput(xml);
			CustomAssertion.AssertContains("Tests run: 28", actualXml);
			CustomAssertion.AssertContains("Failures: 1", actualXml);
			CustomAssertion.AssertContains("Not run: 1", actualXml);
			CustomAssertion.AssertNotContains("All tests passed.", actualXml);
			CustomAssertion.AssertContains("ShouldNotAcceptWrongNumberOfParameters", actualXml);
			CustomAssertion.AssertContains("System.ApplicationException", actualXml);
			CustomAssertion.AssertContains("Stacktrace", actualXml);
			CustomAssertion.AssertNotContains("FooTest", actualXml);
		}

		private string WrapInTestsElement(string xml)
		{
			return WrapInBuildResultsElement(string.Format(@"<Tests>{0}</Tests>", xml));
		}
	}
}