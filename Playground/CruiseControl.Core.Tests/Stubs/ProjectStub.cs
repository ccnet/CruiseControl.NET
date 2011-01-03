namespace CruiseControl.Core.Tests.Stubs
{
    using System;
    using CruiseControl.Core.Interfaces;

    public class ProjectStub
        : Project
    {
        public Action<IValidationLog> OnValidate { get; set; }
        public override void Validate(IValidationLog validationLog)
        {
            base.Validate(validationLog);
            if (this.OnValidate != null)
            {
                this.OnValidate(validationLog);
            }
        }

        public Action OnStart { get; set; }
        protected override void OnStarted()
        {
            base.OnStarted();
            if (this.OnStart != null)
            {
                this.OnStart();
            }
        }

        public Action OnStop { get; set; }
        protected override void OnStopped()
        {
            base.OnStopped();
            if (this.OnStop != null)
            {
                this.OnStop();
            }
        }
    }
}
