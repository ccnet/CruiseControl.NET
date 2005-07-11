using System.Collections;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("multi")]
	public class MultiSourceControl : ISourceControl
	{
		private ISourceControl[] _sourceControls;

		[ReflectorProperty("requireChangesFromAll", Required=false)]
		public bool RequireChangesFromAll = false;

		[ReflectorArray("sourceControls", Required=true)]
		public ISourceControl[] SourceControls 
		{
			get 
			{
				if (_sourceControls == null)
					_sourceControls = new ISourceControl[0];

				return _sourceControls;
			}

			set { _sourceControls = value; }
		}

		public virtual Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			ArrayList modifications = new ArrayList();
			foreach (ISourceControl sourceControl in SourceControls)
			{
				Modification[] mods = sourceControl.GetModifications(from, to);
				if (mods != null && mods.Length > 0)
				{
					modifications.AddRange(mods);
				}
				else if (RequireChangesFromAll)
				{
					modifications.Clear();
					break;
				}
			}

			return (Modification[]) modifications.ToArray(typeof(Modification));
		}

		public void LabelSourceControl(IIntegrationResult result)
		{
			foreach (ISourceControl sourceControl in SourceControls)
			{
				sourceControl.LabelSourceControl(result);
			}
		}

		public void GetSource(IIntegrationResult result) 
		{
			foreach (ISourceControl sourceControl in SourceControls)
			{
				sourceControl.GetSource(result);
			}
		}

		public void Initialize(IProject project)
		{
		}

		public void Purge(IProject project)
		{
		}
	}
}