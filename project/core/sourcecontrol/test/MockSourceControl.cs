using System;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test
{
	[ReflectorType("mocksourcecontrol")]
	public class SourceControlMock : ISourceControl
	{
		private Modification[] _expectedMods;
		private bool _invoked = false;
		private string _anOptionalProperty;

		[ReflectorProperty("anOptionalProperty", Required=false)]
		public string AnOptionalProperty
		{
			get { return _anOptionalProperty; }
			set { _anOptionalProperty = value; }
		}

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

		public bool ShouldRun(IntegrationResult result, IProject project)
		{
			return true;
		}

		public void Run(IntegrationResult result, IProject project)
		{
			result.Modifications = GetModifications(result.LastModificationDate, DateTime.Now);
		}

		public void LabelSourceControl(string label, DateTime timeStamp) 
		{
		}

		public void GetSource(IntegrationResult result)
		{
		}

		public void Initialize(IProject project)
		{
		}

		public void Purge(IProject project)
		{
		}
	}
}
