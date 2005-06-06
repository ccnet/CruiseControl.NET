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
			Assert.AreEqual("02/22/2002;20:00", vssLocale.FormatCommandDate(new DateTime(2002, 2, 22, 20, 0, 0)));
			Assert.AreEqual("02/22/2002;12:00", vssLocale.FormatCommandDate(new DateTime(2002, 2, 22, 12, 0, 0)));
		}

		[Test]
		public void FormatDateInUSFormat()
		{
			IVssLocale vssLocale = new VssLocale(UsCuture());
			Assert.AreEqual("2/22/2002;8:00p", vssLocale.FormatCommandDate(new DateTime(2002, 2, 22, 20, 0, 0)));
			Assert.AreEqual("2/22/2002;12:00p", vssLocale.FormatCommandDate(new DateTime(2002, 2, 22, 12, 0, 0)));
		}

		[Test]
		public void FormatDateInUKFormat()
		{
			IVssLocale vssLocale = new VssLocale(UkCulture());
			Assert.AreEqual("22/02/2002;20:00", vssLocale.FormatCommandDate(new DateTime(2002, 2, 22, 20, 0, 0, 34)));
			Assert.AreEqual("22/02/2002;12:00", vssLocale.FormatCommandDate(new DateTime(2002, 2, 22, 12, 0, 0)));
		}

		[Test]
		public void FormatDateInUKFormatWithAMPMIndicator()
		{
			IVssLocale vssLocale = new VssLocale(UkCultureWithAmPmTimeFormat());
			Assert.AreEqual("22/02/2002;8:00p", vssLocale.FormatCommandDate(new DateTime(2002, 2, 22, 20, 0, 0, 34)));
			Assert.AreEqual("22/02/2002;12:00p", vssLocale.FormatCommandDate(new DateTime(2002, 2, 22, 12, 0, 0)));
		}

		[Test]
		public void ParseDateAndTime()
		{
			IVssLocale vssLocale = new VssLocale(CultureInfo.InvariantCulture);
			Assert.AreEqual(new DateTime(2002, 2, 22, 12, 0, 0), vssLocale.ParseDateTime("02/22/2002", "12:00"));
		}

		private CultureInfo UsCuture()
		{
			return new CultureInfo("en-US", false);
		}

		private CultureInfo UkCulture()
		{
			return new CultureInfo("en-GB", false);
		}

		private CultureInfo UkCultureWithAmPmTimeFormat()
		{
			CultureInfo cultureInfo = UkCulture();
			cultureInfo.DateTimeFormat.LongTimePattern = cultureInfo.DateTimeFormat.LongTimePattern + " tt";
			return cultureInfo;
		}
	}
}
