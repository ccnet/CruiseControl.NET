using System;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test
{
	public class ClearCaseMother
	{
		public static TextReader ContentReader
		{
			get
			{
				return new StringReader(OUTPUT);
			}
		}
		 
		public static DateTime OLDEST_ENTRY = DateTime.Parse("02-Feb-2002");
		public static DateTime NEWEST_ENTRY = DateTime.Now ;
		public static string LINE_TERMINATOR = "\r\n";

		public readonly static string VOB_ERROR_ONLY =
			@"cleartool: Error: Not a vob object: ""D:\CCase\ppunjani_view\DotNetRefArch\AllCorp1\bootstrap.include""." + "\r\n" +
			@"cleartool: Error: Not a vob object: ""D:\CCase\ppunjani_view\DotNetRefArch\AllCorp1\Framework\CCNET\server\ccnet.config""." + "\r\n" +
			@"cleartool: Error: Not a vob object: ""D:\CCase\ppunjani_view\DotNetRefArch\AllCorp1\Framework\CCNET\server\ccnet.exe""." + "\r\n" +
			@"cleartool: Error: Not a vob object: ""D:\CCase\ppunjani_view\DotNetRefArch\AllCorp1\Framework\CCNET\server\ccnet.exe.config""." + "\r\n";

		public readonly static string REAL_ERROR_WITH_VOB =
			@"cleartool: Error: Not a vob object: ""D:\CCase\ppunjani_view\DotNetRefArch\AllCorp1\bootstrap.include""." + "\r\n" +
			@"cleartool: Error: Not a vob object: ""D:\CCase\ppunjani_view\DotNetRefArch\AllCorp1\Framework\CCNET\server\ccnet.config""." + "\r\n" +
			@"cleartool: Error: Not a vob object: ""D:\CCase\ppunjani_view\DotNetRefArch\AllCorp1\Framework\CCNET\server\ccnet.exe""." + "\r\n" +
			@"this is a real error; this is not a test..." + "\r\n" +
			@"cleartool: Error: Not a vob object: ""D:\CCase\ppunjani_view\DotNetRefArch\AllCorp1\Framework\CCNET\server\ccnet.exe.config""." + "\r\n";
		
		#region test cleartool output

		public static string OUTPUT 
		{
			get 
			{
				return
					@"ppunjani#~#Wednesday, March 10, 2004 08:54:18 AM#~#D:\CCase\ppunjani_view\RefArch\tutorial\wwhdata\common\context.js#~#\main\4#~#checkin#~#!#~#!#~#@#@#@#@#@#@#@#@#@#@#@#@" + LINE_TERMINATOR +
					@"ppunjani#~#Wednesday, March 10, 2004 08:52:05 AM#~#D:\CCase\ppunjani_view\RefArch\tutorial\wwhdata\common\context.js#~#\main\3#~#checkin#~#!#~#!#~#@#@#@#@#@#@#@#@#@#@#@#@" + LINE_TERMINATOR +
					@"ppunjani#~#Tuesday, March 09, 2004 02:02:17 PM#~#D:\CCase\ppunjani_view\RefArch\tutorial\wwhdata\common\context.js#~#\main\2#~#checkin#~#!#~#!#~#@#@#@#@#@#@#@#@#@#@#@#@" + LINE_TERMINATOR +
					@"ppunjani#~#Wednesday, February 25, 2004 01:09:37 PM#~#D:\CCase\ppunjani_view\RefArch\tutorial\wwhdata\common\wwhpagef.js#~##~#**null operation kind**#~#!#~#!#~#@#@#@#@#@#@#@#@#@#@#@#@" + LINE_TERMINATOR +
					@"ppunjani#~#Wednesday, February 25, 2004 01:09:36 PM#~#D:\CCase\ppunjani_view\RefArch\tutorial\wwhdata\common\towwhdir.js#~##~#**null operation kind**#~#!#~#!#~#@#@#@#@#@#@#@#@#@#@#@#@" + LINE_TERMINATOR +
					@"ppunjani#~#Wednesday, February 25, 2004 01:09:36 PM#~#D:\CCase\ppunjani_view\RefArch\tutorial\wwhdata\common\topics.js#~##~#**null operation kind**#~#!#~#!#~#@#@#@#@#@#@#@#@#@#@#@#@" + LINE_TERMINATOR +
					@"ppunjani#~#Wednesday, February 25, 2004 01:09:35 PM#~#D:\CCase\ppunjani_view\RefArch\tutorial\wwhdata\common\title.js#~##~#**null operation kind**#~#!#~#!#~#@#@#@#@#@#@#@#@#@#@#@#@" + LINE_TERMINATOR +
					@"ppunjani#~#Wednesday, February 25, 2004 01:09:34 PM#~#D:\CCase\ppunjani_view\RefArch\tutorial\wwhdata\common\popups.js#~##~#**null operation kind**#~#!#~#!#~#@#@#@#@#@#@#@#@#@#@#@#@" + LINE_TERMINATOR +
					@"ppunjani#~#Wednesday, February 25, 2004 01:09:34 PM#~#D:\CCase\ppunjani_view\RefArch\tutorial\wwhdata\common\files.js#~##~#**null operation kind**#~#!#~#!#~#@#@#@#@#@#@#@#@#@#@#@#@" + LINE_TERMINATOR +
					@"ppunjani#~#Wednesday, February 25, 2004 01:09:33 PM#~#D:\CCase\ppunjani_view\RefArch\tutorial\wwhdata\common\context.js#~##~#**null operation kind**#~#!#~#!#~#@#@#@#@#@#@#@#@#@#@#@#@" + LINE_TERMINATOR +
					@"ppunjani#~#Friday, March 21, 2003 03:32:24 PM#~#D:\CCase\ppunjani_view\RefArch\tutorial\wwhdata\common\files.js#~#\main\1#~#checkin#~#!#~#!#~#made from flat file@#@#@#@#@#@#@#@#@#@#@#@" + LINE_TERMINATOR +
					@"ppunjani#~#Friday, March 21, 2003 03:32:24 PM#~#D:\CCase\ppunjani_view\RefArch\tutorial\wwhdata\common\files.js#~#\main\0#~#mkelem#~#!#~#!#~#@#@#@#@#@#@#@#@#@#@#@#@" + LINE_TERMINATOR +
					@"ppunjani#~#Friday, March 21, 2003 03:32:24 PM#~#D:\CCase\ppunjani_view\RefArch\tutorial\wwhdata\common\files.js#~#\main#~#mkelem#~#!#~#!#~#@#@#@#@#@#@#@#@#@#@#@#@" + LINE_TERMINATOR +
					@"ppunjani#~#Friday, March 21, 2003 03:32:24 PM#~#D:\CCase\ppunjani_view\RefArch\tutorial\wwhdata\common\files.js#~##~#mkelem#~#!#~#!#~#made from flat file@#@#@#@#@#@#@#@#@#@#@#@" + LINE_TERMINATOR +
					@"ppunjani#~#Tuesday, February 18, 2003 05:09:14 PM#~#D:\CCase\ppunjani_view\RefArch\tutorial\wwhdata\common\wwhpagef.js#~#\main\1#~#checkin#~#!#~#!#~#made from flat file@#@#@#@#@#@#@#@#@#@#@#@" + LINE_TERMINATOR +
					@"ppunjani#~#Tuesday, February 18, 2003 05:09:14 PM#~#D:\CCase\ppunjani_view\RefArch\tutorial\wwhdata\common\wwhpagef.js#~#\main\0#~#mkelem#~#!#~#!#~#@#@#@#@#@#@#@#@#@#@#@#@" + LINE_TERMINATOR +
					@"ppunjani#~#Tuesday, February 18, 2003 05:09:14 PM#~#D:\CCase\ppunjani_view\RefArch\tutorial\wwhdata\common\wwhpagef.js#~#\main#~#mkelem#~#!#~#!#~#@#@#@#@#@#@#@#@#@#@#@#@" + LINE_TERMINATOR +
					@"ppunjani#~#Tuesday, February 18, 2003 05:09:14 PM#~#D:\CCase\ppunjani_view\RefArch\tutorial\wwhdata\common\wwhpagef.js#~##~#mkelem#~#!#~#!#~#made from flat file@#@#@#@#@#@#@#@#@#@#@#@" + LINE_TERMINATOR +
					@"ppunjani#~#Wednesday, November 20, 2002 07:37:22 PM#~#D:\CCase\ppunjani_view\RefArch\tutorial\wwhdata\common\towwhdir.js#~#\main\1#~#checkin#~#!#~#!#~#made from flat file@#@#@#@#@#@#@#@#@#@#@#@" + LINE_TERMINATOR +
					@"ppunjani#~#Wednesday, November 20, 2002 07:37:22 PM#~#D:\CCase\ppunjani_view\RefArch\tutorial\wwhdata\common\towwhdir.js#~#\main\0#~#mkelem#~#!#~#!#~#@#@#@#@#@#@#@#@#@#@#@#@" + LINE_TERMINATOR +
					@"ppunjani#~#Wednesday, November 20, 2002 07:37:22 PM#~#D:\CCase\ppunjani_view\RefArch\tutorial\wwhdata\common\towwhdir.js#~#\main#~#mkelem#~#!#~#!#~#@#@#@#@#@#@#@#@#@#@#@#@" + LINE_TERMINATOR +
					@"ppunjani#~#Wednesday, November 20, 2002 07:37:22 PM#~#D:\CCase\ppunjani_view\RefArch\tutorial\wwhdata\common\towwhdir.js#~##~#mkelem#~#!#~#!#~#made from flat file@#@#@#@#@#@#@#@#@#@#@#@" + LINE_TERMINATOR +
					@"ppunjani#~#Friday, September 27, 2002 06:31:38 PM#~#D:\CCase\ppunjani_view\RefArch\tutorial\wwhdata\common\topics.js#~#\main\1#~#checkin#~#!#~#!#~#made from flat file@#@#@#@#@#@#@#@#@#@#@#@" + LINE_TERMINATOR +
					@"ppunjani#~#Friday, September 27, 2002 06:31:38 PM#~#D:\CCase\ppunjani_view\RefArch\tutorial\wwhdata\common\topics.js#~#\main\0#~#mkelem#~#!#~#!#~#@#@#@#@#@#@#@#@#@#@#@#@" + LINE_TERMINATOR +
					@"ppunjani#~#Friday, September 27, 2002 06:31:38 PM#~#D:\CCase\ppunjani_view\RefArch\tutorial\wwhdata\common\topics.js#~#\main#~#mkelem#~#!#~#!#~#@#@#@#@#@#@#@#@#@#@#@#@" + LINE_TERMINATOR +
					@"ppunjani#~#Friday, September 27, 2002 06:31:38 PM#~#D:\CCase\ppunjani_view\RefArch\tutorial\wwhdata\common\topics.js#~##~#mkelem#~#!#~#!#~#made from flat file@#@#@#@#@#@#@#@#@#@#@#@" + LINE_TERMINATOR +
					@"ppunjani#~#Friday, September 27, 2002 06:31:36 PM#~#D:\CCase\ppunjani_view\RefArch\tutorial\wwhdata\common\context.js#~#\main\1#~#checkin#~#!#~#!#~#made from flat file@#@#@#@#@#@#@#@#@#@#@#@" + LINE_TERMINATOR +
					@"ppunjani#~#Friday, September 27, 2002 06:31:36 PM#~#D:\CCase\ppunjani_view\RefArch\tutorial\wwhdata\common\context.js#~#\main\0#~#mkelem#~#!#~#!#~#@#@#@#@#@#@#@#@#@#@#@#@" + LINE_TERMINATOR;
			}
		}
		#endregion
	}
}
