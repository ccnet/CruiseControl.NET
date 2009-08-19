using System;
using System.IO;
using System.Text.RegularExpressions;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class SystemPath
	{
		public static readonly SystemPath Temp = new SystemPath(Path.GetTempPath());
		
		private readonly string path;

		public SystemPath(string path) : this(path, new ExecutionEnvironment())
		{
		}

		public SystemPath(string path, IExecutionEnvironment environment)
		{
			this.path = Convert(path, environment);
			if (PathIsInvalid(path)) throw new ArgumentException("Path contains invalid characters: " + path, "path");
		}

		public SystemPath Combine(string subpath)
		{
			return new SystemPath(Path.Combine(path, subpath));
		}

		private string Convert(string newpath, IExecutionEnvironment environment)
		{
			return Regex.Replace(newpath, @"[/\\]", environment.DirectorySeparator.ToString());	
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

        public void DeleteFile()
        {
            if (!File.Exists(path)) return;
            
            try { File.Delete(path); }
            catch (Exception e) { throw new IOException("Unable to delete file : " + path, e); }
            
        }


		public static SystemPath UniqueTempPath()
		{
			return Temp.Combine(Guid.NewGuid().ToString());
		}

		public static bool PathIsInvalid(string path)
		{
			return (-1 != path.IndexOfAny(Path.GetInvalidPathChars()));
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
		
		public string ReadTextFile()
		{
			using (StreamReader textReader = File.OpenText(path))
			{
				return textReader.ReadToEnd();
			}
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