using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.Core.Triggers
{
    /// <summary>
    /// Trigger to add build parameters to an integration request.
    /// </summary>
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
        /// The inner trigger that provides the actual build requests.
        /// </summary>
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