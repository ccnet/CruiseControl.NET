using System;

namespace tw.ccnet.core
{
	public interface ISourceControl : ITask
	{
		Modification[] GetModifications(DateTime from, DateTime to);
		void LabelSourceControl(string label, DateTime timeStamp);
	}
}
