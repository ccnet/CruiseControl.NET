using System;

namespace ThoughtWorks.CruiseControl.Core.tasks
{
	/// <summary>
	/// Summary description for IInstrumenter.
	/// </summary>
	public interface ICoverage
	{
		void Instrument();
		void Report();
		string ReportName
		{
			get ;
			set	;
		}
	}
}
