using System;
using System.Globalization;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.SourceControl.Vss
{
	[TestFixture]
	public class VssLocaleTest
	{
		[Test]
		public void FormatDateInCultureInvariantFormat()
		{
			IVssLocale vssLocale = new VssLocale(CultureInfo.InvariantCulture);
			DateTime date = new DateTime(2002, 2, 22, 20, 0, 0);
			string expected = "02/22/2002;20:00";
			string actual = vssLocale.FormatCommandDate(date);
			Assert.AreEqual(expected, actual);

			date = new DateTime(2002, 2, 22, 12, 0, 0);
			expected = "02/22/2002;12:00";
			actual = vssLocale.FormatCommandDate(date);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void FormatDateInUSFormat()
		{
			IVssLocale vssLocale = new VssLocale(new CultureInfo("en-US"));
			DateTime date = new DateTime(2002, 2, 22, 20, 0, 0);
			string expected = "2/22/2002;8:00P";
			string actual = vssLocale.FormatCommandDate(date);
			Assert.AreEqual(expected, actual);

			date = new DateTime(2002, 2, 22, 12, 0, 0);
			expected = "2/22/2002;12:00P";
			actual = vssLocale.FormatCommandDate(date);
			Assert.AreEqual(expected, actual);
		}
	}
}
