using System;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	public class CvsMother
	{
		public static DateTime OLDEST_ENTRY = DateTime.Parse("2002/03/13 19:38:42");
		public static DateTime NEWEST_ENTRY = DateTime.Parse("2002/03/15 19:20:28");

		public static string CVS_LOGFILE_CONTENT
		{
			get { return @"RCS file: /cvsroot/cruisecontrol/cruisecontrol/main/README.txt,v
head: 1.1
branch:
locks: strict
access list:
keyword substitution: kv
total revisions: 1;	selected revisions: 0
description:
=============================================================================

RCS file: /cvsroot/cruisecontrol/cruisecontrol/main/build.bat,v
head: 1.6
branch:
locks: strict
access list:
keyword substitution: kv
total revisions: 6;	selected revisions: 0
description:
=============================================================================

RCS file: /cvsroot/cruisecontrol/cruisecontrol/main/build.sh,v
head: 1.4
branch:
locks: strict
access list:
keyword substitution: kv
total revisions: 4;	selected revisions: 0
description:
=============================================================================

RCS file: /cvsroot/cruisecontrol/cruisecontrol/main/build.xml,v
head: 1.42
branch:
locks: strict
access list:
keyword substitution: kv
total revisions: 42;	selected revisions: 2
description:
----------------------------
revision 1.42
date: 2002/03/15 19:20:28;  author: alden;  state: Exp;  lines: +1 -1
enabled debug info when compiling tests.
----------------------------
revision 1.41
date: 2002/03/14 01:56:34;  author: alden;  state: Exp;  lines: +63 -49
Added target to clean up test results.
=============================================================================

RCS file: /cvsroot/cruisecontrol/cruisecontrol/main/cruise.jsp,v
head: 1.3
branch:
locks: strict
access list:
keyword substitution: kv
total revisions: 3;	selected revisions: 0
description:
=============================================================================

RCS file: /cvsroot/cruisecontrol/cruisecontrol/main/Attic/cruiseControl.bat,v
head: 1.6
branch:
locks: strict
access list:
keyword substitution: kv
total revisions: 6;	selected revisions: 0
description:
=============================================================================

RCS file: /cvsroot/cruisecontrol/cruisecontrol/main/Attic/cruiseControl.sh,v
head: 1.5
branch:
locks: strict
access list:
keyword substitution: kv
total revisions: 5;	selected revisions: 0
description:
=============================================================================

RCS file: /cvsroot/cruisecontrol/cruisecontrol/main/cruisecontrol.properties,v
head: 1.5
branch:
locks: strict
access list:
keyword substitution: kv
total revisions: 5;	selected revisions: 0
description:
=============================================================================

RCS file: /cvsroot/cruisecontrol/cruisecontrol/main/cruisecontrol.xsl,v
head: 1.15
branch:
locks: strict
access list:
keyword substitution: kv
total revisions: 15;	selected revisions: 0
description:
=============================================================================

RCS file: /cvsroot/cruisecontrol/cruisecontrol/main/emailmap.properties,v
head: 1.1
branch:
locks: strict
access list:
keyword substitution: kv
total revisions: 1;	selected revisions: 0
description:
=============================================================================

RCS file: /cvsroot/cruisecontrol/cruisecontrol/main/license.txt,v
head: 1.2
branch:
locks: strict
access list:
keyword substitution: kv
total revisions: 2;	selected revisions: 0
description:
=============================================================================

RCS file: /cvsroot/cruisecontrol/cruisecontrol/main/log4j.properties,v
head: 1.2
branch:
locks: strict
access list:
keyword substitution: kv
total revisions: 2;	selected revisions: 1
description:
----------------------------
revision 1.2
date: 2002/03/13 19:45:50;  author: alden;  state: Exp;  lines: +1 -1
Shortening ConversionPattern so we don't use up all of the available screen space.
=============================================================================

RCS file: /cvsroot/cruisecontrol/cruisecontrol/main/masterbuild.bat,v
head: 1.3
branch:
locks: strict
access list:
keyword substitution: kv
total revisions: 3;	selected revisions: 0
description:
=============================================================================

RCS file: /cvsroot/cruisecontrol/cruisecontrol/main/Attic/kungfu.xml,v
head: 1.2
branch:
locks: strict
access list:
keyword substitution: kv
total revisions: 2;     selected revisions: 1
description:
----------------------------
revision 1.2
date: 2002/03/13 19:45:42;  author: alden;  state: dead;  lines: +0 -0
Hey, look, a deleted file.
=============================================================================

RCS file: /cvsroot/cruisecontrol/cruisecontrol/main/Attic/stuff.xml,v
head: 1.4
branch:
locks: strict
access list:
keyword substitution: kv
total revisions: 4;     selected revisions: 1
description:
----------------------------
revision 1.4
date: 2002/03/13 19:38:42;  author: alden;  state: dead;  lines: +0 -0
Hey, look, another deleted file.
=============================================================================

RCS file: /cvsroot/cruisecontrol/cruisecontrol/Project/File.cs,v
head: 1.11
branch:
locks: strict
access list:
keyword substitution: kv
total revisions: 12; selected revisions: 1
description:
----------------------------
revision 1.11
date: 2005-09-12 15:01:10 +0000;  author: szko;  state: Exp;  lines: +68 -132
Fixed some bugs.
============================================================================= "; }
		}

		public static string Cvs112Examples()
		{
			return @"RCS file: /home/jerome/cvs-repository/CVSROOT/cvswrappers,v
head: 1.1
branch:
locks: strict
access list:
symbolic names:
keyword substitution: kv
total revisions: 1;	selected revisions: 1
description:
----------------------------
revision 1.1
date: 2004-03-25 00:58:49 +0000;  author: jerome;  state: Exp;
initial checkin
=============================================================================

RCS file: /var/lib/cvs/test/file.txt,v
head: 1.49
branch:
locks: strict
access list:
symbolic names:
	TEST2: 1.47.0.2
	TEST: 1.49
	test/: 1.44
	start: 1.1.1.1
	cb: 1.1.1
keyword substitution: kv
total revisions: 52; selected revisions: 1
description:
----------------------------
revision 1.49
date: 2005-08-22 17:28:13 +0000; author: jerome; state: Exp; lines: +1 -2
Test commit
=============================================================================
";
		}

		public static string CvsNTExamples()
		{
			return @"RCS file: /var/lib/cvs/test/file.txt,v
head: 1.49
branch:
locks: strict
access list:
symbolic names:
	TEST2: 1.47.0.2
	TEST: 1.49
	test/: 1.44
	start: 1.1.1.1
	cb: 1.1.1
keyword substitution: kv
total revisions: 52; selected revisions: 1
description:
----------------------------
revision 1.49
date: 2005/05/21 14:05:46; author: abc; state: Exp; kopt: kv; commitid: 3586428f402fbe6a; filename: abc.cs;
Test commit
=============================================================================

RCS file: /var/lib/cvs/test/file.txt,v
head: 1.49
branch:
locks: strict
access list:
symbolic names:
	TEST2: 1.47.0.2
	TEST: 1.49
	test/: 1.44
	start: 1.1.1.1
	cb: 1.1.1
keyword substitution: kv
total revisions: 52; selected revisions: 1
description:
----------------------------
revision 1.49
date: 2005/05/21 14:05:46; author: abc; state: Exp; lines: +2 -0; kopt: kv; commitid: 3586428f402fbe6a; filename: abcd.cs;
Test commit
";
		}

		public static string ExampleOfFileAddedOnBranch()
		{
			return @"RCS file: /cvsroot/ccnet/ccnet/project/core/label/DateLabeller.cs,v
head: 1.2
branch:
locks: strict
access list:
symbolic names:
	ver-1_1_0_2014: 1.2
	ver-1_1_0_2013: 1.2
	ver-1_0_0_1211: 1.1.2.1
	ver-1_1_0_2012: 1.2
	ver-1_1_0_2011: 1.2
	ver-1_0_0_1210: 1.1.2.1
	ver-1_1_0_2010: 1.2
	ver-1_1_0_2009: 1.2
	ver-1_0_0_1209: 1.1.2.1
	ver-1_0_0_1208: 1.1.2.1
	ver-1_1_0_2008: 1.2
	ver-1_1_0_2007: 1.2
	ver-1_0_0_1207: 1.1.2.1
	ver-1_1_0_2006: 1.2
	ver-1_0_0_1206: 1.1.2.1
	ver-1_1_0_2005: 1.2
	ver-1_0_0_1205: 1.1.2.1
	RB_1_0: 1.1.0.2
keyword substitution: kv
total revisions: 3;	selected revisions: 3
description:
----------------------------
revision 1.1
date: 2005/09/30 17:46:06;  author: exortech;  state: dead;
branches:  1.1.2;
file DateLabeller.cs was initially added on branch RB_1_0.
=============================================================================";
		}

		public static string CvsModifiedFileExample()
		{
			return @"RCS file: /var/lib/cvs/test/file.txt,v
head: 1.49
branch:
locks: strict
access list:
symbolic names:
	TEST2: 1.47.0.2
	TEST: 1.49
	test/: 1.44
	start: 1.1.1.1
	cb: 1.1.1
keyword substitution: kv
total revisions: 52; selected revisions: 1
description:
----------------------------
revision 1.49
date: 2005/11/19 18:15:09;  author: abc;  state: Exp;  lines: +2 -1;  kopt: kv;  commitid: 1b3d437f6bac72b4;  filename: abc.c;
Test commit
=============================================================================";
		}
	}
}