using System;
using System.Collections;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("filtered")]
	public class FilteredSourceControl: ISourceControl, ITemporaryLabeller
	{
		private ISourceControl _realScProvider;
		private IList _inclusionFilters;
		private IList _exclusionFilters;

		[ReflectorProperty("sourceControlProvider", Required=true, InstanceTypeKey="type")]
		public ISourceControl SourceControlProvider
		{
			get { return _realScProvider; }
			set { _realScProvider = value; }
		}

		[ReflectorCollection("exclusionFilters", InstanceType=typeof(ArrayList), Required=false)]
		public IList ExclusionFilters 
		{
			get 
			{
				if ( _exclusionFilters == null )
					_exclusionFilters = new ArrayList();

				return _exclusionFilters; 
			}
			set { _exclusionFilters = value; }
		}

		[ReflectorCollection("inclusionFilters", InstanceType=typeof(ArrayList), Required=false)]
		public IList InclusionFilters 
		{
			get 
			{
				if ( _inclusionFilters == null )
					_inclusionFilters = new ArrayList();

				return _inclusionFilters;
			}
			set { _inclusionFilters = value; }
		}

		public Modification[] GetModifications(DateTime from, DateTime to)
		{
			Modification[] allModifications = _realScProvider.GetModifications(from, to);
			ArrayList acceptedModifications = new ArrayList();

			foreach (Modification modification in allModifications) 
			{
				if ( IsAcceptedByInclusionFilters(modification) && 
					(! IsAcceptedByExclusionFilters(modification)) )
					acceptedModifications.Add(modification);
			}

			return (Modification[])acceptedModifications.ToArray(typeof(Modification));
		}

		public void LabelSourceControl(string label, DateTime timeStamp)
		{
			_realScProvider.LabelSourceControl(label, timeStamp);
		}

		public bool ShouldRun(IIntegrationResult result)
		{
			return _realScProvider.ShouldRun(result);
		}

		public void Run(IIntegrationResult result)
		{
			result.Modifications = GetModifications(result.LastModificationDate, DateTime.Now);
		}

		public void CreateTemporaryLabel()
		{
			if ( _realScProvider is ITemporaryLabeller )
			{
				( (ITemporaryLabeller) _realScProvider ).CreateTemporaryLabel();
			}
		}

		public void DeleteTemporaryLabel()
		{
			if ( _realScProvider is ITemporaryLabeller )
			{
				( (ITemporaryLabeller) _realScProvider ).DeleteTemporaryLabel();
			}
		}

		public void GetSource(IIntegrationResult result) 
		{
			_realScProvider.GetSource(result);
		}

		public void Initialize(IProject project)
		{
		}

		public void Purge(IProject project)
		{
		}

		/// <remarks>
		/// Modification is accepted by default if there isn't any
		/// inclusion filter or if the modification is accepted by
		/// at least one of the defined filters.
		/// </remarks>
		private bool IsAcceptedByInclusionFilters(Modification m) 
		{
			if ( _inclusionFilters == null || _inclusionFilters.Count == 0 )
				return true;

			foreach (IModificationFilter mf in _inclusionFilters) 
			{
				if ( mf.Accept(m) )
					return true;
			}

			return false;
		}

		/// <remarks>
		/// Modification is not accepted if there isn't any exclusion
		/// filter. Modification is accepted if it is accepted by at 
		/// least one of the defined exclusion filters.
		/// </remarks>
		private bool IsAcceptedByExclusionFilters(Modification m) 
		{
			if ( _exclusionFilters == null || _exclusionFilters.Count == 0 )
				return false;

			foreach (IModificationFilter mf in _exclusionFilters) 
			{
				if ( mf.Accept(m) )
					return true;
			}

			return false;
		}
	}
}