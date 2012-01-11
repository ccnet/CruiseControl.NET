using System;
using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;


namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
    [TestFixture]
    public class PlasticSCMHistoryParserTest : CustomAssertion
    {
        private PlasticSCMHistoryParser parser;

        private const string DATE_STRING_FROM = "24/05/2007 17:04:10";
        private const string DATE_STRING_TO = "24/05/2007 17:14:10";
        private const string DATE_STRING_CS1 = "24/05/2007 17:06:10";
        private const string DATE_STRING_CS2 = "24/05/2007 17:07:10";

        private readonly DateTime from = DateTime.ParseExact(DATE_STRING_FROM, PlasticSCM.DATEFORMAT, System.Globalization.CultureInfo.InvariantCulture);
        private readonly DateTime to = DateTime.ParseExact(DATE_STRING_TO, PlasticSCM.DATEFORMAT, System.Globalization.CultureInfo.InvariantCulture);

        private const string queryresult = @"?comment1?testing01?" + DATE_STRING_CS1 + "?27" + "\n"
                              + @"?comment2?edmund?" + DATE_STRING_CS2 + "?29" + "\n"
                              + "\nTotal: 2\n";
        [SetUp]
        public void SetUp()
        {
            parser = new PlasticSCMHistoryParser();
        }

        [Test]
        public void VerifyParseQueryResult()
        {
            Modification[] mods = parser.Parse(new StringReader(queryresult), from, to);
            Modification[] expecteds = GetExpectedModifications();
            Assert.AreEqual(expecteds.Length, mods.Length);
            for (int i = 0; i < mods.Length; i++)
            {
                Assert.AreEqual(expecteds[i], mods[i]);
            }
        }

        private static Modification[] GetExpectedModifications()
        {
            Modification[] mod = new Modification[2];

            mod[0] = new Modification();
            mod[0].Comment = "comment1";
            mod[0].ChangeNumber = "27";
            mod[0].ModifiedTime = DateTime.ParseExact(DATE_STRING_CS1, PlasticSCM.DATEFORMAT, System.Globalization.CultureInfo.InvariantCulture);
            mod[0].UserName = "testing01";

            mod[1] = new Modification();
            mod[1].FileName = "comment2";
            mod[1].ChangeNumber = "29";
            mod[1].ModifiedTime = DateTime.ParseExact(DATE_STRING_CS2, PlasticSCM.DATEFORMAT, System.Globalization.CultureInfo.InvariantCulture);
            mod[1].UserName = "edmund";

            return mod;
        }
    }
}