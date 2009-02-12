namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	/// <summary>
	/// A summary of the project state as interesting to cctray
	/// </summary>
	public class ProjectState
	{
		/// <summary>
		///  The build is OK
		/// </summary>
		public static readonly ProjectState Success = new ProjectState("Success", 1, 0);

		/// <summary>
		///  The build is broken
		/// </summary>
		public static readonly ProjectState Broken = new ProjectState("Broken", 2, 100);

		/// <summary>
		///  The build is broken
		/// </summary>
		public static readonly ProjectState BrokenAndBuilding = new ProjectState("Broken and building", 4, 30);

		/// <summary>
		/// The build is building
		/// </summary>
		public static readonly ProjectState Building = new ProjectState("Building", 3, 20);

		/// <summary>
		/// Not currently connected to the build server (or the build server is in an
		/// unknown state)
		/// </summary>
		public static readonly ProjectState NotConnected = new ProjectState("Not Connected", 0, 10);


		public readonly string Name;
		public readonly int ImageIndex;

		/// <summary>
		///  A relative rating of how "important" this state is, higher == more important
		/// </summary>
		private readonly int importance;

		private ProjectState(string name, int imageIndex, int importance)
		{
			Name = name;
			ImageIndex = imageIndex;
			this.importance = importance;
		}

		public bool IsMoreImportantThan(ProjectState state)
		{
			return importance > state.importance;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
