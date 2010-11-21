namespace ThoughtWorks.CruiseControl.Core.Triggers
{
    using System;
    using System.Globalization;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Config;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// <para>
    /// The Schedule Trigger is used to specify that an integration should be run at a certain time on certain days. By default, an integration will only
    /// be triggered if modifications have been detected since the last integration. The trigger can be configured to force a build even if have occurred
    /// to source control. The items to watch for modifications are specified with <link>Source Control Blocks</link>.
    /// </para>
    /// <para type="info">
    /// Like all triggers, the scheduleTrigger must be enclosed within a triggers element in the appropriate <link>Project Configuration Block</link>.
    /// </para>
    /// </summary>
    /// <title>Schedule Trigger</title>
    /// <version>1.0</version>
    /// <remarks>
    /// <para>
    /// Use the <b>buildCondition</b> property if you want to run a scheduled forced build.
    /// </para>
    /// <para type="warning">
    /// this class replaces the <b>PollingScheduleTrigger</b> and the <b>ForceBuildScheduleTrigger</b>.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// &lt;scheduleTrigger time="23:30" buildCondition="ForceBuild" name="Scheduled"&gt;
    /// &lt;weekDays&gt;
    /// &lt;weekDay&gt;Monday&lt;/weekDay&gt;
    /// &lt;/weekDays&gt;
    /// &lt;/scheduleTrigger&gt;
    /// </code>
    /// </example>
    [ReflectorType("scheduleTrigger")]
    public class ScheduleTrigger : ITrigger, IConfigurationValidation
    {
        private string name;
        private DateTimeProvider dtProvider;
        private TimeSpan integrationTime;
        private DateTime nextBuild;
        private bool triggered;
        private Int32 randomOffSetInMinutesFromTime = 0;
        Random randomizer = new Random();

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleTrigger"/> class.
        /// </summary>
        public ScheduleTrigger()
            : this(new DateTimeProvider())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleTrigger"/> class.
        /// </summary>
        /// <param name="dtProvider">The dt provider.</param>
        public ScheduleTrigger(DateTimeProvider dtProvider)
        {
            this.dtProvider = dtProvider;
            this.BuildCondition = BuildCondition.IfModificationExists;
            WeekDays = (DayOfWeek[])DayOfWeek.GetValues(typeof(DayOfWeek));
        }

        /// <summary>
        /// The time of day that the build should run at. The time should be specified in a locale-specific format (ie. H:mm am/pm is acceptable for US locales.)
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("time")]
        public virtual string Time
        {
            get { return integrationTime.ToString(); }
            set
            {
                try
                {
                    integrationTime = TimeSpan.Parse(value);
                }
                catch (Exception ex)
                {
                    string msg = "Unable to parse daily schedule integration time: {0}.  The integration time should be specified in the format: {1}.";
                    throw new ConfigurationException(string.Format(CultureInfo.CurrentCulture, msg, value, CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern), ex);
                }
            }
        }

        /// <summary>
        /// Adds a random amount of minutes between 0 and set value to the time. This is mainly meant for spreading the load of actions to a central server. 
        /// Value must be between 0 and 59.
        /// </summary>
        /// <version>1.4</version>
        /// <default>0</default>
        [ReflectorProperty("randomOffSetInMinutesFromTime", Required = false)]
        public int RandomOffSetInMinutesFromTime
        {
            get { return randomOffSetInMinutesFromTime; }
            set
            {
                randomOffSetInMinutesFromTime = value;
                if (randomOffSetInMinutesFromTime < 0 || randomOffSetInMinutesFromTime >= 60)
                    throw new ConfigurationException("randomOffSetInMinutesFromTime must be in the range 0 - 59");
            }
        }

        /// <summary>
        /// The name of the trigger. This name is passed to external tools as a means to identify the trigger that requested the build.
        /// </summary>
        /// <version>1.1</version>
        /// <default>ScheduleTrigger</default>
        [ReflectorProperty("name", Required = false)]
        public string Name
        {
            get
            {
                if (name == null) name = GetType().Name;
                return name;
            }
            set { name = value; }
        }

        /// <summary>
        /// The condition that should be used to launch the integration. By default, this value is <b>IfModificationExists</b>, meaning that an integration will
        /// only be triggered if modifications have been detected. Set this attribute to <b>ForceBuild</b> in order to ensure that a build should be launched 
        /// regardless of whether new modifications are detected. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>IfModificationExists</default>
        [ReflectorProperty("buildCondition", Required = false)]
        public BuildCondition BuildCondition { get; set; }

        /// <summary>
        /// The week days on which the build should be run (eg. Monday, Tuesday). By default, all days of the week are set.
        /// </summary>
        /// <version>1.0</version>
        /// <default>Monday-Sunday</default>
        [ReflectorProperty("weekDays", Required = false)]
        public DayOfWeek[] WeekDays { get; set; }

        private void SetNextIntegrationDateTime()
        {

            if (integrationTime.Minutes + RandomOffSetInMinutesFromTime >= 60)
                throw new ConfigurationException(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Scheduled time {0}:{1} + randomOffSetInMinutesFromTime {2} would exceed the hour, this is not allowed", integrationTime.Hours, integrationTime.Minutes, RandomOffSetInMinutesFromTime));

            DateTime now = dtProvider.Now;
            nextBuild = new DateTime(now.Year, now.Month, now.Day, integrationTime.Hours, integrationTime.Minutes, 0, 0);

            if (randomOffSetInMinutesFromTime > 0)
            {
                Int32 randomNumber = randomizer.Next(randomOffSetInMinutesFromTime);
                nextBuild = nextBuild.AddMinutes(randomNumber);
            }

            if (now >= nextBuild)
            {
                nextBuild = nextBuild.AddDays(1);
            }

            nextBuild = CalculateNextIntegrationTime(nextBuild);
        }

        private DateTime CalculateNextIntegrationTime(DateTime nextIntegration)
        {
            while (true)
            {
                if (IsValidWeekDay(nextIntegration.DayOfWeek))
                    break;
                nextIntegration = nextIntegration.AddDays(1);
            }
            return nextIntegration;
        }

        private bool IsValidWeekDay(DayOfWeek nextIntegrationDay)
        {
            return Array.IndexOf(WeekDays, nextIntegrationDay) >= 0;
        }

        /// <summary>
        /// Integrations the completed.	
        /// </summary>
        /// <remarks></remarks>
        public virtual void IntegrationCompleted()
        {
            if (triggered)
            {
                SetNextIntegrationDateTime();
            }
            triggered = false;
        }

        /// <summary>
        /// Gets the next build.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public DateTime NextBuild
        {
            get
            {
                if (nextBuild == DateTime.MinValue)
                {
                    SetNextIntegrationDateTime();
                }
                return nextBuild;
            }
        }

        /// <summary>
        /// Fires this instance.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public IntegrationRequest Fire()
        {
            DateTime now = dtProvider.Now;
            if (now > NextBuild && IsValidWeekDay(now.DayOfWeek))
            {
                triggered = true;
                return new IntegrationRequest(BuildCondition, Name, null);
            }
            return null;
        }


        void IConfigurationValidation.Validate(IConfiguration configuration, ConfigurationTrace parent, IConfigurationErrorProcesser errorProcesser)
        {
            string projectName = "(Unknown)";

            var project = parent.GetAncestorValue<Project>();
            if (project != null)
            {
                projectName = project.Name;
            }

            if (integrationTime.Minutes + RandomOffSetInMinutesFromTime >= 60)
            {
                errorProcesser.ProcessError("Scheduled time {0}:{1} + randomOffSetInMinutesFromTime {2} would exceed the hour, this is not allowed. Conflicting project {3}", integrationTime.Hours, integrationTime.Minutes, RandomOffSetInMinutesFromTime, projectName);
            }
        }

    }
}