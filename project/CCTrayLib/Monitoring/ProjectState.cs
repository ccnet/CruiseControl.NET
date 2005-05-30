using System;

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
		/// The build is building
		/// </summary>
		public static readonly ProjectState Building = new ProjectState("Building", 3, 10);

		/// <summary>
		/// Not currently connected to the build server (or the build server is in an
		/// unknown state)
		/// </summary>
		public static readonly ProjectState NotConnected = new ProjectState("Not Connected", 0, 20);


		public readonly string Name;
		public readonly int ImageIndex;

		/// <summary>
		///  A relative rating of how "bad" this state is, higher == badder
		/// </summary>
		private int badness;

		private ProjectState( string name, int imageIndex, int badness )
		{
			Name = name;
			ImageIndex = imageIndex;
			this.badness = badness;
		}

		public bool IsWorseThan( ProjectState state)
		{
			return this.badness > state.badness;
		}

		public override string ToString()
		{
			return Name;
		}

	}
}