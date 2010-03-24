namespace ThoughtWorks.CruiseControl.Core.Triggers
{
    using System;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// <para>
    /// The Filter Trigger allows you to prevent builds from occurring at certain times or on certain days (such as when your source control
    /// repository is undergoing backup). It is used to decorate an existing trigger. For example, if you have set up a <link>Interval 
    /// Trigger</link> to cause a new build every 5 minutes, you can use the Filter Trigger to create a window during which the build will
    /// not run.
    /// The filter will exclude modifications that occur between the start time and the end time on the days specified. If the start time is
    /// greater than the end time then the filtered time will span across midnight. For example, if the start time is 23:00 and the end time
    /// is 3:00 then builds will be suppressed from 23:00-23:59 and 0:00-3:00 on the days specified.
    /// </para>
    /// <para type="info">
    /// Like all triggers, the scheduleTrigger must be enclosed within a triggers element in the appropriate <link>Project Configuration
    /// Block</link>.
    /// </para>
    /// <para type="warning">
    /// <title>Nested trigger syntax is different</title>
    /// As shown below, the configuration of the nested trigger is not the same as when using that trigger outside a filter trigger. When
    /// using the &lt;filterTrigger&gt; element, the inner trigger must be specified with the &lt;trigger&gt; element. You could not use the
    /// &lt;intervalTrigger&gt; trigger element in this example.
    /// </para>
    /// </summary>
    /// <title>Filter Trigger</title>
    /// <version>1.0</version>
    /// <remarks>
    /// <para type="info">
    /// Times should be specified in hh:mm or hh:mm:ss 24 hour format (i.e., ranging from 00:00:00 to 23:59:59).
    /// </para>
    /// <heading>Nested Filter Triggers</heading>
    /// <para>
    /// Sometimes you would like to suppress builds that occur either between certain times or on certain days or in multiple combinations
    /// of dates and times. To acheive this, you can nest multiple filter triggers. For example, the following xml configures a trigger to
    /// filter builds between 7pm and 7am on weekdays and at any time on Saturdays and Sundays.
    /// </para>
    /// <code>
    /// &lt;filterTrigger startTime="19:00" endTime="07:00"&gt;
    /// &lt;trigger type="filterTrigger" startTime="0:00" endTime="23:59:59"&gt;
    /// &lt;trigger type="intervalTrigger" name="continuous" seconds="900" buildCondition="ForceBuild"/&gt;
    /// &lt;weekDays&gt;
    /// &lt;weekDay&gt;Saturday&lt;/weekDay&gt;
    /// &lt;weekDay&gt;Sunday&lt;/weekDay&gt;
    /// &lt;/weekDays&gt;
    /// &lt;/trigger&gt;
    /// &lt;/filterTrigger&gt;
    /// </code>
    /// </remarks>
    /// <example>
    /// <code>
    /// &lt;filterTrigger startTime="23:30" endTime="23:45"&gt;
    /// &lt;trigger type="intervalTrigger" seconds="60" /&gt;
    /// &lt;weekDays&gt;
    /// &lt;weekDay&gt;Sunday&lt;/weekDay&gt;
    /// &lt;/weekDays&gt;
    /// &lt;/filterTrigger&gt;
    /// </code>
    /// </example>
    [ReflectorType("filterTrigger")]
	public class FilterTrigger : ITrigger
	{
		private readonly DateTimeProvider dtProvider;
        private TimeSpan startTime = new TimeSpan(0, 0, 0);
	    private TimeSpan endTime = new TimeSpan(23, 59, 59);
		private DayOfWeek[] weekDays = (DayOfWeek[]) DayOfWeek.GetValues(typeof (DayOfWeek));

		public FilterTrigger() : this(new DateTimeProvider())
		{
		}

		public FilterTrigger(DateTimeProvider dtProvider)
		{
			this.dtProvider = dtProvider;
		}

        /// <summary>
        /// The inner trigger to filter.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
		[ReflectorProperty("trigger", InstanceTypeKey="type")]
		public ITrigger InnerTrigger;

        /// <summary>
        /// The start of the filter window. Builds will not occur after this time and before the end time. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>00:00:00</default>
        [ReflectorProperty("startTime", Required = false)]
		public string StartTime
		{
			get { return startTime.ToString(); }
			set { startTime = ParseTime(value); }
		}

        /// <summary>
        /// The end of the filter window. Builds will not occur before this time and after the start time.
        /// </summary>
        /// <version>1.0</version>
        /// <default>23:59:59</default>
		[ReflectorProperty("endTime", Required = false)]
		public string EndTime
		{
			get { return endTime.ToString(); }
			set { endTime = ParseTime(value); }
		}

		private TimeSpan ParseTime(string timeString)
		{
			return TimeSpan.Parse(timeString);
		}

        /// <summary>
        /// The condition that will be returned if a build is requested during the filter window. The default value is <b>NoBuild</b>
        /// indicating that no build will be performed
        /// </summary>
        /// <default>NoBuild</default>
        /// <version>1.0</version>
		[ReflectorProperty("buildCondition", Required=false)]
		public BuildCondition BuildCondition = BuildCondition.NoBuild;

		private bool IsInFilterRange(DateTime now)
		{
			return IsDateInFilterWeekDays(now) && IsTimeInFilterTimeRange(now);
		}

		private bool IsDateInFilterWeekDays(DateTime dateTime)
		{
			return Array.IndexOf(WeekDays, dateTime.DayOfWeek) >= 0;
		}

		private bool IsTimeInFilterTimeRange(DateTime dateTime)
		{
			TimeSpan timeOfDay = dateTime.TimeOfDay;
			if (startTime < endTime){
				return timeOfDay >= startTime && dateTime.TimeOfDay <= endTime;
			} 
			else 
			{
				return !(timeOfDay <= startTime) || !(dateTime.TimeOfDay >= endTime);
			}
		}

		public void IntegrationCompleted()
		{
			InnerTrigger.IntegrationCompleted();
		}

		public DateTime NextBuild
		{
			get
			{
				DateTime innerTriggerBuild = InnerTrigger.NextBuild;
				if (IsInFilterRange(innerTriggerBuild))
				{
					DateTime nextBuild = new DateTime(innerTriggerBuild.Year, innerTriggerBuild.Month, innerTriggerBuild.Day);
					nextBuild += endTime;
					return nextBuild;
				}
				return innerTriggerBuild;
			}
		}

		public IntegrationRequest Fire()
		{
			DateTime now = dtProvider.Now;
			if (IsInFilterRange(now))
			{
				return null;
			}
			return InnerTrigger.Fire();
		}

        /// <summary>
        /// The week days on which the filter should be applied (eg. Monday, Tuesday). By default, all days of the week are set. The filter
        /// will have no effect on other days.
        /// </summary>
        /// <version>1.0</version>
        /// <default>Monday-Sunday</default>
        [ReflectorProperty("weekDays", Required = false)]
		public DayOfWeek[] WeekDays
		{
			get { return weekDays; }
			set
			{
				if (value.Length != 0)
				{
					weekDays = value;
				}
			}
		}
	}
}