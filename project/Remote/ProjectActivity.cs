using System;

namespace ThoughtWorks.CruiseControl.Remote
{
	/// <summary>
	/// Enumeration of the possible activities of a project under continuous
	/// integration by CruiseControl.NET.
	/// </summary>
	[Serializable]
	public class ProjectActivity
	{
		private readonly string type;

		public ProjectActivity(string type)
		{
			this.type = type;
		}

		public bool IsBuilding()
		{
			return type == Building.type;
		}

		public bool IsSleeping()
		{
			return type == Sleeping.type;
		}

		public bool IsPending()
		{
			return type == Pending.type;
		}

		public override bool Equals(object obj)
		{
			ProjectActivity other = obj as ProjectActivity;
			return other != null && other.ToString() == ToString();
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return type;
		}

		public static bool operator == (ProjectActivity left, ProjectActivity right) 
		{
			return Object.Equals(left, right);
		}
		public static bool operator != (ProjectActivity left, ProjectActivity right) 
		{
			return !(left == right);
		}

		/// <summary>
		/// CruiseControl.NET is checking for modifications in this project's
		/// source control system.
		/// </summary>
		public static ProjectActivity CheckingModifications = new ProjectActivity("CheckingModifications");

		/// <summary>
		/// CruiseControl.NET is running the build phase of the project's
		/// integration.
		/// </summary>
		public static ProjectActivity Building = new ProjectActivity("Building");

		/// <summary>
		/// CruiseControl.NET is sleeping, and no activity is being performed
		/// for this project.
		/// </summary>
		public static ProjectActivity Sleeping = new ProjectActivity("Sleeping");

		/// <summary>
		/// CruiseControl.NET is queuing a pending build integration request for this project.
		/// </summary>
		public static ProjectActivity Pending = new ProjectActivity("Pending");
	}
}
