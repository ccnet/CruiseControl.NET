using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Builder;
using ThoughtWorks.CruiseControl.Core.Publishers;
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

		public Control BuildView(AddProjectModel model)
		{
			HtmlTable table = Table();
			if (model.Status != null && model.Status != "")
			{
				table.Rows.Add(TR(TD(), TD(model.Status)));
			}
			table.Rows.Add(TR(TD("Servers"), TD(DropDown("ServersDropDown", model.ServerNames, model.SelectedServerName))));
			table.Rows.Add(TR(TD(), TD(BuildProjectView(model.Project))));
			table.Rows.Add(TR(TD(), TD("* denotes currently mandatory fields")));
			if (model.AllowSave)
			{
				table.Rows.Add(TR(TD(Button(CruiseActionFactory.ADD_PROJECT_SAVE_ACTION_NAME, "Save")), TD()));
			}
			return table;
		}

		private Control BuildProjectView(Project project)
		{
			ArrayList rows = new ArrayList();
			rows.Add(TR(TD("Project Name *"), TD(TextBox("Project.Name", project.Name))));
			rows.Add(TR(TD("Source Control"), TD("Perforce")));
			rows.Add(TR(TD(), TD(BuildPerforceView((P4) project.SourceControl))));
			rows.AddRange(BuildBuilderSelectionAndView(project.Builder));
			rows.Add(TR(TD("Files To Merge"), TD(MultiLineTextBox("Project.Tasks.0.MergeFilesForPresentation", ((MergeFilesTask) project.Tasks[0]).MergeFilesForPresentation))));
			rows.Add(TR(TD("Output Log Directory *"), TD(TextBox("Project.Publishers.0.LogDir", ((XmlLogPublisher) project.Publishers[0]).LogDir))));
			rows.Add(TR(TD("Reporting URL *"), TD(TextBox("Project.WebURL", project.WebURL))));

			return Table((HtmlTableRow[]) rows.ToArray(typeof (HtmlTableRow)));
		}

		private Control BuildPerforceView(P4 p4)
		{
			return Table(
				TR(TD("View *"), TD(TextBox("Project.SourceControl.View", p4.View))),
				TR(TD("Executable"), TD(TextBox("Project.SourceControl.Executable", p4.Executable))),
				TR(TD("Client"), TD(TextBox("Project.SourceControl.Client", p4.Client))),
				TR(TD("User"), TD(TextBox("Project.SourceControl.User", p4.User))),
				TR(TD("Port"), TD(TextBox("Project.SourceControl.Port", p4.Port))),
				TR(TD("ApplyLabel"), TD(BooleanCheckBox("Project.SourceControl.ApplyLabel", p4.ApplyLabel))),
				TR(TD("AutoGetSource *"), TD(BooleanCheckBox("Project.SourceControl.AutoGetSource", p4.AutoGetSource)))
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

		*/
	}
}
