using System;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{	
	public interface IHistoryParser
	{
		Modification[] Parse(TextReader history);
	}
}
