using System;
using System.IO;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
    #region Factory class
    public class AccuRevMother
    {
        protected const string EOL = "\r\n";

        public DateTime oldestHistoryModification;
        public DateTime newestHistoryModification;
        public TextReader historyOutputReader;
        public Modification[] historyOutputModifications;

        protected AccuRevMother() 
        {
        }

        public static AccuRevMother GetInstance() {
            AccuRevMother implementation; 
            if ((new ExecutionEnvironment()).IsRunningOnWindows)
                implementation = new AccuRevMotherWindows();
            else
                implementation = new AccuRevMotherUnix();
            return implementation;
        }
    }
    #endregion
            
    #region Windows version

    public class AccuRevMotherWindows : AccuRevMother
    {
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
            
        public AccuRevMotherWindows()
        {
            historyOutputReader = new StringReader(historyOutputDataWindows);

            /// The date ranges.
            oldestHistoryModification = DateTime.Parse("2006/11/22 11:10:44");
            newestHistoryModification = DateTime.Parse("2006/11/22 11:11:00");

            historyOutputModifications = new Modification[5];

            historyOutputModifications[0] = new Modification();
            historyOutputModifications[0].ChangeNumber = 12245;
            historyOutputModifications[0].Comment = "New Project for accessing SICS/nt web services";
            historyOutputModifications[0].FileName = "AssemblyInfo.cs";
            historyOutputModifications[0].FolderName = @"Dev\Server\Interface\Properties\";
            historyOutputModifications[0].ModifiedTime = new DateTime(2006,11,22,11,11,00);
            historyOutputModifications[0].Type = "add";
            historyOutputModifications[0].UserName = "joe_user";

            historyOutputModifications[1] = new Modification();
            historyOutputModifications[1].ChangeNumber = 12244;
            historyOutputModifications[1].Comment = "New Project for accessing web services";
            historyOutputModifications[1].FileName = "Interface";
            historyOutputModifications[1].FolderName = @"Dev\Server\";
            historyOutputModifications[1].ModifiedTime = new DateTime(2006,11,22,11,10,44);
            historyOutputModifications[1].Type = "add";
            historyOutputModifications[1].UserName = "sam_spade";

            historyOutputModifications[2] = new Modification();
            historyOutputModifications[2].ChangeNumber = 12244;
            historyOutputModifications[2].Comment = "New Project for accessing web services";
            historyOutputModifications[2].FileName = "App.config";
            historyOutputModifications[2].FolderName = @"Dev\Server\Interface\";
            historyOutputModifications[2].ModifiedTime = new DateTime(2006, 11, 22, 11, 10, 44);
            historyOutputModifications[2].Type = "add";
            historyOutputModifications[2].UserName = "sam_spade";

            historyOutputModifications[3] = new Modification();
            historyOutputModifications[3].ChangeNumber = 12244;
            historyOutputModifications[3].Comment = "New Project for accessing web services";
            historyOutputModifications[3].FileName = "CommonTypes.cs";
            historyOutputModifications[3].FolderName = @"Dev\Server\Interface\";
            historyOutputModifications[3].ModifiedTime = new DateTime(2006, 11, 22, 11, 10, 44);
            historyOutputModifications[3].Type = "add";
            historyOutputModifications[3].UserName = "sam_spade";

            historyOutputModifications[4] = new Modification();
            historyOutputModifications[4].ChangeNumber = 12244;
            historyOutputModifications[4].Comment = "New Project for accessing web services";
            historyOutputModifications[4].FileName = "Connection.cs";
            historyOutputModifications[4].FolderName = @"Dev\Server\Interface\";
            historyOutputModifications[4].ModifiedTime = new DateTime(2006, 11, 22, 11, 10, 44);
            historyOutputModifications[4].Type = "add";
            historyOutputModifications[4].UserName = "sam_spade";
        }

    }
    #endregion

    #region Unix version
    public class AccuRevMotherUnix : AccuRevMother
    {
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

        public AccuRevMotherUnix()
        {
            historyOutputReader = new StringReader(historyOutputDataUnix);

            /// The date ranges.
            oldestHistoryModification = DateTime.Parse("2006/11/22 11:10:44");
            newestHistoryModification = DateTime.Parse("2006/11/22 11:11:00");

            historyOutputModifications = new Modification[5];

            historyOutputModifications[0] = new Modification();
            historyOutputModifications[0].ChangeNumber = 12245;
            historyOutputModifications[0].Comment = "New Project for accessing SICS/nt web services";
            historyOutputModifications[0].FileName = "AssemblyInfo.cs";
            historyOutputModifications[0].FolderName = @"Dev/Server/Interface/Properties/";
            historyOutputModifications[0].ModifiedTime = new DateTime(2006,11,22,11,11,00);
            historyOutputModifications[0].Type = "add";
            historyOutputModifications[0].UserName = "joe_user";

            historyOutputModifications[1] = new Modification();
            historyOutputModifications[1].ChangeNumber = 12244;
            historyOutputModifications[1].Comment = "New Project for accessing web services";
            historyOutputModifications[1].FileName = "Interface";
            historyOutputModifications[1].FolderName = @"Dev/Server/";
            historyOutputModifications[1].ModifiedTime = new DateTime(2006,11,22,11,10,44);
            historyOutputModifications[1].Type = "add";
            historyOutputModifications[1].UserName = "sam_spade";

            historyOutputModifications[2] = new Modification();
            historyOutputModifications[2].ChangeNumber = 12244;
            historyOutputModifications[2].Comment = "New Project for accessing web services";
            historyOutputModifications[2].FileName = "App.config";
            historyOutputModifications[2].FolderName = @"Dev/Server/Interface/";
            historyOutputModifications[2].ModifiedTime = new DateTime(2006, 11, 22, 11, 10, 44);
            historyOutputModifications[2].Type = "add";
            historyOutputModifications[2].UserName = "sam_spade";

            historyOutputModifications[3] = new Modification();
            historyOutputModifications[3].ChangeNumber = 12244;
            historyOutputModifications[3].Comment = "New Project for accessing web services";
            historyOutputModifications[3].FileName = "CommonTypes.cs";
            historyOutputModifications[3].FolderName = @"Dev/Server/Interface/";
            historyOutputModifications[3].ModifiedTime = new DateTime(2006, 11, 22, 11, 10, 44);
            historyOutputModifications[3].Type = "add";
            historyOutputModifications[3].UserName = "sam_spade";

            historyOutputModifications[4] = new Modification();
            historyOutputModifications[4].ChangeNumber = 12244;
            historyOutputModifications[4].Comment = "New Project for accessing web services";
            historyOutputModifications[4].FileName = "Connection.cs";
            historyOutputModifications[4].FolderName = @"Dev/Server/Interface/";
            historyOutputModifications[4].ModifiedTime = new DateTime(2006, 11, 22, 11, 10, 44);
            historyOutputModifications[4].Type = "add";
            historyOutputModifications[4].UserName = "sam_spade";
        }

    }
    #endregion
}
