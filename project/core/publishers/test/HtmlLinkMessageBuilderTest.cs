using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Test
{
	[TestFixture]
	public class HtmlLinkMessageBuilderTest
	{
		[Test]
		public void BuildLinkMessageWithoutAnchorTag()
		{
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Success);

			HtmlLinkMessageBuilder linkMessageBuilder = new HtmlLinkMessageBuilder(false);
			string message = linkMessageBuilder.BuildMessage(result, "http://localhost/ccnet");
			Assert.AreEqual(@"CruiseControl.NET Build Results for project Project#9 (http://localhost/ccnet?log=log19800101000000Lbuild.0.xml)", message);
		}

		[Test]
		public void BuildLinkMessageWithAnchorTag()
		{
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Success);

			HtmlLinkMessageBuilder linkMessageBuilder = new HtmlLinkMessageBuilder(true);
			string message = linkMessageBuilder.BuildMessage(result, "http://localhost/ccnet");
			Assert.AreEqual(@"CruiseControl.NET Build Results for project Project#9 (<a href=""http://localhost/ccnet?log=log19800101000000Lbuild.0.xml"">web page</a>)", message);
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