using System;
using System.Collections.Specialized;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.IO
{
	[TestFixture]
	public class QueryStringRequestWrapperTest
	{
		private NameValueCollection queryString;
		private QueryStringRequestWrapper wrapper;

		[SetUp]
		public void Setup()
		{
			queryString = new NameValueCollection();
			wrapper = new QueryStringRequestWrapper(queryString);
		}

		[Test]
		public void ReturnsNoLogSpecifiedIfNoLogParameterSpecified()
		{
			Assert.IsTrue(wrapper.GetBuildSpecifier() is NoBuildSpecified);
		}

		[Test]
		public void ReturnsLogSpecifierWithNameOfFileIfLogParameterSpecified()
		{
			queryString.Add("build", "mylog.xml");
			NamedBuildSpecifier Specifier = (NamedBuildSpecifier) wrapper.GetBuildSpecifier();
			Assert.AreEqual("mylog.xml", Specifier.Filename);
		}

		[Test]
		public void ReturnsEmptyStringIfNoProjectSpecified()
		{
			Assert.AreEqual(string.Empty, wrapper.ProjectName);
		}

		[Test]
		public void ReturnsProjectNameIfProjectSpecified()
		{
			queryString.Add("project", "myproject");
			Assert.AreEqual("myproject", wrapper.ProjectName);
		}

		[Test]
		public void ReturnsEmptyStringIfNoServerSpecified()
		{
			Assert.AreEqual(string.Empty, wrapper.ServerName);
		}

		[Test]
		public void ReturnsServerNameIfServerSpecified()
		{
			queryString.Add("server", "myserver");
			Assert.AreEqual("myserver", wrapper.ServerName);
		}

		[Test]
		public void ReturnsEmptyStringIfNoBuildSpecified()
		{
			Assert.AreEqual(string.Empty, wrapper.BuildName);
		}

		[Test]
		public void ReturnsBuildNameIfBuildSpecified()
		{
			queryString.Add("build", "mybuild");
			Assert.AreEqual("mybuild", wrapper.BuildName);
		}
	}
}
