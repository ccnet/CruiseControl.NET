using System;

namespace tw.ccnet.core.sourcecontrol
{
	//TODO: convert to interface
	public abstract class SourceControl
	{
		public abstract Modification[] GetModifications(DateTime from, DateTime to);
	}
}