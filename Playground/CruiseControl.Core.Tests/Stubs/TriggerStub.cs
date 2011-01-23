namespace CruiseControl.Core.Tests.Stubs
{
    using System;
    using CruiseControl.Core.Interfaces;

    public class TriggerStub
        : Trigger
    {
        public DateTime? NextTimeValue { get; set; }
        public override DateTime? NextTime
        {
            get
            {
                return this.NextTimeValue ?? base.NextTime;
            }
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

        public Action OnInitialise { get; set; }
        public override void Initialise()
        {
            base.Initialise();
            if (this.OnInitialise != null)
            {
                this.OnInitialise();
            }
        }

        public Func<IntegrationRequest> OnCheckAction { get; set; }
        protected override IntegrationRequest OnCheck()
        {
            if (this.OnCheckAction != null)
            {
                return this.OnCheckAction();
            }

            throw new NotImplementedException();
        }

        public Action OnResetAction { get; set; }
        protected override void OnReset()
        {
            base.OnReset();
            if (this.OnResetAction != null)
            {
                this.OnResetAction();
            }
        }

        public Action OnCleanUp { get; set; }
        public override void CleanUp()
        {
            base.CleanUp();
            if (this.OnCleanUp != null)
            {
                this.OnCleanUp();
            }
        }
    }
}
