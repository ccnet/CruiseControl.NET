using System;
using System.IO;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	public class AccuRevMother
	{
		public static readonly DateTime oldestHistoryModification = DateTime.Parse("2006/11/22 11:10:44") ;
		public static readonly DateTime newestHistoryModification = DateTime.Parse("2006/11/22 11:11:00");
		private const string EOL = "\r\n";
		
		public static TextReader historyOutputReaderWindows
		{
			get
			{
				return new StringReader(historyOutputDataWindows);
			}
		}
		
		public static TextReader historyOutputReaderUnix
		{
			get
			{
				return new StringReader(historyOutputDataUnix);
			}
		}
		 
		#region Sample "accurev hist" output for testing
		/// Number of file modifications in the history output.
		public const int historyOutputModifications = 5;
		/// Output from an "accurev hist" command on Windows, cut-and-pasted with end-of-line sequences inserted.
		private const string historyOutputDataWindows =
			         @"transaction 12245; add; 2006/11/22 11:11:00 ; user: joe_user" +
			EOL + @"  # New Project for accessing SICS/nt web services" +
			EOL + @"  \.\Dev\Server\Interface\Properties\AssemblyInfo.cs 62/1 (62/1)" + 
			EOL + @"  ancestor: (none - initial version)" + 
			EOL + @"" + 
			EOL + @"transaction 12244; add; 2006/11/22 11:10:44 ; user: sam_spade" + 
			EOL + @"  # New Project for accessing web services" + 
			EOL + @"  \.\Dev\Server\Interface 62/2 (62/2)" + 
			EOL + @"  ancestor: 62/1" + 
			EOL + @"  type: dir" + 
			EOL + @"" + 
			EOL + @"  \.\Dev\Server\Interface\App.config 62/1 (62/1)" + 
			EOL + @"  ancestor: (none - initial version)" + 
			EOL + @"" + 
			EOL + @"  \.\Dev\Server\Interface\CommonTypes.cs 62/1 (62/1)" + 
			EOL + @"  ancestor: (none - initial version)" + 
			EOL + @"" + 
			EOL + @"  \.\Dev\Server\Interface\Connection.cs 62/1 (62/1)" + 
			EOL + @"  ancestor: (none - initial version)";
		/// Output from an "accurev hist" command, cut-and-pasted with end-of-line sequences inserted and with
		/// directory separators changed for Unix.
		private const string historyOutputDataUnix =
			@"transaction 12245; add; 2006/11/22 11:11:00 ; user: joe_user" +
			EOL + @"  # New Project for accessing SICS/nt web services" +
			EOL + @"  /./Dev/Server/Interface/Properties/AssemblyInfo.cs 62/1 (62/1)" + 
			EOL + @"  ancestor: (none - initial version)" + 
			EOL + @"" + 
			EOL + @"transaction 12244; add; 2006/11/22 11:10:44 ; user: sam_spade" + 
			EOL + @"  # New Project for accessing web services" + 
			EOL + @"  /./Dev/Server/Interface 62/2 (62/2)" + 
			EOL + @"  ancestor: 62/1" + 
			EOL + @"  type: dir" + 
			EOL + @"" + 
			EOL + @"  /./Dev/Server/Interface/App.config 62/1 (62/1)" + 
			EOL + @"  ancestor: (none - initial version)" + 
			EOL + @"" + 
			EOL + @"  /./Dev/Server/Interface/CommonTypes.cs 62/1 (62/1)" + 
			EOL + @"  ancestor: (none - initial version)" + 
			EOL + @"" + 
			EOL + @"  /./Dev/Server/Interface/Connection.cs 62/1 (62/1)" + 
			EOL + @"  ancestor: (none - initial version)";
		#endregion
		}
}
