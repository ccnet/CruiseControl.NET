using System;

namespace tw.ccnet.core
{
	public interface ISourceControl
	{
		Modification[] GetModifications(DateTime from, DateTime to);
		void LabelSourceControl(string label, DateTime timeStamp);
	}
}
