namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce
{
	public interface IP4Initializer
	{
		void Initialize(P4 p4, string projectName, string workingDirectory);
	}
}
