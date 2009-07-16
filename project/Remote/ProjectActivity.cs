using System;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote
{
	/// <summary>
	/// Enumeration of the possible activities of a project under continuous
	/// integration by CruiseControl.NET.
	/// </summary>
	[Serializable]
    [XmlRoot("projectActivity")]
	public class ProjectActivity
	{
		private string type;

        #region Constructors
        /// <summary>
        /// Starts a new blank <see cref="ProjectActivity"/>.
        /// </summary>
        public ProjectActivity()
        {
        }

        /// <summary>
        /// Initialise a new populated <see cref="ProjectActivity"/>.
        /// </summary>
        /// <param name="type"></param>
		public ProjectActivity(string type)
		{
			this.type = type;
		}
        #endregion

        #region Type
        /// <summary>
        /// The type of project activity.
        /// </summary>
        [XmlAttribute("type")]
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        #endregion

        /// <summary>
        /// Is this a building status?
        /// </summary>
        /// <returns></returns>
		public bool IsBuilding()
		{
			return type == Building.type;
		}

        /// <summary>
        /// Is this a sleeping status?
        /// </summary>
        /// <returns></returns>
		public bool IsSleeping()
		{
			return type == Sleeping.type;
		}

        /// <summary>
        /// Is this a pending status?
        /// </summary>
        /// <returns></returns>
		public bool IsPending()
		{
			return type == Pending.type;
		}

        /// <summary>
        /// Is this a CheckingModifications status?
        /// </summary>
        /// <returns></returns>
        public bool IsCheckingModifications()
        {
            return type == CheckingModifications.type;
        }

        /// <summary>
        /// Checks if two <see cref="ProjectActivity"/> are the same.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
		public override bool Equals(object obj)
		{
			ProjectActivity other = obj as ProjectActivity;
			return other != null && other.ToString() == ToString();
		}

        /// <summary>
        /// Retrieves the hashcode for this <see cref="ProjectActivity"/>.
        /// </summary>
        /// <returns></returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

        /// <summary>
        /// Returns the type of activity.
        /// </summary>
        /// <returns></returns>
		public override string ToString()
		{
			return type;
		}

        /// <summary>
        /// Compares if two <see cref="ProjectActivity"/> are the same.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
		public static bool operator == (ProjectActivity left, ProjectActivity right) 
		{
			return Object.Equals(left, right);
		}
        /// <summary>
        /// Compares if two <see cref="ProjectActivity"/> are different.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
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
