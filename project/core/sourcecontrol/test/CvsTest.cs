using Exortech.NetReflector;
using NUnit.Framework;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core.Test;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test
{
	[TestFixture]
	public class CvsTest : CustomAssertion
	{
		public static string CreateSourceControlXml(string cvsroot, string branch)
		{
			cvsroot = (cvsroot == null) ? String.Empty : "<cvsroot>" + cvsroot + "</cvsroot>";
			branch = (branch == null) ? String.Empty : "<branch>" + branch + "</branch>";

			return string.Format(
				@"    <sourceControl type=""cvs"">
      <executable>..\tools\cvs.exe</executable>
      <workingDirectory>..</workingDirectory>
      {0}
	  {1}
    </sourceControl>
"
				, cvsroot, branch);	
		}

		[Test]
		public void ValuePopulation()
		{
			Cvs cvs = CreateCvs(CreateSourceControlXml("myCvsRoot", "branch"));
			AssertEquals(@"..\tools\cvs.exe", cvs.Executable);
			AssertEquals("..",cvs.WorkingDirectory);
			AssertEquals("myCvsRoot", cvs.CvsRoot);
			AssertEquals("branch", cvs.Branch);
		}

		[Test]
		public void CreateProcess()
		{
			Cvs cvs = CreateCvs();
			DateTime from = new DateTime(2001, 1, 21, 20, 0, 0);
			ProcessInfo actualProcess = cvs.CreateHistoryProcessInfo(from, new DateTime());

			string expected = string.Format(@"-q log -N ""-d>{0}""", cvs.FormatCommandDate(from));
			string actual = actualProcess.Arguments;
			AssertEquals(expected, actual);
		}

		[Test]
		public void HistoryArgs()
		{
			DateTime from = new DateTime(2001, 1, 21, 20, 0, 0);

			Cvs cvs = new Cvs();
			cvs.CvsRoot = "myCvsRoot";
			cvs.Branch = "branch"; 
			string args = cvs.BuildHistoryProcessInfoArgs(from);
			string expected = string.Format(@"-d myCvsRoot -q log -N ""-d>{0}"" -rbranch", cvs.FormatCommandDate(from));
			AssertEquals(expected, args);
		}

		[Test]
		public void CreateProcess_MissingCvsRoot()
		{
			Cvs cvs = CreateCvs();
			DateTime from = new DateTime(2001, 1, 21, 20, 0, 0);
			ProcessInfo actualProcess = cvs.CreateHistoryProcessInfo(from, new DateTime());

			string expected = string.Format(@"-q log -N ""-d>{0}""", cvs.FormatCommandDate(from));
			string actual = actualProcess.Arguments;
			AssertEquals(expected, actual);
		}

		[Test]
		public void Executable_default()
		{
			AssertEquals("cvs.exe", new Cvs().Executable);
		}

		[Test]
		public void VerifyDateIsFormatedCorrectly()
		{
			DateTime dt = DateTime.Parse("2003-01-01 01:01:01 GMT", CultureInfo.InvariantCulture);
			AssertEquals("2003-01-01 01:01:01 GMT", CreateCvs().FormatCommandDate(dt));
		}

		private Cvs CreateCvs()
		{
			return CreateCvs(CreateSourceControlXml(null, null));
		}

		private Cvs CreateCvs(string xml)
		{
			Cvs cvs = new Cvs();
			NetReflector.Read(xml, cvs);
			return cvs;
		}
	} 
	
}
