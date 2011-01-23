namespace ThoughtWorks.CruiseControl.Core.Triggers
{
    using System;
    using System.Collections.Generic;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Core.Util;

    /// <title>Roll-up Trigger</title>
    /// <summary>
    /// <para>
    /// A trigger that will "roll-up" multiple changes into a single trigger.
    /// </para>
    /// <para>
    /// This trigger can be used to reduce the load on a build server for in very active development
    /// environment.
    /// </para>
    /// </summary>
    /// <version>1.6</version>
    /// <example>
    /// <code title="Time Period Example">
    /// <![CDATA[
    /// <rollUpTrigger>
    /// <trigger type="intervalTrigger" seconds="60" />
    /// <time units="hours">1</time>
    /// </rollUpTrigger>
    /// ]]>
    /// </code>
    /// </example>
    [ReflectorType("rollUpTrigger")]
    public class RollUpTrigger
        : ITrigger
    {
        #region Private fields
        private readonly IClock clock;
        private DateTime nextAllowed;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RollUpTrigger"/> class.
        /// </summary>
        public RollUpTrigger()
            : this(new SystemClock())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RollUpTrigger"/> class with a clock.
        /// </summary>
        /// <remarks>
        /// This override is primarily for testing.
        /// </remarks>
        /// <param name="clock">The clock.</param>
        public RollUpTrigger(IClock clock)
        {
            this.clock = clock;
            this.nextAllowed = this.clock.Now.AddMinutes(-1);   // Set a time in the past
        }
        #endregion

        #region Public properties
        #region NextBuild
        /// <summary>
        /// Returns the time of the next build.
        /// </summary>
        /// <value></value>
        public DateTime NextBuild
        {
            get { return this.nextAllowed; }
        }
        #endregion

        #region InnerTrigger
        /// <summary>
        /// The inner trigger to filter. 
        /// </summary>
        /// <version>1.6</version>
        /// <default>n/a</default>
        [ReflectorProperty("trigger", InstanceTypeKey = "type")]
        public ITrigger InnerTrigger { get; set; }
        #endregion

        #region MinimumTime
        /// <summary>
        /// The minimum allowed time between builds.
        /// </summary>
        /// <version>1.6</version>
        /// <default>none</default>
        [ReflectorProperty("time", typeof(TimeoutSerializerFactory))]
        public Timeout MinimumTime { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region IntegrationCompleted()
        /// <summary>
        /// Notifies the trigger that an integration has completed.
        /// </summary>
        public void IntegrationCompleted()
        {
            this.InnerTrigger.IntegrationCompleted();
            this.nextAllowed = this.clock.Now.AddMilliseconds(this.MinimumTime.Millis);
        }
        #endregion

        #region Fire()
        /// <summary>
        /// Fires this instance.
        /// </summary>
        /// <returns>
        /// An <see cref="IntegrationRequest"/> if this trigger has fired, <c>null</c> otherwise.
        /// </returns>
        public IntegrationRequest Fire()
        {
            IntegrationRequest request = null;
            if (this.clock.Now > this.nextAllowed)
            {
                request = this.InnerTrigger.Fire();
            }

            return request;
        }
        #endregion
        #endregion
    }
}
