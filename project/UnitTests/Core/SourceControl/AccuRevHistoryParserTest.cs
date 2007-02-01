using System.IO;
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
			TextReader historyReader;
			if ((new ExecutionEnvironment()).IsRunningOnWindows)
				historyReader = AccuRevMother.historyOutputReaderWindows;
			else
				historyReader = AccuRevMother.historyOutputReaderUnix;
			
			Modification[] mods = parser.Parse(historyReader, AccuRevMother.oldestHistoryModification, 
			                                   AccuRevMother.newestHistoryModification);
			Assert.IsNotNull(mods, "mods should not be null");
			Assert.AreEqual(AccuRevMother.historyOutputModifications, mods.Length);			
		}
	}
}
