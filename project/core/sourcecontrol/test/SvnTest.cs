using System;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Diagnostics;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core.Test;
using Exortech.NetReflector;
using NMock;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test
{
	[TestFixture]
	public class SvnTest : CustomAssertion
	{
		public static string CreateSourceControlXml(string trunkUrl)
		{
			return string.Format(
				@"    <sourceControl type=""svn"">
					      <executable>..\tools\subversion-0.37.0\svn.exe</executable>
						  <trunkUrl>{0}</trunkUrl>
						  <timeout>5</timeout>
					  </sourceControl>"
				, trunkUrl);	
		}

		[Test]
		public void PropertyPopulationFromXml()
		{
			Svn svn = CreateSvn(CreateSourceControlXml("svn://myserver/mypath"));
			Assert.AreEqual("..\\tools\\subversion-0.37.0\\svn.exe", svn.Executable);
			Assert.AreEqual("svn://myserver/mypath", svn.TrunkUrl);
			Assert.AreEqual(5, svn.Timeout);
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
		public void ApplyLabel()
		{
			DynamicMock executor = new DynamicMock(typeof(ProcessExecutor));
			Svn svn = new Svn((ProcessExecutor) executor.MockInstance);

			svn.TagOnSuccess = true;
			executor.ExpectAndReturn("Execute", new ProcessResult("foo", null, 0, false), new NMock.Constraints.IsAnything());
			svn.LabelSourceControl("foo", DateTime.Now);
			executor.Verify();

			svn.TagOnSuccess = false;
			executor.ExpectNoCall("Execute", typeof(ProcessInfo));
			svn.LabelSourceControl("foo", DateTime.Now);
			executor.Verify();
		}

		private Svn CreateSvn(string xml)
		{
			Svn svn = new Svn();
			NetReflector.Read(xml, svn);
			return svn;
		}
	} 
}
