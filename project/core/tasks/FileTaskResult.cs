using System;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	public class FileTaskResult : ITaskResult
	{
		private string _data;

		public FileTaskResult(string filename) : this(new FileInfo(filename))
		{}

		public FileTaskResult(FileInfo file)
		{
			ReadFileContents(file);
		}

		public string Data
		{
			get { return _data; }
		}

		private void ReadFileContents(FileInfo file)
		{
			try
			{
				using (StreamReader reader = file.OpenText())
				{
					_data = reader.ReadToEnd();
				}
			}
			catch (Exception ex)
			{
				throw new CruiseControlException("Unable to read the contents of the file: " + file.FullName, ex);
			}
		}
	}
}