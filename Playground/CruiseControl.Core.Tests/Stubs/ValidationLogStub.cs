namespace CruiseControl.Core.Tests.Stubs
{
    using System;
    using CruiseControl.Core.Interfaces;

    public class ValidationLogStub
        : IValidationLog
    {
        public Action<string, object[]> OnAddErrorMessage { get; set; }
        public void AddError(string message, params object[] args)
        {
            if (this.OnAddErrorMessage == null)
            {
                throw new NotImplementedException();
            }

            this.OnAddErrorMessage(message, args);
        }

        public void AddError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void AddWarning(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void AddWarning(Exception error)
        {
            throw new NotImplementedException();
        }
    }
}
