using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Label
{
	/// <summary>
	/// The AssemblyVersionLabeller uses the CruiseControl.NET change number, provided by
	/// source control systems like Subversion, to build a valid Assembly Version label.
	/// </summary>
	[ReflectorType("assemblyVersionLabeller")]
	public class AssemblyVersionLabeller : ILabeller
	{
		#region public properties

		/// <summary>
		/// Sets the value of the major component of the version number.
		/// </summary>
		[ReflectorProperty("major", Required = false)]
		public int Major;

		/// <summary>
		/// Sets the value of the minor component of the version number.
		/// </summary>
		[ReflectorProperty("minor", Required = false)]
		public int Minor;

		/// <summary>
		/// Sets the value of the build component of the version number.
		/// </summary>
		[ReflectorProperty("build", Required = false)]
		public int Build = -1;

		/// <summary>
		/// Sets the value of the revision component of the version number.
		/// </summary>
		[ReflectorProperty("revision", Required = false)]
		public int Revision = -1;

		/// <summary>
		/// If true, the label will be incremented even if the build fails.
		/// Otherwise it will only be incremented if the build succeeds.
		/// </summary>
		[ReflectorProperty("incrementOnFailure", Required = false)]
		public bool IncrementOnFailure;

		#endregion

		#region ILabeller Members

		public string Generate(IIntegrationResult integrationResult)
		{
			Version oldVersion;

			// try getting old version
			try
			{
				Log.Debug(string.Concat("Old build label is: ", integrationResult.LastIntegration.Label));
				oldVersion = new Version(integrationResult.LastIntegration.Label);
			}
			catch (Exception)
			{
				oldVersion = new Version(0, 0, 0, 0);
			}

			Log.Debug(string.Concat("Old version is: ", oldVersion.ToString()));

			// get current change number
			int currentRevision = 0;

			if (Revision > -1)
			{
				currentRevision = Revision;
			}
			else
			{
                if (int.TryParse(integrationResult.LastChangeNumber, out currentRevision))
                {
                    Log.Debug(
                        string.Format("LastChangeNumber retrieved - {0}", 
                        currentRevision));
                }
                else
                {
                    Log.Debug("LastChangeNumber defaulted to 0");
                }

				// use the revision from last build,
				// because LastChangeNumber is 0 on ForceBuild or other failures
				if (currentRevision <= 0) currentRevision = oldVersion.Revision;
			}

			// get current build number
			int currentBuild;

			if (Build > -1)
			{
				currentBuild = Build;
			}
			else
			{
				currentBuild = oldVersion.Build;

				// check whenever the version has changed or build forced and if the
				// integration is success or incrementOnFailure is true
				// to increase the build number
				if ((Major != oldVersion.Major ||
					Minor != oldVersion.Minor ||
					currentRevision != oldVersion.Revision ||
					integrationResult.BuildCondition == BuildCondition.ForceBuild) &&
					(integrationResult.LastIntegrationStatus == IntegrationStatus.Success || IncrementOnFailure))
				{
					currentBuild++;
				}
			}

			Log.Debug(string.Format(System.Globalization.CultureInfo.InvariantCulture,
									"Major: {0} Minor: {1} Build: {2} Revision: {3}", Major, Minor, currentBuild, currentRevision));

			Version newVersion = new Version(Major, Minor, currentBuild, currentRevision);
			Log.Debug(string.Concat("New version is: ", newVersion.ToString()));

			// return new version string
			return newVersion.ToString();
		}

		#endregion

		#region ITask Members

		public void Run(IIntegrationResult result)
		{
			result.Label = Generate(result);
		}

		#endregion
	}
}