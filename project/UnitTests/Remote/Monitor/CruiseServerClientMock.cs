using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Monitor
{
    public class CruiseServerClientMock
        : CruiseServerClientBase
    {
        public override string TargetServer
        {
            get { return string.Empty; }
            set { }
        }

        public override bool IsBusy
        {
            get { return false; }
        }

        public override string Address
        {
            get { return string.Empty; }
        }
    }
}
