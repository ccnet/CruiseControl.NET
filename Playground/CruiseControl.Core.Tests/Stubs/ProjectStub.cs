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
    }
}
