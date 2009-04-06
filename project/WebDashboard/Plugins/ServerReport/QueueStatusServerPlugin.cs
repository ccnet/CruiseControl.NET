using System.Collections;
using System.Collections.Generic;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerReport
{
	/// <summary>
	/// The status of the queues on a server.
	/// </summary>
    [ReflectorType("queueStatusServerPlugin")]
    public class QueueStatusServerPlugin : ICruiseAction, IPlugin
	{
		private readonly IFarmService farmService;
		private readonly IVelocityViewGenerator viewGenerator;

		public QueueStatusServerPlugin(IFarmService farmService, IVelocityViewGenerator viewGenerator)
		{
			this.farmService = farmService;
			this.viewGenerator = viewGenerator;
		}

		public IResponse Execute(ICruiseRequest request)
		{
			Hashtable velocityContext = new Hashtable();

            CruiseServerSnapshotListAndExceptions snapshot = farmService.GetCruiseServerSnapshotListAndExceptions(request.ServerSpecifier);
            List<QueueSnapshot> queues = new List<QueueSnapshot>();
            for (int snapshotLoop = 0; snapshotLoop < snapshot.Snapshots.Length; snapshotLoop++)
            {
                QueueSetSnapshot queueSnapshot = snapshot.Snapshots[snapshotLoop].QueueSetSnapshot;
                for (int queueLoop = 0; queueLoop < queueSnapshot.Queues.Count; queueLoop++)
                {
                    queues.Add(queueSnapshot.Queues[queueLoop]);
                }
            }
            velocityContext["queues"] = queues.ToArray();
			
			return viewGenerator.GenerateView(@"ServerQueueStatus.vm", velocityContext);
		}

		public string LinkDescription
		{
			get { return "View Queue Status"; }
		}

		public INamedAction[] NamedActions
		{
			get {  return new INamedAction[] { new ImmutableNamedAction("ViewServerQueues", this) }; }
		}

	}
}
