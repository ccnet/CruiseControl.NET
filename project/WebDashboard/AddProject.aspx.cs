using System;
using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.AddProjectPlugin;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	public class AddProject : Page
	{
		protected System.Web.UI.WebControls.DropDownList ServerDropDown;
		protected System.Web.UI.WebControls.Button SaveButton;
		protected System.Web.UI.WebControls.TextBox RepositoryRoot;
		protected System.Web.UI.WebControls.TextBox BuilderExecutable;
		protected System.Web.UI.WebControls.TextBox BuilderBaseDirectory;
		protected System.Web.UI.WebControls.TextBox BuilderBuildArgs;
		protected System.Web.UI.WebControls.Label StatusMessageLabel;
		protected System.Web.UI.WebControls.TextBox ProjectName;
		private AddProjectPageModel model;

		private void Page_Load(object sender, EventArgs e)
		{
			model = new PluginPageRendererFactory(new DashboardComponentFactory(Request, Context, this)).AddProjectPageModel;
			if (!IsPostBack)
			{
				PopulateControls();
			}
			StatusMessageLabel.Visible = false;
		}

		private void PopulateControls()
		{
			ServerDropDown.DataSource = model.ServerNames;
			ServerDropDown.DataBind();
		}

		private void DoSave(object sender, System.EventArgs e)
		{
			model.SelectedServerName = ServerDropDown.SelectedValue;
			model.ProjectName = ProjectName.Text;
			model.RepositoryRoot = RepositoryRoot.Text;
			model.BuilderExecutable = BuilderExecutable.Text;
			model.BuilderBaseDirectory = BuilderBaseDirectory.Text;
			model.BuilderBuildArgs = BuilderBuildArgs.Text;
			StatusMessageLabel.Text = model.Save();
			StatusMessageLabel.Visible = true;
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}
		
		private void InitializeComponent()
		{    
			this.SaveButton.Click += new System.EventHandler(this.DoSave);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

	}
}
