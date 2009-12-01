namespace ThoughtWorks.CruiseControl.Core.Triggers
{
    using System;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// <para>
    /// The Interval Trigger is used to specify that an integration should be run periodically, after a certain amount of time. By default, an integration
    /// will only be triggered if modifications have been detected since the last integration. The trigger can also be configured to force a build even if
    /// no changes have occurred to source control. The items to watch for modifications are specified with <link>Source Control Blocks</link>.
    /// </para>
    /// <para type="info">
    /// Like all triggers, the intervalTrigger must be enclosed within a triggers element in the appropriate <link>Project Configuration Block</link>.
    /// </para>
    /// </summary>
    /// <title>Interval Trigger</title>
    /// <version>1.0</version>
    /// <remarks>
    /// <para type="warning">
    /// This trigger replaces the <b>PollingIntervalTrigger</b> and the <b>ForceBuildIntervalTrigger</b>.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;intervalTrigger /&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;intervalTrigger name="continuous" seconds="30" buildCondition="ForceBuild" initialSeconds="30" /&gt;
    /// </code>
    /// </example>
    [ReflectorType("intervalTrigger")]
	public class IntervalTrigger : ITrigger
	{
		public const double DefaultIntervalSeconds = 60;
		private readonly DateTimeProvider dateTimeProvider;
		private string name;
		private double intervalSeconds = DefaultIntervalSeconds;
        private double initialIntervalSeconds = -1;         // -1 indicates unset
	    private bool isInitialInterval = true;

        private DateTime nextBuildTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntervalTrigger"/> class.
        /// </summary>
		public IntervalTrigger() : this(new DateTimeProvider()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntervalTrigger"/> class.
        /// </summary>
        /// <param name="dtProvider">The dt provider.</param>
		public IntervalTrigger(DateTimeProvider dtProvider)
		{
			this.dateTimeProvider = dtProvider;
            IncrementNextBuildTime();
		}

        /// <summary>
        /// The name of the trigger. This name is passed to external tools as a means to identify the trigger that requested the build.
        /// </summary>
        /// <version>1.1</version>
        /// <default>IntervalTrigger</default>
		[ReflectorProperty("name", Required=false)]
		public virtual string Name
		{
			get
			{
				if (name == null) name = GetType().Name;
				return name;
			}
			set { name = value; }
		}

        /// <summary>
        /// The number of seconds after an integration cycle completes before triggering the next integration cycle.
        /// </summary>
        /// <version>1.0</version>
        /// <default>60</default>
        [ReflectorProperty("seconds", Required=false)]
        public double IntervalSeconds
        {
            get { return intervalSeconds; }
            set
            {
                intervalSeconds = value;
                IncrementNextBuildTime();
            }
        }

        /// <summary>
        /// The delay (in seconds) from CCNet startup to the first check for modifications.
        /// </summary>
        /// <version>1.4</version>
        /// <default>Defaults to the IntervalSettings value.</default>
		[ReflectorProperty("initialSeconds", Required = false)]
		public double InitialIntervalSeconds
		{
			get
			{
                if (initialIntervalSeconds == -1) 
                    return IntervalSeconds;     // If no setting for this, use IntervalSeconds instead.
                else
                    return initialIntervalSeconds;
			}
			set
			{
				initialIntervalSeconds = value;
				IncrementNextBuildTime();
			}
		}                    
		
        /// <summary>
        /// The condition that should be used to launch the integration. By default, this value is <b>IfModificationExists</b>, meaning that an integration will
        /// only be triggered if modifications have been detected. Set this attribute to <b>ForceBuild</b> in order to ensure that a build should be launched 
        /// regardless of whether new modifications are detected. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>IfModificationExists</default>
		[ReflectorProperty("buildCondition", Required=false)]
		public BuildCondition BuildCondition = BuildCondition.IfModificationExists;

		public virtual void IntegrationCompleted()
		{
            isInitialInterval = false;

			IncrementNextBuildTime();
		}

		protected DateTime IncrementNextBuildTime()
		{
		    double delaySeconds;
            if (isInitialInterval)
				delaySeconds = InitialIntervalSeconds;
            else
                delaySeconds = IntervalSeconds;

            return nextBuildTime = dateTimeProvider.Now.AddSeconds(delaySeconds);
		}

		public DateTime NextBuild
		{
			get {  return nextBuildTime;}
		}

		public virtual IntegrationRequest Fire()
		{
			BuildCondition buildCondition = ShouldRunIntegration();
			if (buildCondition == BuildCondition.NoBuild) return null;
			return new IntegrationRequest(buildCondition, Name, null);
		}

		private BuildCondition ShouldRunIntegration()
		{
			if (dateTimeProvider.Now < nextBuildTime)
				return BuildCondition.NoBuild;

			return BuildCondition;
		}
	}
}