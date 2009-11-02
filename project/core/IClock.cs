using System;

namespace ThoughtWorks.CruiseControl.Core
{
    public interface IClock
    {
        DateTime Now { get; }
    }
}