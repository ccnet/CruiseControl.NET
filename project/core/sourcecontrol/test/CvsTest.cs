using System;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Diagnostics;
using NUnit.Framework;
using tw.ccnet.core.util;
using tw.ccnet.core.test;
using Exortech.NetReflector;

namespace tw.ccnet.core.sourcecontrol.test
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
			Process actualProcess = cvs.CreateHistoryProcess(from, new DateTime());

			string expected = string.Format(@"-q log -N ""-d>{0}""", cvs.FormatCommandDate(from));
			string actual = actualProcess.StartInfo.Arguments;
			AssertEquals(expected, actual);
		}

		[Test]
		public void HistoryArgs()
		{
			DateTime from = new DateTime(2001, 1, 21, 20, 0, 0);

			Cvs cvs = new Cvs();
			cvs.CvsRoot = "myCvsRoot";
			cvs.Branch = "branch"; 
			string args = cvs.BuildHistoryProcessArgs(from);
			string expected = string.Format(@"-d myCvsRoot -q log -N ""-d>{0}"" -rbranch", cvs.FormatCommandDate(from));
			AssertEquals(expected, args);
		}

		[Test]
		public void CreateProcess_MissingCvsRoot()
		{
			Cvs cvs = CreateCvs();
			DateTime from = new DateTime(2001, 1, 21, 20, 0, 0);
			Process actualProcess = cvs.CreateHistoryProcess(from, new DateTime());

			string expected = string.Format(@"-q log -N ""-d>{0}""", cvs.FormatCommandDate(from));
			string actual = actualProcess.StartInfo.Arguments;
			AssertEquals(expected, actual);
		}

		[Test]
		public void LabelOnSuccessProcess()
		{
			Cvs cvs = CreateCvs();

			AssertNull(cvs.CreateLabelProcess("", new DateTime()));

			cvs.LabelOnSuccess = true;
			AssertNotNull(cvs.CreateLabelProcess("", new DateTime()));
		}
	
		[Test]
		public void Executable_default()
		{
			AssertEquals("cvs.exe", new Cvs().Executable);
		}

		private Cvs CreateCvs()
		{
			return CreateCvs(CreateSourceControlXml(null, null));
		}

		private Cvs CreateCvs(string xml)
		{
			XmlPopulator populator = new XmlPopulator();
			Cvs cvs = new Cvs();
			populator.Populate(XmlUtil.CreateDocumentElement(xml), cvs);
			return cvs;
		}
	} 
	
}
