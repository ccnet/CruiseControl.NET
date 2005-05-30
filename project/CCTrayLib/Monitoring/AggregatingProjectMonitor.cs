using System;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class AggregatingProjectMonitor : IProjectMonitor
	{
		private readonly IProjectMonitor[] monitors;

		public AggregatingProjectMonitor( params IProjectMonitor[] monitors )
		{
			this.monitors = monitors;

		}

		public string ProjectName
		{
			get
			{
				// ok, this is a bit of a smell -- the class implements the interface but not all of 
				// the members make sense...  Probably should be two interfaces?
				throw new InvalidOperationException();
			}
		}
		public ProjectStatus ProjectStatus
		{
			get { throw new InvalidOperationException(); }
		}

		public void ForceBuild()
		{
			throw new NotImplementedException();
		}

		public event MonitorBuildOccurredEventHandler BuildOccurred
		{
			add
			{
				foreach (IProjectMonitor monitor in monitors)
				{
					monitor.BuildOccurred += value;
				}
			}
			remove
			{
				foreach (IProjectMonitor monitor in monitors)
				{
					monitor.BuildOccurred -= value;
				}
			}
		}

		public event MonitorPolledEventHandler Polled
		{
			add
			{
				foreach (IProjectMonitor monitor in monitors)
				{
					monitor.Polled += value;
				}
			}

			remove
			{
				foreach (IProjectMonitor monitor in monitors)
				{
					monitor.Polled -= value;
				}
			}
		}


		public void Poll()
		{
			foreach (IProjectMonitor monitor in monitors)
			{
				monitor.Poll();
			}
		}

		public ProjectState ProjectState
		{
			get
			{
				ProjectState worstState = ProjectState.Success;

				foreach (IProjectMonitor monitor in monitors)
				{
					if (monitor.ProjectState.IsWorseThan(worstState))
						worstState = monitor.ProjectState;
				}

				return worstState;
			}
		}

	}
}