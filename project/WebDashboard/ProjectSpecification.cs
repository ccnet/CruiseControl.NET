using System;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	public class ProjectSpecification
	{
		public readonly string name;
		public readonly string logDirectory;
		public readonly string serverLogFilePath;
		public readonly int serverLogFileLines;

		public ProjectSpecification(string name, string logDirectory, string serverLogFilePath, int serverLogFileLines)
		{
			this.name = name;
			this.logDirectory = logDirectory;
			this.serverLogFilePath = serverLogFilePath;
			this.serverLogFileLines = serverLogFileLines;
		}
	}
}
