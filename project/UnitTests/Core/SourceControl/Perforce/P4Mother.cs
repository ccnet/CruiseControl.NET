using System;
using System.IO;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.SourceControl.Perforce
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
		
		public static DateTime OLDEST_ENTRY = DateTime.Parse("2002/10/31 11:20:59");
		public static DateTime NEWEST_ENTRY = DateTime.Parse("2002/10/31 18:20:59");

		#region P4_LOGFILE_CONTENT

		public static string P4_LOGFILE_CONTENT 
		{
			get 
			{
				// NOTE!  on comment lines, there's a tab at position 7 (not spaces) (immediately before comment text)
				return
					"text: Change 3328 by someone@somewhere on 2002/10/31 18:20:59\r\n" +
					"text: \r\n" +
					"text: \tSomething important\r\n" +
					"text: \tso there!\r\n" +
					"text: \r\n" +
					"text: Affected files ...\r\n" +
					"text: \r\n" +
					"info1: //depot/myproject/something/SpoonCrusher.cs#3 edit\r\n" +
					"info1: //depot/myproject/something/AppleEater.cs#1 add\r\n" +
					"info1: //depot/myproject/something/MonkeyToucher.cs#811 edit\r\n" +
					"text: \r\n" +
					"text: Change 3327 by someone@somewhere on 2002/10/31 14:20:59\r\n" +
					"text: \r\n" +
					"text: \tOne line\r\n" +
					"text: \t\r\n" +
					"text: \tAnother line\r\n" +
					"text: \r\n" +
					"text: Affected files ...\r\n" +
					"text: \r\n" +
					"info1: //depot/myproject/foo/IPerson.cs#3 edit\r\n" +
					"info1: //depot/myproject/foo/MiniMike.cs#1 add\r\n" +
					"info1: //depot/myproject/foo/JoeJoeJoe.cs#1 add\r\n" +
					"text: \r\n" +
					"text: Change 332 by bob@nowhere on 2002/10/31 11:20:59\r\n" +
					"text: \r\n" +
					"text: \tthingy\r\n" +
					"text: \t(evil below)\r\n" +
					"text: \tAffected files ...\r\n" +
					"text: \tChange 123 by someone@somewhere on 2002/10/31 11:20:59\r\n" +
					"text: \t(end of evil)\r\n" +
					"text: \r\n" +
					"text: Affected files ...\r\n" +
					"text: \r\n" +
					"info1: //depot/myproject/tank/Fish.cs#3 add\r\n" +
					"text: \r\n" +
					"exit: 0\r\n";
			}
		}

		#endregion 
	}
}
