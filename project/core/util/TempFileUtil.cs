
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
        /// <summary>
        /// Creates the temp dir.	
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public static string CreateTempDir(object obj)
		{
			return CreateTempDir(obj.GetType().FullName);
		}

        /// <summary>
        /// Creates the temp dir.	
        /// </summary>
        /// <param name="dirname">The dirname.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public static string CreateTempDir(string dirname)
		{
			return CreateTempDir(dirname, true);
		}

        /// <summary>
        /// Creates the temp dir.	
        /// </summary>
        /// <param name="dirname">The dirname.</param>
        /// <param name="overwrite">The overwrite.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public static string CreateTempDir(string dirname, bool overwrite)
		{
			if (overwrite)
			{
				DeleteTempDir(dirname);
			}
			return Directory.CreateDirectory(GetTempPath(dirname)).FullName;
		}

        /// <summary>
        /// Gets the temp path.	
        /// </summary>
        /// <param name="dirname">The dirname.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public static string GetTempPath(string dirname)
		{
			return Path.Combine(Path.GetTempPath(), dirname);
		}

        /// <summary>
        /// Gets the temp path.	
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public static string GetTempPath(object obj)
		{
			return GetTempPath(obj.GetType().FullName);
		}

        /// <summary>
        /// Gets the temp file path.	
        /// </summary>
        /// <param name="dirname">The dirname.</param>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public static string GetTempFilePath(string dirname, string filename)
		{
			return Path.Combine(GetTempPath(dirname), filename);
		}

        /// <summary>
        /// Deletes the temp dir.	
        /// </summary>
        /// <param name="dirname">The dirname.</param>
        /// <returns></returns>
        /// <remarks></remarks>
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

        /// <summary>
        /// Deletes the temp dir.	
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public static bool DeleteTempDir(object obj)
		{
			return DeleteTempDir(obj.GetType().FullName);
		}

        /// <summary>
        /// Temps the file exists.	
        /// </summary>
        /// <param name="dirname">The dirname.</param>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public static bool TempFileExists(string dirname, string filename)
		{
			return File.Exists(GetTempFilePath(dirname, filename));
		}

        /// <summary>
        /// Creates the temp XML file.	
        /// </summary>
        /// <param name="dirname">The dirname.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="contents">The contents.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public static string CreateTempXmlFile(string dirname, string filename, string contents)
		{
			string path = Path.Combine(GetTempPath(dirname), filename);
			CreateTempXmlFile(path, contents);
			return path;
		}

        /// <summary>
        /// Creates the temp XML file.	
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="contents">The contents.</param>
        /// <remarks></remarks>
		public static void CreateTempXmlFile(string path, string contents)
		{
			XmlTextWriter writer = new XmlTextWriter(path, Encoding.Unicode);
			writer.WriteRaw(contents);
			writer.Close();
		}

        /// <summary>
        /// Creates the temp file.	
        /// </summary>
        /// <param name="tempDir">The temp dir.</param>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public static string CreateTempFile(string tempDir, string filename)
		{
			return CreateTempFile(tempDir, filename, string.Empty);
		}

        /// <summary>
        /// Creates the temp file.	
        /// </summary>
        /// <param name="tempDir">The temp dir.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        /// <remarks></remarks>
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

        /// <summary>
        /// Creates the temp files.	
        /// </summary>
        /// <param name="tempDir">The temp dir.</param>
        /// <param name="filenames">The filenames.</param>
        /// <remarks></remarks>
		public static void CreateTempFiles(string tempDir, string[] filenames)
		{
			for (int i = 0; i < filenames.Length; i++)
			{
				CreateTempFile(tempDir, filenames[i]);
			}
		}

        /// <summary>
        /// Updates the temp file.	
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="text">The text.</param>
        /// <remarks></remarks>
		public static void UpdateTempFile(string filename, string text)
		{
			using (StreamWriter writer = File.AppendText(filename))
			{
				writer.Write(text);
			}
		}

        /// <summary>
        /// Deletes the temp file.	
        /// </summary>
        /// <param name="path">The path.</param>
        /// <remarks></remarks>
		public static void DeleteTempFile(string path)
		{
			if (path != null && File.Exists(path))
			{
				File.Delete(path);
			}
		}
	}
}