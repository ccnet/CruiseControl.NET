using ThoughtWorks.CruiseControl.Core.Tasks;

namespace ThoughtWorks.CruiseControl.Core.tasks
{
	public interface ICoverage
	{
		void Instrument();
		void Report();
		
		NUnitTask NUnitTask
		{
			get;
			set;
		}

		string ReportName
		{
			get ;
			set	;
		}
	}
}
