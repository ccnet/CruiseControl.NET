using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Label
{
	[ReflectorType("remoteProjectLabeller")]
	public class RemoteProjectLabeller : ILabeller
	{
		private IRemotingService remotingService;

		public RemoteProjectLabeller() : this(new RemotingServiceAdapter())
		{}

		public RemoteProjectLabeller(IRemotingService service)
		{
			remotingService = service;
		}

		[ReflectorProperty("serverUri", Required=false)]
		public string ServerUri = RemoteCruiseServer.DefaultUri;

		[ReflectorProperty("project")]
		public string ProjectName;

		public string Generate(IIntegrationResult result)
		{
			ICruiseManager manager = (ICruiseManager) remotingService.Connect(typeof (ICruiseManager), ServerUri);

			ProjectStatus[] statuses = manager.GetProjectStatus();
			foreach (ProjectStatus status in statuses)
			{
				if (status.Name == ProjectName)
				{
					return status.LastSuccessfulBuildLabel;
				}
			}
			throw new NoSuchProjectException(ProjectName);
		}

		public void Run(IIntegrationResult result)
		{
			result.Label = Generate(result);
		}
	}
}