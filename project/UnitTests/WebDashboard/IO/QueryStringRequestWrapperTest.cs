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
			Assert(wrapper.GetBuildSpecifier() is NoLogSpecified);
		}

		[Test]
		public void ReturnsLogSpecifierWithNameOfFileIfLogParameterSpecified()
		{
			queryString.Add("log", "mylog.xml");
			FileNameLogSpecifier logSpecifier = (FileNameLogSpecifier) wrapper.GetBuildSpecifier();
			AssertEquals("mylog.xml", logSpecifier.Filename);
		}

		[Test]
		public void ReturnsEmptyStringIfNoProjectSpecified()
		{
			AssertEquals(string.Empty, wrapper.GetProjectName());
		}

		[Test]
		public void ReturnsProjectNameIfProjectSpecified()
		{
			queryString.Add("project", "myproject");
			AssertEquals("myproject", wrapper.GetProjectName());
		}

		[Test]
		public void ReturnsEmptyStringIfNoServerSpecified()
		{
			AssertEquals(string.Empty, wrapper.GetServerName());
		}

		[Test]
		public void ReturnsServerNameIfServerSpecified()
		{
			queryString.Add("server", "myserver");
			AssertEquals("myserver", wrapper.GetServerName());
		}
	}
}
