using System;

using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Test
{
	[TestFixture]
	public class HtmlLinkMessageBuilderTest : Assertion
	{
		private HtmlLinkMessageBuilder _linkBuilder;
	
		[SetUp]
		public void SetUp()
		{
			_linkBuilder = new HtmlLinkMessageBuilder();
		}
		
		[Test]
		public void BuildLinkMessage()
		{
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, IntegrationStatus.Success);
			
		    string message = _linkBuilder.BuildMessage(result,"http://localhost/ccnet");
			AssertEquals(@"CruiseControl.NET Build Results for project Project#9 (http://localhost/ccnet?log=log19800101000000Lbuild.0.xml)", message);
		}

		private IntegrationResult CreateIntegrationResult(IntegrationStatus current, IntegrationStatus last)
		{
			IntegrationResult result = new IntegrationResult();
			result.ProjectName = "Project#9";
			result.Status = current;
			result.LastIntegrationStatus = last;
			result.Label = "0";
			return result;
		}
	}
}
