using Exortech.NetReflector;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	/// <remarks>
	/// This code is based on code\sourcecontrol\ClearCase.cs.
	/// </remarks>
	[TestFixture]
	public class AccuRevTest
	{
		private const string EOL = "\r\n";
		
		[Test]
		public void ShouldPopulateCorrectlyFromXml()
		{
			const string AccuRev_XML =
				      "<sourceControl type=\"accuRev\">" +
				EOL + "    <autoGetSource>false</autoGetSource>" +
				EOL + "    <executable>accurev.exe</executable>" +
				EOL + "    <labelOnSuccess>true</labelOnSuccess>" +
				EOL + "    <workspace>C:\\DOES NOT\\EXIST</workspace>" +
				EOL + "</sourceControl>";
			
			AccuRev accurev = new AccuRev();
			NetReflector.Read(AccuRev_XML, accurev);
			Assert.AreEqual(false, accurev.AutoGetSource);
			Assert.AreEqual("accurev.exe", accurev.Executable);
			Assert.AreEqual(true, accurev.LabelOnSuccess);
			Assert.AreEqual("C:\\DOES NOT\\EXIST", accurev.Workspace);
		}

		[Test, ExpectedException(typeof (NetReflectorException))]
		public void CanCatchConfigInvalidAutoGetSource()
		{
			AccuRev accurev = new AccuRev();
			const string invalidXml = "<sourcecontrol type=\"accurev\"><autoGetSource>NOT_A_BOOLEAN</autoGetSource></sourcecontrol>";
			NetReflector.Read(invalidXml, accurev);
		}

		[Test, ExpectedException(typeof (NetReflectorException))]
		public void CanCatchConfigInvalidLabelOnSuccess()
		{
			AccuRev accurev = new AccuRev();
			const string invalidXml = "<sourcecontrol type=\"accurev\"><labelOnSuccess>NOT_A_BOOLEAN</labelOnSuccess></sourcecontrol>";
			NetReflector.Read(invalidXml, accurev);
		}

		[Test]
		public void ShouldGetSourceIfAutoGetSourceTrue()
		{
			DynamicMock executor = new DynamicMock(typeof(ProcessExecutor));
			AccuRev accurev = new AccuRev((ProcessExecutor) executor.MockInstance);
			accurev.AutoGetSource = true;

			ProcessInfo expectedProcessRequest = new ProcessInfo("accurev.exe", "update");
			expectedProcessRequest.TimeOut = Timeout.DefaultTimeout.Millis;

			executor.ExpectAndReturn("Execute", new ProcessResult("foo", null, 0, false), expectedProcessRequest);
			accurev.GetSource(new IntegrationResult());
			executor.Verify();
		}

		[Test]
		public void ShouldNotGetSourceIfAutoGetSourceFalse()
		{
			DynamicMock executor = new DynamicMock(typeof(ProcessExecutor));
			AccuRev accurev = new AccuRev((ProcessExecutor) executor.MockInstance);
			accurev.AutoGetSource = false;

			executor.ExpectNoCall("Execute", typeof(ProcessInfo));
			accurev.GetSource(new IntegrationResult());
			executor.Verify();
		}
	}
}
