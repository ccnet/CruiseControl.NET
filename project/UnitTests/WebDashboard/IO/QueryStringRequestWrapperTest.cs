using System;
using System.Collections.Specialized;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.IO
{
	[TestFixture]
	public class QueryStringRequestWrapperTest : Assertion
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
			Assert(wrapper.GetBuildSpecifier() is NoBuildSpecified);
		}

		[Test]
		public void ReturnsLogSpecifierWithNameOfFileIfLogParameterSpecified()
		{
			queryString.Add("build", "mylog.xml");
			NamedBuildSpecifier Specifier = (NamedBuildSpecifier) wrapper.GetBuildSpecifier();
			AssertEquals("mylog.xml", Specifier.Filename);
		}

		[Test]
		public void ReturnsEmptyStringIfNoProjectSpecified()
		{
			AssertEquals(string.Empty, wrapper.ProjectName);
		}

		[Test]
		public void ReturnsProjectNameIfProjectSpecified()
		{
			queryString.Add("project", "myproject");
			AssertEquals("myproject", wrapper.ProjectName);
		}

		[Test]
		public void ReturnsEmptyStringIfNoServerSpecified()
		{
			AssertEquals(string.Empty, wrapper.ServerName);
		}

		[Test]
		public void ReturnsServerNameIfServerSpecified()
		{
			queryString.Add("server", "myserver");
			AssertEquals("myserver", wrapper.ServerName);
		}

		[Test]
		public void ReturnsEmptyStringIfNoBuildSpecified()
		{
			AssertEquals(string.Empty, wrapper.BuildName);
		}

		[Test]
		public void ReturnsBuildNameIfBuildSpecified()
		{
			queryString.Add("build", "mybuild");
			AssertEquals("mybuild", wrapper.BuildName);
		}
	}
}
