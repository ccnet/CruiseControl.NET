using Exortech.NetReflector;
using System;
using tw.ccnet.remote;
using tw.ccnet.core.schedule;
using tw.ccnet.core.state;

namespace tw.ccnet.core
{
	public abstract class ProjectBase
	{
		private string _name;
		private ISchedule _schedule = new Schedule();
		private IStateManager _state = new IntegrationStateManager();

		[ReflectorProperty("name")]
		public virtual string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		[ReflectorProperty("schedule", InstanceTypeKey="type", Required=false)]
		public virtual ISchedule Schedule
		{
			get { return _schedule; }
			set { _schedule = value; }
		}

		[ReflectorProperty("state", InstanceTypeKey="type", Required=false)]
		public virtual IStateManager StateManager
		{
			get { return _state; }
			set { _state = value; }
		}
	}
}
