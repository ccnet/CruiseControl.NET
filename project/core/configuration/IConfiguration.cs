using System;

namespace ThoughtWorks.CruiseControl.Core
{
	public interface IConfiguration
	{
//		IProject GetProject(string name);
		string ReadXml();
		void WriteXml(string xml);
	}
}
