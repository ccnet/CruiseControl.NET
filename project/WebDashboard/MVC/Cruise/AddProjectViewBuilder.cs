using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Builder;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class AddProjectViewBuilder : HtmlBuilderViewBuilder
	{
		public AddProjectViewBuilder(IHtmlBuilder htmlBuilder) : base(htmlBuilder)
		{
		}

		public Control BuildView(AddEditProjectModel model)
		{
			HtmlTable table = Table();
			if (model.Status != null && model.Status != "")
			{
				table.Rows.Add(TR(TD(), TD(model.Status)));
			}
			if (model.IsAdd)
			{
				table.Rows.Add(TR(TD("Servers"), TD(DropDown("ServersDropDown", model.ServerNames, model.SelectedServerName))));	
			}
			table.Rows.Add(TR(TD(), TD(BuildProjectView(model.Project, model.IsAdd))));
			table.Rows.Add(TR(TD(), TD("* denotes currently mandatory fields")));
			if (model.SaveActionName != string.Empty)
			{
				table.Rows.Add(TR(TD(Button(model.SaveActionName, "Save")), TD()));
			}
			return table;
		}

		private Control BuildProjectView(Project project, bool isAdd)
		{
			ArrayList rows = new ArrayList();
			if (isAdd)
			{
				rows.Add(TR(TD("Project Name *"), TD(TextBox("Project.Name", project.Name))));
			}
			else
			{
				rows.Add(TR(TD("Project Name *"), TD(project.Name)));
			}
			rows.AddRange(BuildSourceControlSelectionAndView(project.SourceControl));
			rows.AddRange(BuildBuilderSelectionAndView(project.Builder));
			rows.Add(TR(TD("Files To Merge"), TD(MultiLineTextBox("Project.Tasks.0.MergeFilesForPresentation", GetMergeFiles(project)))));
			rows.Add(TR(TD("Working Directory"), TD(TextBox("Project.ConfiguredWorkingDirectory", project.ConfiguredWorkingDirectory))));
			rows.Add(TR(TD("Reporting URL"), TD(TextBox("Project.WebURL", project.WebURL))));

			return Table((HtmlTableRow[]) rows.ToArray(typeof (HtmlTableRow)));
		}

		private string GetMergeFiles(Project project)
		{
			string mergefiles = "";
			foreach (ITask task in project.Tasks)
			{
				if (task is MergeFilesTask)
				{
					mergefiles = ((MergeFilesTask) task).MergeFilesForPresentation;
				}
			}
			return mergefiles;
		}

		private HtmlTableRow[] BuildSourceControlSelectionAndView(ISourceControl sourceControl)
		{
			if (sourceControl == null)
			{
				throw new CruiseControlException("Internal Error - SourceControl object not set on model so cannot create view");
			}

			DropDownList dropDownList = new DropDownList();
			Control sourceControlView = Table(TR(TD("Unsupported Source Control type - " + sourceControl.GetType().FullName)));

			foreach(Type type in new Type[] { typeof(P4), typeof(Cvs), typeof(FileSourceControl)})
			{
				ListItem item = new ListItem(type.Name);
				if (sourceControl.GetType() == type)
				{
					sourceControlView = BuildSourceControlView(sourceControl, type);
					item.Selected = true;
				}
				dropDownList.Items.Add(item);
			}

			dropDownList.ID = "Project.SourceControl";
			dropDownList.AutoPostBack = true;

			return new HtmlTableRow[] { TR(TD("Source Control"), TD(dropDownList)), TR(TD(),TD(sourceControlView))};
		}

		private Control BuildSourceControlView(ISourceControl sourceControl, Type type)
		{
			if (type == typeof(P4))
			{
				return BuildP4View((P4) sourceControl);
			}
			else if (type == typeof(Cvs))
			{
				return BuildCvsView((Cvs) sourceControl);
			}
			else
			{
				return BuildFileSourceControlView((FileSourceControl) sourceControl);
			}
		}

		private Control BuildP4View(P4 p4)
		{
			return Table(
				TR(TD("View *"), TD(MultiLineTextBox("Project.SourceControl.View", p4.View.Replace(",", Environment.NewLine)))),
				TR(TD("Executable"), TD(TextBox("Project.SourceControl.Executable", p4.Executable))),
				TR(TD("Client"), TD(TextBox("Project.SourceControl.Client", p4.Client))),
				TR(TD("User"), TD(TextBox("Project.SourceControl.User", p4.User))),
				TR(TD("Port"), TD(TextBox("Project.SourceControl.Port", p4.Port))),
				TR(TD("ApplyLabel"), TD(BooleanCheckBox("Project.SourceControl.ApplyLabel", p4.ApplyLabel))),
				TR(TD("AutoGetSource *"), TD(BooleanCheckBox("Project.SourceControl.AutoGetSource", p4.AutoGetSource)))
				);
		}

		private Control BuildCvsView(Cvs cvs)
		{
			return Table(
				TR(TD("Executable"), TD(TextBox("Project.SourceControl.Executable", cvs.Executable))),
				TR(TD("Timeout"), TD(TextBox("Project.SourceControl.Client", cvs.Timeout.ToString()))),
				TR(TD("CvsRoot"), TD(TextBox("Project.SourceControl.CvsRoot", cvs.CvsRoot))),
				TR(TD("WorkingDirectory"), TD(TextBox("Project.SourceControl.Port", cvs.WorkingDirectory))),
				TR(TD("LabelOnSuccess"), TD(BooleanCheckBox("Project.SourceControl.LabelOnSuccess", cvs.LabelOnSuccess))),
				TR(TD("AutoGetSource"), TD(BooleanCheckBox("Project.SourceControl.AutoGetSource", cvs.AutoGetSource))),
				TR(TD("RestrictLogins"), TD(TextBox("Project.SourceControl.RestrictLogins", cvs.RestrictLogins))),
				TR(TD("Branch"), TD(TextBox("Project.SourceControl.Branch", cvs.Branch))));
		}

		private Control BuildFileSourceControlView(FileSourceControl fileSourceControl)
		{
			return Table(
				TR(TD("RepositoryRoot"), TD(TextBox("Project.SourceControl.RepositoryRoot", fileSourceControl.RepositoryRoot))),
				TR(TD("IgnoreMissingRoot"), TD(BooleanCheckBox("Project.SourceControl.IgnoreMissingRoot", fileSourceControl.IgnoreMissingRoot)))
				);
		}

		private HtmlTableRow[] BuildBuilderSelectionAndView(IBuilder builder)
		{
			if (builder == null)
			{
				throw new CruiseControlException("Internal Error - Builder object not set on model so cannot create view");
			}

			DropDownList dropDownList = new DropDownList();
			Control builderControl = Table(TR(TD("Unsupported Builder type - " + builder.GetType().FullName)));

			foreach(Type type in new Type[] { typeof(NAntBuilder), typeof(CommandLineBuilder)})
			{
				ListItem item = new ListItem(type.Name);
				if (builder.GetType() == type)
				{
					builderControl = BuildBuilderView(builder, type);
					item.Selected = true;
				}
				dropDownList.Items.Add(item);
			}

			dropDownList.ID = "Project.Builder";
			dropDownList.AutoPostBack = true;

			return new HtmlTableRow[] { TR(TD("Builder"), TD(dropDownList)), TR(TD(),TD(builderControl))};
		}

		private Control BuildBuilderView(IBuilder builder, Type type)
		{
			if (type == typeof(NAntBuilder))
			{
				return BuildNAntBuilderView((NAntBuilder) builder);
			}
			else
			{
				return BuildCommandLineBuilderView((CommandLineBuilder) builder);
			}
		}

		private Control BuildNAntBuilderView(NAntBuilder nantBuilder)
		{
			return Table(
				TR(TD("Base Directory *"), TD(TextBox("Project.Builder.BaseDirectory", nantBuilder.ConfiguredBaseDirectory))),
				TR(TD("Executable *"), TD(TextBox("Project.Builder.Executable", nantBuilder.Executable))),
				TR(TD("BuildFile *"), TD(TextBox("Project.Builder.BuildFile", nantBuilder.BuildFile))),
				TR(TD("BuildArgs"), TD(TextBox("Project.Builder.BuildArgs", nantBuilder.BuildArgs))),
				TR(TD("Targets"), TD(MultiLineTextBox("Project.Builder.TargetsForPresentation", nantBuilder.TargetsForPresentation))),
				TR(TD("BuildTimeoutSeconds *"), TD(TextBox("Project.Builder.BuildTimeoutSeconds", nantBuilder.BuildTimeoutSeconds.ToString())))
				);
		}

		private Control BuildCommandLineBuilderView(CommandLineBuilder builder)
		{
			return Table(
				TR(TD("Base Directory *"), TD(TextBox("Project.Builder.BaseDirectory", builder.ConfiguredBaseDirectory))),
				TR(TD("Executable *"), TD(TextBox("Project.Builder.Executable", builder.Executable))),
				TR(TD("BuildArgs"), TD(TextBox("Project.Builder.BuildArgs", builder.BuildArgs))),
				TR(TD("BuildTimeoutSeconds *"), TD(TextBox("Project.Builder.BuildTimeoutSeconds", builder.BuildTimeoutSeconds.ToString())))
				);
		}
	}
}
