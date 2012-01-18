using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Text;
using ThoughtWorks.CruiseControl.Remote.Mono;

namespace Validator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        internal static int Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool help = false;
            bool nogui = false;
            string logfile = null;
            List<string> extra = new List<string>();
            var format = "t";

            OptionSet opts = new OptionSet();
            opts.Add("h|?|help", "display this help screen", delegate(string v) { help = v != null; })
                .Add("l|logfile=", "the log file to use", delegate(string v) { logfile = v; })
                .Add("f|format=", "the format to use for logging (t[ext], x[ml])", delegate(string v) { format = v.Substring(0, 1); })
                .Add("n|nogui", "do not open a graphical user interface", delegate(string v) { nogui = v != null; });


            try
            {
                extra = opts.Parse(args);
            }
            catch (OptionException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return 1;
            }

            if (help)
            {
                DisplayHelp(opts);
                return 0;
            }

            using (var main = new MainForm())
            {
                var isValid = true;
                if (!string.IsNullOrEmpty(logfile))
                {
                    main.LogFile = logfile;
                    switch (format.ToLowerInvariant())
                    {
                        case "x":
                            main.LogFileFormat = LogFileFormat.Xml;
                            break;

                        default:
                            main.LogFileFormat = LogFileFormat.Text;
                            break;
                    }

                    FileInfo fi = new FileInfo(logfile);
                    string dir = fi.DirectoryName;

                    if (dir != null)
                    {
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                    }

                }
                if (extra.Count == 1) isValid = main.ValidateConfig(extra[0]);
                if (!nogui)
                {
                    Application.Run(main);
                }
                else
                {
                    main.CleanUpLog();
                }

                return isValid ? 0 : 1;
            }
        }

        private static void DisplayHelp(OptionSet opts)
        {
            StringBuilder sb = new StringBuilder();

            Assembly thisApp = Assembly.GetExecutingAssembly();
            Stream helpStream = thisApp.GetManifestResourceStream("Validator.Help.txt");
            try
            {
                StreamReader reader = new StreamReader(helpStream);
                string data = reader.ReadToEnd();
                reader.Close();
                sb.Append(data);
            }
            finally
            {
                helpStream.Close();
            }

            StringWriter writer = new StringWriter(sb);
            opts.WriteOptionDescriptions(writer);

            MessageBox.Show(sb.ToString());

            writer.Close();
        }
    }
}
