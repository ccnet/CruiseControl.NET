namespace CruiseControl.Core.Tests.Stubs
{
    using System;
    using CruiseControl.Core.Interfaces;

    public class SourceControlBlockStub
        : SourceControlBlock
    {
        public override void Validate(IValidationLog validationLog)
        {
            throw new NotImplementedException();
        }

        public override void Initialise()
        {
            throw new NotImplementedException();
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

        public override void CleanUp()
        {
            throw new NotImplementedException();
        }
    }
}
