using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("nullSourceControl")]
	public class NullSourceControl : ISourceControl
	{

        [ReflectorProperty("failGetModifications", Required = false)]
        public bool FailGetModifications = false;

        [ReflectorProperty("failLabelSourceControl", Required = false)]
        public bool FailLabelSourceControl = false;

        [ReflectorProperty("failGetSource", Required = false)]
        public bool FailGetSource = false;



		public Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
            if (FailGetModifications)
            {
                throw new System.Exception("Failing GetModifications");
            }
            else
            {
                return new Modification[0];
            }
		}

		public void LabelSourceControl(IIntegrationResult result) 
		{
            if (FailLabelSourceControl)
            {
                throw new System.Exception("Failing label source control");
            }
		}

		public void GetSource(IIntegrationResult result)
		{
            if (FailGetSource)
            {
                throw new System.Exception("Failing getting the source");
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
