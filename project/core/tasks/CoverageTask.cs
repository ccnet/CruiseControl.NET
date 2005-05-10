using System;
using System.IO;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	// ToDo - is this used???
	[ReflectorType("coverage")]
	public class CoverageTask : ITask
	{
		private NUnitTask _nunit;
		private DevenvTask _builder;
		private ICoverage _instrument;
		private string _reportFileName = String.Empty;

		public CoverageTask()
		{}

		public CoverageTask(NUnitTask nunit, DevenvTask task, ICoverage instrument)
		{
			_nunit = nunit;
			_builder = task;
			_instrument = instrument;
		}

		[ReflectorProperty("nunit")]
		public NUnitTask NUnit
		{
			get { return _nunit; }
			set { _nunit = value; }
		}

		[ReflectorProperty("coverage", InstanceTypeKey="type")]
		public ICoverage Coverage
		{
			get { return _instrument; }
			set { _instrument = value; }
		}

		[ReflectorProperty("devenv")]
		public DevenvTask DevEnvTask
		{
			get { return _builder; }
			set { _builder = value; }
		}

		[ReflectorProperty("reportName")]
		public string ReportFileName
		{
			get { return _reportFileName; }
			set { _reportFileName = value; }
		}

		public void Run(IIntegrationResult result)
		{
			if (_reportFileName.Equals(String.Empty))
				_reportFileName = result.ProjectName;
			_instrument.NUnitTask = _nunit;
			_instrument.ReportName = _reportFileName;
			_instrument.Instrument();
			_builder.Run(result);
			_nunit.Run(result);
			_instrument.Report();
			result.AddTaskResult(new FileTaskResult(new FileInfo(_reportFileName)));
		}
	}
}