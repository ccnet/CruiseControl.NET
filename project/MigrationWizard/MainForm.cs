using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.MigrationWizard
{
    public partial class MainForm : Form
    {
        #region Private fields
        private MainFormController controller;
        private WizardPageBase currentPage;
        #endregion

        #region Constructors
        public MainForm(MainFormController controller)
        {
            this.controller = controller;
            InitializeComponent();
        }
        #endregion

        #region Public methods
        #region Start()
        /// <summary>
        /// Starts the wizard.
        /// </summary>
        public void Start()
        {
            LoadPage(new IntroductionPage());
        }
        #endregion
        #endregion

        #region Private methods
        #region LoadPage()
        /// <summary>
        /// Loads a new wizard page.
        /// </summary>
        /// <param name="newPage"></param>
        private void LoadPage(WizardPageBase newPage)
        {
            // Clear the previous page
            if (currentPage != null)
            {
                currentPage.CompletePage();
                currentPage.PreviousPageChanged -= currentPage_PageChanged;
                currentPage.NextPageChanged -= currentPage_PageChanged;
                currentPage.PageCompeleted -= currentPage_Completed;
            }
            pageContainer.Controls.Clear();

            // Set the new page
            currentPage = newPage;
            currentPage.Controller = controller;
            currentPage.MigrationOptions = controller.MigrationOptions;
            pageContainer.Controls.Add(currentPage);
            currentPage.Dock = DockStyle.Fill;
            currentPage.Visible = true;
            currentPage.PreviousPageChanged += currentPage_PageChanged;
            currentPage.NextPageChanged += currentPage_PageChanged;
            currentPage.PageCompeleted += currentPage_Completed;
            ChangeNavigation();
            currentPage.RunPage();
        }
        #endregion

        #region ChangeNavigation()
        /// <summary>
        /// Change the navigation buttons.
        /// </summary>
        private void ChangeNavigation()
        {
            cancelButton.Enabled = currentPage.CanCancel;
            previousButton.Enabled = (currentPage.PreviousPage != null);
            nextButton.Enabled = (currentPage.NextPage != null) && currentPage.IsValid;
            finishButton.Enabled = currentPage.CanFinish;
        }
        #endregion
        #endregion

        #region Event handlers
        #region finishButton
        private void finishButton_Click(object sender, EventArgs e)
        {
            controller.Close();
        }
        #endregion

        #region currentPage
        private void currentPage_PageChanged(object sender, EventArgs e)
        {
            ChangeNavigation();
        }

        private void currentPage_Completed(object sender, EventArgs e)
        {
            LoadPage(currentPage.NextPage);
        }
        #endregion

        #region cancelButton
        private void cancelButton_Click(object sender, EventArgs e)
        {
            var canCancel = true;
            if (currentPage.ConfirmCancel)
            {
                canCancel = (MessageBox.Show(this, 
                    "Are you sure you want to cancel this wizard?", 
                    "Confirm cancel", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Information, 
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes);
            }
            if (canCancel) controller.Close();
        }
        #endregion

        #region nextButton
        private void nextButton_Click(object sender, EventArgs e)
        {
            LoadPage(currentPage.NextPage);
        }
        #endregion

        #region previousButton
        private void previousButton_Click(object sender, EventArgs e)
        {
            LoadPage(currentPage.PreviousPage);
        }
        #endregion
        #endregion
    }
}
