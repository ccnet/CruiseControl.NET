using System;
using System.Configuration;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.config;
using ThoughtWorks.CruiseControl.WebDashboard.Config;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	public class Default : Page
	{
		public static readonly string FORCE_BUILD_COMMAND = "forcebuild";

		protected Label ExceptionTitleLabel;
		protected DataGrid ExceptionGrid;
		protected Label StatusLabel;
		protected DataGrid StatusGrid;
		protected Button RefreshButton;
		private LocalCruiseManagerAggregator cruiseManager = new LocalCruiseManagerAggregator((ServerSpecification[])ConfigurationSettings.GetConfig(ServersSectionHandler.SectionName));
	
		private void RefreshDetails()
		{
			if (cruiseManager.ProjectDetails.Count > 0)
			{
				StatusGrid.DataSource = new ProjectDetailsListGenerator(cruiseManager).ProjectDetailsList;
				StatusGrid.DataBind();
				StatusGrid.Visible = true;
			}
			else
			{
				StatusGrid.Visible = false;
			}			

			if (cruiseManager.ConnectionExceptions.Count > 0)
			{
				ExceptionGrid.DataSource = cruiseManager.ConnectionExceptions;
				ExceptionGrid.DataBind();
				ExceptionGrid.Visible = true;
				ExceptionTitleLabel.Visible = true;
			}
			else
			{
				ExceptionGrid.Visible = false;
				ExceptionTitleLabel.Visible = false;
			}
		}

		private void ForceBuild(string projectName)
		{
			try
			{
				cruiseManager.ForceBuild(projectName);
				StatusLabel.Text = "Build Successfully Forced for  " + projectName;
			}
			catch (Exception e)
			{
				StatusLabel.Text = "Build could not be forced for " + projectName + " , exception was: " + e.ToString();
			}
			StatusLabel.Visible = true;
		}

		private void StatusGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if((e.Item.ItemType == ListItemType.Item) ||
				(e.Item.ItemType == ListItemType.AlternatingItem))
			{
				ProjectStatus thisProjectsStatus = (ProjectStatus) e.Item.DataItem;
				TableCell buildStatusCell = e.Item.Cells[1];

				if (thisProjectsStatus.BuildStatus == IntegrationStatus.Success)
				{
					buildStatusCell.ForeColor = Color.Green;
				}
				else if (thisProjectsStatus.BuildStatus == IntegrationStatus.Unknown)
				{
					buildStatusCell.ForeColor = Color.Yellow;
				}
				else
				{
					buildStatusCell.ForeColor = Color.Red;
				}
			}
		}

		private void Page_Load(object sender, EventArgs e)
		{
			// We have to check for postback otherwise the 'force build' button gets swallowed (http://weblogs.asp.net/benmiller/archive/2003/04/04/4844.aspx)
			if (!IsPostBack)
			{
				RefreshDetails();
			}
		}

		private void StatusGrid_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			if (e.CommandName == FORCE_BUILD_COMMAND)
			{
				// The command argument of the embedded button is the nsame of the project to build
				// See comments in the HTML to see the data binding to make this work
				ForceBuild(e.CommandArgument.ToString());
			}
		}

		private void RefreshButton_Click(object sender, EventArgs e)
		{
			RefreshDetails();
			StatusLabel.Visible = false;
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}
		
		private void InitializeComponent()
		{    
			this.StatusGrid.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.StatusGrid_ItemCommand);
			this.StatusGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.StatusGrid_ItemDataBound);
			this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
