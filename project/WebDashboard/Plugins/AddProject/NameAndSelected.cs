namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.AddProject
{
	public class NameAndSelected
	{
		private readonly string name;
		private bool selected = false;

		public NameAndSelected(string name)
		{
			this.name = name;
		}

		public string Name
		{
			get { return name; }
		}

		public bool Selected
		{
			get { return selected; }
			set { selected = value; }
		}
	}
}

