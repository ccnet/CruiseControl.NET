using System;
using System.Collections;
using Exortech.NetReflector;

using tw.ccnet.core;

namespace tw.ccnet.core.sourcecontrol
{
	[ReflectorType("multi")]
	public class MultiSourceControl : ISourceControl
	{
		private IList _sourceControls;

		[ReflectorCollection("sourceControls", InstanceType=typeof(ArrayList), Required=true)]
		public IList SourceControls 
		{
			get 
			{
				if (_sourceControls == null)
					_sourceControls = new ArrayList();

				return _sourceControls;
			}

			set { _sourceControls = value; }
		}

		public Modification[] GetModifications(DateTime from, DateTime to)
		{
			ArrayList modifications = new ArrayList();
			foreach (ISourceControl sourceControl in SourceControls)
			{
				Modification[] mods = sourceControl.GetModifications(from, to);
				if (mods != null)
				{
					modifications.AddRange(mods);
				}
			}

			return (Modification[]) modifications.ToArray(typeof(Modification));
		}

		public void LabelSourceControl(string label, DateTime timeStamp)
		{
			foreach (ISourceControl sourceControl in SourceControls)
			{
				sourceControl.LabelSourceControl(label, timeStamp);
			}
		}

		public void Run(IntegrationResult result)
		{
			result.Modifications = GetModifications(result.LastModificationDate, DateTime.Now);
		}
	}
}
