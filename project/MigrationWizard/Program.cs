using System;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.MigrationWizard
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var controller = new MainFormController();
            controller.ShowUI();
        }
    }
}
