using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	/// <remarks>
	/// This code is based on code\sourcecontrol\ClearCase.cs.
	/// </remarks>
	[TestFixture]
	public class AccuRevHistoryParserTest 
	{
		AccuRevHistoryParser parser;

		[SetUp]
		protected void Setup()
		{
			parser = new AccuRevHistoryParser();
		}

		/// <summary>
		/// Test that we still know how to parse the output of an AccuRev "accurev hist" command.
		/// </summary>
		/// <remarks>
		/// This tests either the Windows or the Unix version of the output, depending on which 
		/// type of system the test is being executed on.  I'd love to test both on all systems,
		/// but that would mean faking out ExecutionEnvironment and we're not ready for that yet.
		/// </remarks> 
		[Test]
		public void CanParse()
		{
            AccuRevMother histData = AccuRevMother.GetInstance();
		    TextReader historyReader = histData.historyOutputReader;

            Modification[] mods = parser.Parse(historyReader, histData.oldestHistoryModification,
                                               histData.newestHistoryModification);
			Assert.IsNotNull(mods, "mods should not be null");
		    Assert.AreEqual(histData.historyOutputModifications.Length, mods.Length);
		    for (int i = 0; i < histData.historyOutputModifications.Length; i++)
		    {
		        Assert.AreEqual(histData.historyOutputModifications[i].ChangeNumber, mods[i].ChangeNumber);
		        Assert.AreEqual(histData.historyOutputModifications[i].Comment, mods[i].Comment);
		        Assert.AreEqual(histData.historyOutputModifications[i].FileName, mods[i].FileName);
		        Assert.AreEqual(histData.historyOutputModifications[i].FolderName, mods[i].FolderName);
		        Assert.AreEqual(histData.historyOutputModifications[i].ModifiedTime, mods[i].ModifiedTime);
		        Assert.AreEqual(histData.historyOutputModifications[i].Type, mods[i].Type);
		        Assert.AreEqual(histData.historyOutputModifications[i].UserName, mods[i].UserName);
		    }
		}
	}
}
