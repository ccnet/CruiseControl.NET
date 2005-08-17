using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Telelogic
{
	/// <summary>
	///     Used to parse the output of a Synergy <c>finduse</c> query for all objects
	///     that have changed in the current project since the last integration run.
	/// </summary>
	public class SynergyParser : IHistoryParser
	{
		/// <summary>
		///     The regular expression pattern used to match tasks added by a ccm task query.
		/// </summary>
		/// <remarks>
		///     See the inline code comments for details on the intended pattern logic.
		///     <para />
		///     See <see href="https://support.telelogic.com/en/synergy/kb/show_content.cfm?id=6200#6200">
		///     CM Synergy Date and Time Formats (TB237)</see> for details.
		/// </remarks>
		private const string TaskFormat = @"(?imn:" + // enable multiline, case-insensitive search,  explicit named captures
			@"(?<displayname>.*)" + // capture the displayname for the task
			@"(?<sep>\ \#{4}\ )" + // match the custom column separator
			@"(?<task_number>\d+)" + // capture the integer value of the task number (without the DCM prefix)
			@"\k<sep>" + // match the custom column separator
			@"(?<completion_date>.*)" + // match the Windows long date format between the delimiters
			@"\k<sep>" + // match the custom column separator
			@"(?<resolver>.*)" + // capture the resolver
			@"\k<sep>" + // match the custom column separator
			@"(?<task_synopsis>.*)" + // optionally subcapture the last string in the semi-colon delimiter list
			@"\k<sep>)"; // match the custom column separator

		/// <summary>
		///     The regular expression pattern used to match objects from a ccm finduse query.
		/// </summary>
		/// <remarks>
		///     See the inline code comments for details on the intended pattern logic.
		/// </remarks>
		private const string ObjectFormat = @"(?imn:" + // enable multiline, case-insensitive search,  explicit named captures
			@"(?<displayname>\S+)\s+" + // capture [filename][delimiter][version]
			@"(?<status>\S+)\s+" + // capture the object status
			@"(?<resolver>\S+)\s+" + // capture the resolver
			@"(?<cvtype>\S+)\s+" + // capture the file type
			@"(?<project>\S+)\s+" + // match the configured project name (which may not be not the subproject name)
			@"(?<instance>(\D+\W)?\d+)\s+" + // match the object instance number, with an optional DCM prefix
			@"((?<task>[^,\r]+),?)+\r\n" + // capture displayname of all associated tasks, with optional DCM prefixes
			@"(\t" + // each project usages is prefixed by tab
			@"(?<folder>\S+(?=\\\k<displayname>@))?" + // capture all configured project name followed by zero or more directories
			// positive lookahead for <folder>\<displayname>@[project][delim][project_version] (i.e., @project spec)
			@"[^\r]+\r\n)+" + // look for one or more lines, with the optional folder capture
			@")";

		/// <summary>
		///     Explicit interface implementation of the <see cref="IHistoryParser.Parse"/> 
		/// </summary>
		/// <remarks>
		///     Provided for completeness only.  Usable by interface references only.
		///     <see cref="Parse(string,string,DateTime)"/>
		///     should be used instead for all Synergy specific implementations.
		/// </remarks>
		/// <param name="history">The stream from a Synergy CLI finduser command's standard output.</param>
		/// <param name="from">The start date of the integration run.</param>
		/// <param name="to">Not used.</param>
		/// <returns>
		///     <c>null</c> by default.  Otherwise, an array of modifications, with default values
		///     for <see cref="Modification.Comment"/> and <see cref="Modification.ModifiedTime"/>.
		/// </returns>
		Modification[] IHistoryParser.Parse(TextReader history, DateTime from, DateTime to)
		{
			return Parse(String.Empty, history.ReadToEnd(), from);
		}

		/// <summary>
		///     Synergy specific implemtation of <see cref="IHistoryParser.Parse"/> 
		/// </summary>
		/// <remarks>
		///     Processes both the task query and the object query to fully populate each
		///     <see cref="Modification"/> object in the returned array.
		/// </remarks>
		/// <param name="newTasks">
		///     Standard output stream from the Synergy query command.
		/// </param>
		/// <param name="newObjects">
		///     Standard output stream from the Synergy finduse command.
		/// </param>
		/// <param name="from">
		///     The date since the last successful integration run.  Not used, since the finduse
		///     query includes this parameter.
		/// </param>
		/// <returns>
		///     <c>null</c> by default.
		///     If changes have occurred since the last integration attempt, an array containing
		///     each new modification is returned.
		/// </returns>
		public virtual Modification[] Parse(string newTasks, string newObjects, DateTime from)
		{
			ArrayList modifications = new ArrayList();
			Hashtable tasks = new Hashtable();

			// don't bother doing anything if no modified objects were found
			if (StringUtil.IsBlank(newObjects)) return new Modification[0];

			// optionally, parse the comments from each associated task
			if (! StringUtil.IsBlank(newTasks))
			{
				tasks = ParseTasks(newTasks);
			}

			// look for modifications in the output from the finduse command
			Regex grep = new Regex(ObjectFormat, RegexOptions.CultureInvariant);
			MatchCollection matches = grep.Matches(newObjects);

			// each match is a detected modification
			foreach (Match match in matches)
			{
				Modification modification = new Modification();
				modification.FolderName = match.Groups["folder"].Value;
				modification.FileName = match.Groups["displayname"].Value;
				modification.Type = match.Groups["cvtype"].Value;
				modification.EmailAddress = match.Groups["resolver"].Value;
				modification.UserName = match.Groups["resolver"].Value;

				/* normalize the folder path to resemble other SCM systems 
                     * vis a vis the "$/project/folder/file" format */
				if (modification.FolderName.Length > 0)
				{
					modification.FolderName = String.Concat("$/", modification.FolderName.Replace('\\', '/'));
				}

				// Retrieve the comment, if available
				CaptureCollection captures = match.Groups["task"].Captures;
				if (null != captures)
				{
					foreach (Capture capture in captures)
					{
						SynergyTaskInfo info = (SynergyTaskInfo) tasks[capture.Value];
						if (info == null)
						{
							modification.ChangeNumber = Int32.Parse(Regex.Match(capture.Value, @"\d+").Value);
						}
						else
						{
							modification.ChangeNumber = info.TaskNumber;
							modification.ModifiedTime = info.CompletionDate;
							if (null != info.TaskSynopsis)
								modification.Comment = info.TaskSynopsis;
						}
					}
				}
				modifications.Add(modification);
			}
			return (Modification[]) modifications.ToArray(typeof (Modification));
		}

		/// <summary>
		///     Creates a string collection of task numbers with their respective
		///     task synopsis, for use by <see cref="Parse"/>.
		/// </summary>
		/// <param name="comments">One or more lines of comments from a Synergy task query.</param>
		/// <returns>A non-null collection, with zero or more tasks with their comments.</returns>
		public Hashtable ParseTasks(string comments)
		{
			Hashtable retVal = new Hashtable();

			// substitute the parameter values in the formatted regex pattern
			Regex grep = new Regex(TaskFormat, RegexOptions.CultureInvariant);

			// look for modifications in the output from the finduse command
			MatchCollection matches = grep.Matches(comments);

			// add each task number/comment to the 
			foreach (Match match in matches)
			{
				try
				{
					SynergyTaskInfo info = new SynergyTaskInfo();
					info.DisplayName = match.Groups["displayname"].Value;
					info.TaskNumber = int.Parse(match.Groups["task_number"].Value, CultureInfo.InvariantCulture);

					if (null != match.Groups["completion_date"] && "<void>" != match.Groups["completion_date"].Value)
						info.CompletionDate = DateTime.Parse(match.Groups["completion_date"].Value, CultureInfo.InvariantCulture);

					if (null != match.Groups["resolver"])
						info.Resolver = match.Groups["resolver"].Value;

					if (null != match.Groups["task_synopsis"])
						info.TaskSynopsis = match.Groups["task_synopsis"].Value;

					retVal.Add(info.DisplayName, info);
				}
				catch (FormatException ex)
				{
					// Handle parse errors gracefully in a RELEASE build, so that 
					// the server does not hang when a parse error occurs
					Debug.Assert(false, "Failed to parse task " + match.Groups["displayname"].Value, ex.Message);
				}
			}
			return retVal;
		}

		/// <summary>
		///     Inner class that serves as a data structure to cache the information parsed
		///     from the task query.
		/// </summary>
		/// <remarks>
		///     Marked public so that is can be unit tested.
		/// </remarks>
		public class SynergyTaskInfo
		{
			public string DisplayName = String.Empty;
			public int TaskNumber = int.MinValue;
			public string TaskSynopsis = String.Empty;
			public DateTime CompletionDate = DateTime.MinValue;
			public string Resolver = String.Empty;
		}
	}
}