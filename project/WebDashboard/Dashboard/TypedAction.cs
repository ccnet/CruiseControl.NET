using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class TypedAction
	{
		private readonly Type actionType;
		private readonly string actionName;

		public TypedAction(string actionName, Type actionType)
		{
			this.actionName = actionName;
			this.actionType = actionType;
		}

		public Type ActionType
		{
			get { return actionType; }
		}

		public string ActionName
		{
			get { return actionName; }
		}
	}
}
