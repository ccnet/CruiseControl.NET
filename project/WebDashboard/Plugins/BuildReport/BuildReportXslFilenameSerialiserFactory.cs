using System;
using System.Collections.Generic;
using System.Web;
using Exortech.NetReflector;
using Exortech.NetReflector.Util;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport
{
    public class BuildReportXslFilenameSerialiserFactory
        : ISerialiserFactory
    {
        public IXmlMemberSerialiser Create(ReflectorMember memberInfo, ReflectorPropertyAttribute attribute)
        {
            return new BuildReportXslFilenameSerialiser(memberInfo, attribute);
        }
    }
}
