using System;

namespace tw.ccnet.core
{
	public interface IConfiguration
	{
//		IProject GetProject(string name);
		string ReadXml();
		void WriteXml(string xml);
	}
}
