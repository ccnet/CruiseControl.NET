using System;
using Exortech.NetReflector;

namespace tw.ccnet.core.sourcecontrol.test
{
	[ReflectorType("mocksourcecontrol")]
	public class SourceControlMock : ISourceControl
	{
		private Modification[] _expectedMods;
		private bool _invoked = false;

		public Modification[] ExpectedModifications
		{
			set { _expectedMods = value; }
		}

		public bool IsInvoked()
		{
			return _invoked;
		}

		public Modification[] GetModifications(DateTime from, DateTime to)
		{
			_invoked = true;
			return _expectedMods;
		}

		public void Run(IntegrationResult result)
		{
			result.Modifications = GetModifications(result.LastModificationDate, DateTime.Now);
		}

		public void LabelSourceControl(string label, DateTime timeStamp) 
		{
		}
	}
}
