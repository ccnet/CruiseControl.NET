using System;
using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test
{
	[TestFixture]
	public class SvnTest : CustomAssertion
	{
		private string CreateSourceControlXml(string trunkUrl)
		{
			return CreateSourceControlXml(trunkUrl, null, null);
		}

		private string CreateSourceControlXml(string trunkUrl, string username, string password)
		{
			string usernameAndPassword = (username == null) ? string.Empty : string.Format("<username>{0}</username><password>{1}</password>", username, password);
			return string.Format(
				@"
<sourceControl type=""svn"">
	<executable>..\tools\subversion-0.37.0\svn.exe</executable>
	<trunkUrl>{0}</trunkUrl>
	<timeout>5</timeout>
	<workingDirectory>c:\dev\src</workingDirectory>
	{1}
</sourceControl>"
				, trunkUrl, usernameAndPassword);	
		}

		[Test]
		public void DefaultPropertyPopulationFromXml()
		{
			Svn svn = CreateSvn(CreateSourceControlXml("svn://myserver/mypath"));
			Assert.AreEqual("..\\tools\\subversion-0.37.0\\svn.exe", svn.Executable);
			Assert.AreEqual("svn://myserver/mypath", svn.TrunkUrl);
			Assert.AreEqual(5, svn.Timeout);
			Assert.AreEqual(@"c:\dev\src", svn.WorkingDirectory);
		}

		[Test]
		public void UserAndPasswordPropertyPopulationFromXml()
		{
			Svn svn = CreateSvn(CreateSourceControlXml("svn://myserver/mypath", "user", "password"));
			Assert.AreEqual("user", svn.Username);
			Assert.AreEqual("password", svn.Password);
		}

		[Test]
		public void CreatingHistoryProcessIncludesCorrectlyFormattedArguments()
		{
			Svn svn = CreateSvn(CreateSourceControlXml("svn://someserver/"));
			DateTime from = DateTime.Parse("2001-01-21  20:00:00 'GMT'");
			DateTime to = DateTime.Parse("2001-01-21  20:30:50 'GMT'");
			ProcessInfo actualProcess = svn.CreateHistoryProcessInfo(from, to);

			Assert.AreEqual("log -v -r \"{2001-01-21T20:00:00Z}:{2001-01-21T20:30:50Z}\" --xml svn://someserver/", actualProcess.Arguments);
		}

		[Test]
		public void CreatingHistoryProcessIncludesCorrectlyFormattedArgumentsForUsernameAndPassword()
		{
			Svn svn = CreateSvn(CreateSourceControlXml("svn://someserver/", "user", "password"));
			DateTime from = DateTime.Parse("2001-01-21  20:00:00 'GMT'");
			DateTime to = DateTime.Parse("2001-01-21  20:30:50 'GMT'");
			ProcessInfo actualProcess = svn.CreateHistoryProcessInfo(from, to);

			string expectedOutput = @"log -v -r ""{2001-01-21T20:00:00Z}:{2001-01-21T20:30:50Z}"" --xml svn://someserver/ --username ""user"" --password ""password""";
			Assert.AreEqual(expectedOutput, actualProcess.Arguments);
		}

		[Test]
		public void ApplyLabel()
		{
			DynamicMock executor = new DynamicMock(typeof(ProcessExecutor));
			Svn svn = new Svn((ProcessExecutor) executor.MockInstance);

			svn.TagOnSuccess = true;
			executor.ExpectAndReturn("Execute", new ProcessResult("foo", null, 0, false), new IsAnything());
			svn.LabelSourceControl("foo", DateTime.Now);
			executor.Verify();

			svn.TagOnSuccess = false;
			executor.ExpectNoCall("Execute", typeof(ProcessInfo));
			svn.LabelSourceControl("foo", DateTime.Now);
			executor.Verify();
		}

		[Test]
		public void CreatingLabelProcessIncludesCorrectlyFormattedArgumentsForUsernameAndPassword()
		{
			Svn svn = CreateSvn(CreateSourceControlXml("svn://someserver/", "user", "password"));
			svn.TagBaseUrl = "svn://someserver/tags";
			DateTime date = DateTime.Parse("2001-01-21  20:00:00 'GMT'");
			ProcessInfo actualProcess = svn.CreateLabelProcessInfo("foo", date);

			string expectedOutput = @"copy -m ""CCNET build foo"" svn://someserver/ svn://someserver/tags/foo --username ""user"" --password ""password""";
			Assert.AreEqual(expectedOutput, actualProcess.Arguments);
		}

		private Svn CreateSvn(string xml)
		{
			Svn svn = new Svn();
			NetReflector.Read(xml, svn);
			return svn;
		}
	} 
}
