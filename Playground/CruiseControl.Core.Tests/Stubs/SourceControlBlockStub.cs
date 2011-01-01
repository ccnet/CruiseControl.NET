namespace CruiseControl.Core.Tests.Stubs
{
    using System;
    using CruiseControl.Core.Interfaces;

    public class SourceControlBlockStub
        : SourceControlBlock
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

        public Action OnInitialise { get; set; }
        public override void Initialise()
        {
            base.Initialise();
            if (this.OnInitialise != null)
            {
                this.OnInitialise();
            }
        }

        public override void GetModifications(GetModificationsParameters parameters)
        {
            throw new NotImplementedException();
        }

        public override void Label(LabelParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Action<GetSourceParameters> GetSourceAction { get; set; }
        public override void GetSource(GetSourceParameters parameters)
        {
            if (this.GetSourceAction != null)
            {
                this.GetSourceAction(parameters);
            }
            else
            {
                throw new NotImplementedException();
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
