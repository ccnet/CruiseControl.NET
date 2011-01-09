namespace CruiseControl.Core.Tests.Stubs
{
    using System;

    public class ChannelStub
        : ClientChannel
    {
        public Action OnInitialiseAction { get; set; }
        protected override void OnInitialise()
        {
            if (this.OnInitialiseAction == null)
            {
                throw new NotImplementedException();
            }

            this.OnInitialiseAction();
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