using System;
using NUnit.Framework;
using NMock;
using ThoughtWorks.CruiseControl.Core.Test;
using ThoughtWorks.CruiseControl.Core.Util;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test
{
	[TestFixture]
	public class P4Test : CustomAssertion
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
			AssertEquals(@"c:\bin\p4.exe", p4.Executable);
			AssertEquals("//depot/myproject/...", p4.View);
			AssertEquals("myclient", p4.Client);
			AssertEquals("me", p4.User);
			AssertEquals("anotherserver:2666", p4.Port);
		}

		private P4 CreateP4(string p4root)
		{
			P4 perforce = new P4();
			NetReflector.Read(p4root, perforce);
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
			AssertEquals("p4", p4.Executable);
			AssertEquals("//depot/anotherproject/...", p4.View);
			AssertNull(p4.Client);
			AssertNull(p4.User);
			AssertNull(p4.Port);
		}

		[Test]
		[ExpectedException(typeof(NetReflectorException))]
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

			ProcessInfo process = p4.CreateChangeListProcess(from, to);

			string expectedArgs = "-s changes -s submitted //depot/myproj/...@2002/10/20:02:00:00,@2002/10/31:05:05:00";

			AssertEquals("p4", process.FileName);
			AssertEquals(expectedArgs, process.Arguments);
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
			ProcessInfo process = p4.CreateChangeListProcess(from, to);

			AssertEquals("c:\\bin\\p4.exe", process.FileName);
			AssertEquals(expectedArgs, process.Arguments);
		}

		[Test]
		public void CreateGetDescribeProcess()
		{
			string changes = "3327 3328 332";
			ProcessInfo process = new P4().CreateDescribeProcess(changes);

			string expectedArgs = "-s describe -s " + changes;
			AssertEquals("p4", process.FileName);
			AssertEquals(expectedArgs, process.Arguments);
		}

		[Test]
		public void CreateGetDescribeProcessWithSpecifiedArgs() {
			string xml = @"
<sourceControl name=""p4"">
  <executable>c:\bin\p4.exe</executable>
  <view>//depot/myproject/...</view>
  <client>myclient</client>
  <user>me</user>
  <port>anotherserver:2666</port>
</sourceControl>
";
			string changes = "3327 3328 332";
			
			string expectedArgs = "-s -c myclient -p anotherserver:2666 -u me"
				+ " describe -s " + changes;
			
			P4 p4 = CreateP4(xml);
			ProcessInfo process = p4.CreateDescribeProcess(changes);

			AssertEquals("c:\\bin\\p4.exe", process.FileName);
			AssertEquals(expectedArgs, process.Arguments);
		}

		[Test]
		[ExpectedException(typeof(CruiseControlException))]
		public void CreateGetDescribeProcessWithEvilCode()
		{
			string changes = "3327 3328 332; echo 'rm -rf /'";
			ProcessInfo process = new P4().CreateDescribeProcess(changes);
		}

		[Test]
		[ExpectedException(typeof(Exception))]
		public void CreateGetDescribeProcessWithNoChanges()
		{
			string changes = "";
			ProcessInfo process = new P4().CreateDescribeProcess(changes);
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
			mock.ExpectAndReturn("Execute", changes, new NMock.Constraints.IsTypeOf(typeof(ProcessInfo)));

			mock.ExpectAndReturn("Execute", P4Mother.P4_LOGFILE_CONTENT, new NMock.Constraints.IsAnything()); 

			P4 p4 = (P4)mock.MockInstance;
			Modification[] result = p4.GetModifications(from, to);

			mock.Verify();
			AssertEquals(7, result.Length);
		}

	}
}
