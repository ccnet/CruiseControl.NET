using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;
using ThoughtWorks.CruiseControl.Remote;
using Mono.Options;

namespace ThoughtWorks.CruiseControl.CCTray
{
    public class Bootstrap
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
        	bool help = false;
        	List<string> extra = new List<string>();
        	
        	OptionSet opts = new OptionSet();
        	opts.Add("h|?|help", "display this help screen", delegate(string v) { help = v != null; });
        	
        	try
        	{
        		extra = opts.Parse(args);
        	}
        	catch (OptionException e)
        	{
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
				return;
			}
        	
            if (help)
            {
            	DisplayHelp(opts);
                return;
            }

            Application.EnableVisualStyles();
            Application.DoEvents();

            Application.ThreadException += UnhandledWinFormException;
            MainForm mainForm;
            try
            {
                mainForm = GetMainForm(GetSettingsFilename(extra));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to start: " + ex, AppDomain.CurrentDomain.FriendlyName, MessageBoxButtons.OK , MessageBoxIcon.Error);
                return;
            }

            try
            {
                Application.Run(mainForm);
            }
            catch (Exception ex)
            {
                string error_message = "Unhandled runtime error: " + ex;
                Debug.WriteLine(error_message);
                MessageBox.Show(error_message, AppDomain.CurrentDomain.FriendlyName, MessageBoxButtons.OK , MessageBoxIcon.Error);
                return;
            }
        }

        private static MainForm GetMainForm(string settingsFilename)
        {
            var remoteCruiseManagerFactory = new CruiseServerClientFactory();
            ICruiseServerManagerFactory cruiseServerManagerFactory = new CruiseServerManagerFactory(remoteCruiseManagerFactory);
            ICruiseProjectManagerFactory cruiseProjectManagerFactory = new CruiseProjectManagerFactory(remoteCruiseManagerFactory);
            CCTrayMultiConfiguration configuration = new CCTrayMultiConfiguration(cruiseServerManagerFactory, cruiseProjectManagerFactory, settingsFilename);

            return new MainForm(configuration);
        }

        private static void UnhandledWinFormException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show("Unhandled exception: " + e.Exception);
        }

        private static string GetSettingsFilename(List<string> extra)
        {
            if (extra.Count == 1) return extra[0]; // use settings file specified on command line

            string oldFashionedSettingsFilename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "settings.xml");
            string newSettingsFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "cctray-settings.xml");

            if (File.Exists(oldFashionedSettingsFilename) && !File.Exists(newSettingsFilename))
                File.Copy(oldFashionedSettingsFilename, newSettingsFilename);

            return newSettingsFilename;
        }
        
        private static void DisplayHelp(OptionSet opts)
        {
        	StringBuilder sb = new StringBuilder();
        	
            Assembly thisApp = Assembly.GetExecutingAssembly();
            Stream helpStream = thisApp.GetManifestResourceStream("ThoughtWorks.CruiseControl.CCTray.Help.txt");
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
            opts.WriteOptionDescriptions (writer);
            
            MessageBox.Show(sb.ToString());
            
            writer.Close();
        }
    }
}
