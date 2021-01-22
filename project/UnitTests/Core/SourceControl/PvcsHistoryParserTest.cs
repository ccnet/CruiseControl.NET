using System;
using System.Globalization;
using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
    [TestFixture]
    public class PvcsHistoryParserTest
    {
        public static DateTime OLDEST_ENTRY = DateTime.Parse("2000-Jan-01 13:34:52");
        public static DateTime NEWEST_ENTRY = DateTime.Parse("2005-Sep-13 13:34:52");

        private static string windowsPath = @"D:\root\PVCS\vm\common\SampleDB\archives\chess\client";
        private string path = Platform.IsWindows ? windowsPath : @"/root/PVCS/vm/common/SampleDB/archives/chess/client";

        private PvcsHistoryParser parser;
        private DateTimeFormatInfo dfi;

        [SetUp]
        public void SetUp()
        {
            parser = new PvcsHistoryParser();
            dfi = new DateTimeFormatInfo();
            dfi.AMDesignator = "AM";
            dfi.PMDesignator = "PM";
            dfi.MonthDayPattern = @"M/d/yy h:mm:ss tt";
        }

        [Test]
        public void ParseModifications()
        {
            Modification[] actual = parser.Parse(ContentReader, OLDEST_ENTRY, NEWEST_ENTRY);
            Modification[] expected = GetExpectedModifications();
            CustomAssertion.AssertEqualArrays(expected, actual);
        }

        [Test]
        public void AnalyzeModifications()
        {
            Modification[] expected = new Modification[1];
            expected[0] = new Modification();
            expected[0].Comment = @"Added new labels.";
            expected[0].EmailAddress = null;
            expected[0].Version = "1.4";
            expected[0].FileName = @"ChessViewer.java-arc";
            expected[0].FolderName = path;
            expected[0].ModifiedTime = DateTime.Parse("2/1/00 4:52:46 PM", dfi);
            expected[0].Type = "Checked in";
            expected[0].UserName = "kerstinb";

            Modification[] actual = PvcsHistoryParser.AnalyzeModifications(GetExpectedModificationsForAnalyze());

            CustomAssertion.AssertEqualArrays(actual, expected);
        }

        #region Pvcs Version Information

        private TextReader ContentReader
        {
            get { return new StringReader(PVCS_VERSION_INFO); }
        }

        private Modification[] GetExpectedModificationsForAnalyze()
        {
            Modification[] mod = new Modification[4];
            mod[0] = new Modification();
            mod[0].Comment = @"Added new labels.";
            mod[0].EmailAddress = null;
            mod[0].Version = "1.2";
            mod[0].FileName = @"ChessViewer.java-arc";
            mod[0].FolderName = path;
            mod[0].ModifiedTime = DateTime.Parse("2/1/00 4:52:46 PM", dfi);
            mod[0].Type = "Checked in";
            mod[0].UserName = "kerstinb";

            mod[1] = new Modification();
            mod[1].Comment = @"Added new labels.";
            mod[1].EmailAddress = null;
            mod[1].Version = "1.1";
            mod[1].FileName = @"ChessViewer.java-arc";
            mod[1].FolderName = path;
            mod[1].ModifiedTime = DateTime.Parse("2/1/00 4:52:46 PM", dfi);
            mod[1].Type = "Checked in";
            mod[1].UserName = "kerstinb";

            mod[2] = new Modification();
            mod[2].Comment = @"Added new labels.";
            mod[2].EmailAddress = null;
            mod[2].Version = "1.4";
            mod[2].FileName = @"ChessViewer.java-arc";
            mod[2].FolderName = path;
            mod[2].ModifiedTime = DateTime.Parse("2/1/00 4:52:46 PM", dfi);
            mod[2].Type = "Checked in";
            mod[2].UserName = "kerstinb";

            mod[3] = new Modification();
            mod[3].Comment = @"Added new labels.";
            mod[3].EmailAddress = null;
            mod[3].Version = "1.3";
            mod[3].FileName = @"ChessViewer.java-arc";
            mod[3].FolderName = path;
            mod[3].ModifiedTime = DateTime.Parse("2/1/00 4:52:46 PM", dfi);
            mod[3].Type = "Checked in";
            mod[3].UserName = "kerstinb";

            return mod;
        }

        private Modification[] GetExpectedModifications()
        {
            Modification[] mod = new Modification[4];

            mod[0] = new Modification();
            mod[0].Comment = @"Added new msg string.";
            mod[0].EmailAddress = null;
            mod[0].Version = "1.3";
            mod[0].FileName = @"BoardOptions.java-arc";
            mod[0].FolderName = path;
            mod[0].ModifiedTime = DateTime.Parse("2/1/00 4:22:36 PM", dfi);
            mod[0].Type = "Checked in";
            mod[0].UserName = "kerstinb";

            mod[1] = new Modification();
            mod[1].Comment = @"Enabled system printouts.";
            mod[1].EmailAddress = null;
            mod[1].Version = "1.2";
            mod[1].FileName = @"ChessRules.java-arc";
            mod[1].FolderName = path;
            mod[1].ModifiedTime = DateTime.Parse("2/1/00 4:26:14 PM", dfi);
            mod[1].Type = "Checked in";
            mod[1].UserName = "kerstinb";

            mod[2] = new Modification();
            mod[2].Comment = @"Added more explantions.";
            mod[2].EmailAddress = null;
            mod[2].Version = "1.1";
            mod[2].FileName = @"chessviewer.html-arc";
            mod[2].FolderName = path;
            mod[2].ModifiedTime = DateTime.Parse("5/18/98 4:46:38 AM", dfi);
            mod[2].Type = "Checked in";
            mod[2].UserName = "kerstinb";

            mod[3] = new Modification();
            mod[3].Comment = @"Added new labels.";
            mod[3].EmailAddress = null;
            mod[3].Version = "1.2";
            mod[3].FileName = @"ChessViewer.java-arc";
            mod[3].FolderName = path;
            mod[3].ModifiedTime = DateTime.Parse("2/1/00 4:52:46 PM", dfi);
            mod[3].Type = "Checked in";
            mod[3].UserName = "kerstinb";

            return mod;
        }

        private string PVCS_VERSION_INFO { get { return
            @"
Change History  

--------------------------------------------------------------------------------
 

Archive:          D:\root\PVCS\vm\common\SampleDB\archives\chess\client\BoardOptions.java-arc
Workfile:         BoardOptions.java
Archive created:  Feb 01 2000 16:16:48
Owner:            Admin
Last trunk rev:   1.3
Locks:            
Groups:           
Rev count:        4
Attributes:
   WRITEPROTECT
   CHECKLOCK
   NOEXCLUSIVELOCK
   EXPANDKEYWORDS
   NOTRANSLATE
   NOCOMPRESSDELTA
   NOCOMPRESSWORKIMAGE
   GENERATEDELTA
   COMMENTPREFIX = ""   ""
   NEWLINE = ""\r\n""
Version labels:
   ""LATEST"" = 1.*
   ""REL_1.0"" = 1.3
Description:
Sample Project Database - first revision.

-----------------------------------
Rev 1.3
Checked in:     Feb 01 2000 16:22:36
Last modified:  May 18 1998 04:51:30
Author id: kerstinb     lines deleted/added/moved: 0/1/0
Added new msg string.
-----------------------------------
Rev 1.2
Checked in:     Feb 01 2000 16:21:56
Last modified:  May 17 1998 22:16:16
Author id: kerstinb     lines deleted/added/moved: 0/1/0
Added getLastMove method.
-----------------------------------
Rev 1.1
Checked in:     Feb 01 2000 16:20:24
Last modified:  May 17 1998 16:53:20
Author id: kasiaj     lines deleted/added/moved: 0/1/0
Changed implementation of processWindowEvent.
-----------------------------------
Rev 1.0
Checked in:     Feb 01 2000 16:16:48
Last modified:  May 17 1998 16:37:08
Author id: Admin     lines deleted/added/moved: 0/0/0
Initial revision.
===================================

Archive:          D:\root\PVCS\vm\common\SampleDB\archives\chess\client\ChessRules.java-arc
Workfile:         ChessRules.java
Archive created:  Feb 01 2000 16:16:48
Owner:            Admin
Last trunk rev:   1.2
Locks:            
Groups:           
Rev count:        4
Attributes:
   WRITEPROTECT
   CHECKLOCK
   NOEXCLUSIVELOCK
   EXPANDKEYWORDS
   NOTRANSLATE
   NOCOMPRESSDELTA
   NOCOMPRESSWORKIMAGE
   GENERATEDELTA
   COMMENTPREFIX = ""   ""
   NEWLINE = ""\r\n""
Version labels:
   ""LATEST"" = 1.*
   ""REL_1.0"" = 1.2
Description:
Sample Project Database - first revision.

-----------------------------------
Rev 1.2
Checked in:     Feb 01 2000 16:26:14
Last modified:  May 18 1998 04:53:12
Author id: kerstinb     lines deleted/added/moved: 0/1/0
Branches:  1.2.1
Enabled system printouts.
-----------------------------------
    Rev 1.2.1.0
    Checked in:     Feb 01 2000 16:29:00
    Last modified:  May 18 1998 05:22:18
    Author id: krisb     lines deleted/added/moved: 1/0/0
    Increased max number of moves.
-----------------------------------
Rev 1.1
Checked in:     Feb 01 2000 16:25:12
Last modified:  May 17 1998 22:19:10
Author id: kerstinb     lines deleted/added/moved: 0/1/0
Added getLastMove method.
-----------------------------------
Rev 1.0
Checked in:     Feb 01 2000 16:16:48
Last modified:  May 17 1998 16:37:10
Author id: Admin     lines deleted/added/moved: 0/0/0
Initial revision.
===================================

Archive:          D:\root\PVCS\vm\common\SampleDB\archives\chess\client\chessviewer.html-arc
Workfile:         chessviewer.html
Archive created:  May 17 1998 16:37:08
Owner:            Admin
Last trunk rev:   1.1
Locks:            
Groups:           
Rev count:        2
Attributes:
   WRITEPROTECT
   CHECKLOCK
   NOEXCLUSIVELOCK
   NOEXPANDKEYWORDS
   NOTRANSLATE
   NOCOMPRESSDELTA
   NOCOMPRESSWORKIMAGE
   GENERATEDELTA
   COMMENTPREFIX = ""   ""
   NEWLINE = ""\r\n""
Version labels:
   ""LATEST"" = 1.*
   ""REL_1.0"" = 1.1
Description:
Sample Project Database - first revision.

-----------------------------------
Rev 1.1
Checked in:     18 May 1998 04:46:38
Last modified:  18 May 1998 04:45:30
Author id: kerstinb     lines deleted/added/moved: 0/0/0
Added more explantions.
-----------------------------------
Rev 1.0
Checked in:     May 17 1998 16:37:10
Last modified:  May 17 1998 16:37:10
Author id: Admin     lines deleted/added/moved: 0/0/0
Initial revision.
===================================

Archive:          D:\root\PVCS\vm\common\SampleDB\archives\chess\client\ChessViewer.java-arc
Workfile:         ChessViewer.java
Archive created:  Feb 01 2000 16:16:48
Owner:            Admin
Last trunk rev:   1.2
Locks:            
Groups:           
Rev count:        3
Attributes:
   WRITEPROTECT
   CHECKLOCK
   NOEXCLUSIVELOCK
   EXPANDKEYWORDS
   NOTRANSLATE
   NOCOMPRESSDELTA
   NOCOMPRESSWORKIMAGE
   GENERATEDELTA
   COMMENTPREFIX = ""   ""
   NEWLINE = ""\r\n""
Version labels:
   ""LATEST"" = 1.*
   ""REL_1.0"" = 1.2
Description:
Sample Project Database - first revision.

-----------------------------------
Rev 1.2
Checked in:     Feb 01 2000 16:52:46
Last modified:  May 18 1998 04:55:32
Author id: kerstinb     lines deleted/added/moved: 0/1/0
Added new labels.
-----------------------------------
Rev 1.1
Checked in:     Feb 01 2000 16:51:44
Last modified:  May 17 1998 22:35:58
Author id: kerstinb     lines deleted/added/moved: 0/1/0
Changed blackOnTop to false.
-----------------------------------
Rev 1.0
Checked in:     Feb 01 2000 16:16:48
Last modified:  May 17 1998 16:37:08
Author id: Admin     lines deleted/added/moved: 0/0/0
Initial revision.
===================================
--------------------------------------------------------------------------------
				".Replace(windowsPath, path).Replace("\\", System.IO.Path.DirectorySeparatorChar.ToString());
            } 
        }

        #endregion
    }
}