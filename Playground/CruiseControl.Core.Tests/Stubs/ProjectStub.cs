namespace CruiseControl.Core.Tests.Stubs
{
    using System;
    using CruiseControl.Core.Interfaces;

    public class ProjectStub
        : Project
    {
        public ProjectStub()
        {
        }

        public ProjectStub(string name, params Task[] tasks)
            : base(name, tasks)
        {
        }

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

        public Action OnLoadState { get; set; }
        public override void LoadPersistedState()
        {
            if (this.OnLoadState != null)
            {
                this.OnLoadState();
            }
            else
            {
                base.LoadPersistedState();
            }
        }

        public Action OnSaveState { get; set; }
        public override void SavePersistedState()
        {
            if (this.OnSaveState != null)
            {
                this.OnSaveState();
            }
            else
            {
                base.SavePersistedState();
            }
        }

        public Func<IntegrationRequest, IntegrationStatus> OnIntegrate { get; set; }
        public override IntegrationStatus Integrate(IntegrationRequest request)
        {
            return this.OnIntegrate == null ? 
                base.Integrate(request) : 
                this.OnIntegrate(request);
        }
    }
}
