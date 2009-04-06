using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    [ReflectorType("subject")]
    public class EmailSubject
    {

        public enum BuildResultType
        {
            Success, Broken, StillBroken, Fixed, Exception
        }

        public EmailSubject()
        {
        }


        public EmailSubject(BuildResultType result, string value )
		{
            Value = value;
			BuildResult = result;
		}

        [ReflectorProperty("value")]
        public string Value;


        [ReflectorProperty("buildResult")]
        public BuildResultType BuildResult;

        public override bool Equals(Object o)
        {
            if (o == null || o.GetType() != GetType())
            {
                return false;
            }
            EmailSubject g = (EmailSubject)o;
            return BuildResult == g.BuildResult;
        }

        public override int GetHashCode()
        {
            return BuildResult.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("EmailSubject: [BuildResult: {0}, subject: {1}]", BuildResult, Value);
        }

    }
}
