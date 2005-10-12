using System;
using System.Globalization;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class SurroundTest : CustomAssertion
	{
		public const string SSCM_XML =
			@"<sourceControl type=""sscm"">
				<executable>C:\Program Files\Seapine\Surround SCM\sscm.exe</executable>
				<serverlogin>build:build</serverlogin>
				<serverconnect>198.187.17.157:4900</serverconnect>
            <branch>m20040908</branch>
            <repository>m20040908/scctt3</repository>
            <workingDirectory>C:\scctt3</workingDirectory>
            <recursive>1</recursive>
			</sourceControl>";

		private Surround surround;

		[SetUp]
		protected void SetUp()
		{
			surround = new Surround();
			NetReflector.Read(SSCM_XML, surround);
		}

		[Test]
		public void VerifyValuesSetByNetReflector()
		{
			Assert.AreEqual(@"C:\Program Files\Seapine\Surround SCM\sscm.exe", surround.Executable);
			Assert.AreEqual("build:build", surround.ServerLogin);
			Assert.AreEqual("198.187.17.157:4900", surround.ServerConnect);
			Assert.AreEqual("m20040908", surround.Branch);
			Assert.AreEqual("m20040908/scctt3", surround.Repository);
			Assert.AreEqual(@"C:\scctt3", surround.WorkingDirectory);
			Assert.AreEqual(1, surround.Recursive);
		}

		[Test]
		public void VerifyFormatDate()
		{
			DateTime dateExpected = new DateTime(2005, 9, 30, 1, 2, 3);
			string strDateExpected = "20050930010203";

			DateTime checkDate = DateTime.ParseExact(strDateExpected, Surround.TO_SSCM_DATE_FORMAT, CultureInfo.InvariantCulture);
			Assert.AreEqual(dateExpected, checkDate);

			string checkStrDate = dateExpected.ToString(Surround.TO_SSCM_DATE_FORMAT);
			Assert.AreEqual(strDateExpected, checkStrDate);
		}

		[Test]
		public void VerifyDefaults()
		{
			surround = new Surround();
			Assert.AreEqual("127.0.0.1:4900", surround.ServerConnect);
			Assert.AreEqual("Administrator:", surround.ServerLogin);
			Assert.AreEqual(0, surround.SearchRegExp);
			Assert.AreEqual(0, surround.Recursive);
		}
	}
}