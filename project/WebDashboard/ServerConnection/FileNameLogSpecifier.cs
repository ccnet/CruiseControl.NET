namespace ThoughtWorks.CruiseControl.WebDashboard.ServerConnection
{
	public class FileNameLogSpecifier : ILogSpecifier
	{
		private readonly string filename;

		public FileNameLogSpecifier(string filename)
		{
			this.filename = filename;
		}

		public string Filename
		{
			get { return filename; }
		}
	}
}
