namespace CruiseControl.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    public static class DateTimeAssert
    {
        private static Dictionary<DateTimeCompare, string> comparers
            = new Dictionary<DateTimeCompare, string>
                  {
                      {DateTimeCompare.IgnoreSeconds, "yyyyMMddHHmm"}
                  };

        public static void AreEqual(DateTime expected, DateTime actual, DateTimeCompare compare)
        {
            var comparer = comparers[compare];
            Assert.AreEqual(expected.ToString(comparer), actual.ToString(comparer));
        }
    }

    public enum DateTimeCompare
    {
        IgnoreSeconds,
    }
}
