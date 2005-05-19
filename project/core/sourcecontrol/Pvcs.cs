using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("pvcs")]
	public class Pvcs : ProcessSourceControl
	{
		public const string COMMAND = "run -s";

		private const string DELETE_LABEL_TEMPLATE =
			@"run -y -xe""{0}"" -xo""{1}"" DeleteLabel -pr""{2}"" {3} {4} {5} {7}";

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

		private TimeZone _currentTimeZone = TimeZone.CurrentTimeZone;
		private string _baseLabelName = "";
		private Modification[] _modifications = null;
		private Modification[] _baseModifications = null;
		private string _errorFile = "";
		private string _logFile = "";
		private string _tempFile = "";
		private string _tempLabel = "";

		public Pvcs() : this(new PvcsHistoryParser(), new ProcessExecutor())
		{}

		public Pvcs(IHistoryParser parser, ProcessExecutor executor) : base(parser, executor)
		{}

		[ReflectorProperty("executable")]
		public string Executable = "pcli";

		[ReflectorProperty("arguments", Required=false)]
		public string Arguments = COMMAND;

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

		//TODO: Support Promotion Groups -- [ReflectorProperty("isPromotionGroup", Required=false)]
		public bool IsPromotionGroup = false;

		[ReflectorProperty("labelOrPromotionName", Required=false)]
		public string LabelOrPromotionName
		{
			get { return _baseLabelName; }
			set
			{
				_baseLabelName = value;
				LabelOnSuccess = ! StringUtil.IsBlank(_baseLabelName);
			}
		}

		public TimeZone CurrentTimeZone
		{
			set { _currentTimeZone = value; }
		}

		public string ErrorFile
		{
			get { return _errorFile = TempFileNameIfBlank(_errorFile); }
		}

		public string LogFile
		{
			get { return _logFile = TempFileNameIfBlank(_logFile); }
		}

		public string TempFile
		{
			get { return _tempFile = TempFileNameIfBlank(_tempFile); }
		}

		public string LabelOrPromotionInput(string label)
		{
			return (label.Length == 0) ? "" : (this.IsPromotionGroup == false ? "-v" : "-g") + label;
		}

		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			this._baseModifications = null;

			using (TextReader reader = ExecuteVLog(from.StartTime, to.StartTime))
			{
				_modifications = base.ParseModifications(reader, from.StartTime, to.StartTime);
			}
			return _modifications;
		}

		private string GetRecursiveValue()
		{
			return Recursive == true ? "-z" : string.Empty;
		}

		private TextReader ExecuteVLog(DateTime from, DateTime to)
		{
			// required due to DayLightSavings bug in PVCS 7.5.1
			from = AdjustForDayLightSavingsBug(from);
			to = AdjustForDayLightSavingsBug(to);
			Execute(CreatePcliContentsForCreatingVLog(GetDateString(from), GetDateString(to)));
			return GetTextReader(LogFile);
		}

		public string GetLogin(bool doubleQuotes)
		{
			//Two conditions need to be checked: We need to make sure either the username is empty
			//but the password is not or that both are not empty.  This allows us to create the id
			//logic needed by PVCS
			if (Password.Length > 0)
			{
				string quotes = doubleQuotes ? "\"\"" : string.Empty;
				return string.Format(" {2}-id\"{0}\":\"{1}\"{2} ", Username, Password, quotes);
			}
			else
				return string.Empty;
		}

		private void ExecuteNonPvcsFunction(string content)
		{
			string filename = Path.GetTempFileName();
			filename = filename.Substring(0, filename.Length - 3) + "cmd";
			try
			{
				CreatePVCSInstructionFile(filename, content);
				Execute(CreatePVCSProcessInfo("cmd.exe", "/c ", filename));
			}
			finally
			{
				if (File.Exists(filename))
					File.Delete(filename);
			}
		}

		private void Execute(string pcliContent)
		{
			Execute(CreatePVCSProcessInfo(Executable, pcliContent, ""));
		}

		private void CreatePVCSInstructionFile(string filename, string content)
		{
			using (StreamWriter stream = File.CreateText(filename))
			{
				stream.Write(content);
			}
		}

		public ProcessInfo CreatePVCSProcessInfo(string executable, string arguments, string filename)
		{
			return new ProcessInfo(executable, arguments + filename);
		}

		public string CreatePcliContentsForGet()
		{
			return string.Format(GET_INSTRUCTIONS_TEMPLATE, ErrorFile, LogFile, Project, GetLogin(false), GetRecursiveValue(), Workspace, this.LabelOrPromotionInput(_tempLabel), this.Subproject);
		}

		public string CreatePcliContentsForCreatingVLog(string beforedate, string afterdate)
		{
			return string.Format(VLOG_INSTRUCTIONS_TEMPLATE, ErrorFile, LogFile, Project, GetLogin(false), GetRecursiveValue(), beforedate, afterdate, Subproject);
		}

		public string CreatePcliContentsForCreatingVlogByLabel(string label)
		{
			return string.Format(VLOG_LABEL_INSTRUCTIONS_TEMPLATE, ErrorFile, this.LogFile, this.Project, this.GetLogin(false), this.GetRecursiveValue(), label, this.Subproject);
		}

		public string CreatePcliContentsForDeletingLabel(string label)
		{
			return string.Format(DELETE_LABEL_TEMPLATE, ErrorFile, LogFile, this.Project, GetLogin(false), GetRecursiveValue(), this.LabelOrPromotionInput(label), this.Subproject);
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
			if (_currentTimeZone.IsDaylightSavingTime(DateTime.Now))
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
			// If no changes or no Auto Get Source, exit
			if (result.Modifications.Length < 1 || !AutoGetSource)
				return;

			// Commented out until TemporaryLabeller logic can be worked out.
			// Execute( this.CreatePcliContentsForGet() );

			//Ensure that the working directory and Modifications are not empty
			WorkingDirectory = (WorkingDirectory.Length < 3) ? result.WorkingDirectory : WorkingDirectory; // why checking for length < 3?

			//Ensure that the Modifications are set locally
			_modifications = result.Modifications;

			if (this.LabelOnSuccess && this.LabelOrPromotionName.Length > 0)
				this.DetermineMaxRevisions(this.LabelOrPromotionName);

			// Write the revision to pull from Version Manager and close the file
			StringDictionary createFolders = new StringDictionary();
			using (TextWriter stream = File.CreateText(TempFile))
			{
				foreach (Modification mod in this._modifications)
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

		protected void ExecutePvcsGet(StringDictionary folders)
		{
			StringBuilder content = new StringBuilder();
			content.Append("@echo off \r\necho Create all necessary folders first\r\n");
			foreach (string key in folders.Keys)
			{
				content.AppendFormat("IF NOT EXIST \"{0}\" mkdir \"{0}\\\" >NUL \r\n", folders[key]);
			}

			content.Append("\r\necho Get all of the files by version number and archive location \r\n");
			// Build the Get Command based on the revisions -- Executable.Substring(0,Executable.LastIndexOf("\\"))
			string dir = Path.GetDirectoryName(Executable);
			content.AppendFormat("\"{0}\\Get.exe\" -W -Y -xo\"{1}\" -xe\"{2}\" @\"{3}\"\r{4}", dir, LogFile, ErrorFile, TempFile, Environment.NewLine);

			// Allow us to use same logic for Executing files
			ExecuteNonPvcsFunction(content.ToString());
		}

		#endregion

		#region LabelSourceControl Logic

		public override void LabelSourceControl(IIntegrationResult result)
		{
			// If no changes or LabelOnSuccess is false exit
			if (result.Modifications.Length < 1 || LabelOnSuccess == false || ! result.Succeeded) //|| _temporaryLabel.Length == 0
				return;

			_modifications = result.Modifications;

			// Ensure the Label Or Promotion Name exist
			if (this.LabelOrPromotionName.Length > 0)
				LabelSourceControl("", LabelOrPromotionName);
			// This allows for the labeller concept to support absolute labelling
			if (result.Label != this.LabelOrPromotionName)
				LabelSourceControl(LabelOrPromotionName, result.Label);
		}

		private void LabelSourceControl(string oldLabel, string newLabel)
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
				foreach (Modification mod in _modifications)
				{
					stream.WriteLine(CreateIndividualLabelString(mod, (oldLabel.Length > 0 ? newLabel : "")));
				}
			}
			Log.Info("Applying PVCS Label " + newLabel + " on Project " + Thread.CurrentThread.Name);
			// Allow us to use same logic for Executing files
			ExecuteNonPvcsFunction(CreatePcliContentsForLabeling(newLabel));
		}

		private void DetermineMaxRevisions(string oldLabel)
		{
			// Only Execute this one-time during this process
			// until the GetModifications is run again.
			if (_baseModifications == null)
			{
				Log.Info("Determine Revisions based on Promotion Group/Label : " + oldLabel);
				// Execute new VLog Session to get revision for old Label
				Execute(CreatePcliContentsForCreatingVlogByLabel(oldLabel));

				using (TextReader reader = GetTextReader(LogFile))
				{
					_baseModifications = this._historyParser.Parse(reader, DateTime.Now, DateTime.Now);
				}
			}

			ArrayList allMods = new ArrayList();

			foreach (Modification mod in this._baseModifications)
			{
				allMods.Add(mod);
			}
			foreach (Modification mod in this._modifications)
			{
				allMods.Add(mod);
			}

			// Only Modifications that need stamp should be generated
			_modifications = PvcsHistoryParser.AnalyzeModifications(allMods);

			// Cleanup
			allMods.Clear();
			allMods = null;
		}

		#endregion

		#region Date Functions

		public static string GetDateString(DateTime dateToConvert)
		{
			return GetDateString(dateToConvert, CultureInfo.CurrentCulture.DateTimeFormat);
		}

		public static string GetDateString(DateTime dateToConvert, DateTimeFormatInfo format)
		{
			string pattern = String.Format("{0} {1}", format.ShortDatePattern, format.ShortTimePattern);
			return dateToConvert.ToString(pattern);
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

		#endregion

		#region ITemporaryLabeller Members - Not Implemented At this Time 

		public void CreateDateSpecificTemporaryLabel(DateTime dt)
		{
			// this was created public for testing
			_tempLabel = Thread.CurrentThread.Name + "_" + Convert.ToString(dt.Ticks);
		}

		public void CreateTemporaryLabel()
		{
			CreateDateSpecificTemporaryLabel(DateTime.Now);

			this.LabelSourceControl("", _tempLabel);

			// Copy the revisions of the Label / Promotion Group into the temporary label
			if (this.LabelOnSuccess && this.LabelOrPromotionName.Length > 0)
				this.LabelSourceControl(this.LabelOrPromotionName, _tempLabel);
		}

		public void DeleteTemporaryLabel()
		{
			Execute(this.CreatePcliContentsForDeletingLabel(_tempLabel));
		}

		#endregion
	}
}