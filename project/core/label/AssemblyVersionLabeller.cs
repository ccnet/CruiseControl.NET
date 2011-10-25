using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Label
{
    /// <summary>
    /// Provides a valid System.Version label for your .NET assemblies that could be used to set the AssemblyVersionAttribute(). It increments
    /// the build number on every successful integration and uses the CruiseControl.NET change number, provided by source control systems like
    /// Subversion, for the revision number component.
	/// </summary>
    /// <title>Assembly Version Labeller</title>
    /// <version>1.4.4</version>
    /// <remarks>This labeller will not generate revision version numbers until a commit-triggered build is performed.
    /// A forced build re-uses the revision number from last commit (which defaults to 1 if no commits were yet performed
    /// after enabling this labeller).</remarks>
    /// <example>
    /// <code title="Minimalist Example">
    /// &lt;labeller type="assemblyVersionLabeller" /&gt;
    /// </code>
    /// <code title="Full Example (build number and revision number component are incremented automatically)">
    /// &lt;labeller type="assemblyVersionLabeller"&gt;
    /// &lt;major&gt;1&lt;/major&gt;
    /// &lt;minor&gt;2&lt;/minor&gt;
    /// &lt;incrementOnFailure&gt;false&lt;/incrementOnFailure&gt;
    /// &lt;/labeller&gt;
    /// </code>
    /// <code title="Full Example (all properties)">
    /// &lt;labeller type="assemblyVersionLabeller"&gt;
    /// &lt;major&gt;1&lt;/major&gt;
    /// &lt;minor&gt;2&lt;/minor&gt;
    /// &lt;build&gt;250&lt;/build&gt;
    /// &lt;revision&gt;1765&lt;/revision&gt;
    /// &lt;incrementOnFailure&gt;false&lt;/incrementOnFailure&gt;
    /// &lt;/labeller&gt;
    /// </code>
    /// </example>
	[ReflectorType("assemblyVersionLabeller")]
	public class AssemblyVersionLabeller
        : LabellerBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyVersionLabeller"/> class.
        /// </summary>
        public AssemblyVersionLabeller()
        {
            this.Build = -1;
            this.Revision = -1;
        }
        #endregion

        #region public properties
        /// <summary>
        /// Major number component of the version. 
		/// </summary>
        /// <version>1.4.4</version>
        /// <default>0</default>
        [ReflectorProperty("major", Required = false)]
        public int Major { get; set; }

		/// <summary>
        /// Minor number component of the version. 
		/// </summary>
        /// <version>1.4.4</version>
        /// <default>0</default>
        [ReflectorProperty("minor", Required = false)]
        public int Minor { get; set; }

		/// <summary>
        /// Build number component of the version. If not specified the build number is incremented on every successful integration. 
		/// </summary>
        /// <version>1.4.4</version>
        /// <default>-1</default>
        [ReflectorProperty("build", Required = false)]
        public int Build { get; set; }

		/// <summary>
        /// Revision number component of the version. If not specified the revision number is the LastChangeNumber, provided by some VCS (e.g.
        /// the svn revision with the Subversion task).
		/// </summary>
        /// <version>1.4.4</version>
        /// <default>-1</default>
        [ReflectorProperty("revision", Required = false)]
        public int Revision { get; set; }

		/// <summary>
        /// Whether to increase the build number component if the integration fails. By default the build number component will only increase
        /// if the integration was successful.
		/// </summary>
        /// <version>1.4.4</version>
        /// <default>false</default>
        [ReflectorProperty("incrementOnFailure", Required = false)]
        public bool IncrementOnFailure { get; set; }

		#endregion

		#region ILabeller Members

        /// <summary>
        /// Generates the specified integration result.	
        /// </summary>
        /// <param name="integrationResult">The integration result.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public override string Generate(IIntegrationResult integrationResult)
		{
			Version oldVersion;

			// try getting old version
			try
			{
                Log.Debug(string.Concat("[assemblyVersionLabeller] Old build label is: ", integrationResult.LastIntegration.Label));
				oldVersion = new Version(integrationResult.LastIntegration.Label);
			}
			catch (Exception)
			{
				oldVersion = new Version(0, 0, 0, 0);
			}

            Log.Debug(string.Concat("[assemblyVersionLabeller] Old version is: ", oldVersion.ToString()));

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
                        string.Format(System.Globalization.CultureInfo.CurrentCulture,"[assemblyVersionLabeller] LastChangeNumber retrieved: {0}", 
                        currentRevision));
                }
                else
                {
                    Log.Debug("[assemblyVersionLabeller] LastChangeNumber of source control is '{0}', set revision number to '0'.",
                              string.IsNullOrEmpty(integrationResult.LastChangeNumber)
                                  ? "N/A"
                                  : integrationResult.LastChangeNumber);
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
                Log.Debug("[assemblyVersionLabeller] Build number ist set to '{0}' via configuration.", Build);
			}
			else
			{
				currentBuild = oldVersion.Build;

                // check whenever the integration is succeeded or incrementOnFailure is true
				// to increase the build number
                if (integrationResult.LastIntegration.Status == IntegrationStatus.Success || IncrementOnFailure)
				{
					currentBuild++;
				}
                else
				{
				    Log.Debug(
				        "[assemblyVersionLabeller] Not increasing build number because the integration is not succeeded and 'incrementOnFailure' property is set to 'false'.");
				}
			}

			Log.Debug(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                                    "[assemblyVersionLabeller] Major: {0} Minor: {1} Build: {2} Revision: {3}", Major, Minor, currentBuild, currentRevision));

			Version newVersion = new Version(Major, Minor, currentBuild, currentRevision);
            Log.Debug(string.Concat("[assemblyVersionLabeller] New version is: ", newVersion.ToString()));

			// return new version string
			return newVersion.ToString();
		}

		#endregion
	}
}