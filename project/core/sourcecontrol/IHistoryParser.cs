using System;
using System.IO;

namespace tw.ccnet.core.sourcecontrol
{	
	public interface IHistoryParser
	{
		Modification[] Parse(TextReader history);
	}
}
