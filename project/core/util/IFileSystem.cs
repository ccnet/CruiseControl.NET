
namespace ThoughtWorks.CruiseControl.Core.Util
{
	public interface IFileSystem
	{
		void Copy(string sourcePath, string destPath);
		void Save(string file, string content);
	}
}
