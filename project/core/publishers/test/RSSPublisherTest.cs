using System;
using NUnit.Framework;
using NMock;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Test
{
	[TestFixture]
	public class RSSPublisherTest : Assertion
	{
		Project project;
		IntegrationResult result;
		RssPublisher publisher;

		[SetUp]
		public void Setup()
		{
			project = new Project();
			project.Name = "myProject";
			project.WebURL = "http://somewhere/someplace.html";
			result = new IntegrationResult("myProject");
			publisher = new RssPublisher();
		}

		[Test]
		public void ChannelContainsCorrectDetails()
		{
			/// Execute
			Document document = publisher.GenarateDocument(project, result);

			/// Verify
			AssertEquals("CruiseControl.NET - myProject", document.Channel.Title);
			AssertEquals("http://somewhere/someplace.html", document.Channel.Link);
			AssertEquals("The latest build results for myProject", document.Channel.Description);
		}

		[Test]
		public void FirstItemContainsDetailsFromCurrentResult()
		{
			result.Status = IntegrationStatus.Success;
			/// Execute
			Document document = publisher.GenarateDocument(project, result);

			Item firstItem = (Item) document.Channel.Items[0];
			AssertEquals("Successful Build", firstItem.Title);
		}
	}
}
