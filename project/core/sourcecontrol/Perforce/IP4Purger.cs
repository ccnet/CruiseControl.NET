namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce
{
	public interface IP4Purger
	{
		void Purge(P4 p4, string workingDirectory);
	}
}
