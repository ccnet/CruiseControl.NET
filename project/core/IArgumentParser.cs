namespace ThoughtWorks.CruiseControl.Core
{
	public interface IArgumentParser
	{
		bool IsRemote { get; }
		string Project { get; }
		string ConfigFile { get; }
		bool ShowHelp { get; }
	}
}