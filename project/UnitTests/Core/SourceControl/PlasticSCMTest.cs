using System;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
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

        public const string PLASTICSCM_BASIC_XML =
            @"<sourceControl type=""plasticscm"">
                <branch>br:/main</branch>
                <workingDirectory>c:\workspace</workingDirectory>
            </sourceControl>";

        [Test]
        public void ShouldFailToConfigureWithoutRequiredAttributes()
        {
            const string PLASTICSCM_ERR2_XML =
                @"<sourceControl type=""plasticscm"">
                    <repository>mainrep</repository>
                    <workingDirectory>c:\workspace</workingDirectory>
                    <forced>true</forced>
                 </sourceControl>";

            PlasticSCM plasticscm = new PlasticSCM();
            Assert.That(delegate { NetReflector.Read(PLASTICSCM_ERR2_XML, plasticscm); },
                        Throws.TypeOf<NetReflectorException>());

        }

        [Test]
        public void ShouldConfigureWithBasicXml()
        {
            PlasticSCM plasticscm = new PlasticSCM();
            NetReflector.Read(PLASTICSCM_BASIC_XML, plasticscm);
        }

        [Test]
        public void VerifyValuesSetByNetReflector()
        {
            PlasticSCM plasticscm = new PlasticSCM();
            NetReflector.Read(PLASTICSCM_XML, plasticscm);

            Assert.AreEqual(@"c:\plastic\client\cm.exe", plasticscm.Executable);
            Assert.AreEqual("mainrep", plasticscm.Repository);
            Assert.AreEqual("br:/main", plasticscm.Branch);
            Assert.AreEqual(true, plasticscm.Forced);
            Assert.AreEqual(true, plasticscm.LabelOnSuccess);
            Assert.AreEqual("BL", plasticscm.LabelPrefix);
            Assert.AreEqual(@"c:\workspace", plasticscm.WorkingDirectory);
        }

        [Test]
        public void VerifyGoToBranchProcessInfoBasic()
        {
            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, "source", null);
            IntegrationSummary lastSummary =
                new IntegrationSummary(IntegrationStatus.Success, "label", "lastlabel", DateTime.Now);
            IntegrationResult result = new IntegrationResult("test", @"c:\workspace", @"c:\artifacts", request, lastSummary);

            PlasticSCM plasticscm = new PlasticSCM();
            NetReflector.Read(PLASTICSCM_XML, plasticscm);
            string expected = @"c:\plastic\client\cm.exe stb br:/main -repository=mainrep";
            ProcessInfo info = plasticscm.GoToBranchProcessInfo(result);
            Assert.AreEqual(expected, info.FileName + " " + info.Arguments);
        }

        [Test]
        public void VerifyNewGetSourceProcessInfoWithAttributes()
        {
            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, "source", null);
            IntegrationSummary lastSummary = new IntegrationSummary(IntegrationStatus.Success, "label", "lastlabel", DateTime.Now);
            IntegrationResult result = new IntegrationResult("test", @"c:\workspace", @"c:\artifacts", request, lastSummary);

            PlasticSCM plasticscm = new PlasticSCM();
            NetReflector.Read(PLASTICSCM_XML, plasticscm);
            string expected = @"c:\plastic\client\cm.exe stb br:/main -repository=mainrep";
            ProcessInfo info = plasticscm.GoToBranchProcessInfo(result);
            Assert.AreEqual(expected, info.FileName + " " + info.Arguments);

        }

        [Test]
        public void VerifyCreateQueryProcessInfoBasic()
        {
            string fromtime = "01/02/2003 00:00:00";
            string totime = "23/02/2006 23:14:05";
            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, "source", null);
            IntegrationSummary lastSummary =
                new IntegrationSummary(IntegrationStatus.Success, "label", "lastlabel", DateTime.Now);
            IntegrationResult from =
                new IntegrationResult("test", @"c:\workspace", @"c:\artifacts", request, lastSummary);
            from.StartTime =
                DateTime.ParseExact(fromtime, PlasticSCM.DATEFORMAT, System.Globalization.CultureInfo.InvariantCulture);
            IntegrationResult to = new IntegrationResult("test", @"c:\workspace", @"c:\artifacts", request, lastSummary);
            to.StartTime =
                DateTime.ParseExact(totime, PlasticSCM.DATEFORMAT, System.Globalization.CultureInfo.InvariantCulture);

            PlasticSCM plasticscm = new PlasticSCM();
            NetReflector.Read(PLASTICSCM_BASIC_XML, plasticscm);
            string query = string.Format(
                "cm find changesets where branch = 'br:/main' "
                + "and date between '{0}' and '{1}' ", fromtime, totime);
            string dateformat = string.Format(System.Globalization.CultureInfo.CurrentCulture, "--dateformat=\"{0}\" ", PlasticSCM.DATEFORMAT);
            string format = string.Format(System.Globalization.CultureInfo.CurrentCulture, "--format=\"{0}\"", PlasticSCM.FORMAT);

            ProcessInfo info = plasticscm.CreateQueryProcessInfo(from, to);
            Assert.AreEqual(query + dateformat + format, info.FileName + " " + info.Arguments);
        }

        public void VerifyCreateQueryProcessInfoWithAttributes()
        {
            string fromtime = "01/02/2003 00:00:00";
            string totime = "23/02/2006 23:14:05";
            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, "source", null);
            IntegrationSummary lastSummary = new IntegrationSummary(IntegrationStatus.Success, "label", "lastlabel", DateTime.Now);
            IntegrationResult from = new IntegrationResult("test", @"c:\workspace", @"c:\artifacts", request, lastSummary);
            from.StartTime = DateTime.ParseExact(fromtime, PlasticSCM.DATEFORMAT, System.Globalization.CultureInfo.InvariantCulture);
            IntegrationResult to = new IntegrationResult("test", @"c:\workspace", @"c:\artifacts", request, lastSummary);
            to.StartTime = DateTime.ParseExact(totime, PlasticSCM.DATEFORMAT, System.Globalization.CultureInfo.InvariantCulture);

            PlasticSCM plasticscm = new PlasticSCM();
            NetReflector.Read(PLASTICSCM_XML, plasticscm);
            string query = string.Format(
                @"c:\plastic\client\cm.exe find changesets where branch = 'br:/main' "
                + "and date between '{0}' and '{1}' on repository 'mainrep' ", fromtime, totime);
            string dateformat = string.Format(System.Globalization.CultureInfo.CurrentCulture, "--dateformat=\"{0}\" ", PlasticSCM.DATEFORMAT);
            string format = string.Format(System.Globalization.CultureInfo.CurrentCulture, "--format=\"{0}\"", PlasticSCM.FORMAT);

            ProcessInfo info = plasticscm.CreateQueryProcessInfo(from, to);
            Assert.AreEqual(query + dateformat + format, info.FileName + " " + info.Arguments);

        }

        [Test]
        public void VerifyCreateLabelProcessInfoBasic()
        {
            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, "source", null);
            IntegrationSummary lastSummary = new IntegrationSummary(IntegrationStatus.Success, "label", "lastlabel", DateTime.Now);
            IntegrationResult result = new IntegrationResult("test", @"c:\workspace", @"c:\artifacts", request, lastSummary);
            result.Label = "1";

            //basic check
            PlasticSCM plasticscm = new PlasticSCM();
            NetReflector.Read(PLASTICSCM_BASIC_XML, plasticscm);
            string expected = @"cm mklb ccver-1";
            ProcessInfo info = plasticscm.CreateLabelProcessInfo(result);
            Assert.AreEqual(expected, info.FileName + " " + info.Arguments);
        }

        [Test]
        public void VerifyCreateLabelProcessInfoWithAttributes()
        {
            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, "source", null);
            IntegrationSummary lastSummary = new IntegrationSummary(IntegrationStatus.Success, "label", "lastlabel", DateTime.Now);
            IntegrationResult result = new IntegrationResult("test", @"c:\workspace", @"c:\artifacts", request, lastSummary);
            result.Label = "1";

            //check with attributes
            PlasticSCM plasticscm = new PlasticSCM();
            NetReflector.Read(PLASTICSCM_XML, plasticscm);
            string expected = @"c:\plastic\client\cm.exe mklb BL1";
            ProcessInfo info = plasticscm.CreateLabelProcessInfo(result);
            Assert.AreEqual(expected, info.FileName + " " + info.Arguments);
        }

        [Test]
        public void VerifyLabelProcessInfoBasic()
        {

            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, "source", null);
            IntegrationSummary lastSummary = new IntegrationSummary(IntegrationStatus.Success, "label", "lastlabel", DateTime.Now);
            IntegrationResult result = new IntegrationResult("test", @"c:\workspace", @"c:\artifacts", request, lastSummary);
            result.Label = "1";

            PlasticSCM plasticscm = new PlasticSCM();
            NetReflector.Read(PLASTICSCM_BASIC_XML, plasticscm);
            string expected = @"cm label lb:ccver-1 .";
            ProcessInfo info = plasticscm.LabelProcessInfo(result);
            Assert.AreEqual(expected, info.FileName + " " + info.Arguments);
        }

        [Test]
        public void VerifyLabelProcessInfoWithAttributes()
        {

            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, "source", null);
            IntegrationSummary lastSummary = new IntegrationSummary(IntegrationStatus.Success, "label", "lastlabel", DateTime.Now);
            IntegrationResult result = new IntegrationResult("test", @"c:\workspace", @"c:\artifacts", request, lastSummary);
            result.Label = "1";

            PlasticSCM plasticscm = new PlasticSCM();
            NetReflector.Read(PLASTICSCM_XML, plasticscm);
            string expected = @"c:\plastic\client\cm.exe label lb:BL1 .";
            ProcessInfo info = plasticscm.LabelProcessInfo(result);
            Assert.AreEqual(expected, info.FileName + " " + info.Arguments);
        }

        [Test]
        public void VerifyDefaults()
        {
            PlasticSCM plasticscm = new PlasticSCM();

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