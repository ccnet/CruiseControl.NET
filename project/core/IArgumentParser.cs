namespace ThoughtWorks.CruiseControl.Core
{
	public interface IArgumentParser
	{
		bool UseRemoting { get; }
		string Project { get; }
		string ConfigFile { get; }
		bool ShowHelp { get; }
	}
}