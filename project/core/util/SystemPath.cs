using System;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class SystemPath
	{
		public static readonly SystemPath Temp = new SystemPath(Path.GetTempPath());
		
		private readonly string path;

		public SystemPath(string path)
		{
			if (PathIsInvalid(path)) throw new ArgumentException("Path contains invalid characters: " + path, "path");
			this.path = path;
		}

		public SystemPath Combine(string subpath)
		{
			return new SystemPath(Path.Combine(path, subpath));
		}

		public bool Exists()
		{
			return File.Exists(path);
		}

		public override string ToString()
		{
			return path;
		}
		
		public SystemPath CreateDirectory()
		{
			Directory.CreateDirectory(path);
			return this;
		}

		public void DeleteDirectory()
		{
			if (! Directory.Exists(path)) return;
			try { Directory.Delete(path, true); }
			catch (Exception e) { throw new IOException("Unable to delete directory: " + path, e); }
		}
		
		public static SystemPath UniqueTempPath()
		{
			return Temp.Combine(Guid.NewGuid().ToString());
		}

		public static bool PathIsInvalid(string path)
		{
			return (-1 != path.IndexOfAny(Path.InvalidPathChars));
		}

		public SystemPath CreateSubDirectory(string dir)
		{
			return Combine(dir).CreateDirectory();
		}

		public SystemPath CreateEmptyFile(string file)
		{
			return Combine(file).CreateEmptyFile();
		}
		
		public SystemPath CreateEmptyFile()
		{
			return CreateTextFile(string.Empty);
		}

		private SystemPath CreateTextFile(string content)
		{
			using (StreamWriter stream = File.CreateText(path))
			{
				stream.Write(content);
			}
			return this;
		}

		public SystemPath CreateTextFile(string filename, string content)
		{
			return Combine(filename).CreateTextFile(content);
		}

	}
	
	public class TempDirectory : SystemPath, IDisposable
	{
		public TempDirectory() : base(UniqueTempPath().ToString())
		{
			CreateDirectory();
		}

		void IDisposable.Dispose()
		{
			DeleteDirectory();
		}		
	}
}