namespace ThoughtWorks.CruiseControl.Console
{
	public interface IArgumentParser
	{
		bool IsRemote { get; }
		string Project { get; }
		string ConfigFile { get; }
		bool ShowHelp { get; }
	}
}