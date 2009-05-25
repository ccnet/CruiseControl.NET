using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Validator
{
    static class Program
    {
        private static Dictionary<string, string> parsedArgs = new Dictionary<string, string>();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (var main = new MainForm())
            {
                var isValid = true;
                if ((args != null) && (args.Length > 0)) ParseArgs(args);
                if (parsedArgs.ContainsKey("-l")) main.LogFile = parsedArgs["-l"];
                if (parsedArgs.ContainsKey(string.Empty)) isValid = main.ValidateConfig(parsedArgs[string.Empty]);
                if (!parsedArgs.ContainsKey("-nui")) Application.Run(main);
                return isValid ? 0 : 1;
            }
        }

        private static void ParseArgs(string[] args)
        {
            for (var loop = 0; loop < args.Length; loop++)
            {
                if (args[loop].StartsWith("-"))
                {
                    var arg = args[loop].ToLower();
                    switch (arg)
                    {
                        case "-nui":
                            parsedArgs.Add(arg, string.Empty);
                            break;
                        default:
                            parsedArgs.Add(arg, args[++loop]);
                            break;
                    }
                }
                else
                {
                    parsedArgs.Add(string.Empty, args[0]);
                }
            }
        }
    }
}
