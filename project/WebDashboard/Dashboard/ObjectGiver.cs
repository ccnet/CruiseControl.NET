using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface ObjectGiver
	{
		object GiveObjectByType(Type type);
		object GiveObjectById(string id);
	}
}
