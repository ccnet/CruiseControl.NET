using System.Collections;
using Exortech.NetReflector;
using System;
using System.IO;
using System.Globalization;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce
{
	[ReflectorType("p4")]
	public class P4 : ISourceControl
	{
		private readonly IP4Purger p4Purger;
		internal readonly static string COMMAND_DATE_FORMAT = "yyyy/MM/dd:HH:mm:ss";

		private string _executable = "p4";
		private string _view;
		private string _client;
		private string _user;
		private string _port;
		private string _workingDirectory;
		private readonly ProcessExecutor processExecutor;
		private readonly IP4Initializer p4Initializer;
		private readonly IP4ProcessInfoCreator processInfoCreator;

		public P4()
		{
			processExecutor = new ProcessExecutor();
			processInfoCreator = new P4ConfigProcessInfoCreator();
			p4Initializer = new ProcessP4Initializer(processExecutor, processInfoCreator);
			p4Purger = new ProcessP4Purger(processExecutor, processInfoCreator);
		}

		public P4(ProcessExecutor processExecutor, IP4Initializer initializer, IP4Purger p4Purger, IP4ProcessInfoCreator processInfoCreator)
		{
			this.processExecutor = processExecutor;
			this.p4Initializer = initializer;
			this.processInfoCreator = processInfoCreator;
			this.p4Purger = p4Purger;
		}

		[ReflectorProperty("executable", Required=false)]
		public string Executable
		{
			get{ return _executable;}
			set{ _executable = value;}
		}

		[ReflectorProperty("view")]
		public string View
		{
			get{ return _view;}
			set{ _view = value;}
		}

		[ReflectorProperty("client", Required=false)]
		public virtual string Client
		{
			get{ return _client;}
			set{ _client = value;}
		}

		[ReflectorProperty("user", Required=false)]
		public string User
		{
			get{ return _user;}
			set{ _user = value;}
		}

		[ReflectorProperty("port", Required=false)]
		public string Port
		{
			get{ return _port;}
			set{ _port = value;}
		}

		[ReflectorProperty("workingDirectory", Required=false)]
		public string WorkingDirectory
		{
			get{ return _workingDirectory;}
			set{ _workingDirectory = value;}
		}

		[ReflectorProperty("applyLabel", Required = false)]
		public bool ApplyLabel = false;

		[ReflectorProperty("autoGetSource", Required = false)]
		public bool AutoGetSource = false;

		public string BuildModificationsCommandArguments(DateTime from, DateTime to)
		{
			return string.Format("changes -s submitted {0}", GenerateRevisionsForView(from, to));
		}

		private string GenerateRevisionsForView(DateTime from, DateTime to)
		{
			StringBuilder args = new StringBuilder();
			foreach (string viewline in View.Split(','))
			{
				args.Append(viewline);
				if (from==DateTime.MinValue) 
				{
					args.Append("@" + FormatDate(to));
				} 
				else 
				{
					args.Append(string.Format("@{0},@{1} ", FormatDate(from), FormatDate(to)));
				}
			}
			return args.ToString();
		}

		private string FormatDate(DateTime date)
		{
			return date.ToString(COMMAND_DATE_FORMAT, CultureInfo.InvariantCulture);
		}
		
		public virtual ProcessInfo CreateChangeListProcess(DateTime from, DateTime to) 
		{
			return processInfoCreator.CreateProcessInfo(this, BuildModificationsCommandArguments(from, to));
		}

		public virtual ProcessInfo CreateDescribeProcess(string changes)
		{
			if (changes.Length == 0)
				throw new Exception("Empty changes list found - this should not happen");

			foreach (char c in changes)
			{
				if (! (Char.IsDigit(c) || c == ' ') )
					throw new CruiseControlException("Invalid changes list encountered");
			}

			return processInfoCreator.CreateProcessInfo(this, "describe -s " + changes);
		}

		public Modification[] GetModifications(DateTime from, DateTime to) 
		{
			P4HistoryParser parser = new P4HistoryParser();
			ProcessInfo process = CreateChangeListProcess(from, to);
			string processResult = Execute(process);
			String changes = parser.ParseChanges(processResult);
			if (changes.Length == 0)
			{
				return new Modification[0];
			}
			else
			{
				process = CreateDescribeProcess(changes);
				return parser.Parse(new StringReader(Execute(process)), from, to);
			}
		}

		public bool ShouldRun(IntegrationResult result, IProject project)
		{
			return true;
		}

		public void Run(IntegrationResult result, IProject project)
		{
			result.Modifications = GetModifications(result.LastModificationDate, DateTime.Now);
		}

		/// <summary>
		/// Labelling in Perforce requires 2 activities. First you create a 'label specification' which is the name of the label, and what
		/// part of the source repository it is associated with. Secondly you actually populate the label with files and associated
		/// revisions by performing a 'label sync'. We take the versioned file set as being the versions that are currently 
		/// checked out on the client (In theory this could be refined by using the timeStamp, but it would be better
		/// to wait until CCNet has proper support for atomic-commit change groups, and use that instead)
		/// </summary>
		public void LabelSourceControl(string label, DateTime timeStamp) 
		{
			if (ApplyLabel)
			{
				if (label == null || label.Length == 0)
					throw new ApplicationException("Internal Exception - Invalid (null or empty) label passed");

				try
				{
					int.Parse(label);
					throw new CruiseControlException("Perforce cannot handle purely numeric labels - you must use a label prefix for your project");
				}
				catch (FormatException) { }
				ProcessInfo process = CreateLabelSpecificationProcess(label);

				string processOutput = Execute(process);
				if (containsErrors(processOutput))
				{
					Log.Error(string.Format("Perforce labelling failed:\r\n\t process was : {0} \r\n\t output from process was: {1}", process.ToString(),  processOutput));
					return;
				}

				process = CreateLabelSyncProcess(label);
				processOutput = Execute(process);
				if (containsErrors(processOutput))
				{
					Log.Error(string.Format("Perforce labelling failed:\r\n\t process was : {0} \r\n\t output from process was: {1}", process.ToString(),  processOutput));
					return;
				}
			}
		}

		private bool containsErrors(string processOutput)
		{
			return processOutput.IndexOf("error:") > -1;
		}

		private ProcessInfo CreateLabelSpecificationProcess(string label)
		{
			ProcessInfo processInfo = processInfoCreator.CreateProcessInfo(this, "label -i");
			processInfo.StandardInputContent = string.Format(@"Label:	{0}

Description:
	Created by CCNet

Options:	unlocked

View:
{1}", label, ViewForSpecificationsAsNewlineSeparatedString);
			return processInfo;
		}

		public virtual string[] ViewForSpecifications
		{
			get
			{
				ArrayList viewLineList = new ArrayList();
				foreach (string viewLine in View.Split(','))
				{
					viewLineList.Add(viewLine);
				}
				return (string[]) viewLineList.ToArray(typeof (string));
			}
		}

		private string ViewForSpecificationsAsNewlineSeparatedString
		{
			get
			{
				StringBuilder builder = new StringBuilder();
				foreach (string viewLine in ViewForSpecifications)
				{
					builder.Append(" ");
					builder.Append(viewLine);
					builder.Append(Environment.NewLine);
				}
				return builder.ToString();
			}
		}

		private ProcessInfo CreateLabelSyncProcess(string label)
		{
			return processInfoCreator.CreateProcessInfo(this, "labelsync -l " + label);
		}

		public void GetSource(IntegrationResult result)
		{
			if (AutoGetSource)
			{
				ProcessInfo info = processInfoCreator.CreateProcessInfo(this, "sync");
				Log.Info(string.Format("Getting source from Perforce: {0} {1}", info.FileName, info.Arguments));
				Execute(info);
			}
		}

		protected virtual string Execute(ProcessInfo p)
		{
			Log.Debug("Perforce plugin - running:" + p.ToString());
			ProcessResult result = processExecutor.Execute(p);
			return result.StandardOutput.Trim() + Environment.NewLine + result.StandardError.Trim();
		}

		public void Initialize(IProject project)
		{
			if (_workingDirectory == null || _workingDirectory == string.Empty)
			{
				p4Initializer.Initialize(this, project.Name, project.WorkingDirectory);	
			}
			else
			{
				p4Initializer.Initialize(this, project.Name, _workingDirectory);
			}
		}

		public void Purge(IProject project)
		{
			if (_workingDirectory == null || _workingDirectory == string.Empty)
			{
				p4Purger.Purge(this, project.WorkingDirectory);	
			}
			else
			{
				p4Purger.Purge(this, _workingDirectory);
			}
		}
	}
}
