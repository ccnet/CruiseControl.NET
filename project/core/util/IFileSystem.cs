using System.IO;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public interface IFileSystem
	{
		void Copy(string sourcePath, string destPath);
		void Save(string file, string content);
        void AtomicSave(string file, string content);
        void AtomicSave(string file, string content, Encoding encoding);
        TextReader Load(string file);
		bool FileExists(string file);
		bool DirectoryExists(string folder);
	}
}