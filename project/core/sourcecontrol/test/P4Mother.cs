using System;
using System.IO;
using tw.ccnet.core.util;

namespace tw.ccnet.core.sourcecontrol.test
{
	public class P4Mother
	{
		public static TextReader ContentReader
		{
			get
			{
				return new StringReader(P4_LOGFILE_CONTENT);
			}
		}
		
		#region P4_LOGFILE_CONTENT
		public static string P4_LOGFILE_CONTENT 
		{
			get 
			{
				return @"
text: Change 3328 by someone@somewhere on 2002/10/31 18:20:59
text:
text:   Something important
text:   so there!
text:
text: Affected files ...
text:
info1: //depot/myproject/something/SpoonCrusher.cs#3 edit
info1: //depot/myproject/something/AppleEater.cs#1 add
info1: //depot/myproject/something/MonkeyToucher.cs#811 edit
text:
text: Change 3327 by someone@somewhere on 2002/10/31 14:20:59
text:
text:   One line
text:   
text:   Another line
text:
text: Affected files ...
text:
info1: //depot/myproject/foo/IPerson.cs#3 edit
info1: //depot/myproject/foo/MiniMike.cs#1 add
info1: //depot/myproject/foo/JoeJoeJoe.cs#1 add
text:
text: Change 332 by bob@nowhere on 2002/10/31 11:20:59
text:
text:   thingy
text:   (evil below)
text:   Affected files ...
text:   Change 123 by someone@somewhere on 2002/10/31 11:20:59
text:   (end of evil)
text:
text: Affected files ...
text:
info1: //depot/myproject/tank/Fish.cs#3 add
text:
exit: 0
";
			}
		}
		#endregion 
	}
}
