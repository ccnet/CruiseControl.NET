using System;
using System.Collections;
using System.Configuration;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Web.UI;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	public class Default : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label ExceptionTitleLabel;
		protected System.Web.UI.WebControls.DataGrid ExceptionGrid;
		protected System.Web.UI.WebControls.Label StatusLabel;
		protected System.Web.UI.WebControls.DataGrid StatusGrid;
		private LocalCruiseManagerAggregator cruiseManager = new LocalCruiseManagerAggregator((IList)ConfigurationSettings.GetConfig("projectURLs"));
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			CheckForceBuild();
			ShowProjectDetails();
			ShowExceptionDetails();
		}

		private void ShowProjectDetails()
		{
			if (cruiseManager.ProjectDetails.Count > 0)
			{
				StatusGrid.DataSource = cruiseManager.ProjectDetails;
				StatusGrid.DataBind();
				StatusGrid.Visible = true;
			}
			else
			{
				StatusGrid.Visible = false;
			}			
		}

		private void ShowExceptionDetails()
		{

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

		private void CheckForceBuild()
		{
			string forceBuildProject = Request.QueryString["project"];
			if (forceBuildProject != null)
			{
				ForceBuild(forceBuildProject);
				Response.Redirect(Request.Url.GetLeftPart(UriPartial.Path), true);
			}
		}

		private void ForceBuild(string projectName)
		{
			try
			{
				cruiseManager.ForceBuild(projectName);
				StatusLabel.Text = "Build Successfully Forced for project [ " + projectName + " ]";
			}
			catch (Exception e)
			{
				StatusLabel.Text = "Build could not be forced for project [ " + projectName + " ], exception was: " + e.ToString();
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

//		private void StatusGrid_ItemCommand(object source, DataGridCommandEventArgs e)
//		{
//			if (e.CommandName == "Force")
//			{
//				HyperLink link = (HyperLink)e.Item.Cells[0].Controls[0];
//				ForceBuild(link.Text);
//			}
//		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}
		
		private void InitializeComponent()
		{    
			this.StatusGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.StatusGrid_ItemDataBound);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}

	public struct ConnectionException
	{
		private string _url;
		private Exception _exception;

		public ConnectionException(string URL, Exception exception)
		{
			this._url = URL;
			this._exception = exception;
		}

		public string URL
		{
			get { return _url; }
		}

		public string Message
		{
			get { return _exception.Message; }
		}

		public Exception Exception
		{
			get { return _exception; }
		}
	}
}
