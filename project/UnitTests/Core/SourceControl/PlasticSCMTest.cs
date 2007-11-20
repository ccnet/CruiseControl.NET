using System;
using System.Globalization;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class PlasticSCMTest : CustomAssertion
	{
		public const string PLASTICSCM_XML =
			@"<sourceControl type=""plasticscm"" branch=""br:/main"">
				<executable>c:\plastic\client\cm.exe</executable>
				<repository>mainrep</repository>
				<workingDirectory>c:\workspace</workingDirectory>
				<labelOnSuccess>true</labelOnSuccess>
				<labelPrefix>BL</labelPrefix>
                <forced>true</forced>
			</sourceControl>";

		public const string PLASTICSCM_ERR1_XML =
			@"<sourceControl type=""plasticscm"" branch=""br:/main"">
				<repository>mainrep</repository>
                <forced>true</forced>
			</sourceControl>";

		public const string PLASTICSCM_ERR2_XML =
			@"<sourceControl type=""plasticscm"">
				<repository>mainrep</repository>
                <workingDirectory>c:\workspace</workingDirectory>
                <forced>true</forced>
			</sourceControl>";

		public const string PLASTICSCM_BASIC_XML =
			@"<sourceControl type=""plasticscm"">
				<branch>br:/main</branch>
                <workingDirectory>c:\workspace</workingDirectory>
			</sourceControl>";

		private PlasticSCM plasticscm;

		[SetUp]
		protected void SetUp()
		{
			plasticscm = new PlasticSCM();
			NetReflector.Read(PLASTICSCM_XML, plasticscm);
		}

		[Test]
		public void VerifyNoRequiredAttribute()
		{
			plasticscm = new PlasticSCM();
			try
			{
				NetReflector.Read(PLASTICSCM_ERR1_XML, plasticscm);
				Assert.Fail ("without alll required attributes should fail");
			}
			catch(Exception)  { }

			plasticscm = new PlasticSCM();
			try
			{
				NetReflector.Read(PLASTICSCM_ERR2_XML, plasticscm);
				Assert.Fail ("without alll required attributes should fail");
			}
			catch(Exception) { }

			plasticscm = new PlasticSCM();
			NetReflector.Read(PLASTICSCM_BASIC_XML, plasticscm);

		}

		[Test]
		public void VerifyValuesSetByNetReflector()
		{
			Assert.AreEqual(@"c:\plastic\client\cm.exe", plasticscm.Executable);
			Assert.AreEqual("mainrep", plasticscm.Repository);
			Assert.AreEqual("br:/main", plasticscm.Branch);
			Assert.AreEqual(true, plasticscm.Forced);
			Assert.AreEqual(true, plasticscm.LabelOnSuccess);
			Assert.AreEqual("BL", plasticscm.LabelPrefix);
			Assert.AreEqual(@"c:\workspace", plasticscm.WorkingDirectory);
		}

		[Test]
		public void VerifyNewGetSourceProcessInfo()
		{
            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, "source");
            IntegrationSummary lastSummary =
                new IntegrationSummary(IntegrationStatus.Success, "label", "lastlabel", DateTime.Now);
            IntegrationResult result = new IntegrationResult("test", @"c:\workspace", @"c:\artifacts", request, lastSummary);

			//basic check
			plasticscm = new PlasticSCM();
			NetReflector.Read(PLASTICSCM_BASIC_XML, plasticscm);
			string expected = @"cm update c:\workspace";
			ProcessInfo info = plasticscm.NewGetSourceProcessInfo(result);
			Assert.AreEqual (expected, info.FileName + " " + info.Arguments);

			//check with attributes
			plasticscm = new PlasticSCM();
			NetReflector.Read(PLASTICSCM_XML, plasticscm);
			expected = @"c:\plastic\client\cm.exe update c:\workspace --forced";
			info = plasticscm.NewGetSourceProcessInfo(result);
			Assert.AreEqual (expected, info.FileName + " " + info.Arguments);
		
		}

		[Test]
		public void VerifyGoToBranchProcessInfo()
		{
            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, "source");
            IntegrationSummary lastSummary =
                new IntegrationSummary(IntegrationStatus.Success, "label", "lastlabel", DateTime.Now);
            IntegrationResult result = new IntegrationResult("test", @"c:\workspace", @"c:\artifacts", request, lastSummary);

			//basic check
			plasticscm = new PlasticSCM();
			NetReflector.Read(PLASTICSCM_BASIC_XML, plasticscm);
			string expected = @"cm stb br:/main --noupdate";
			ProcessInfo info = plasticscm.GoToBranchProcessInfo(result);
			Assert.AreEqual (expected, info.FileName + " " + info.Arguments);

			//check with attributes
			plasticscm = new PlasticSCM();
			NetReflector.Read(PLASTICSCM_XML, plasticscm);
			expected = @"c:\plastic\client\cm.exe stb br:/main -repository=mainrep --noupdate";
			info = plasticscm.GoToBranchProcessInfo(result);
			Assert.AreEqual (expected, info.FileName + " " + info.Arguments);

		}

		[Test]
		public void VerifyCreateQueryProcessInfo()
		{
			string fromtime = "01/02/2003 00:00:00";
			string totime = "23/02/2006 23:14:05";
            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, "source");
            IntegrationSummary lastSummary =
                new IntegrationSummary(IntegrationStatus.Success, "label", "lastlabel", DateTime.Now);
            IntegrationResult from =
                new IntegrationResult("test", @"c:\workspace", @"c:\artifacts", request, lastSummary);
			from.StartTime = DateTime.ParseExact (fromtime, PlasticSCM.DATEFORMAT, System.Globalization.CultureInfo.InvariantCulture);
            IntegrationResult to = new IntegrationResult("test", @"c:\workspace", @"c:\artifacts", request, lastSummary);
			to.StartTime = DateTime.ParseExact (totime, PlasticSCM.DATEFORMAT, System.Globalization.CultureInfo.InvariantCulture);
			
			//basic check
			plasticscm = new PlasticSCM();
			NetReflector.Read(PLASTICSCM_BASIC_XML, plasticscm);
			string query = string.Format (
                            "cm find revision where branch = 'br:/main' and revno != 'CO' "
							+ "and date between '{0}' and '{1}' ", fromtime, totime);
			string dateformat = string.Format ("--dateformat=\"{0}\" ", PlasticSCM.DATEFORMAT);
			string format = string.Format ("--format=\"{0}\"", PlasticSCM.FORMAT);

			ProcessInfo info = plasticscm.CreateQueryProcessInfo(from, to);
			Assert.AreEqual (query + dateformat + format, info.FileName + " " + info.Arguments);

			//check with attributes
			plasticscm = new PlasticSCM();
			NetReflector.Read(PLASTICSCM_XML, plasticscm);
			query = string.Format (
				@"c:\plastic\client\cm.exe find revision where branch = 'br:/main' and revno != 'CO' "
				+ "and date between '{0}' and '{1}' on repository 'mainrep' ", fromtime, totime);

			info = plasticscm.CreateQueryProcessInfo(from, to);
			Assert.AreEqual (query + dateformat + format, info.FileName + " " + info.Arguments);

		}

		[Test]
		public void VerifyCreateLabelProcessInfo()
		{
            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, "source");
            IntegrationSummary lastSummary = new IntegrationSummary(IntegrationStatus.Success, "label", "lastlabel", DateTime.Now);
            IntegrationResult result = new IntegrationResult("test", @"c:\workspace", @"c:\artifacts", request, lastSummary);
			result.Label = "1";

			//basic check
			plasticscm = new PlasticSCM();
			NetReflector.Read(PLASTICSCM_BASIC_XML, plasticscm);
			string expected = @"cm mklb ccver-1";
			ProcessInfo info = plasticscm.CreateLabelProcessInfo(result);
			Assert.AreEqual (expected, info.FileName + " " + info.Arguments);

			//check with attributes
			plasticscm = new PlasticSCM();
			NetReflector.Read(PLASTICSCM_XML, plasticscm);
			expected = @"c:\plastic\client\cm.exe mklb BL1";
			info = plasticscm.CreateLabelProcessInfo(result);
			Assert.AreEqual (expected, info.FileName + " " + info.Arguments);
		}

		[Test]
		public void VerifyLabelProcessInfo()
		{

            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, "source");
            IntegrationSummary lastSummary = new IntegrationSummary(IntegrationStatus.Success, "label", "lastlabel", DateTime.Now);
            IntegrationResult result = new IntegrationResult("test", @"c:\workspace", @"c:\artifacts", request, lastSummary);
			result.Label = "1";

			//basic check
			plasticscm = new PlasticSCM();
			NetReflector.Read(PLASTICSCM_BASIC_XML, plasticscm);
			string expected = @"cm label -R lb:ccver-1 .";
			ProcessInfo info = plasticscm.LabelProcessInfo(result);
			Assert.AreEqual (expected, info.FileName + " " + info.Arguments);

			//check with attributes
			plasticscm = new PlasticSCM();
			NetReflector.Read(PLASTICSCM_XML, plasticscm);
			expected = @"c:\plastic\client\cm.exe label -R lb:BL1 .";
			info = plasticscm.LabelProcessInfo(result);
			Assert.AreEqual (expected, info.FileName + " " + info.Arguments);
		}

		[Test]
		public void VerifyDefaults()
		{
			plasticscm = new PlasticSCM();

			Assert.AreEqual("cm", plasticscm.Executable);
			Assert.AreEqual(string.Empty, plasticscm.Repository);
			Assert.AreEqual(string.Empty, plasticscm.Branch);
			Assert.AreEqual(false, plasticscm.Forced);
			Assert.AreEqual(false, plasticscm.LabelOnSuccess);
			Assert.AreEqual("ccver-", plasticscm.LabelPrefix);
			Assert.AreEqual(string.Empty, plasticscm.WorkingDirectory);
		}
	}
}