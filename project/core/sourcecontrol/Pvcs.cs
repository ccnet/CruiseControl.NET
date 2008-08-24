using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("pvcs")]
	public class Pvcs : ProcessSourceControl
	{
		private const string DELETE_LABEL_TEMPLATE =
			@"run -y -xe""{0}"" -xo""{1}"" DeleteLabel -pr""{2}"" {3} {4} {5} {6}";

		private const string APPLY_LABEL_TEMPLATE =
			@"Vcs -q -xo""{0}"" -xe""{1}"" {2} -v""{3}"" ""@{4}""";

		private const string VLOG_INSTRUCTIONS_TEMPLATE =
			@"run -xe""{0}"" -xo""{1}"" -q vlog -pr""{2}"" {3} {4} -ds""{5}"" -de""{6}"" {7}";

		private const string VLOG_LABEL_INSTRUCTIONS_TEMPLATE =
			@"run -xe""{0}"" -xo""{1}"" -q vlog -pr""{2}"" {3} {4} -r""{5}"" {6}";

		private const string GET_INSTRUCTIONS_TEMPLATE =
			@"run -y -xe""{0}"" -xo""{1}"" -q Get -pr""{2}"" {3} {4} -sp""{5}"" {6} {7} ";

		private const string INDIVIDUAL_GET_REVISION_TEMPLATE =
			@"-r{0} ""{1}{2}\{3}""(""{4}"") ";

		private const string INDIVIDUAL_LABEL_REVISION_TEMPLATE =
			@"{0} ""{1}{2}\{3}"" ";

		private TimeZone currentTimeZone = TimeZone.CurrentTimeZone;
		private string baseLabelName = "";
		private Modification[] modifications = null;
		private Modification[] baseModifications = null;
		private string errorFile = "";
		private string logFile = "";
		private string tempFile = "";
		private string tempLabel = "";

		public Pvcs() : this(new PvcsHistoryParser(), new ProcessExecutor())
		{}

		public Pvcs(IHistoryParser parser, ProcessExecutor executor) : base(parser, executor)
		{}

		[ReflectorProperty("executable")]
		public string Executable = "pcli.exe";

		[ReflectorProperty("project")]
		public string Project;

		[ReflectorProperty("subproject")]
		public string Subproject;

		[ReflectorProperty("username", Required = false)]
		public string Username = "";

		[ReflectorProperty("password", Required = false)]
		public string Password = "";

		[ReflectorProperty("workingdirectory", Required = false)]
		public string WorkingDirectory = "";

		[ReflectorProperty("workspace", Required = false)]
		public string Workspace = "/@/RootWorkspace";

		[ReflectorProperty("recursive", Required = false)]
		public bool Recursive = true;

		[ReflectorProperty("labelOnSuccess", Required=false)]
		public bool LabelOnSuccess = false;

		[ReflectorProperty("autoGetSource", Required=false)]
		public bool AutoGetSource = true;

		[ReflectorProperty("manuallyAdjustForDaylightSavings", Required=false)]
		public bool ManuallyAdjustForDaylightSavings = false;

		//TODO: Support Promotion Groups -- [ReflectorProperty("isPromotionGroup", Required=false)]
		public bool IsPromotionGroup = false;

		[ReflectorProperty("labelOrPromotionName", Required=false)]
		public string LabelOrPromotionName
		{
			get { return baseLabelName; }
			set
			{
				baseLabelName = value;
				LabelOnSuccess = ! StringUtil.IsBlank(baseLabelName);
			}
		}

		public TimeZone CurrentTimeZone
		{
			set { currentTimeZone = value; }
		}

		public string ErrorFile
		{
			get { return errorFile = TempFileNameIfBlank(errorFile); }
		}

		public string LogFile
		{
			get { return logFile = TempFileNameIfBlank(logFile); }
		}

		public string TempFile
		{
			get { return tempFile = TempFileNameIfBlank(tempFile); }
		}

		public string LabelOrPromotionInput(string label)
		{
			return (label.Length == 0) ? "" : (IsPromotionGroup == false ? "-v" : "-g") + label;
		}

		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			baseModifications = null;

			using (TextReader reader = ExecuteVLog(from.StartTime, to.StartTime))
			{
				modifications = ParseModifications(reader, from.StartTime, to.StartTime);
			}
            base.FillIssueUrl(modifications);
			return modifications;
		}

		private string GetRecursiveValue()
		{
			return Recursive ? "-z" : string.Empty;
		}

		private TextReader ExecuteVLog(DateTime from, DateTime to)
		{
			// required due to DayLightSavings bug in PVCS 7.5.1
			if (ManuallyAdjustForDaylightSavings)
			{
				from = AdjustForDayLightSavingsBug(from);
				to = AdjustForDayLightSavingsBug(to);				
			}
			Execute(CreatePcliContentsForCreatingVLog(GetDateString(from), GetDateString(to)));
			return GetTextReader(LogFile);
		}

        /// <summary>
        /// Generate a userid-login option if needed.
        /// </summary>
        /// <param name="doubleQuotes">If true, wrap the entire option in double-quotes ('"').</param>
        /// <returns>The option, possibly <see cref="String.Empty"/>.</returns>
        /// <remarks>
        /// PVCS allows users to have no password, so we have three different choices (five if you count
        /// the double quotes):
        /// <list type="ul">
        /// <item>(nothing)</item>
        /// <item> -id"username" </item>
        /// <item> -id"username":"password" </item>
        /// <item> "-id"username"" </item>
        /// <item> "-id"username":"password"" </item>
        /// </list>
        /// </remarks>
		public string GetLogin(bool doubleQuotes)
		{
            if (Username.Length == 0)
                return string.Empty;
            string quotes = doubleQuotes ? "\"\"" : string.Empty;
            if (Password.Length == 0)
				return string.Format(" {1}-id\"{0}\"{1} ", Username, quotes);
            return string.Format(" {2}-id\"{0}\":\"{1}\"{2} ", Username, Password, quotes);
        }

		private void ExecuteNonPvcsFunction(string content)
		{
			string filename = Path.GetTempFileName();
			filename = filename.Substring(0, filename.Length - 3) + "cmd";
			try
			{
				CreatePVCSInstructionFile(filename, content);
				string arguments = string.Format(@"/c ""{0}""", filename);
				Execute(CreatePVCSProcessInfo("cmd.exe", arguments));
			}
			finally
			{
				if (File.Exists(filename))
					File.Delete(filename);
			}
		}

		private void Execute(string pcliContent)
		{
			Execute(CreatePVCSProcessInfo(Executable, pcliContent));
		}

		private void CreatePVCSInstructionFile(string filename, string content)
		{
			using (StreamWriter stream = File.CreateText(filename))
			{
				stream.Write(content);
			}
		}

		private ProcessInfo CreatePVCSProcessInfo(string executable, string arguments)
		{
			return new ProcessInfo(executable, arguments);
		}

		public string CreatePcliContentsForGet()
		{
			return string.Format(GET_INSTRUCTIONS_TEMPLATE, ErrorFile, LogFile, Project, GetLogin(false), GetRecursiveValue(), Workspace, LabelOrPromotionInput(tempLabel), Subproject);
		}

		public string CreatePcliContentsForCreatingVLog(string beforedate, string afterdate)
		{
			return string.Format(VLOG_INSTRUCTIONS_TEMPLATE, ErrorFile, LogFile, Project, GetLogin(false), GetRecursiveValue(), beforedate, afterdate, Subproject);
		}

		public string CreatePcliContentsForCreatingVlogByLabel(string label)
		{
			return string.Format(VLOG_LABEL_INSTRUCTIONS_TEMPLATE, ErrorFile, LogFile, Project, GetLogin(false), GetRecursiveValue(), label, Subproject);
		}

		public string CreatePcliContentsForDeletingLabel(string label)
		{
			return string.Format(DELETE_LABEL_TEMPLATE, ErrorFile, LogFile, Project, GetLogin(false), GetRecursiveValue(), LabelOrPromotionInput(label), Subproject);
		}

		public string CreatePcliContentsForLabeling(string label)
		{
			return string.Format(APPLY_LABEL_TEMPLATE, LogFile, ErrorFile, GetLogin(false), label, TempFile);
		}

		public string CreateIndividualLabelString(Modification mod, string label)
		{
			return string.Format(INDIVIDUAL_LABEL_REVISION_TEMPLATE, GetVersion(mod, label), GetUncPathPrefix(mod), mod.FolderName, mod.FileName);
		}

		public string CreateIndividualGetString(Modification mod, string fileLocation)
		{
			return string.Format(INDIVIDUAL_GET_REVISION_TEMPLATE, GetVersion(mod, ""), GetUncPathPrefix(mod), mod.FolderName, mod.FileName, fileLocation);
		}

		private string GetUncPathPrefix(Modification mod)
		{
			//Add extract \ for UNC paths
			return mod.FolderName.StartsWith("\\") ? @"\" : "";
		}

		private string GetVersion(Modification mod, string label)
		{
			return ((label == null || label.Length == 0) ? (mod.Version == null ? "1.0" : mod.Version) : (LabelOrPromotionInput(label)));
		}

		private TextReader GetTextReader(string path)
		{
			FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
			return new StreamReader(stream);
		}

		public DateTime AdjustForDayLightSavingsBug(DateTime date)
		{
			if (currentTimeZone.IsDaylightSavingTime(DateTime.Now))
			{
				TimeSpan anHour = new TimeSpan(1, 0, 0);
				return date.Subtract(anHour);
			}
			return date;
		}

		private string TempFileNameIfBlank(string file)
		{
			return StringUtil.IsBlank(file) ? Path.GetTempFileName() : file;
		}

		#region GetSource Logic

		public override void GetSource(IIntegrationResult result)
		{
            result.BuildProgressInformation.SignalStartRunTask("Getting source from Pvcs");

			// If no changes or no Auto Get Source, exit
			if (result.Modifications.Length < 1 || !AutoGetSource)
				return;

			// Commented out until TemporaryLabeller logic can be worked out.
			// Execute( CreatePcliContentsForGet() );

			//Ensure that the working directory and Modifications are not empty
			WorkingDirectory = (WorkingDirectory.Length < 3) ? result.WorkingDirectory : WorkingDirectory; // why checking for length < 3?

			//Ensure that the Modifications are set locally
			modifications = result.Modifications;

			if (LabelOnSuccess && LabelOrPromotionName.Length > 0)
				DetermineMaxRevisions(LabelOrPromotionName);

			// Write the revision to pull from Version Manager and close the file
			StringDictionary createFolders = new StringDictionary();
			using (TextWriter stream = File.CreateText(TempFile))
			{
				foreach (Modification mod in modifications)
				{
					string fileLoc = DetermineFileLocation(mod.FolderName);

					if (!createFolders.ContainsKey(fileLoc))
						createFolders.Add(fileLoc, fileLoc);

					stream.WriteLine(CreateIndividualGetString(mod, fileLoc));

				}
			}
			ExecutePvcsGet(createFolders);
		}

		private string DetermineFileLocation(string folderName)
		{
			// Determine the Local File Location for when the Version Manager Get logic
			// Gets the revision to be built
			string folder = Path.GetFullPath(folderName).ToLower();
			string archive = Project.ToLower() + @"\archives";
			if (folder.IndexOf(archive) < 0)
			{
				return WorkingDirectory + folder;
			}
			else
			{
				return folder.Replace(archive, WorkingDirectory);
			}
		}

		private void ExecutePvcsGet(StringDictionary folders)
		{
			StringBuilder content = new StringBuilder();
			content.Append("@echo off \r\necho Create all necessary folders first\r\n");
			foreach (string key in folders.Keys)
			{
				content.AppendFormat("IF NOT EXIST \"{0}\" mkdir \"{0}\\\" >NUL \r\n", folders[key]);
			}

			content.Append("\r\necho Get all of the files by version number and archive location \r\n");
			// Build the Get Command based on the revisions -- Executable.Substring(0,Executable.LastIndexOf("\\"))
			content.AppendFormat("\"{0}\" -W -Y -xo\"{1}\" -xe\"{2}\" @\"{3}\"\r{4}", GetExeFilename(), LogFile, ErrorFile, TempFile, Environment.NewLine);

			// Allow us to use same logic for Executing files
			ExecuteNonPvcsFunction(content.ToString());
		}

		public string GetExeFilename()
		{
			string dir = Path.GetDirectoryName(Executable);
			return Path.Combine(dir, "Get.exe");
		}

		#endregion

		#region LabelSourceControl Logic

		public override void LabelSourceControl(IIntegrationResult result)
		{
			// If no changes or LabelOnSuccess is false exit
			if (result.Modifications.Length < 1 || LabelOnSuccess == false || ! result.Succeeded) //|| _temporaryLabel.Length == 0
				return;

			modifications = result.Modifications;

			// Ensure the Label Or Promotion Name exist
			if (LabelOrPromotionName.Length > 0)
				LabelSourceControl("", LabelOrPromotionName, result.ProjectName);
			// This allows for the labeller concept to support absolute labelling
			if (result.Label != LabelOrPromotionName)
				LabelSourceControl(LabelOrPromotionName, result.Label, result.ProjectName);
		}

		private void LabelSourceControl(string oldLabel, string newLabel, string project)
		{
			if (oldLabel.Length > 0)
			{
				Log.Info("Copying PVCS Label " + oldLabel + " to " + newLabel);

				// Determine what revisions between old label and new label
				// Get assigned the new label
				DetermineMaxRevisions(oldLabel);
			}
			using (TextWriter stream = File.CreateText(TempFile))
			{
				foreach (Modification mod in modifications)
				{
					stream.WriteLine(CreateIndividualLabelString(mod, (oldLabel.Length > 0 ? newLabel : "")));
				}
			}
			Log.Info("Applying PVCS Label " + newLabel + " on Project " + project);
			// Allow us to use same logic for Executing files
			ExecuteNonPvcsFunction(CreatePcliContentsForLabeling(newLabel));
		}

		private void DetermineMaxRevisions(string oldLabel)
		{
			// Only Execute this one-time during this process
			// until the GetModifications is run again.
			if (baseModifications == null)
			{
				Log.Info("Determine Revisions based on Promotion Group/Label : " + oldLabel);
				// Execute new VLog Session to get revision for old Label
				Execute(CreatePcliContentsForCreatingVlogByLabel(oldLabel));

				using (TextReader reader = GetTextReader(LogFile))
				{
					baseModifications = historyParser.Parse(reader, DateTime.Now, DateTime.Now);
				}
			}

			ArrayList allMods = new ArrayList();
			foreach (Modification mod in baseModifications)
			{
				allMods.Add(mod);
			}
			foreach (Modification mod in modifications)
			{
				allMods.Add(mod);
			}

			// Only Modifications that need stamp should be generated
			modifications = PvcsHistoryParser.AnalyzeModifications(allMods);
		}

		#endregion

		public static string GetDateString(DateTime dateToConvert)
		{
			return GetDateString(dateToConvert, CultureInfo.CurrentCulture.DateTimeFormat);
		}

		public static string GetDateString(DateTime dateToConvert, DateTimeFormatInfo format)
		{
			string pattern = String.Format("{0} {1}", format.ShortDatePattern, format.ShortTimePattern);
			return dateToConvert.ToString(pattern, format);
		}

		public static DateTime GetDate(string dateToParse)
		{
			return GetDate(dateToParse, CultureInfo.CurrentCulture);
		}

		public static DateTime GetDate(string dateToParse, IFormatProvider format)
		{
			try
			{
				return DateTime.Parse(dateToParse, format);
			}
			catch (Exception ex)
			{
				throw new CruiseControlException("Unable to parse: " + dateToParse, ex);
			}
		}
	}
}