using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Util;


namespace ThoughtWorks.CruiseControl.Core.Triggers
{
    /// <summary>
    /// A crontab expression are a very compact way to express a recurring schedule. A single expression is composed of 5 space-delimited fields :
    /// <para> MINUTES HOURS DAYS MONTHS DAYS-OF-WEEK</para>
    /// <list type="bullet">
    /// <item><description>A single wildcard (*), which covers all values for the field. So a * in days means all days of a month (which varies with month and year). </description></item>
    /// <item><description>A single value, e.g. 5. Naturally, the set of values that are valid for each field varies.  </description></item>
    /// <item><description>A comma-delimited list of values, e.g. 1,2,3,4. The list can be unordered as in 3,4,2,6,1.  </description></item>
    /// <item><description>A range where the minimum and maximum are separated by a dash, e.g. 1-10. You can also specify these in the wrong order and they will be fixed. So 10-5 will be treated as 5-10.  </description></item>
    /// <item><description>An interval specification using a slash, e.g. */4. This means every 4th value of the field. You can also use it in a range, as in 1-6/2. </description></item>
    /// <item><description>You can also mix all of the above, as in: 1-5,10,12,20-30/5 </description></item>   
    /// </list> 
    /// </summary>
    /// <title>Cron Trigger</title>
    /// <version>1.6</version>
    /// <example>
    /// 1st januari of each year
    /// <code>
    /// &lt;cronTrigger&gt;
    /// &lt;cronExpression&gt;* * 1 1 *&lt;/cronExpression&gt;
    /// &lt;/cronTrigger&gt;
    /// </code>
    /// </example>
    /// <example>
    /// 12th of every month at 8 am
    /// <code>
    /// &lt;cronTrigger&gt;
    /// &lt;cronExpression&gt;0 8 12 * *&lt;/cronExpression&gt;
    /// &lt;/cronTrigger&gt;
    /// </code>
    /// </example>
    /// <example>
    /// Every 5 minutes between 06:00 and 18:00
    /// <code>
    /// &lt;cronTrigger&gt;
    /// &lt;cronExpression&gt;0/5 6-18 * * *&lt;/cronExpression&gt;
    /// &lt;/cronTrigger&gt;
    /// </code>
    /// </example>

    [ReflectorType("cronTrigger")]
    public class CronTrigger : ITrigger
    {
        private NCrontab.CrontabSchedule schedule;
        private DateTime nextBuild = DateTime.MinValue;
        private DateTimeProvider dtProvider;
        private bool triggered;


        public CronTrigger()
            : this(new DateTimeProvider())
        {
        }

        public CronTrigger(DateTimeProvider dtProvider)
        {
            this.dtProvider = dtProvider;
            this.BuildCondition = BuildCondition.IfModificationExists;
            this.StartDate = DateTime.Now;
            this.EndDate = DateTime.MaxValue;
        }


        #region ITrigger

        public void IntegrationCompleted()
        {
            if (triggered)
            {
                SetNextIntegrationDateTime();
            }
            triggered = false;

        }

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

        public IntegrationRequest Fire()
        {
            schedule = NCrontab.CrontabSchedule.Parse(CronExpression);
            DateTime now = dtProvider.Now;

            if (now > NextBuild)
            {
                triggered = true;
                return new IntegrationRequest(BuildCondition, Name, null);
            }
            return null;

        }

        #endregion

        /// <summary>
        /// The expression in Cron format when to trigger the build
        /// see http://code.google.com/p/ncrontab/wiki/CrontabExpression for an example
        /// </summary>
        [ReflectorProperty("cronExpression", Required = true)]
        public string CronExpression { get; set; }


        private string name;
        /// <summary>
        /// The name of the trigger. This name is passed to external tools as a means to identify the trigger that requested the build.
        /// </summary>
        /// <version>1.6</version>
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
        /// The startdate to use for the cron schedule.
        /// Defaults to now
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The end data to use for the cron schedule
        /// Defaults to DateTime.MaxDate
        /// </summary>
        public DateTime EndDate { get; set; }


        private void SetNextIntegrationDateTime()
        {
            nextBuild = schedule.GetNextOccurrence(StartDate, EndDate);
        }


    }
}
