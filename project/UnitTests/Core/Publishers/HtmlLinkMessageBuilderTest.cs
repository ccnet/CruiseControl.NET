using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	[TestFixture]
	public class HtmlLinkMessageBuilderTest
	{
		[Test]
		public void BuildLinkMessageWithoutAnchorTag()
		{
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Success);
			result.ProjectUrl = "http://localhost/ccnet";

			HtmlLinkMessageBuilder linkMessageBuilder = new HtmlLinkMessageBuilder(false);
			string message = linkMessageBuilder.BuildMessage(result);
			Assert.AreEqual(@"CruiseControl.NET Build Results for project Project#9 (http://localhost/ccnet)", message);
		}

		[Test]
		public void BuildLinkMessageWithAnchorTag()
		{
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Success);
			result.ProjectUrl = "http://localhost/ccnet";

			HtmlLinkMessageBuilder linkMessageBuilder = new HtmlLinkMessageBuilder(true);
			string message = linkMessageBuilder.BuildMessage(result);
			Assert.AreEqual(@"CruiseControl.NET Build Results for project Project#9 (<a href=""http://localhost/ccnet"">web page</a>)", message);
		}

		private IntegrationResult CreateIntegrationResult(IntegrationStatus current, IntegrationStatus last)
		{
			IntegrationResult result = new IntegrationResult();
			result.StartTime = new DateTime(1980, 1, 1);
			result.ProjectName = "Project#9";
			result.Status = current;
			result.LastIntegrationStatus = last;
			result.Label = "0";
			return result;
		}
	}
}