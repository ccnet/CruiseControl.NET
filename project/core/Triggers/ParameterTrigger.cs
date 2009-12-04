using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.Core.Triggers
{
    /// <summary>
    /// <para>
    /// Trigger to add build parameters to an integration request.
    /// </para>
    /// <para type="info">
    /// Like all triggers, the parameterTrigger must be enclosed within a triggers element in the appropriate <link>Project Configuration
    /// Block</link>. 
    /// </para>
    /// <para type="warning">
    /// <title>Nested trigger syntax is different</title>
    /// As shown below, the configuration of the nested trigger is not the same as when using that trigger outside a filter trigger. When
    /// using the &lt;parameterTrigger&gt; element, the inner trigger must be specified with the &lt;trigger&gt; element. You could not use the
    /// &lt;intervalTrigger&gt; trigger element in this example.
    /// </para>
    /// </summary>
    /// <title>Parameter Trigger</title>
    /// <version>1.5</version>
    /// <example>
    /// <code>
    /// &lt;parameterTrigger&gt;
    /// &lt;trigger type="intervalTrigger" seconds="60" /&gt;
    /// &lt;parameters&gt;
    /// &lt;!-- Parameters here--&gt;
    /// &lt;/parameters&gt;
    /// &lt;/parameterTrigger&gt;
    /// </code>
    /// </example>
    [ReflectorType("parameterTrigger")]
    public class ParameterTrigger : ITrigger
    {
        #region Private fields
        private ITrigger innerTrigger;
        private NameValuePair[] parameters = new NameValuePair[0];
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new blank instance of <see cref="ParameterTrigger"/>.
        /// </summary>
        public ParameterTrigger()
        {
        }
        #endregion

        #region Public properties
        #region InnerTrigger
        /// <summary>
        /// The inner trigger to filter. 
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("trigger", InstanceTypeKey = "type")]
        public ITrigger InnerTrigger
        {
            get { return innerTrigger; }
            set { innerTrigger = value; }
        }
        #endregion

        #region Parameters
        /// <summary>
        /// The parameters to pass onto the inner trigger.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("parameters")]
        public NameValuePair[] Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }
        #endregion

        #region NextBuild
        /// <summary>
        /// The date and time of the next build.
        /// </summary>
        public DateTime NextBuild
        {
            get { return innerTrigger.NextBuild; }
        }
        #endregion
        #endregion

        #region Public methods
        #region IntegrationCompleted()
        /// <summary>
        /// An integration has completed, the trigger can now be reactivated.
        /// </summary>
        public void IntegrationCompleted()
        {
            innerTrigger.IntegrationCompleted();
        }
        #endregion

        #region Fire()
        /// <summary>
        /// Checks if the trigger needs to actually fire.
        /// </summary>
        /// <returns></returns>
        public IntegrationRequest Fire()
        {
            IntegrationRequest request = innerTrigger.Fire();
            if (request != null)
            {
                List<NameValuePair> values = new List<NameValuePair>(parameters);
                request.BuildValues = NameValuePair.ToDictionary(values);
            }
            return request;
        }
        #endregion
        #endregion
    }
}