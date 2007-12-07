using System.Collections;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("filtered")]
	public class FilteredSourceControl : ISourceControl
	{
		private ISourceControl _realScProvider;
        private IModificationFilter[] _inclusionFilters = new IModificationFilter[0];
        private IModificationFilter[] _exclusionFilters = new IModificationFilter[0];

		[ReflectorProperty("sourceControlProvider", Required=true, InstanceTypeKey="type")]
		public ISourceControl SourceControlProvider
		{
			get { return _realScProvider; }
			set { _realScProvider = value; }
		}

        /// <summary>
        /// The list of filters that decide what modifications to exclude.
        /// </summary>
        [ReflectorProperty("exclusionFilters", Required = false)]
        public IModificationFilter[] ExclusionFilters
        {
			get { return _exclusionFilters; }
			set { _exclusionFilters = value; }
		}

        /// <summary>
        /// The list of filters that decide what modifications to include.
        /// </summary>
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
		public Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			Modification[] allModifications = _realScProvider.GetModifications(from, to);
			ArrayList acceptedModifications = new ArrayList();

			foreach (Modification modification in allModifications)
			{
				if (IsAcceptedByInclusionFilters(modification) &&
					(! IsAcceptedByExclusionFilters(modification)))
					acceptedModifications.Add(modification);
			}

			return (Modification[]) acceptedModifications.ToArray(typeof (Modification));
		}

		public void LabelSourceControl(IIntegrationResult result)
		{
			_realScProvider.LabelSourceControl(result);
		}

		public void GetSource(IIntegrationResult result)
		{
			_realScProvider.GetSource(result);
		}

		public void Initialize(IProject project)
		{
            _realScProvider.Initialize(project);
		}

		public void Purge(IProject project)
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
					return true;
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
					return true;
			}

			return false;
		}
	}
}