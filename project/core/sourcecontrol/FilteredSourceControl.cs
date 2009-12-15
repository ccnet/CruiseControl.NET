using System;
using System.Collections.Generic;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// <para>
    /// The FilteredSourceControl allows you to filter out modifications that are used to trigger a build. If for example, you have certain
    /// files (such as web pages or document files) under source control that you don't want to have trigger the build, you can use this class
    /// to ensure that their changes will keep a new build from launching.
    /// </para>
    /// <para>
    /// The FilteredSourceControl works together with all of the source controls supported by CCNet (including the <link>Multi Source Control
    /// Block</link>). It can also be included under the <link>Multi Source Control Block</link> provider so that you could have multiple
    /// FilterSourceControls each filtering a different set of modifications from different source control providers. Essentially, it acts as a
    /// decorator (or an example of the pipes and filters pattern ), wrapping around the specific SourceControl provider that you want to use.
    /// </para>
    /// <para>
    /// The FilteredSourceControl includes both inclusion and exclusion filters for specifying what modifications should be included/excluded.
    /// Multiple inclusion and exclusion filters can be specified or, alternately, no inclusion or exclusion filter could be specified. If a
    /// modification is matched by both the inclusion and exclusion filter, then the exclusion filter will take preference and the modification
    /// will not be included in the modification set. It is relatively straightforward to build new filters, (such as one to filter
    /// modifications based on email address).
    /// </para>
    /// </summary>
    /// <title>Filtered Source Control Block</title>
    /// <version>1.0</version>
    /// <key name="type">
    /// <description>The type of source control block.</description>
    /// <value>filtered</value>
    /// </key>
    /// <example>
    /// <code>
    /// &lt;sourcecontrol type="filtered"&gt;
    /// &lt;sourceControlProvider type="vss" autoGetSource="true"&gt;
    /// &lt;project&gt;$/Kunigunda/ServiceLocator&lt;/project&gt;
    /// &lt;workingDirectory&gt;C:\CCNet\Kunigunda\ServiceLocator&lt;/workingDirectory&gt;
    /// &lt;username&gt;urosn&lt;/username&gt;
    /// &lt;password&gt;&lt;/password&gt;
    /// &lt;ssdir&gt;c:\localvss&lt;/ssdir&gt;
    /// &lt;/sourceControlProvider&gt;
    /// &lt;inclusionFilters&gt;
    /// &lt;pathFilter&gt;
    /// &lt;pattern&gt;$/Kunigunda/ServiceLocator/Sources/**/*.*&lt;/pattern&gt;
    /// &lt;/pathFilter&gt;
    /// &lt;/inclusionFilters&gt;
    /// &lt;exclusionFilters&gt;
    /// &lt;pathFilter&gt;
    /// &lt;pattern&gt;$/Kunigunda/ServiceLocator/Sources/Kunigunda.ServiceLocator/AssemblyInfo.cs&lt;/pattern&gt;
    /// &lt;/pathFilter&gt;
    /// &lt;pathFilter&gt;
    /// &lt;pattern&gt;$/**/*.vssscc&lt;/pattern&gt;
    /// &lt;/pathFilter&gt;
    /// &lt;userFilter&gt;
    /// &lt;names&gt;&lt;name&gt;Perry&lt;/name&gt;&lt;name&gt;Joe&lt;/name&gt;&lt;/names&gt;
    /// &lt;/userFilter&gt;
    /// &lt;actionFilter&gt;
    /// &lt;actions&gt;&lt;action&gt;deleted&lt;/action&gt;&lt;/actions&gt;
    /// &lt;/actionFilter&gt;
    /// &lt;commentFilter&gt;
    /// &lt;pattern&gt;Ignore: .*&lt;/pattern&gt;
    /// &lt;/commentFilter&gt;
    /// &lt;/exclusionFilters&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// Implemented and contributed by Uros Novak.
    /// </remarks>
	[ReflectorType("filtered")]
	public class FilteredSourceControl 
        : SourceControlBase
	{
		private ISourceControl _realScProvider;
        private IModificationFilter[] _inclusionFilters = new IModificationFilter[0];
        private IModificationFilter[] _exclusionFilters = new IModificationFilter[0];

        /// <summary>
        /// This element is used to specify the type of source control provider to retrieve modifications from. With the exception of the
        /// element name, the configuration for this element is identical to the xml configuration for the specific source control provider you
        /// intend to use.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
		[ReflectorProperty("sourceControlProvider", Required=true, InstanceTypeKey="type")]
		public ISourceControl SourceControlProvider
		{
			get { return _realScProvider; }
			set { _realScProvider = value; }
		}

        /// <summary>
        /// The list of filters that decide what modifications to exclude.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("exclusionFilters", Required = false)]
        public IModificationFilter[] ExclusionFilters
        {
			get { return _exclusionFilters; }
			set { _exclusionFilters = value; }
		}

        /// <summary>
        /// The list of filters that decide what modifications to include.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("inclusionFilters", Required = false)]
        public IModificationFilter[] InclusionFilters
        {
            get { return _inclusionFilters; }
            set { _inclusionFilters = value; }
        }

        /// <summary>
        /// Get the list of modifications from the inner source control provider and filter it.
        /// </summary>
        /// <returns>The filtered modification list.</returns>
        /// <remarks>
        /// A modification survives filtering if it is accepted by the inclusion filters and not accepted
        /// by the exclusion filters.
        /// </remarks>
        public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			Modification[] allModifications = _realScProvider.GetModifications(from, to);
            var acceptedModifications = new List<Modification>();

			foreach (Modification modification in allModifications)
			{
                if (IsAcceptedByInclusionFilters(modification) &&
                    (!IsAcceptedByExclusionFilters(modification)))
                {
                    Log.Debug(String.Format("Modification {0} was accepted by the filter specification.",
                        modification));
                    acceptedModifications.Add(modification);
                }
                else
                    Log.Debug(String.Format("Modification {0} was not accepted by the filter specification.",
                        modification));
            }

			return acceptedModifications.ToArray();
		}

        public override void LabelSourceControl(IIntegrationResult result)
		{
			_realScProvider.LabelSourceControl(result);
		}

        public override void GetSource(IIntegrationResult result)
		{
			_realScProvider.GetSource(result);
		}

        public override void Initialize(IProject project)
		{
            _realScProvider.Initialize(project);
		}

        public override void Purge(IProject project)
		{
             _realScProvider.Purge(project);
		}

		/// <summary>
		/// Determine if the specified modification should be included.
		/// </summary>
		/// <param name="m">The modification to check.</param>
		/// <returns>True if the modification should be included, false otherwise.</returns>
		/// <remarks>
		/// Modification is accepted by default if there isn't any
		/// inclusion filter or if the modification is accepted by
		/// at least one of the defined filters.
		/// </remarks>
		private bool IsAcceptedByInclusionFilters(Modification m)
		{
			if (_inclusionFilters.Length == 0)
				return true;

			foreach (IModificationFilter mf in _inclusionFilters)
			{
				if (mf.Accept(m))
                {
                    Log.Debug(String.Format("Modification {0} was included by filter {1}.", m, mf));
                    return true;
                }
            }

			return false;
		}

        /// <summary>
        /// Determine if the specified modification should be excluded.
        /// </summary>
        /// <param name="m">The modification to check.</param>
        /// <returns>True if the modification should be excluded, false otherwise.</returns>
        /// <remarks>
		/// Modification is not accepted if there isn't any exclusion
		/// filter. Modification is accepted if it is accepted by at 
		/// least one of the defined exclusion filters.
		/// </remarks>
		private bool IsAcceptedByExclusionFilters(Modification m)
		{
            if (_exclusionFilters.Length == 0)
				return false;

			foreach (IModificationFilter mf in _exclusionFilters)
			{
                if (mf.Accept(m))
                {
                    Log.Debug(String.Format("Modification {0} was excluded by filter {1}.", m, mf));
                    return true;
                }
			}

			return false;
		}

        #region ApplyParameters()
        /// <summary>
        /// Applies the input parameters to the task.
        /// </summary>
        /// <param name="parameters">The parameters to apply.</param>
        /// <param name="parameterDefinitions">The original parameter definitions.</param>
        public override void ApplyParameters(Dictionary<string, string> parameters, IEnumerable<ParameterBase> parameterDefinitions)
        {
            base.ApplyParameters(parameters, parameterDefinitions);
            var dynamicChild = _realScProvider as IParamatisedItem;
            if (dynamicChild != null) dynamicChild.ApplyParameters(parameters, parameterDefinitions);
        }
        #endregion
    }
}