using System;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test
{
	public class PvcsMother
	{
		public static DateTime OLDEST_ENTRY = DateTime.Parse("1996-Sep-13 13:34:52");		
		public static DateTime NEWEST_ENTRY = DateTime.Parse("2005-Sep-13 13:34:52");

		public static string LOGFILE_CONTENT 
		{
			get 
			{
				return @"
Archive:      piddy dee
Workfile:01234567 foo.txt
Last modified:  Sep 13 2001 13:34:52
Author id: dante 
made a first hello world comment
garbage
more garbage
Archive:    raise the
Workfile:01234567 bar.txt
Last modified:  Oct 31 2001 18:52:13
Author id: virgil
made a second hello world comment
				";
			}
		}

		public static string EXTENDED_LOGFILE_CONTENT 
		{
			get 
			{
				return @"
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

Archive:          D:\root\PVCS\vm\common\SampleDB\archives\chess\client\ChessViewer.jpr-arc
Workfile:         ChessViewer.jpr
Archive created:  May 17 1998 16:37:10
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
Checked in:     May 18 1998 04:48:44
Last modified:  May 18 1998 04:48:24
Author id: kerstinb     lines deleted/added/moved: 0/0/0
Changed the source path.
-----------------------------------
Rev 1.0
Checked in:     May 17 1998 16:37:10
Last modified:  May 17 1998 16:37:12
Author id: Admin     lines deleted/added/moved: 0/0/0
Initial revision.
===================================

Archive:          D:\root\PVCS\vm\common\SampleDB\archives\chess\client\ClientListener.java-arc
Workfile:         ClientListener.java
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
Checked in:     Feb 01 2000 16:49:56
Last modified:  May 18 1998 05:07:36
Author id: kerstinb     lines deleted/added/moved: 0/1/0
Improved moving display.
-----------------------------------
Rev 1.2
Checked in:     Feb 01 2000 16:49:12
Last modified:  May 17 1998 22:38:54
Author id: kerstinb     lines deleted/added/moved: 0/1/0
Added initialization of msg string.
-----------------------------------
Rev 1.1
Checked in:     Feb 01 2000 16:48:20
Last modified:  May 17 1998 17:05:54
Author id: kasiaj     lines deleted/added/moved: 0/1/0
Added MoveStart in run.
-----------------------------------
Rev 1.0
Checked in:     Feb 01 2000 16:16:48
Last modified:  May 17 1998 16:37:10
Author id: Admin     lines deleted/added/moved: 0/0/0
Initial revision.
===================================

Archive:          D:\root\PVCS\vm\common\SampleDB\archives\chess\client\ClientSender.java-arc
Workfile:         ClientSender.java
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
Checked in:     Feb 01 2000 16:46:04
Last modified:  May 18 1998 05:07:46
Author id: kerstinb     lines deleted/added/moved: 0/1/0
Uncommented some lines of code
-----------------------------------
Rev 1.1
Checked in:     Feb 01 2000 16:40:16
Last modified:  May 17 1998 17:07:14
Author id: kasiaj     lines deleted/added/moved: 0/1/0
Changed a display string.
-----------------------------------
Rev 1.0
Checked in:     Feb 01 2000 16:16:48
Last modified:  May 17 1998 16:37:08
Author id: Admin     lines deleted/added/moved: 0/0/0
Initial revision.
===================================

Archive:          D:\root\PVCS\vm\common\SampleDB\archives\chess\client\CVS.java-arc
Workfile:         CVS.java
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
   EXPANDKEYWORDS
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
Checked in:     May 18 1998 05:07:46
Last modified:  May 18 1998 05:07:46
Author id: kerstinb     lines deleted/added/moved: 0/0/0
-----------------------------------
Rev 1.0
Checked in:     May 17 1998 16:37:08
Last modified:  May 17 1998 16:37:10
Author id: Admin     lines deleted/added/moved: 0/0/0
Initial revision.
===================================

Archive:          D:\root\PVCS\vm\common\SampleDB\archives\chess\client\DrawDlg.java-arc
Workfile:         DrawDlg.java
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
   EXPANDKEYWORDS
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
Checked in:     May 17 1998 23:03:56
Last modified:  May 17 1998 23:03:56
Author id: kerstinb     lines deleted/added/moved: 0/0/0
In ok set modal to true.
-----------------------------------
Rev 1.0
Checked in:     May 17 1998 16:37:08
Last modified:  May 17 1998 16:37:10
Author id: Admin     lines deleted/added/moved: 0/0/0
Initial revision.
===================================

Archive:          D:\root\PVCS\vm\common\SampleDB\archives\chess\client\EastPanel.java-arc
Workfile:         EastPanel.java
Archive created:  May 17 1998 16:37:10
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
   ""REL_1.0"" = 1.1
Description:
Sample Project Database - first revision.

-----------------------------------
Rev 1.2
Checked in:     Oct 11 2002 10:24:22
Last modified:  Oct 11 2002 10:24:10
Author id: Admin     lines deleted/added/moved: 2/4/0
sample change
-----------------------------------
Rev 1.1
Checked in:     May 18 1998 05:07:46
Last modified:  May 18 1998 05:07:46
Author id: kerstinb     lines deleted/added/moved: 0/0/0
-----------------------------------
Rev 1.0
Checked in:     May 17 1998 16:37:10
Last modified:  May 17 1998 16:37:12
Author id: Admin     lines deleted/added/moved: 0/0/0
Initial revision.
===================================

Archive:          D:\root\PVCS\vm\common\SampleDB\archives\chess\client\ImportDlg.java-arc
Workfile:         ImportDlg.java
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
   EXPANDKEYWORDS
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
Checked in:     May 18 1998 05:07:46
Last modified:  May 18 1998 05:07:46
Author id: kerstinb     lines deleted/added/moved: 0/0/0
-----------------------------------
Rev 1.0
Checked in:     May 17 1998 16:37:08
Last modified:  May 17 1998 16:37:10
Author id: Admin     lines deleted/added/moved: 0/0/0
Initial revision.
===================================

Archive:          D:\root\PVCS\vm\common\SampleDB\archives\chess\client\InfoDlg.java-arc
Workfile:         InfoDlg.java
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
   EXPANDKEYWORDS
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
Checked in:     May 18 1998 05:07:48
Last modified:  May 18 1998 05:07:48
Author id: kerstinb     lines deleted/added/moved: 0/0/0
-----------------------------------
Rev 1.0
Checked in:     May 17 1998 16:37:08
Last modified:  May 17 1998 16:37:10
Author id: Admin     lines deleted/added/moved: 0/0/0
Initial revision.
===================================

Archive:          D:\root\PVCS\vm\common\SampleDB\archives\chess\client\KingPos.java-arc
Workfile:         KingPos.java
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
   EXPANDKEYWORDS
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
Checked in:     May 18 1998 05:07:48
Last modified:  May 18 1998 05:07:48
Author id: kerstinb     lines deleted/added/moved: 0/0/0
-----------------------------------
Rev 1.0
Checked in:     May 17 1998 16:37:08
Last modified:  May 17 1998 16:37:10
Author id: Admin     lines deleted/added/moved: 0/0/0
Initial revision.
===================================

Archive:          D:\root\PVCS\vm\common\SampleDB\archives\chess\client\LogonDialog.java-arc
Workfile:         LogonDialog.java
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
   EXPANDKEYWORDS
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
Checked in:     May 18 1998 05:07:48
Last modified:  May 18 1998 05:07:48
Author id: kerstinb     lines deleted/added/moved: 0/0/0
-----------------------------------
Rev 1.0
Checked in:     May 17 1998 16:37:08
Last modified:  May 17 1998 16:37:10
Author id: Admin     lines deleted/added/moved: 0/0/0
Initial revision.
===================================

Archive:          D:\root\PVCS\vm\common\SampleDB\archives\chess\client\ModalParent.java-arc
Workfile:         ModalParent.java
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
   EXPANDKEYWORDS
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
Checked in:     May 18 1998 05:07:48
Last modified:  May 18 1998 05:07:48
Author id: kerstinb     lines deleted/added/moved: 0/0/0
-----------------------------------
Rev 1.0
Checked in:     May 17 1998 16:37:08
Last modified:  May 17 1998 16:37:10
Author id: Admin     lines deleted/added/moved: 0/0/0
Initial revision.
===================================

Archive:          D:\root\PVCS\vm\common\SampleDB\archives\chess\client\MoveNode.java-arc
Workfile:         MoveNode.java
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
   EXPANDKEYWORDS
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
Checked in:     May 18 1998 05:07:48
Last modified:  May 18 1998 05:07:48
Author id: kerstinb     lines deleted/added/moved: 0/0/0
-----------------------------------
Rev 1.0
Checked in:     May 17 1998 16:37:08
Last modified:  May 17 1998 16:37:08
Author id: Admin     lines deleted/added/moved: 0/0/0
Initial revision.
===================================

Archive:          D:\root\PVCS\vm\common\SampleDB\archives\chess\client\PersistString.java-arc
Workfile:         PersistString.java
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
   EXPANDKEYWORDS
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
Checked in:     May 18 1998 05:07:48
Last modified:  May 18 1998 05:07:48
Author id: kerstinb     lines deleted/added/moved: 0/0/0
-----------------------------------
Rev 1.0
Checked in:     May 17 1998 16:37:08
Last modified:  May 17 1998 16:37:10
Author id: Admin     lines deleted/added/moved: 0/0/0
Initial revision.
===================================

Archive:          D:\root\PVCS\vm\common\SampleDB\archives\chess\client\PlayerListListener.java-arc
Workfile:         PlayerListListener.java
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
   EXPANDKEYWORDS
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
Checked in:     May 18 1998 05:07:48
Last modified:  May 18 1998 05:07:48
Author id: kerstinb     lines deleted/added/moved: 0/0/0
-----------------------------------
Rev 1.0
Checked in:     May 17 1998 16:37:08
Last modified:  May 17 1998 16:37:10
Author id: Admin     lines deleted/added/moved: 0/0/0
Initial revision.
===================================

Archive:          D:\root\PVCS\vm\common\SampleDB\archives\chess\client\readme.html-arc
Workfile:         readme.html
Archive created:  May 17 1998 16:37:10
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
Checked in:     May 18 1998 05:07:50
Last modified:  May 18 1998 05:05:58
Author id: kerstinb     lines deleted/added/moved: 0/0/0
-----------------------------------
Rev 1.0
Checked in:     May 17 1998 16:37:10
Last modified:  May 17 1998 16:37:12
Author id: Admin     lines deleted/added/moved: 0/0/0
Initial revision.
===================================

Archive:          D:\root\PVCS\vm\common\SampleDB\archives\chess\client\ServerMessage.java-arc
Workfile:         ServerMessage.java
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
   EXPANDKEYWORDS
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
Checked in:     May 18 1998 05:07:50
Last modified:  May 18 1998 05:07:50
Author id: kerstinb     lines deleted/added/moved: 0/0/0
-----------------------------------
Rev 1.0
Checked in:     May 17 1998 16:37:08
Last modified:  May 17 1998 16:37:10
Author id: Admin     lines deleted/added/moved: 0/0/0
Initial revision.
===================================

Archive:          D:\root\PVCS\vm\common\SampleDB\archives\chess\client\WannaPlay.java-arc
Workfile:         WannaPlay.java
Archive created:  May 17 1998 16:37:06
Owner:            Admin
Last trunk rev:   1.1
Locks:            
Groups:           
Rev count:        2
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
   ""REL_1.0"" = 1.1
Description:
Sample Project Database - first revision.

-----------------------------------
Rev 1.1
Checked in:     May 18 1998 05:07:50
Last modified:  May 18 1998 05:07:50
Author id: kerstinb     lines deleted/added/moved: 0/0/0
-----------------------------------
Rev 1.0
Checked in:     May 17 1998 16:37:06
Last modified:  May 17 1998 16:37:08
Author id: Admin     lines deleted/added/moved: 0/0/0
Initial revision.
===================================



 

--------------------------------------------------------------------------------
				";
			}
		}
	}
}