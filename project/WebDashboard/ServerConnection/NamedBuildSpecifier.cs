namespace ThoughtWorks.CruiseControl.WebDashboard.ServerConnection
{
	public class NamedBuildSpecifier : IBuildSpecifier
	{
		private readonly string filename;

		public NamedBuildSpecifier(string filename)
		{
			this.filename = filename;
		}

		public string Filename
		{
			get { return filename; }
		}
	}
}
