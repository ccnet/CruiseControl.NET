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
Working file: main/README.txt
head: 1.1
branch:
locks: strict
access list:
keyword substitution: kv
total revisions: 1;	selected revisions: 0
description:
=============================================================================

RCS file: /cvsroot/cruisecontrol/cruisecontrol/main/build.bat,v
Working file: main/build.bat
head: 1.6
branch:
locks: strict
access list:
keyword substitution: kv
total revisions: 6;	selected revisions: 0
description:
=============================================================================

RCS file: /cvsroot/cruisecontrol/cruisecontrol/main/build.sh,v
Working file: main/build.sh
head: 1.4
branch:
locks: strict
access list:
keyword substitution: kv
total revisions: 4;	selected revisions: 0
description:
=============================================================================

RCS file: /cvsroot/cruisecontrol/cruisecontrol/main/build.xml,v
Working file: main/build.xml
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
Working file: main/cruise.jsp
head: 1.3
branch:
locks: strict
access list:
keyword substitution: kv
total revisions: 3;	selected revisions: 0
description:
=============================================================================

RCS file: /cvsroot/cruisecontrol/cruisecontrol/main/Attic/cruiseControl.bat,v
Working file: main/cruiseControl.bat
head: 1.6
branch:
locks: strict
access list:
keyword substitution: kv
total revisions: 6;	selected revisions: 0
description:
=============================================================================

RCS file: /cvsroot/cruisecontrol/cruisecontrol/main/Attic/cruiseControl.sh,v
Working file: main/cruiseControl.sh
head: 1.5
branch:
locks: strict
access list:
keyword substitution: kv
total revisions: 5;	selected revisions: 0
description:
=============================================================================

RCS file: /cvsroot/cruisecontrol/cruisecontrol/main/cruisecontrol.properties,v
Working file: main/cruisecontrol.properties
head: 1.5
branch:
locks: strict
access list:
keyword substitution: kv
total revisions: 5;	selected revisions: 0
description:
=============================================================================

RCS file: /cvsroot/cruisecontrol/cruisecontrol/main/cruisecontrol.xsl,v
Working file: main/cruisecontrol.xsl
head: 1.15
branch:
locks: strict
access list:
keyword substitution: kv
total revisions: 15;	selected revisions: 0
description:
=============================================================================

RCS file: /cvsroot/cruisecontrol/cruisecontrol/main/emailmap.properties,v
Working file: main/emailmap.properties
head: 1.1
branch:
locks: strict
access list:
keyword substitution: kv
total revisions: 1;	selected revisions: 0
description:
=============================================================================

RCS file: /cvsroot/cruisecontrol/cruisecontrol/main/license.txt,v
Working file: main/license.txt
head: 1.2
branch:
locks: strict
access list:
keyword substitution: kv
total revisions: 2;	selected revisions: 0
description:
=============================================================================

RCS file: /cvsroot/cruisecontrol/cruisecontrol/main/log4j.properties,v
Working file: foobar/log4j.properties
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
Working file: main/masterbuild.bat
head: 1.3
branch:
locks: strict
access list:
keyword substitution: kv
total revisions: 3;	selected revisions: 0
description:
=============================================================================

RCS file: /cvsroot/cruisecontrol/cruisecontrol/main/Attic/kungfu.xml,v
Working file: main/kungfu.xml
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
Working file: main/stuff.xml
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

RCS file: /data/Project/File.cs,v
Working file: Project/File.cs
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
Working file: cvswrappers
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
Working file: test/file.txt
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
	}
}