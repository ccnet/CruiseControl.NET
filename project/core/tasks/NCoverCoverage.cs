using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.tasks;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	[ReflectorType("ncover")]
	public class NCoverCoverage : ICoverage
	{
		private ProcessExecutor _exec;
		private string _srcFilePath;
		private string _ncoverBinPath;
		private string _reportName= String.Empty;
		private string _ncoverReportBinPath;
		private string _nrecoverBinPath;

		public NCoverCoverage() :this(new ProcessExecutor()){}
		public NCoverCoverage(ProcessExecutor executor)
		{
			_exec = executor;

		}

		public virtual void Instrument()
		{
			string args = @"/report-name:" + _reportName + @" /recurse:src\" + _srcFilePath;
			ProcessInfo pi = new ProcessInfo(_ncoverBinPath, args);
			_exec.Execute(pi);
		}

		public virtual void Report()
		{
			try
			{
				GenerateReport();
			}
			finally
			{
				RemoveInstumentation();	
			}
		}

		private void RemoveInstumentation()
		{
			string args = "/report-name:\"" + _reportName + "\"";
			ProcessInfo pi = new ProcessInfo(_nrecoverBinPath, args);
			_exec.Execute(pi);
		}

		private void GenerateReport()
		{
			string args = "/actual:\"target/actual.xml\" /report-name:" + "\"" + _reportName + "\"";
			ProcessInfo pi = new ProcessInfo(_ncoverReportBinPath, args);
			_exec.Execute(pi);
		}

		[ReflectorProperty("source")] 
		public string SrcFilePath
		{
			set { _srcFilePath = value; }
			get { return _srcFilePath; }
		}

		[ReflectorProperty("report", Required = false)] 
		public string ReportName
		{
			get { return _reportName; }
			set
			{
				if(value != String.Empty) 
					_reportName = value;
			}
		}

		[ReflectorProperty("ncoverBin")] 
		public string NCoverInstrumentBinPath
		{
			set { _ncoverBinPath = value; }
			get { return _ncoverBinPath; }
		}
		
		[ReflectorProperty("ncoverReportBin")] 
		public string NCoverReportBinPath
		{
			get { return _ncoverReportBinPath; }
			set { _ncoverReportBinPath = value; }
		}
		
		[ReflectorProperty("nrecoverBin")] 
		public string NRecoverPath
		{
			get { return _nrecoverBinPath; }
			set { _nrecoverBinPath = value; }
		}
	}
}