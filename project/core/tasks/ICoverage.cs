namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	public interface ICoverage
	{
		void Instrument();
		void Report();

		NUnitTask NUnitTask { get; set; }

		string ReportName { get; set; }
	}
}