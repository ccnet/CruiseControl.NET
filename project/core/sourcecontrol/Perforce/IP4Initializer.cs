namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce
{
	public interface IP4Initializer
	{
		void Initialize(string executable, string view, string client, string user, string port);
	}
}
