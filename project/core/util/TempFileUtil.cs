using System;
using System.IO;
using System.Xml;

namespace tw.ccnet.core.util
{
	/// <summary>
	/// Utility class for managing temp files and folders.
	/// Uses your systems temp folder.
	/// </summary>
	public class TempFileUtil
	{
		public static string CreateTempDir(object obj)
		{
			return CreateTempDir(obj.GetType().FullName);
		}

		public static string CreateTempDir(string dirname)
		{
			return CreateTempDir(dirname, true);
		}

		public static string CreateTempDir(string dirname, bool overwrite)
		{
			string path = GetTempPath(dirname);
			if (overwrite && Directory.Exists(path))
			{
				DeleteTempDir(dirname);
			}
			return Directory.CreateDirectory(path).FullName;
		}

		public static string GetTempPath(string dirname)
		{
			return Path.Combine(Path.GetTempPath(), dirname);
		}

		public static string GetTempPath(object obj)
		{
			return GetTempPath(obj.GetType().FullName);
		}

		public static string GetTempFilePath(string dirname, string filename)
		{
			return Path.Combine(GetTempPath(dirname),filename);
		}

		public static bool DeleteTempDir(string dirname)
		{
			if (Directory.Exists(GetTempPath(dirname)))
			{
				Directory.Delete(GetTempPath(dirname), true); 
				return true;
			}
			else
				return false;
		}
		
		public static bool DeleteTempDir(object obj)
		{
			return DeleteTempDir(obj.GetType().FullName);
		}

		public static bool TempFileExists(string dirname, string filename)
		{
			return File.Exists(TempFileUtil.GetTempFilePath(dirname, filename));
		}

		public static string CreateTempXmlFile(string dirname, string filename, string contents)
		{
			string path = Path.Combine(GetTempPath(dirname),filename);
			XmlTextWriter writer = new XmlTextWriter(path, System.Text.Encoding.UTF8);
			writer.WriteRaw(contents);
			writer.Close();

			return path;
		}

		public static string CreateTempFile(string tempDir, string filename)
		{		
			string path = CreateTempDir(tempDir, false);				
			path = Path.Combine(path, filename);			
			File.CreateText(path).Close();
			return path;
		}

		public static string CreateTempFile(string tempDir, string filename, string content)
		{		
			string path = CreateTempDir(tempDir, false);				
			path = Path.Combine(path, filename);			
			StreamWriter stream = File.CreateText(path);
			stream.Write(content);
			stream.Close();
			return path;
		}

		public static void CreateTempFiles(string tempDir, string[] filenames)
		{
			for (int i = 0; i < filenames.Length; i++)
			{
				CreateTempFile(tempDir,filenames[i]);
			}
		}

		public static void UpdateTempFile(string filename, string text)
		{
			using (StreamWriter writer = File.AppendText(filename)) 
			{
				//System.Threading.Thread.Sleep(1000);
				writer.Write(text);
				//System.Threading.Thread.Sleep(1000);
			}
		}
	}
}
