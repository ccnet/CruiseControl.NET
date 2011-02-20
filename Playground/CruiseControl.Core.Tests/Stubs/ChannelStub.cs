namespace CruiseControl.Core.Tests.Stubs
{
    using System;

    public class ChannelStub
        : ClientChannel
    {
        public Func<bool> OnInitialiseAction { get; set; }
        protected override bool OnInitialise()
        {
            if (this.OnInitialiseAction == null)
            {
                throw new NotImplementedException();
            }

            return this.OnInitialiseAction();
        }

        public Action OnCleanUpAction { get; set; }
        protected override void OnCleanUp()
        {
            if (this.OnCleanUpAction == null)
            {
                throw new NotImplementedException();
            }

            this.OnCleanUpAction();
        }
    }
}