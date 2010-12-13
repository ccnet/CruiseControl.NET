namespace CruiseControl.Core.Tests.Stubs
{
    using System;

    public class ProjectStub
        : Project
    {
        public Action<IntegrationContext> OnIntegration { private get; set; }
        public override void Integrate(IntegrationContext context)
        {
            if (this.OnIntegration != null)
            {
                this.OnIntegration(context);
            }
            else
            {
                base.Integrate(context);
            }
        }
    }
}
