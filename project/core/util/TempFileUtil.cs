
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	/// <summary>
	/// Utility class for managing temp files and folders.
	/// Uses your system's temp folder.
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
			if (overwrite)
			{
				DeleteTempDir(dirname);
			}
			return Directory.CreateDirectory(GetTempPath(dirname)).FullName;
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
			return Path.Combine(GetTempPath(dirname), filename);
		}

		public static bool DeleteTempDir(string dirname)
		{
			string tempDir = GetTempPath(dirname);
			if (Directory.Exists(tempDir))
			{
				try { Directory.Delete(tempDir, true); }
				catch (Exception e) { throw new IOException("Unable to delete directory: " + tempDir, e); }
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
			return File.Exists(GetTempFilePath(dirname, filename));
		}

		public static string CreateTempXmlFile(string dirname, string filename, string contents)
		{
			string path = Path.Combine(GetTempPath(dirname), filename);
			CreateTempXmlFile(path, contents);
			return path;
		}

		public static void CreateTempXmlFile(string path, string contents)
		{
			XmlTextWriter writer = new XmlTextWriter(path, Encoding.Unicode);
			writer.WriteRaw(contents);
			writer.Close();
		}

		public static string CreateTempFile(string tempDir, string filename)
		{
			return CreateTempFile(tempDir, filename, string.Empty);
		}

		public static string CreateTempFile(string tempDir, string filename, string content)
		{
			string path = CreateTempDir(tempDir, false);
			path = Path.Combine(path, filename);
			using (StreamWriter stream = File.CreateText(path))
			{
				stream.Write(content);
			}
			return path;
		}

		public static void CreateTempFiles(string tempDir, string[] filenames)
		{
			for (int i = 0; i < filenames.Length; i++)
			{
				CreateTempFile(tempDir, filenames[i]);
			}
		}

		public static void UpdateTempFile(string filename, string text)
		{
			using (StreamWriter writer = File.AppendText(filename))
			{
				writer.Write(text);
			}
		}

		public static void DeleteTempFile(string path)
		{
			if (path != null && File.Exists(path))
			{
				File.Delete(path);
			}
		}
	}
}