using System;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Diagnostics;
using NUnit.Framework;
using tw.ccnet.core.util;
using Exortech.NetReflector;

namespace tw.ccnet.core.sourcecontrol.test
{
	[TestFixture]
	public class CvsTest 
	{
		public static string CreateSourceControlXml(string cvsroot)
		{
			cvsroot = (cvsroot == null) ? String.Empty : "<cvsroot>" + cvsroot + "</cvsroot>";
			return String.Format(
				@"    <sourceControl type=""cvs"">
      <executable>..\tools\cvs.exe</executable>
      <workingDirectory>..</workingDirectory>
      {0}
    </sourceControl>
"
				, cvsroot);	
		}

		public void TestValuePopulation()
		{
			Cvs cvs = CreateCvs();
			Assertion.AssertEquals(@"..\tools\cvs.exe", cvs.Executable);
			Assertion.AssertEquals("..",cvs.WorkingDirectory);
			Assertion.AssertEquals("myCvsRoot", cvs.CvsRoot);
		}

		public void TestCreateProcess()
		{
			Cvs cvs = CreateCvs();
			DateTime from = new DateTime(2001, 1, 21, 20, 0, 0);
			Process actualProcess = cvs.CreateHistoryProcess(from, new DateTime());

			string expected = String.Format(@"-d myCvsRoot -q log -N ""-d>{0}""", cvs.FormatCommandDate(from));
			string actual = actualProcess.StartInfo.Arguments;
			Assertion.AssertEquals(expected, actual);
		}

		public void TestCreateProcess_MissingCvsRoot()
		{
			Cvs cvs = CreateCvs(null);
			DateTime from = new DateTime(2001, 1, 21, 20, 0, 0);
			Process actualProcess = cvs.CreateHistoryProcess(from, new DateTime());

			string expected = String.Format(@"-q log -N ""-d>{0}""", cvs.FormatCommandDate(from));
			string actual = actualProcess.StartInfo.Arguments;
			Assertion.AssertEquals(expected, actual);
		}

		public void TestLabelOnSuccessProcess()
		{
			Cvs cvs = CreateCvs(null);

			Assertion.AssertNull(cvs.CreateLabelProcess("", new DateTime()));

			cvs.LabelOnSuccess = true;
			Assertion.AssertNotNull(cvs.CreateLabelProcess("", new DateTime()));
		}
	
		public void TestExecutable_default()
		{
			Assertion.AssertEquals("cvs.exe", new Cvs().Executable);
		}

		private Cvs CreateCvs()
		{
			return CreateCvs("myCvsRoot");
		}

		private Cvs CreateCvs(string cvsroot)
		{
			XmlPopulator populator = new XmlPopulator();
			Cvs cvs = new Cvs();
			populator.Populate(XmlUtil.CreateDocumentElement(CreateSourceControlXml(cvsroot)), cvs);
			return cvs;
		}
	} 
	
}
