using System;

namespace ObjectWizard
{
	public interface ObjectGiver
	{
		object GiveObjectByType(Type type);
		object GiveObjectById(string id);
	}
}
