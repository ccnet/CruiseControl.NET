using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Config
{
	public interface IConfigurationFileLoader
	{
		IConfiguration Load(FileInfo file);
	}
}