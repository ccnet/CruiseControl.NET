using System;

namespace ThoughtWorks.CruiseControl.Core
{
	public interface ISourceControl : ITask
	{
		// TODO: is it necessary to specify 'to' date -- just want changes after 'from' date
		Modification[] GetModifications(DateTime from, DateTime to);

		void LabelSourceControl(string label, DateTime timeStamp);
	}
}
