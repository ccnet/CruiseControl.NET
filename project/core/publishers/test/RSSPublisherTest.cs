using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Test
{
	[TestFixture]
	public class RSSPublisherTest
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
			result = new IntegrationResult("myProject", @"c:\temp");
			publisher = new RssPublisher();
		}

		[Test]
		public void ChannelContainsCorrectDetails()
		{
			/// Execute
			Document document = publisher.GenerateDocument(project, result);

			/// Verify
			Assert.AreEqual("CruiseControl.NET - myProject", document.Channel.Title);
			Assert.AreEqual("http://somewhere/someplace.html", document.Channel.Link);
			Assert.AreEqual("The latest build results for myProject", document.Channel.Description);
		}

		[Test]
		public void FirstItemContainsDetailsFromCurrentResult()
		{
			result.Status = IntegrationStatus.Success;
			/// Execute
			Document document = publisher.GenerateDocument(project, result);

			Item firstItem = (Item) document.Channel.Items[0];
			Assert.AreEqual("Successful Build", firstItem.Title);
		}
	}
}
