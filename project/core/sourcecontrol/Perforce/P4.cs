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
		internal readonly static string COMMAND_DATE_FORMAT = "yyyy/MM/dd:HH:mm:ss";

		private string _executable = "p4";
		private string _view;
		private string _client;
		private string _user;
		private string _port;
		private readonly ProcessExecutor processExecutor;
		private readonly IP4Initializer p4Initializer;
		private readonly IP4ProcessInfoCreator processInfoCreator;

		public P4() : this (new ProcessExecutor(), new ProcessP4Initializer(new ProcessExecutor()), new P4ConfigProcessInfoCreator()) { }

		public P4(ProcessExecutor processExecutor, IP4Initializer initializer, IP4ProcessInfoCreator processInfoCreator)
		{
			this.processExecutor = processExecutor;
			this.p4Initializer = initializer;
			this.processInfoCreator = processInfoCreator;
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
		public string Client
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

		[ReflectorProperty("applyLabel", Required = false)]
		public bool ApplyLabel = false;

		[ReflectorProperty("autoGetSource", Required = false)]
		public bool AutoGetSource = false;

		public string BuildCommandArguments(DateTime from, DateTime to)
		{
			StringBuilder args = new StringBuilder();
			args.Append("changes -s submitted ");
			args.Append(View);
			if (from==DateTime.MinValue) 
			{
				args.Append("@" + FormatDate(to));
			} 
			else 
			{
				args.Append(string.Format("@{0},@{1}", FormatDate(from), FormatDate(to)));
			}
			return args.ToString();
		}

		private string FormatDate(DateTime date)
		{
			return date.ToString(COMMAND_DATE_FORMAT, CultureInfo.InvariantCulture);
		}
		
		public virtual ProcessInfo CreateChangeListProcess(DateTime from, DateTime to) 
		{
			return processInfoCreator.CreateProcessInfo(this, BuildCommandArguments(from, to));
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

		public bool ShouldRun(IntegrationResult result)
		{
			return true;
		}

		public void Run(IntegrationResult result)
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
	{1}
", label, View);
			return processInfo;
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
			return result.StandardOutput.Trim() + "\r\n" + result.StandardError.Trim();
		}

		public void InitializeDirectory()
		{
			p4Initializer.Initialize(this);
		}
	}
}
