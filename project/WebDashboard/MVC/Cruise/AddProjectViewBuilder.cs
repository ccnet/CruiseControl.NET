using System.Web.UI;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Builder;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class AddProjectViewBuilder : HtmlBuilderViewBuilder
	{
		public AddProjectViewBuilder(IHtmlBuilder htmlBuilder) : base(htmlBuilder)
		{
		}

		public Control BuildView(AddProjectModel model)
		{
			return Table(
				TR(TD("Servers"), TD(DropDown("ServersDropDown", model.ServerNames, model.SelectedServerName))),
				TR(TD(), TD(BuildProjectView(model.Project)))
				);
		}

		private Control BuildProjectView(Project project)
		{
			return Table(
				TR(TD("Project Name"), TD(TextBox("Project.Name", project.Name))),
				TR(TD("Source Control"), TD("Perforce")),
				TR(TD(), TD(BuildPerforceView((P4) project.SourceControl))),
				TR(TD("Builder"), TD("NAnt")),
				TR(TD(), TD(BuildNAntBuilderView((NAntBuilder) project.Builder))),
				TR(TD("Files To Merge"), TD(MultiLineTextBox("Project.Tasks.0.MergeFiles", ((MergeFilesTask) project.Tasks[0]).MergeFilesForPresentation)))
				);
		}

		private Control BuildPerforceView(P4 p4)
		{
			return Table(
				TR(TD("View"), TD(TextBox("Project.SourceControl.View", p4.View))),
				TR(TD("Executable"), TD(TextBox("Project.SourceControl.Executable", p4.Executable))),
				TR(TD("Client"), TD(TextBox("Project.SourceControl.Client", p4.Client))),
				TR(TD("User"), TD(TextBox("Project.SourceControl.User", p4.User))),
				TR(TD("Port"), TD(TextBox("Project.SourceControl.Port", p4.Port))),
				TR(TD("ApplyLabel"), TD(BooleanCheckBox("Project.SourceControl.ApplyLabel", p4.ApplyLabel))),
				TR(TD("AutoGetSource"), TD(BooleanCheckBox("Project.SourceControl.AutoGetSource", p4.AutoGetSource)))
				);
		}

		private Control BuildNAntBuilderView(NAntBuilder nantBuilder)
		{
			// ToDo - Target List
			return Table(
				TR(TD("Executable"), TD(TextBox("Project.Builder.Executable", nantBuilder.Executable))),
				TR(TD("BaseDirectory"), TD(TextBox("Project.Builder.BaseDirectory", nantBuilder.BaseDirectory))),
				TR(TD("BuildFile"), TD(TextBox("Project.Builder.BuildFile", nantBuilder.BuildFile))),
				TR(TD("BuildArgs"), TD(TextBox("Project.Builder.BuildArgs", nantBuilder.BuildArgs))),
				TR(TD("BuildTimeoutSeconds"), TD(TextBox("Project.Builder.BuildTimeoutSeconds", nantBuilder.BuildTimeoutSeconds.ToString())))
				);
		}
		/*
		private Control CreateSourceControlDropDown(Project project)
		{
			DropDownList dropDownList = new DropDownList();
			dropDownList.ID = "SourceControlDropDown";
			dropDownList.AutoPostBack = true;
			ListItem item = new ListItem("File Source Control");
			dropDownList.Items.Add(item);
			item = new ListItem("Perforce");
			dropDownList.Items.Add(item);
			return dropDownList;
		}

						htmlBuilder.CreateRow(htmlBuilder.CreateCell(SaveButton()))
		private Button SaveButton()
		{
			Button button = new Button();
			button.Text = "Save";
			button.ID = "SaveButtonId";
			button.CommandName = "SaveButtonCommand";
			button.CommandArgument = "SaveButtonArgument";
			return button;
		}
		*/
	}
}
