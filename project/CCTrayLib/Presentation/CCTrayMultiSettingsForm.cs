using System;
using System.ComponentModel;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class CCTrayMultiSettingsForm : Form
	{
		private readonly ICCTrayMultiConfiguration configuration;
		private CheckBox chkShowBalloons;
		private GroupBox grpServers;
		private Button btnRemove;
		private Button btnMoveUp;
		private Button btnAdd;
		private Label label1;
		private Button btnOK;
		private Button btnCancel;
		private ListView lvProjects;
		private Button btnEdit;
		private ColumnHeader columnHeader1;
		private ColumnHeader columnHeader2;

		private ProjectConfigurationListViewItemAdaptor selected = null;
		private int selectedIndex = -1;
		private Button btnMoveDown;

		private Container components = null;

		public CCTrayMultiSettingsForm(ICCTrayMultiConfiguration configuration)
		{
			this.configuration = configuration.Clone();

			InitializeComponent();

			chkShowBalloons.DataBindings.Add("Checked", configuration, "ShouldShowBalloonOnBuildTransition");

			BindListView();
		}

		private void BindListView()
		{
			lvProjects.Items.Clear();

			foreach (Project project in configuration.Projects)
			{
				lvProjects.Items.Add(new ProjectConfigurationListViewItemAdaptor(project).Item);
			}

			UpdateButtons();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.chkShowBalloons = new System.Windows.Forms.CheckBox();
			this.grpServers = new System.Windows.Forms.GroupBox();
			this.btnEdit = new System.Windows.Forms.Button();
			this.lvProjects = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.label1 = new System.Windows.Forms.Label();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnMoveDown = new System.Windows.Forms.Button();
			this.btnMoveUp = new System.Windows.Forms.Button();
			this.btnRemove = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.grpServers.SuspendLayout();
			this.SuspendLayout();
			// 
			// chkShowBalloons
			// 
			this.chkShowBalloons.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chkShowBalloons.Location = new System.Drawing.Point(15, 10);
			this.chkShowBalloons.Name = "chkShowBalloons";
			this.chkShowBalloons.Size = new System.Drawing.Size(248, 24);
			this.chkShowBalloons.TabIndex = 7;
			this.chkShowBalloons.Text = "Show balloon notifications";
			// 
			// grpServers
			// 
			this.grpServers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.grpServers.Controls.Add(this.btnEdit);
			this.grpServers.Controls.Add(this.lvProjects);
			this.grpServers.Controls.Add(this.label1);
			this.grpServers.Controls.Add(this.btnAdd);
			this.grpServers.Controls.Add(this.btnMoveDown);
			this.grpServers.Controls.Add(this.btnMoveUp);
			this.grpServers.Controls.Add(this.btnRemove);
			this.grpServers.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.grpServers.Location = new System.Drawing.Point(15, 45);
			this.grpServers.Name = "grpServers";
			this.grpServers.Size = new System.Drawing.Size(580, 270);
			this.grpServers.TabIndex = 9;
			this.grpServers.TabStop = false;
			this.grpServers.Text = "Build Servers";
			// 
			// btnEdit
			// 
			this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnEdit.Location = new System.Drawing.Point(495, 95);
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.TabIndex = 8;
			this.btnEdit.Text = "&Edit...";
			this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
			// 
			// lvProjects
			// 
			this.lvProjects.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lvProjects.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						 this.columnHeader1,
																						 this.columnHeader2});
			this.lvProjects.FullRowSelect = true;
			this.lvProjects.HideSelection = false;
			this.lvProjects.Location = new System.Drawing.Point(10, 55);
			this.lvProjects.Name = "lvProjects";
			this.lvProjects.Size = new System.Drawing.Size(475, 205);
			this.lvProjects.TabIndex = 7;
			this.lvProjects.View = System.Windows.Forms.View.Details;
			this.lvProjects.DoubleClick += new System.EventHandler(this.lvProjects_DoubleClick);
			this.lvProjects.SelectedIndexChanged += new System.EventHandler(this.lvProjects_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Build Server";
			this.columnHeader1.Width = 339;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Project";
			this.columnHeader2.Width = 124;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(10, 25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(560, 20);
			this.label1.TabIndex = 6;
			this.label1.Text = "Use this section to define the CruiseControl.NET projects to monitor. ";
			// 
			// btnAdd
			// 
			this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnAdd.Location = new System.Drawing.Point(495, 60);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.TabIndex = 5;
			this.btnAdd.Text = "&Add...";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnMoveDown
			// 
			this.btnMoveDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnMoveDown.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnMoveDown.Location = new System.Drawing.Point(495, 200);
			this.btnMoveDown.Name = "btnMoveDown";
			this.btnMoveDown.TabIndex = 4;
			this.btnMoveDown.Text = "Move &Down";
			this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
			// 
			// btnMoveUp
			// 
			this.btnMoveUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnMoveUp.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnMoveUp.Location = new System.Drawing.Point(495, 165);
			this.btnMoveUp.Name = "btnMoveUp";
			this.btnMoveUp.TabIndex = 3;
			this.btnMoveUp.Text = "Move &Up";
			this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
			// 
			// btnRemove
			// 
			this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnRemove.Location = new System.Drawing.Point(495, 130);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.TabIndex = 1;
			this.btnRemove.Text = "&Remove";
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// btnOK
			// 
			this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnOK.Location = new System.Drawing.Point(224, 330);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 10;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnCancel.Location = new System.Drawing.Point(314, 330);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 11;
			this.btnCancel.Text = "Cancel";
			// 
			// CCTrayMultiSettingsForm
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(612, 368);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.grpServers);
			this.Controls.Add(this.chkShowBalloons);
			this.Name = "CCTrayMultiSettingsForm";
			this.Text = "CruiseControl.NET Tray Settings";
			this.grpServers.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private void lvProjects_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lvProjects.SelectedItems.Count == 0)
			{
				selected = null;
				selectedIndex = -1;
			}
			else
			{
				selected = (ProjectConfigurationListViewItemAdaptor) lvProjects.SelectedItems[0].Tag;
				selectedIndex = lvProjects.SelectedIndices[0];
			}

			UpdateButtons();
		}

		private void UpdateButtons()
		{
			btnRemove.Enabled = btnEdit.Enabled = selected != null;

			if (selected != null)
			{
				btnMoveDown.Enabled = selectedIndex < (lvProjects.Items.Count - 1);
				btnMoveUp.Enabled = selectedIndex != 0;
			}
			else
			{
				btnMoveDown.Enabled = btnMoveUp.Enabled = false;
			}

		}

		private void btnRemove_Click(object sender, EventArgs e)
		{
			lvProjects.Items.RemoveAt(selectedIndex);
		}

		private void btnMoveUp_Click(object sender, EventArgs e)
		{
			MoveSelectedItem(-1);
		}

		private void btnMoveDown_Click(object sender, EventArgs e)
		{
			MoveSelectedItem(+1);
		}

		private void MoveSelectedItem(int delta)
		{
			ProjectConfigurationListViewItemAdaptor currentlySelected = selected;
			int reinsertIndex = selectedIndex + delta;

			lvProjects.Items.RemoveAt(selectedIndex);
			lvProjects.Items.Insert(reinsertIndex, currentlySelected.Item);
		}

		private void lvProjects_DoubleClick(object sender, EventArgs e)
		{
			btnEdit_Click(sender, e);
		}

		private void btnEdit_Click(object sender, EventArgs e)
		{
			AddEditProject addEditProject = new AddEditProject(selected.Project);
			addEditProject.ShowDialog(this);

			selected.Rebind();
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			Project newProject = new Project();
			AddEditProject addEditProject = new AddEditProject(newProject);
			if (addEditProject.ShowDialog(this) == DialogResult.OK)
			{
				lvProjects.Items.Add(new ProjectConfigurationListViewItemAdaptor(newProject).Item).Selected = true;
			}
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			Project[] newProjectList = new Project[lvProjects.Items.Count];

			for (int i = 0; i < lvProjects.Items.Count; i++)
			{
				ProjectConfigurationListViewItemAdaptor adaptor = (ProjectConfigurationListViewItemAdaptor) lvProjects.Items[i].Tag;
				newProjectList[i] = adaptor.Project;
			}

			configuration.Projects = newProjectList;
			configuration.Persist();
		}
	}
}