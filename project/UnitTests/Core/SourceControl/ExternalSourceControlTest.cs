using System;
using Exortech.NetReflector;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class ExternalSourceControlTest
	{

        [Test]
        public void VerifyDefaultValues()
        {
            ExternalSourceControl externalSC = new ExternalSourceControl();
            Assert.AreEqual(string.Empty, externalSC.ArgString);
            Assert.AreEqual(false, externalSC.AutoGetSource);
            Assert.AreEqual(0, externalSC.EnvironmentVariables.Length);
            Assert.AreEqual(false, externalSC.LabelOnSuccess);
        }

		[Test]
		public void ShouldPopulateCorrectlyFromXml()
		{
			const string xml =
@"<sourceControl type=""external"">
    <args>arg1 ""arg2 has blanks"" arg3</args>
    <autoGetSource>true</autoGetSource>
    <executable>banana.bat</executable>
    <labelOnSuccess>true</labelOnSuccess>
    <environment>
        <variable name=""name1"" value=""value1""/>
        <variable><name>name2</name></variable>
        <variable name=""name3""><value>value3</value></variable>
    </environment>
</sourceControl>";

            ExternalSourceControl externalSC = new ExternalSourceControl();
            NetReflector.Read(xml, externalSC);
            Assert.AreEqual(@"arg1 ""arg2 has blanks"" arg3", externalSC.ArgString);
            Assert.AreEqual(true, externalSC.AutoGetSource);
            Assert.AreEqual(3, externalSC.EnvironmentVariables.Length);
            Assert.AreEqual("name1", externalSC.EnvironmentVariables[0].name);
            Assert.AreEqual("value1", externalSC.EnvironmentVariables[0].value);
            Assert.AreEqual("name2", externalSC.EnvironmentVariables[1].name);
            Assert.AreEqual("", externalSC.EnvironmentVariables[1].value);
            Assert.AreEqual("name3", externalSC.EnvironmentVariables[2].name);
            Assert.AreEqual("value3", externalSC.EnvironmentVariables[2].value);
            Assert.AreEqual("banana.bat", externalSC.Executable);
            Assert.AreEqual(true, externalSC.LabelOnSuccess);
        }

        [Test]
        public void ShouldPopulateCorrectlyFromMinimalXml()
        {
            const string xml =
@"<sourceControl type=""external"">
    <executable>banana.bat</executable>
</sourceControl>";
            ExternalSourceControl externalSC = new ExternalSourceControl();
            NetReflector.Read(xml, externalSC);
            Assert.AreEqual(string.Empty, externalSC.ArgString);
            Assert.AreEqual(false, externalSC.AutoGetSource);
            Assert.AreEqual(0, externalSC.EnvironmentVariables.Length);
            Assert.AreEqual("banana.bat", externalSC.Executable);
            Assert.AreEqual(false, externalSC.LabelOnSuccess);
        }

        [Test]
        public void ShouldFailToPopulateFromConfigurationMissingRequiredFields()
        {
            const string xml = @"<sourceControl type=""external""></sourceControl>";
            ExternalSourceControl externalSC = new ExternalSourceControl();
            Assert.That(delegate { NetReflector.Read(xml, externalSC); },
                        Throws.TypeOf<NetReflectorException>());
        }

		[Test]
		public void ShouldGetSourceIfAutoGetSourceTrue()
		{
			DynamicMock executor = new DynamicMock(typeof(ProcessExecutor));
            ExternalSourceControl externalSC = new ExternalSourceControl((ProcessExecutor)executor.MockInstance);
            externalSC.AutoGetSource = true;
		    externalSC.Executable = "banana.bat";
		    externalSC.ArgString = @"arg1 ""arg2 is longer"" arg3";

		    IntegrationResult intResult = new IntegrationResult();
            intResult.StartTime = new DateTime(1959,9,11,7,53,0);
		    intResult.WorkingDirectory = @"C:\SomeDir\Or\Other";
            intResult.ProjectName = "MyProject";

			ProcessInfo expectedProcessRequest = new ProcessInfo(
                "banana.bat", 
                @"GETSOURCE ""C:\SomeDir\Or\Other"" ""1959-09-11 07:53:00"" arg1 ""arg2 is longer"" arg3",
                @"C:\SomeDir\Or\Other"
                );
			expectedProcessRequest.TimeOut = Timeout.DefaultTimeout.Millis;

			executor.ExpectAndReturn("Execute", new ProcessResult("foo", null, 0, false), expectedProcessRequest);
            externalSC.GetSource(intResult);
			executor.Verify();
		}

		[Test]
		public void ShouldNotGetSourceIfAutoGetSourceFalse()
		{
			DynamicMock executor = new DynamicMock(typeof(ProcessExecutor));
            ExternalSourceControl externalSC = new ExternalSourceControl((ProcessExecutor)executor.MockInstance);
            externalSC.AutoGetSource = false;

			executor.ExpectNoCall("Execute", typeof(ProcessInfo));
            externalSC.GetSource(new IntegrationResult());
			executor.Verify();
		}
	}
}

