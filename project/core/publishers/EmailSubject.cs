using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    /// <summary>
    /// This element  allows to set specific subject messages according to the state of the build. When a certain state
    /// is not specified, a default will be entered.
    /// </summary>
    /// <title>Email Subject</title>
    /// <version>1.0</version>
    /// <example>
    /// <code>
    /// &lt;subject buildResult="StillBroken" value="Build is still broken for {CCNetProject}" /&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>
    /// It is also possible to use <link>Integration Properties</link> in this section. For example:
    /// </para>
    /// <code>
    /// &lt;subjectSettings&gt;
    /// &lt;subject buildResult="StillBroken" value="Build is still broken for ${CCNetProject}, please check again" /&gt;
    /// &lt;/subjectSettings&gt;
    /// </code>
    /// <para>
    /// or:
    /// </para>
    /// <code>
    /// &lt;subjectSettings&gt;
    /// &lt;subject buildResult="StillBroken" value="Build is still broken for ${CCNetProject}, the fix failed." /&gt;
    /// &lt;subject buildResult="Broken" value="{CCNetProject} broke at ${CCNetBuildDate} ${CCNetBuildTime } , last checkin(s) by ${CCNetFailureUsers}" /&gt;
    /// &lt;subject buildResult="Exception" value="Serious problem for ${CCNetProject}, it is now in Exception! Check status of network / sourcecontrol" /&gt;
    /// &lt;/subjectSettings&gt;
    /// </code>
    /// </remarks>
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

        /// <summary>
        /// The value of the subject line, the text to be used for the subject. This may contain variables, see below. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("value")]
        public string Value;

        /// <summary>
        /// A build result state, see below for the possible values.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
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
