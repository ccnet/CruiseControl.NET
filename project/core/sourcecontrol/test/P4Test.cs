using System;
using System.Diagnostics;
using NUnit.Framework;
using NMock;
using tw.ccnet.core.test;
using tw.ccnet.core.util;
using Exortech.NetReflector;

namespace tw.ccnet.core.sourcecontrol.test
{
	[TestFixture]
	public class P4Test
	{

		[Test]
		public void ReadConfig()
		{
			string xml = @"
<sourceControl type=""p4"">
  <executable>c:\bin\p4.exe</executable>
  <view>//depot/myproject/...</view>
  <client>myclient</client>
  <user>me</user>
  <port>anotherserver:2666</port>
</sourceControl>
";
			P4 p4 = CreateP4(xml);
			Assertion.AssertEquals(@"c:\bin\p4.exe", p4.Executable);
			Assertion.AssertEquals("//depot/myproject/...", p4.View);
			Assertion.AssertEquals("myclient", p4.Client);
			Assertion.AssertEquals("me", p4.User);
			Assertion.AssertEquals("anotherserver:2666", p4.Port);
		}

		private P4 CreateP4(string p4root)
		{
			XmlPopulator populator = new XmlPopulator();
			P4 perforce = new P4();
			populator.Populate(XmlUtil.CreateDocumentElement(p4root), perforce);
			return perforce;
		}
		
		[Test]
		public void ReadConfigDefaults()
		{
			string xml = @"
<sourceControl name=""p4"">
  <view>//depot/anotherproject/...</view>
</sourceControl>
";
			P4 p4 = CreateP4(xml);
			Assertion.AssertEquals("p4", p4.Executable);
			Assertion.AssertEquals("//depot/anotherproject/...", p4.View);
			Assertion.AssertNull(p4.Client);
			Assertion.AssertNull(p4.User);
			Assertion.AssertNull(p4.Port);
		}

		[Test]
		[ExpectedException(typeof(ReflectorException))]
		public void ReadConfigBarfsWhenViewIsExcluded()
		{
			string xml = @"
<sourceControl name=""p4"">
</sourceControl>
";
			P4 p4 = CreateP4(xml);
		}

		[Test]
		public void CreateGetChangeListsProcess()
		{
			P4 p4 = new P4();
			p4.View = "//depot/myproj/...";
			DateTime from = new DateTime(2002, 10, 20, 2, 0, 0);
			DateTime to = new DateTime(2002, 10, 31, 5, 5, 0);

			Process process = p4.CreateChangeListProcess(from, to);

			string expectedArgs = "-s changes -s submitted //depot/myproj/...@2002/10/20:02:00:00,@2002/10/31:05:05:00";

			Assertion.AssertEquals("p4", process.StartInfo.FileName);
			Assertion.AssertEquals(expectedArgs, process.StartInfo.Arguments);
		}

		[Test]
		public void CreateGetChangeListsProcessWithDifferentArgs()
		{

			string xml = @"
<sourceControl name=""p4"">
  <executable>c:\bin\p4.exe</executable>
  <view>//depot/myproject/...</view>
  <client>myclient</client>
  <user>me</user>
  <port>anotherserver:2666</port>
</sourceControl>
";

			DateTime from = new DateTime(2003, 11, 20, 2, 10, 32);
			DateTime to = new DateTime(2004, 10, 31, 5, 5, 1);

			string expectedArgs = "-s -c myclient -p anotherserver:2666 -u me"
				+ " changes -s submitted //depot/myproject/...@2003/11/20:02:10:32,@2004/10/31:05:05:01";
			
			P4 p4 = CreateP4(xml);
			Process process = p4.CreateChangeListProcess(from, to);

			Assertion.AssertEquals("c:\\bin\\p4.exe", process.StartInfo.FileName);
			Assertion.AssertEquals(expectedArgs, process.StartInfo.Arguments);
		}

		[Test]
		public void CreateGetDescribeProcess()
		{
			string changes = "3327 3328 332";
			Process process = new P4().CreateDescribeProcess(changes);

			string expectedArgs = "-s describe -s " + changes;
			Assertion.AssertEquals("p4", process.StartInfo.FileName);
			Assertion.AssertEquals(expectedArgs, process.StartInfo.Arguments);
		}

		[Test]
		[ExpectedException(typeof(CruiseControlException))]
		public void CreateGetDescribeProcessWithEvilCode()
		{
			string changes = "3327 3328 332; echo 'rm -rf /'";
			Process process = new P4().CreateDescribeProcess(changes);
		}

		[Test]
		[ExpectedException(typeof(Exception))]
		public void CreateGetDescribeProcessWithNoChanges()
		{
			string changes = "";
			Process process = new P4().CreateDescribeProcess(changes);
			// this should never happen, but here's a test just in case.
		}

		[Test]
		public void GetModifications()
		{
			DateTime from = new DateTime(2002, 11, 1);
			DateTime to = new DateTime(2002, 11, 14);

			DynamicMock mock = new DynamicMock(typeof(P4));
			mock.Ignore("GetModifications");
			mock.Ignore("CreateChangeListProcess");

			string changes = @"
info: Change 3328 on 2002/10/31 by someone@somewhere 'Something important '
info: Change 3327 on 2002/10/31 by someone@somewhere 'Joe's test '
info: Change 332 on 2002/10/31 by someone@somewhere 'thingy'
exit: 0
";
			mock.ExpectAndReturn("execute", changes);

			mock.ExpectAndReturn("execute", P4Mother.P4_LOGFILE_CONTENT); 

			P4 p4 = (P4)mock.MockInstance;
			Modification[] result = p4.GetModifications(from, to);

			mock.Verify();
			Assertion.AssertEquals(7, result.Length);
		}

	}
}
