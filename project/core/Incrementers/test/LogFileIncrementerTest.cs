using System;
using NUnit.Framework;

namespace tw.ccnet.core.test
{
	[TestFixture]
	public class LogFileIncrementerTest
	{
		[Test]
		public void TestIncrementFromLogFilesSingleDigit() 
		{
			
			string[] filenames = new string[]{
												 "log19710514150000.xml",
												 "log19710514150001.xml",
												 "log20020830164057Lbuild.9.xml",
												 "log19710514150002.xml",
												 "log20020830164057Lbuild.7.xml",
												 "log19710514150003.xml",
												 "log20020830164057Lbuild.6.xml",
												 "log20020830164057Lbuild.8.xml",
												 "log19710514150004.xml"
											 };
			LogFileIncrementer inc = new LogFileIncrementer();
			string actual = inc.getNextLabelFromFileNames(filenames);
			Assertion.AssertEquals("10", actual);
		}

		[Test]
		public void TestIncrementFromLogFilesSingleAllParts() 
		{
			
			string[] filenames = new string[]{
												 "log19710514150000.xml",
												 "log19710514150001.xml",
												 "log20020830164057Lbuild.1.0.3.10311.xml",
												 "log19710514150002.xml",
												 "log20020830164057Lbuild.1.0.3.10314.xml",
												 "log19710514150003.xml",
												 "log20020830164057Lbuild.1.0.3.10313.xml",
												 "log20020830164057Lbuild.1.0.3.10310.xml",
												 "log19710514150004.xml"
											 };
			LogFileIncrementer inc = new LogFileIncrementer();
			string actual = inc.getNextLabelFromFileNames(filenames);
			Assertion.AssertEquals("1.0.3.10315", actual);
		}
	}
}
