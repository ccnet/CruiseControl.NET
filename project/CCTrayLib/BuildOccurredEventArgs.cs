using System;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib
{
	public class BuildOccurredEventArgs : EventArgs
	{
		public readonly ProjectStatus ProjectStatus;
		public readonly BuildTransition BuildTransition;

		public BuildOccurredEventArgs (ProjectStatus newProjectStatus, BuildTransition transition)
		{
			ProjectStatus = newProjectStatus;
			BuildTransition = transition;
		}
	}

	public delegate void BuildOccurredEventHandler (object sauce, BuildOccurredEventArgs e);

}