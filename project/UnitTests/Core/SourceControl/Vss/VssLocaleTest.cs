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
			string expected = "02/22/2002;20:00:00";
			string actual = vssLocale.FormatCommandDate(date);
			Assert.AreEqual(expected, actual);

			date = new DateTime(2002, 2, 22, 12, 0, 0);
			expected = "02/22/2002;12:00:00";
			actual = vssLocale.FormatCommandDate(date);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void FormatDateInUSFormat()
		{
			IVssLocale vssLocale = new VssLocale(new CultureInfo("en-US", false));
			DateTime date = new DateTime(2002, 2, 22, 20, 0, 0);
			string expected = "2/22/2002;8:00:00 p";
			string actual = vssLocale.FormatCommandDate(date);
			Assert.AreEqual(expected, actual);

			date = new DateTime(2002, 2, 22, 12, 0, 0);
			expected = "2/22/2002;12:00:00 p";
			actual = vssLocale.FormatCommandDate(date);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void FormatDateInUKFormat()
		{
			IVssLocale vssLocale = new VssLocale(new CultureInfo("en-GB", false));
			DateTime date = new DateTime(2002, 2, 22, 20, 0, 0, 34);
			string expected = "22/02/2002;20:00:00";
			string actual = vssLocale.FormatCommandDate(date);
			Assert.AreEqual(expected, actual);

			date = new DateTime(2002, 2, 22, 12, 0, 0);
			expected = "22/02/2002;12:00:00";
			actual = vssLocale.FormatCommandDate(date);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void FormatDateInUKFormatWithAMPMIndicator()
		{
			CultureInfo cultureInfo = new CultureInfo("en-GB", false);
			cultureInfo.DateTimeFormat.LongTimePattern = cultureInfo.DateTimeFormat.LongTimePattern + " tt";
			DateTimeFormatInfo format = (DateTimeFormatInfo) cultureInfo.GetFormat(typeof(DateTimeFormatInfo));
			Assert.AreEqual(cultureInfo.DateTimeFormat.LongTimePattern, format.LongTimePattern);

			IVssLocale vssLocale = new VssLocale(cultureInfo);
			DateTime date = new DateTime(2002, 2, 22, 20, 0, 0, 34);

			string expected = "22/02/2002;8:00:00 p";
			string actual = vssLocale.FormatCommandDate(date);
			Assert.AreEqual(expected, actual);

			date = new DateTime(2002, 2, 22, 12, 0, 0);
			expected = "22/02/2002;12:00:00 p";
			actual = vssLocale.FormatCommandDate(date);
			Assert.AreEqual(expected, actual);
		}
	}
}
