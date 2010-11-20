using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Config
{
	public interface IConfigurationFileSaver
	{
		void Save(IConfiguration configuration, FileInfo file);
	}
}
