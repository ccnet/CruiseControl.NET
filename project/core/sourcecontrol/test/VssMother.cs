using System;
using System.IO;
using tw.ccnet.core.util;

namespace tw.ccnet.core.sourcecontrol.test
{
	public class VssMother
	{
		public static TextReader ContentReader
		{
			get
			{
				return new StringReader(VSS_LOGFILE_CONTENT);
			}
		}

		#region VSS_LOGFILE_CONTENT
		public static string VSS_LOGFILE_CONTENT 
		{
			// todo: add a multi-line comment, the 1+nth line of which starts with 
			// our ***** delimiter, just to mess with our parsing assumptions
			get 
			{
				return @"Building list for $/ccnetTestProject..

*****  New Text Document2.txt  *****
Version 2
User: Mctwo        Date: 12/26/02   Time:  2:39p
Checked in
$/CCNETTest/CCNETTEST/level1/level2/level3/level4/level5/level6/level7/level8
Comment: checking my changes in

*****  Class1.cs  *****
Version 2
User: Mctwo        Date: 12/26/02   Time:  2:30p
Checked in $/CCNETTest/CCNETTEST
Comment: this is a really long comment since that might show a problem in the
parser this is a really long comment since that might show a problem in the
parser
this is a really long comment since that might show a problem in the parser
this is a really long comment since that might show a problem in the parser
this is a really long comment since that might show a problem in the parser

*****************  Version 11  *****************
Label: ""1.2.3""
User: Mctwo        Date: 12/26/02   Time:  2:21p
Labeled
Label comment: this is a label comment

*****  plant  *****
Version 8
User: Admin        Date:  9/16/02   Time:  5:29p
shrubbery.txt deleted

*****  plant  *****
Version 7
User: Admin        Date:  9/16/02   Time:  5:29p
shrubbery.txt added

*****  plant  *****
Version 6
User: Admin        Date:  9/16/02   Time:  5:25p
weed.txt destroyed

*****  plant  *****
Version 5
User: Admin        Date:  9/16/02   Time:  5:24p
weed.txt added

*****************  Version 8   *****************
User: Admin        Date:  9/16/02   Time:  5:24p
weed.txt destroyed

*****************  Version 7   *****************
User: Admin        Date:  9/16/02   Time:  5:12p
weed.txt added

*****  cereal.txt  *****
Version 2
User: Admin        Date:  9/16/02   Time:  5:08p
Checked in $/ccnetTestProject
Comment: makin changes to cereal - added froot loops

*****************  Version 6   *****************
User: Admin        Date:  9/16/02   Time:  5:06p
cereal.txt added

*****  tree.txt  *****
Version 3
User: Admin        Date:  9/16/02   Time:  5:01p
Checked in $/ccnetTestProject/plant
Comment: added fir to tree file, checked in recursively from project root

*****  plant  *****
Version 4
User: Admin        Date:  9/16/02   Time:  5:00p
vegetable.txt added

*****  jam.txt  *****
Version 2
User: Admin        Date:  9/16/02   Time:  2:44p
Checked in $/ccnetTestProject
Comment: added blueberry

*****  tree.txt  *****
Version 2
User: Admin        Date:  9/16/02   Time:  2:43p
Checked in $/ccnetTestProject/plant
Comment: added spruce (spruced it up, you might say)

*****************  Version 5   *****************
User: Admin        Date:  9/16/02   Time:  2:41p
$plant added

*****  plant  *****
Version 3
User: Admin        Date:  9/16/02   Time:  2:41p
tree.txt added

*****  plant  *****
Version 2
User: Admin        Date:  9/16/02   Time:  2:41p
flower.txt added

*****  plant  *****
Version 1
User: Admin        Date:  9/16/02   Time:  2:41p
Created
Comment: added subfolder

*****************  Version 4   *****************
User: Admin        Date:  9/16/02   Time:  2:40p
toast.txt added

*****************  Version 3   *****************
User: Admin        Date:  9/16/02   Time:  2:40p
juice.txt added

*****************  Version 2   *****************
User: Admin        Date:  9/16/02   Time:  2:40p
jam.txt added

*****************  Version 1   *****************
User: Admin        Date:  9/16/02   Time:  2:29p
Created
Comment: 

*****  level8  *****
Version 1
User: Mctwo        Date: 12/26/02   Time:  2:38p
Created
Comment: 
";
			}
		}
		#endregion 
	}
}
