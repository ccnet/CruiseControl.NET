using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    public interface IQuietPeriod 
    {
        Modification[] GetModifications(ISourceControl sourceControl, IIntegrationResult from, IIntegrationResult to);
    }
}
