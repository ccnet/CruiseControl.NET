using System;
using System.Globalization;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Telelogic
{
	/// <summary>
	///     Used to build CLI calls for CM Syngery.
	/// </summary>
	/// <remarks>
	///     <seealso href="https://support.telelogic.com/en/synergy/docs/docs_63/help_w/wwhelp/wwhimpl/common/html/wwhelp.htm?context=cmsynergy%26file=genl_descri5.html#wp763272" />
	/// </remarks>
	public class SynergyCommandBuilder
	{
		/// <summary>
		///     Command to check if the session is still alive.
		/// </summary>
		/// <remarks>
		///     <seealso href="https://support.telelogic.com/en/synergy/docs/docs_63/help_w/wwhelp/wwhimpl/common/html/wwhelp.htm?context=cmsynergy%26file=status_cm.html#wp902351">
		///         CM Synergy <c>status</c> Command
		///     </seealso>
		/// </remarks>
		/// <param name="connection">CM Synergy connection parameters.</param>
		public static ProcessInfo Heartbeat(SynergyConnectionInfo connection)
		{
			return CreateProcessInfo(connection, "status");
		}

		/// <summary>
		///     The start command begins a CM Synergy session by starting the engine and interface.
		/// </summary>
		/// <remarks>
		///     <seealso href="https://support.telelogic.com/en/synergy/docs/docs_63/help_w/wwhelp/wwhimpl/common/html/wwhelp.htm?context=cmsynergy%26file=shw_start_inf_cm5.html#wp425801">
		///         CM Synergy <c>start</c> Command
		///     </seealso>
		/// </remarks>
		/// <param name="connection">CM Synergy connection parameters.</param>
		/// <param name="project">Properties of the integration testing project.</param>
		public static ProcessInfo Start(SynergyConnectionInfo connection, SynergyProjectInfo project)
		{
			const string template = @"start -nogui -q -m -h ""{0}"" -d ""{1}"" -p ""{2}"" -n ""{3}"" -pw ""{4}"" -r ""{5}"" -u ""{6}"" -home ""{7}""";
			string arguments = String.Format(template, connection.Host, connection.Database, project.ProjectSpecification, connection.Username, connection.Password, connection.Role, connection.ClientDatabaseDirectory, connection.HomeDirectory);
			return CreateProcessInfo(connection, arguments);
		}

		/// <summary>
		///     The stop command ends a CM Synergy session.
		/// </summary>
		/// <remarks>
		///     <seealso href="https://support.telelogic.com/en/synergy/docs/docs_63/help_w/wwhelp/wwhimpl/common/html/wwhelp.htm?context=cmsynergy%26file=stop_cm4.html#wp900662">
		///         CM Synergy <c>stop</c> command
		///     </seealso>
		/// </remarks>
		/// <param name="connection">CM Synergy connection parameters.</param>
		public static ProcessInfo Stop(SynergyConnectionInfo connection)
		{
			return CreateProcessInfo(connection, "stop");
		}

		/// <summary>
		///     Reads the character used to separate project or object name or version values
		///     for this database.
		/// </summary>
		/// <remarks>
		///     <seealso href="https://support.telelogic.com/en/synergy/docs/docs_63/help_w/wwhelp/wwhimpl/common/html/wwhelp.htm?context=cmsynergy%26file=delimiter.html#wp900646">
		///         CM Synergy <c>delimiter</c> command
		///     </seealso>
		/// </remarks>
		/// <param name="connection">CM Synergy connection parameters.</param>
		public static ProcessInfo GetDelimiter(SynergyConnectionInfo connection)
		{
			return CreateProcessInfo(connection, "delimiter");
		}

		/// <summary>
		///     Reads the character used to separate database ID and object names.
		/// </summary>
		/// <remarks>
		///     <seealso href="https://support.telelogic.com/en/synergy/docs/docs_63/help_w/wwhelp/wwhimpl/common/html/wwhelp.htm?context=cmsynergy%26file=init_dcm4.html#wp438972">
		///         CM Synergy DCM Delimiter
		///     </seealso>
		/// </remarks>
		/// <param name="connection">CM Synergy connection parameters.</param>
		public static ProcessInfo GetDcmDelimiter(SynergyConnectionInfo connection)
		{
			return CreateProcessInfo(connection, "dcm /show /delimiter");
		}

		/// <summary>
		///     Displays the DCM settings for the current database.  Used to determine if the 
		///     database has been initialized for DCM.
		/// </summary>
		/// <remarks>
		///     <seealso href="https://support.telelogic.com/en/synergy/docs/docs_63/help_w/wwhelp/wwhimpl/common/html/wwhelp.htm?context=cmsynergy%26file=dcm.html#wp1328986">
		///          CM Synergy <c>dcm</c> command
		///     </seealso>
		///     If DCM is not initialized, the following stderr message should be displayed:
		///     <c>Warning: You must first initialize DCM before showing the DCM settings.</c>
		/// </remarks>
		/// <param name="connection">CM Synergy connection parameters.</param>
		public static ProcessInfo GetDcmSettings(SynergyConnectionInfo connection)
		{
			return CreateProcessInfo(connection, "dcm /show /settings");
		}

		/// <summary>
		///     Gets the <c>%objectname</c> value for a project.
		/// </summary>
		/// <remarks>
		///     <seealso href="https://support.telelogic.com/en/synergy/docs/docs_63/help_w/wwhelp/wwhimpl/common/html/wwhelp.htm?context=cmsynergy%26file=cp_attr_cm5.html#wp922734">
		///         CM Synergy <c>attribute</c> command
		///     </seealso>
		/// </remarks>
		/// <param name="connection">CM Synergy connection parameters.</param>
		/// <param name="project">Properties of the integration testing project.</param>
		/// <returns>A non-null initialized process structure.</returns>
		public static ProcessInfo GetProjectFullName(SynergyConnectionInfo connection, SynergyProjectInfo project)
		{
			const string template = @"properties /format ""%objectname"" /p ""{0}""";
			string arguments = String.Format(template, project.ProjectSpecification);
			return CreateProcessInfo(connection, arguments);
		}

		/// <summary>
		///     Gets the project and all contained subprojects for a given project spec.
		/// </summary>
		/// <remarks>
		///     <seealso href="https://support.telelogic.com/en/synergy/docs/docs_63/help_w/wwhelp/wwhimpl/common/html/wwhelp.htm?context=cmsynergy&amp;file=qry_cm9.html#wp901559">
		///         CM Synergy <c>query</c> command
		///     </seealso>
		/// </remarks>
		/// <param name="connection">CM Synergy connection parameters.</param>
		/// <param name="project">Properties of the integration testing project.</param>
		/// <returns>A non-null initialized process structure to query for all project members.</returns>
		public static ProcessInfo GetSubProjects(SynergyConnectionInfo connection, SynergyProjectInfo project)
		{
			const string template = @"query hierarchy_project_members('{0}', 'none')";
			string arguments = String.Format(template, project.ObjectName);
			return CreateProcessInfo(connection, arguments);
		}

		/// <summary>
		///     Updates the release value for the project.
		/// </summary>
		/// <remarks>
		///     <seealso href="https://support.telelogic.com/en/synergy/docs/docs_63/help_w/wwhelp/wwhimpl/common/html/wwhelp.htm?context=cmsynergy%26file=cp_attr_cm5.html#wp922734">
		///         CM Synergy <c>attribute</c> command
		///     </seealso>
		/// </remarks>
		/// <param name="connection">CM Synergy connection parameters.</param>
		/// <param name="project">Properties of the integration testing project.</param>
		/// <returns>A non-null initialized process structure.</returns>
		public static ProcessInfo SetProjectRelease(SynergyConnectionInfo connection, SynergyProjectInfo project)
		{
			const string template = @"attribute /m release /v ""{0}"" @ ";
			string arguments = String.Format(template, project.Release);
			return CreateProcessInfo(connection, arguments);
		}

		/// <summary>
		///     Gets the time of the last project reconfiguration.  This is useful for confirming
		///     that no outside processes have touched the project's configuration.
		/// </summary>
		/// <remarks>
		///     <seealso href="https://support.telelogic.com/en/synergy/docs/docs_63/help_w/wwhelp/wwhimpl/common/html/wwhelp.htm?context=cmsynergy%26file=cp_attr_cm5.html#wp922734">
		///         CM Synergy <c>attribute</c> command
		///     </seealso>
		/// </remarks>
		/// <param name="connection">CM Synergy connection parameters.</param>
		/// <param name="project">Properties of the integration testing project.</param>
		/// <returns>A non-null initialized process structure.</returns>
		public static ProcessInfo GetLastReconfigureTime(SynergyConnectionInfo connection, SynergyProjectInfo project)
		{
			const string template = @"attribute /show reconf_time ""{0}""";
			string arguments = String.Format(template, project.ObjectName);
			return CreateProcessInfo(connection, arguments);
		}

		/// <summary>
		///     Sets the reconfigure method to be task based, using the reconfigure template for the project.
		/// </summary>
		/// <remarks>
		///     <seealso href="https://support.telelogic.com/en/synergy/docs/docs_63/help_w/wwhelp/wwhimpl/common/html/wwhelp.htm?context=cmsynergy%26file=reconf_prop.html#wp903137">
		///         CM Synergy <c>reconfigure_properties</c> command
		///     </seealso>
		/// </remarks>
		/// <param name="connection">CM Synergy connection parameters.</param>
		/// <param name="project">Properties of the integration testing project.</param>
		/// <returns>A non-null initialized process structure.</returns>
		public static ProcessInfo UseReconfigureTemplate(SynergyConnectionInfo connection, SynergyProjectInfo project)
		{
			const string template = @"reconfigure_properties /reconf_using template /recurse ""{0}""";
			string arguments = String.Format(template, project.ProjectSpecification);
			return CreateProcessInfo(connection, arguments);
		}

		/// <summary>
		///     Updates the baseline, folder, and tasks on a project to make them consistent with the
		///     reconfigure template. Updating the folder includes performing a query.
		/// </summary>
		/// <remarks>
		///     <seealso href="https://support.telelogic.com/en/synergy/docs/docs_63/help_w/wwhelp/wwhimpl/common/html/wwhelp.htm?context=cmsynergy%26file=reconf_prop.html#wp903137">
		///         CM Synergy <c>reconfigure_properties</c> command
		///     </seealso>
		/// </remarks>
		/// <param name="connection">CM Synergy connection parameters.</param>
		/// <param name="project">Properties of the integration testing project.</param>
		/// <returns>A non-null initialized process structure.</returns>
		public static ProcessInfo UpdateReconfigureProperites(SynergyConnectionInfo connection, SynergyProjectInfo project)
		{
			const string template = @"reconfigure_properties /refresh /recurse ""{0}""";
			string arguments = String.Format(template, project.ProjectSpecification);
			return CreateProcessInfo(connection, arguments);
		}

		/// <summary>
		///     Reconfigures the project to get the lateset source (objects).
		/// </summary>
		/// <remarks>
		///     Updates the specified directory or project object. It uses reconfigure properties to find the appropriate candidates and selection rules to select new versions of the members, if appropriate.
		///     <seealso href="https://support.telelogic.com/en/synergy/docs/docs_63/help_w/wwhelp/wwhimpl/common/html/wwhelp.htm?context=cmsynergy%26file=recon_proj_cm6.html#wp900721">
		///         CM Synergy <c>reconfigure</c> command.
		///     </seealso>
		/// </remarks>
		/// <param name="connection">CM Synergy connection parameters.</param>
		/// <param name="project">Properties of the integration testing project.</param>
		/// <returns>A non-null initialized process structure.</returns>
		public static ProcessInfo Reconfigure(SynergyConnectionInfo connection, SynergyProjectInfo project)
		{
			const string template = @"reconfigure /recurse /keep_subprojects /project ""{0}""";
			string arguments = String.Format(template, project.ProjectSpecification);
			return CreateProcessInfo(connection, arguments);
		}

		/// <summary>
		///     Reconciles (syncs) the work area from the database.
		/// </summary>
		/// <remarks>
		///     <seealso href="https://support.telelogic.com/en/synergy/docs/docs_63/help_w/wwhelp/wwhimpl/common/html/wwhelp.htm?context=cmsynergy%26file=recon_wa_cm19.html#wp900913">
		///         CM Synergy <c>reconcile</c> command.
		///     </seealso>
		/// </remarks>
		/// <param name="connection">CM Synergy connection parameters.</param>
		/// <param name="project">Properties of the integration testing project.</param>
		/// <param name="path">
		///     The work area path to the file or directory to by reconciled.
		/// </param>
		/// <returns>A non-null initialized process structure.</returns>
		public static ProcessInfo Reconcile(SynergyConnectionInfo connection, SynergyProjectInfo project, string path)
		{
			const string template = @"reconcile /consider_uncontrolled /missing_wa_file /recurse /update_wa ""{0}""";
			string arguments = String.Format(template, path);
			return CreateProcessInfo(connection, arguments);
		}

		/// <summary>
		///     Returns the work area path for a specified project.
		/// </summary>
		/// <remarks>
		///     <seealso href="https://support.telelogic.com/en/synergy/docs/docs_63/help_w/wwhelp/wwhimpl/common/html/wwhelp.htm?context=cmsynergy%26file=work_area.html#wp918369">
		///         CM Synergy <c>work_area</c> command.
		///     </seealso>
		/// </remarks>
		/// <param name="connection">CM Synergy connection parameters.</param>
		/// <param name="project">Properties of the integration testing project.</param>
		/// <returns>A non-null initialized process structure.</returns>
		public static ProcessInfo GetWorkArea(SynergyConnectionInfo connection, SynergyProjectInfo project)
		{
			const string template = @"info /format ""%wa_path\%name"" /project ""{0}""";
			string arguments = String.Format(template, project.ProjectSpecification);
			return CreateProcessInfo(connection, arguments);
		}

		/// <summary>
		///     Used to populate the set of tasks that are currently part of the integration project's
		///     reconfigure properties.  The query set is used by <see cref="AddTasksToFolder"/> to
		///     manually add these tasks to the shared folder.
		/// </summary>
		/// <remarks>
		///     <note type="implementnotes">
		///         This query looks for 
		///         <list type="number">
		///             <item>
		///                 All completed tasks within a task folder that is part of a 
		///                 project's reconfigure properties.
		///             </item>
		///             <item>
		///                 All completed tasks that are part of a project's reconfigure properties.
		///             </item>
		///             <item>
		///                 All completed tasks from the baseline project(s).
		///             </item>
		///         </list>
		///         <para />
		///         This query excludes tasks already in the baseline.  Assuming that all project purposes
		///         employ similar baseline selection criteria, this should not be a problem.
        ///			NB. Dates must be formatted as 'yyyy/MM/dd HH:mm:ss': <see href="https://support.telelogic.com/en/synergy/docs/docs_63/help_w/wwhelp/wwhimpl/common/html/wwhelp.htm?context=cmsynergy&file=formats_at.html#wp901144"/>
		///     </note>
		/// </remarks>
		/// <param name="connection">CM Synergy connection parameters.</param>
		/// <param name="project">Properties of the integration testing project.</param>
		/// <param name="startDate">
		///     The minimum completion date for tasks.  Date of the last
		///     successful or unsuccessful integration run.
		/// </param>
		/// <returns>A non-null initialized process structure.</returns>
		public static ProcessInfo GetNewTasks(SynergyConnectionInfo connection, SynergyProjectInfo project, DateTime startDate)
		{
			const string template = @"query /type task /format " + @"""%displayname #### %task_number #### %completion_date #### %resolver #### %task_synopsis #### "" " + @"/nf /u /no_sort """ + /* ignore excluded and automatic tasks (which are used for project creation) */ @"status != 'task_automatic' and status != 'excluded' and " + /* include only tasks completed since the last integration run */ @"completion_date >= time('{2}') and " + /* exclude any tasks that are already in the shared folder */ @"not ( is_task_in_folder_of(folder('{1}')) or " + /* exclude any tasks that are already in the baseline project */ @"is_task_in_folder_of(is_folder_in_rp_of(is_baseline_project_of('{0}'))) or " + /* exclude any tasks that are already in the baseline project */ @"is_task_in_rp_of(is_baseline_project_of('{0}')) ) and " + /* include all tasks in the reconfigure folders or directly in the reconfigure properties */ @"(is_task_in_folder_of(is_folder_in_rp_of('{0}')) or is_task_in_rp_of('{0}'))""";
			string arguments = String.Format(template, project.ObjectName, project.TaskFolder, FormatCommandDate(startDate));
			return CreateProcessInfo(connection, arguments);
		}

		public static string FormatCommandDate(DateTime startDate)
		{
			return startDate.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
		}

		/// <summary>
		///     Gets the list of objects associated with the tasks detected by <see cref="GetNewTasks"/>
		/// </summary>
		/// <returns>A non-null initialized process structure.</returns>
		public static ProcessInfo GetTaskObjects(SynergyConnectionInfo connection, SynergyProjectInfo project)
		{
			const string template = @"task /show objects /no_sort /u @";
			return CreateProcessInfo(connection, template);
		}

		/// <summary>
		///     Used in conjuction with <see cref="GetTaskObjects"/> to get the project reference form
		///     of objects associated with a set of objects.
		/// </summary>
		/// <remarks>
		///     Runs a finduse against all projects for the modified files.  Will the use the 
		///     last row returned by finduse, which should be the lastest usage.
		/// </remarks>
		/// <param name="connection">CM Synergy connection parameters.</param>
		/// <param name="project">Properties of the integration testing project.</param>
		/// <returns>A non-null initialized process structure.</returns>
		public static ProcessInfo GetObjectPaths(SynergyConnectionInfo connection, SynergyProjectInfo project)
		{
			const string template = @"finduse @";
			return CreateProcessInfo(connection, template);
		}

		/// <summary>
		///     Used to add the current selection set to the shared task folder.
		/// </summary>
		/// <exception cref="CruiseControlException">
		///     If <pararef name="result" />.Modifications is null or empty.
		/// </exception>
		/// <param name="connection">CM Synergy connection parameters.</param>
		/// <param name="project">Properties of the integration testing project.</param>
		/// <param name="result">The integration result for this build</param>
		/// <returns>A non-null initialized process structure.</returns>
		public static ProcessInfo AddTasksToFolder(SynergyConnectionInfo connection, SynergyProjectInfo project, IIntegrationResult result)
		{
			const string template = @"folder /modify /add_tasks ""{0}"" /y ""{1}""";
			string tasks = GetTaskList(result.Modifications);
			string arguments = String.Format(template, tasks, project.TaskFolder);

			return CreateProcessInfo(connection, arguments);
		}

		/// <summary>
		///     Appends a text string to the task_description attribute to each task in
		///     the current selection set.
		/// </summary>
		/// <param name="connection">CM Synergy connection parameters.</param>
		/// <param name="project">Properties of the integration testing project.</param>
		/// <param name="result">The change set for the current integration run</param>
		/// <returns>A non-null initialized process structure.</returns>
		public static ProcessInfo AddLabelToTaskComment(SynergyConnectionInfo connection, SynergyProjectInfo project, IIntegrationResult result)
		{
			const string template = @"task /modify /description ""Integrated Successfully with CruiseControl.NET project '{0}' build '{1}' on {2}"" ""{3}""";
			string tasks = GetTaskList(result.Modifications);
			string arguments = String.Format(template, result.ProjectName, result.Label, result.StartTime, tasks);
			return CreateProcessInfo(connection, arguments);
		}

		/// <summary>
		///     Creates a baseline for the current project configuration.
		///     The first format parameter is the baseline name (similar to a label name).
		///     The second format paramter is the baseline description. 
		///     The third format parameter is the project spec.
		///     The fourth format parameter is the purpose (i.e., "Integration Testing").
		/// </summary>
		/// <remarks>
		///     <seealso href="https://support.telelogic.com/en/synergy/docs/docs_63/help_w/wwhelp/wwhimpl/common/html/wwhelp.htm?context=cmsynergy%26file=baseline.html#wp939669">
		///         CM Synergy <c>baseline</c> command
		///     </seealso>
		/// </remarks>
		/// <param name="connection">CM Synergy connection parameters.</param>
		/// <param name="project">Properties of the integration testing project.</param>
		/// <param name="result">The change set for the current integration run</param>
		/// <returns>A non-null initialized process structure.</returns>
		public static ProcessInfo CreateBaseline(SynergyConnectionInfo connection, SynergyProjectInfo project, IIntegrationResult result)
		{
			const string template = @"baseline /create ""{5:yyyyMMdd} CCNET build {1} "" /description ""Integrated Successfully with CruiseControl.NET project '{0}' build '{1}' on {5}"" /release ""{3}"" /purpose ""{4}"" /p ""{2}"" /subprojects";
			string arguments = String.Format(template, result.ProjectName, result.Label, project.ProjectSpecification, project.Release, project.Purpose, result.StartTime);
			return CreateProcessInfo(connection, arguments);
		}

		/// <summary>
		///     Factory method to initialize a new process structure for use with CM Synergy.
		/// </summary>
		/// <param name="connectionInfo">CM Synergy connection parameters.</param>
		/// <param name="arguments">The ccm command to execute.</param>
		/// <returns>A non-null initialized process structure.</returns>
		private static ProcessInfo CreateProcessInfo(SynergyConnectionInfo connectionInfo, string arguments)
		{
			return new ProcessInfo(connectionInfo.Executable, arguments, connectionInfo.WorkingDirectory);
		}

		/// <summary>
		///     Creates a comma separated list of task IDs.
		/// </summary>
		/// <exception cref="CruiseControlException">
		///     If <paramref name="modifications" /> is null or empty;
		/// </exception>
		/// <param name="modifications">
		///     The non-null, non-empty changeset which to use for the list generation.
		/// </param>
		/// <returns>
		///     A comma separated list of task IDs, with no whitespace.
		/// </returns>
		public static string GetTaskList(Modification[] modifications)
		{
			if (null == modifications || 0 == modifications.Length)
				throw(new CruiseControlException("Invalid Argument: The Synergy task list cannot be empty"));

			int length = modifications.Length;
			// initalize a string build with an approximately sized buffer
			var taskList = new string[length];
			StringBuilder retVal = new StringBuilder(10*length);

			// build an array of all unique task numbers
			int j = 0;
			foreach (Modification task in modifications)
			{
				// reset the flag
				bool exists = false;

				for (int i = 0; i < length; i++)
				{
					if (taskList[i] == task.ChangeNumber)
					{
						// exit the for loop when the task number already exists in the array
						exists = true;
						break;
					}
				}

				if (! exists)
				{
					// also build the comma separated list of tasks
					if (j > 0)
					{
						retVal.Append(',');
					}
					retVal.Append(task.ChangeNumber);

					// if the task was not found, add it to the array
					taskList[j++] = task.ChangeNumber;
				}
			}
			return (retVal.ToString());
		}
	}
}