
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// 	
    /// </summary>
	public class SystemPath
	{
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public static SystemPath Temp = new SystemPath(Path.GetTempPath());
		
		private readonly string path;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemPath" /> class.	
        /// </summary>
        /// <param name="path">The path.</param>
        /// <remarks></remarks>
		public SystemPath(string path) : this(path, new ExecutionEnvironment())
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemPath" /> class.	
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="environment">The environment.</param>
        /// <remarks></remarks>
		public SystemPath(string path, IExecutionEnvironment environment)
		{
			this.path = Convert(path, environment);
			if (PathIsInvalid(path)) throw new ArgumentException("Path contains invalid characters: " + path, "path");
		}

        /// <summary>
        /// Combines the specified subpath.	
        /// </summary>
        /// <param name="subpath">The subpath.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public SystemPath Combine(string subpath)
		{
			return new SystemPath(Path.Combine(path, subpath));
		}

		private string Convert(string newpath, IExecutionEnvironment environment)
		{
			return Regex.Replace(newpath, @"[/\\]", environment.DirectorySeparator.ToString(CultureInfo.CurrentCulture));	
		}

        /// <summary>
        /// Existses this instance.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
		public bool Exists()
		{
			return File.Exists(path);
		}

        /// <summary>
        /// Toes the string.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
		public override string ToString()
		{
			return path;
		}

        /// <summary>
        /// Creates the directory.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
		public SystemPath CreateDirectory()
		{
			Directory.CreateDirectory(path);
			return this;
		}

        /// <summary>
        /// Deletes the directory.	
        /// </summary>
        /// <remarks></remarks>
		public void DeleteDirectory()
		{
			if (! Directory.Exists(path)) return;
			try { Directory.Delete(path, true); }
			catch (Exception e) { throw new IOException("Unable to delete directory: " + path, e); }
		}

        /// <summary>
        /// Deletes the file.	
        /// </summary>
        /// <remarks></remarks>
        public void DeleteFile()
        {
            if (!File.Exists(path)) return;
            
            try { File.Delete(path); }
            catch (Exception e) { throw new IOException("Unable to delete file : " + path, e); }
            
        }


        /// <summary>
        /// Uniques the temp path.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
		public static SystemPath UniqueTempPath()
		{
			return Temp.Combine(Guid.NewGuid().ToString());
		}

        /// <summary>
        /// Pathes the is invalid.	
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public static bool PathIsInvalid(string path)
		{
			return (-1 != path.IndexOfAny(Path.GetInvalidPathChars()));
		}

        /// <summary>
        /// Creates the sub directory.	
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public SystemPath CreateSubDirectory(string dir)
		{
			return Combine(dir).CreateDirectory();
		}

        /// <summary>
        /// Creates the empty file.	
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public SystemPath CreateEmptyFile(string file)
		{
			return Combine(file).CreateEmptyFile();
		}

        /// <summary>
        /// Creates the empty file.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
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

        /// <summary>
        /// Reads the text file.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
		public string ReadTextFile()
		{
			using (StreamReader textReader = File.OpenText(path))
			{
				return textReader.ReadToEnd();
			}
		}

        /// <summary>
        /// Creates the text file.	
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public SystemPath CreateTextFile(string filename, string content)
		{
			return Combine(filename).CreateTextFile(content);
		}

	}

    /// <summary>
    /// 	
    /// </summary>
	public class TempDirectory : SystemPath, IDisposable
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="TempDirectory" /> class.	
        /// </summary>
        /// <remarks></remarks>
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