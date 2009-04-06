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

		private readonly DateTime from = DateTime.ParseExact ("24/05/2007 17:04:10", PlasticSCM.DATEFORMAT, System.Globalization.CultureInfo.InvariantCulture);
		private readonly DateTime to = DateTime.ParseExact ("24/05/2007 17:14:10", PlasticSCM.DATEFORMAT, System.Globalization.CultureInfo.InvariantCulture);

		private const string queryresult =  @"?c:\work\cruise\plasticwks\src\HelloWorld.cs?testing01?24/05/2007 17:06:10?27" + "\n"
							  + @"?c:\work\cruise\plasticwks\hello.build?edmund?24/05/2007 17:07:10?29" + "\n"
							  + "\nTotal: 2\n";
		[SetUp]
		public void SetUp()
		{
			parser = new PlasticSCMHistoryParser();
		}

		[Test]
		public void VerifyParseQueryResult()
		{
			Modification[] mods = parser.Parse (new StringReader(queryresult), from, to);
			Modification[] expecteds = GetExpectedModifications();
			Assert.AreEqual(expecteds.Length, mods.Length);
			for (int i=0; i < mods.Length; i++)
			{
				Assert.AreEqual(expecteds[i] , mods[i]);
			}
		}

		private static Modification[] GetExpectedModifications ()
		{
			Modification[] mod = new Modification[2];

			// ?c:\work\cruise\plasticwks\src\HelloWorld.cs?testing01?24/05/2007 17:06:10?27
			mod[0] = new Modification();
			mod[0].FolderName = @"c:\work\cruise\plasticwks\src";
			mod[0].FileName = "HelloWorld.cs";
			mod[0].ChangeNumber = 27;
			mod[0].ModifiedTime = DateTime.ParseExact("24/05/2007 17:06:10", PlasticSCM.DATEFORMAT, System.Globalization.CultureInfo.InvariantCulture);
			mod[0].UserName = "testing01";

			// ?c:\work\cruise\plasticwks\hello.build?edmund?24/05/2007 17:07:10?29\n
			mod[1] = new Modification();
			mod[1].FolderName = @"c:\work\cruise\plasticwks";
			mod[1].FileName = "hello.build";
			mod[1].ChangeNumber = 29;
			mod[1].ModifiedTime = DateTime.ParseExact ("24/05/2007 17:07:10", PlasticSCM.DATEFORMAT, System.Globalization.CultureInfo.InvariantCulture);
			mod[1].UserName = "edmund";

			return mod;
		}
	}
}