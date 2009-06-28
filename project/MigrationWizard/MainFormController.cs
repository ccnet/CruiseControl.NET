using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ThoughtWorks.CruiseControl.MigrationWizard
{
    /// <summary>
    /// Controller for the main form.
    /// </summary>
    public class MainFormController
    {
        #region Private fields
        private MainForm mainForm;
        private List<MigrationEventArgs> migrationEvents = new List<MigrationEventArgs>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="MainFormController"/>
        /// </summary>
        public MainFormController()
        {
            MigrationOptions = new MigrationOptions
            {
                CurrentVersion = "1.4.4",       // Assume the user has the second most recent version
                BackupConfiguration = true,
                BackupServerConfiguration = true,
                ConfigurationLocation = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    Path.Combine("CruiseControl.NET",
                        Path.Combine("Server", "ccnet.config"))),
                CurrentServerLocation = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    Path.Combine("CruiseControl.NET", "Server")),
                CurrentWebDashboardLocation = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    Path.Combine("CruiseControl.NET", "webdashboard")),
                MigrateConfiguration = true,
                MigrateServer = true,
                MigrateWebDashboard = true,
                NewServerLocation = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    Path.Combine("CruiseControl.NET", "server")),
                NewWebDashboardLocation = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    Path.Combine("CruiseControl.NET", "webdashboard"))
            };

            MigrationEngine = new MigrationEngine
            {
                MigrationOptions = MigrationOptions
            };
            MigrationEngine.Message += (o, e) => migrationEvents.Add(e);
        }
        #endregion

        #region Public properties
        #region MigrationOptions
        /// <summary>
        /// The current migration options.
        /// </summary>
        public MigrationOptions MigrationOptions { get; set; }
        #endregion

        #region MigrationEngine
        /// <summary>
        /// The migration engine.
        /// </summary>
        public MigrationEngine MigrationEngine { get; set; }
        #endregion

        #region MigrationEvents
        /// <summary>
        /// The migration events that have occurred.
        /// </summary>
        public List<MigrationEventArgs> MigrationEvents
        {
            get { return migrationEvents; }
        }
        #endregion
        #endregion

        #region Public methods
        #region Show()
        /// <summary>
        /// Show the UI.
        /// </summary>
        public void ShowUI()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            mainForm = new MainForm(this);
            mainForm.Start();
            Application.Run(mainForm);
        }
        #endregion

        #region Close()
        /// <summary>
        /// Closes the wizard.
        /// </summary>
        public void Close()
        {
            if (mainForm != null)
            {
                mainForm.Close();
                mainForm = null;
            }
        }
        #endregion

        #region ViewLog()
        /// <summary>
        /// Display the full log.
        /// </summary>
        /// <param name="owner"></param>
        public void ViewLog(Form owner)
        {
            var form = new LogForm();
            form.LoadMessages(MigrationEvents);
            if (owner != null)
            {
                form.ShowDialog(owner);
            }
            else
            {
                form.ShowDialog();
            }
        }
        #endregion
        #endregion
    }
}
