using System;

namespace ThoughtWorks.CruiseControl.Core
{
	public interface ISourceControl : ITask
	{
		Modification[] GetModifications(DateTime from, DateTime to);
		void LabelSourceControl(string label, DateTime timeStamp);
	}
}
