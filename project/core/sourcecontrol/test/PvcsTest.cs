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
	public class PvcsTest 
	{
		
		public static string CreateSourceControlXml()
		{
			return @"    <sourceControl type=""pvcs"">
      <executable>..\etc\pvcs\mockpcli.bat</executable>
	  <project>fooproject</project>
	  <subproject>barsub</subproject>
    </sourceControl>
";
		}

		public void TestValuePopulation()
		{
			Pvcs pvcs = CreatePvcs();
			Assertion.AssertEquals(@"..\etc\pvcs\mockpcli.bat", pvcs.Executable);
			Assertion.AssertEquals("fooproject",pvcs.Project);
			Assertion.AssertEquals("barsub", pvcs.Subproject);
		}

		public void TestCreateProcess()
		{
			Pvcs pvcs = CreatePvcs();
			DateTime from = new DateTime(2001, 1, 21, 20, 0, 0);
			Process actualProcess = pvcs.CreateHistoryProcess(from, new DateTime());

			string expected = Pvcs.COMMAND;
			string actual = actualProcess.StartInfo.Arguments;
			Assertion.AssertEquals(expected, actual);
		}

		private Pvcs CreatePvcs()
		{
			XmlPopulator populator = new XmlPopulator();
			Pvcs pvcs = new Pvcs();
			populator.Populate(XmlUtil.CreateDocumentElement(CreateSourceControlXml()), pvcs);
			return pvcs;
		}
		
		// TODO: stop cmd window from popping up with this test!!!
		[Ignore("Sort out mockpcli stuff")]
		public void TestGetModifications() 
		{
			Pvcs pvcs = CreatePvcs();
			Modification[] mods = pvcs.GetModifications(new DateTime(), new DateTime());
			Assertion.AssertEquals(2, mods.Length);
			File.Delete(Pvcs.PVCS_LOGOUTPUT_FILE);
			Assertion.Assert("input file missing", File.Exists(Pvcs.PVCS_INSTRUCTIONS_FILE));
			File.Delete(Pvcs.PVCS_INSTRUCTIONS_FILE);
		}
		
		public void TestCreatePcliContents() 
		{
			Pvcs pvcs = CreatePvcs();
			string actual = pvcs.CreatePcliContents("beforedate", "afterdate");
			string expected = CreateExpectedContents();
			Assertion.AssertEquals(expected, actual);
		}
		
		private string CreateExpectedContents() 
		{
			return 
@"set -vProject ""fooproject""
set -vSubProject ""barsub""
run ->pvcstemp.txt listversionedfiles -z -aw $Project $SubProject
run -e vlog  ""-xo+epvcsout.txt"" ""-dbeforedate*afterdate"" ""@pvcstemp.txt""
";
		}
	} 
	
}
