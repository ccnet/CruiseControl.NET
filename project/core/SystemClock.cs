#pragma warning disable 1591
using System;

namespace ThoughtWorks.CruiseControl.Core
{
    public class SystemClock : IClock
    {
        public DateTime Now
        {
            get { return DateTime.Now; }
        }
    }
}