using System;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public interface ICruiseServerFactory
	{
		ICruiseServer Create(bool remote, string configFile);
	}
}
