using System;
using System.IO;
using NUnit.Framework;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test
{
	[TestFixture]
	public class
		MksHistoryParserTest
	{
		#region Test data

		public static readonly string TEST_DATA = @"===============================================================================
member name: nant\mks\tests\NAnt.Zip\Tasks\ZipTaskTest.cs;	working file: c:\Brighton\nant\mks\tests\NAnt.Zip\Tasks\ZipTaskTest.cs
head:	1.1
member:	1.1
branch:	
locks:	dtp33348:1.1; strict
attributes:	
symbolic names:         TestLabel:1.1
file format: text
revision storage: reverse deltas
total revisions: 1; branches: 0; branch revisions: 0
description:
Nant 
CCNet-0| nant\mks\tests\NAnt.Zip\Tasks\ZipTaskTest.cs| 1| Sep 29, 2003 - 9:24 PM| DTP33348| Initial revision
Member added to project e:/Siproj/Brighton/nant.pj

===============================================================================
member name: nant\NAnt.build;	working file: c:\Brighton\nant\NAnt.build
head:	1.1
member:	1.1
branch:	
locks:	dtp33348:1.1; strict
attributes:	
symbolic names:         TestLabel:1.1
file format: text
revision storage: reverse deltas
total revisions: 1; branches: 0; branch revisions: 0
description:
Nant 
CCNet-0| nant\NAnt.build| 1| Dec 7, 2003 - 5:12 PM| DTP33348| Initial revision
Member added to project e:/Siproj/Brighton/nant.pj

===============================================================================
member name: nant\NAnt.key;	working file: c:\Brighton\nant\NAnt.key
head:	1.1
member:	1.1
branch:	
locks:	dtp33348:1.1; strict
attributes:	
symbolic names:         TestLabel:1.1
file format: binary
revision storage: Reference
total revisions: 1; branches: 0; branch revisions: 0
description:
Nant 
CCNet-0| nant\NAnt.key| 1| Aug 12, 2001 - 11:30 AM| DTP33348| Initial revision
Member added to project e:/Siproj/Brighton/nant.pj

===============================================================================
member name: nant\NAnt.sln;	working file: c:\Brighton\nant\NAnt.sln
head:	1.1
member:	1.1
branch:	
locks:	dtp33348:1.1; strict
attributes:	
symbolic names:         TestLabel:1.1
file format: text
revision storage: reverse deltas
total revisions: 1; branches: 0; branch revisions: 0
description:
Nant 
CCNet-0| nant\NAnt.sln| 1| Jul 19, 2003 - 1:36 PM| DTP33348| Initial revision
Member added to project e:/Siproj/Brighton/nant.pj

===============================================================================
member name: nant\README.txt;	working file: c:\Brighton\nant\README.txt
head:	1.3
member:	1.3
branch:	
locks:	DTP33348:1.3; strict
attributes:	
symbolic names:         TestLabel:1.2
file format: text
revision storage: reverse deltas
total revisions: 3; branches: 0; branch revisions: 0
description:
Nant 
CCNet-1| nant\README.txt| 3| Nov 19, 2004 - 11:37 AM| DTP33348| test

CCNet-1| nant\README.txt| 2| Nov 19, 2004 - 10:43 AM| DTP33348| Added test line for ccnet

CCNet-1| nant\README.txt| 1| Sep 12, 2003 - 7:30 PM| DTP33348| Initial revision
Member added to project e:/Siproj/Brighton/nant.pj

===============================================================================
member name: nant\schema\nant-0.84.xsd;	working file: c:\Brighton\nant\schema\nant-0.84.xsd
head:	1.1
member:	1.1
branch:	
locks:	dtp33348:1.1; strict
attributes:	
symbolic names:         TestLabel:1.1
file format: text
revision storage: reverse deltas
total revisions: 1; branches: 0; branch revisions: 0
description:
Nant 
CCNet-0| nant\schema\nant-0.84.xsd| 1| Dec 26, 2003 - 1:59 PM| DTP33348| Initial revision
Member added to project e:/Siproj/Brighton/nant.pj

===============================================================================";

		#endregion

		public MksHistoryParserTest()
		{
		}

		[Test]
		public void AllModificationsParams()
		{
			MksHistoryParser parser = new MksHistoryParser();
			string sampleMatchedLine = @"CCNet-1| nant\README.txt| 3   | Nov 19, 2004 - 11:37 AM| DTP33348| test

";
			string[] modificationParams = parser.AllModificationParams(sampleMatchedLine);
			Assert.AreEqual("CCNet-1", modificationParams[0]);
			Assert.AreEqual("3", modificationParams[2]);
			Assert.AreEqual("Nov 19, 2004 - 11:37 AM", modificationParams[3]);
		}

		[Test]
		public void Parse()
		{
			MksHistoryParser parser = new MksHistoryParser();
			Modification[] modifications = parser.Parse(new StringReader(TEST_DATA), DateTime.Now, DateTime.Now);
			Assert.AreEqual("README.txt", modifications[0].FileName);
			Assert.AreEqual(3, modifications[0].ChangeNumber);
			DateTime expectedTime = DateTime.Parse("Nov 19, 2004 - 11:37 AM");
			Assert.AreEqual(expectedTime, modifications[0].ModifiedTime);
		}
	}
}