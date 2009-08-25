using System;
using System.Globalization;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Triggers
{
    [ReflectorType("scheduleTrigger")]
    public class ScheduleTrigger : ITrigger, IConfigurationValidation
    {
        private string name;
        private DateTimeProvider dtProvider;
        private TimeSpan integrationTime;
        private DateTime nextBuild;
        private DateTime previousBuild;
        private bool triggered;
        private Int32 randomOffSetInMinutesFromTime = 0;
        Random randomizer = new Random();

        public ScheduleTrigger()
            : this(new DateTimeProvider())
        {
        }

        public ScheduleTrigger(DateTimeProvider dtProvider)
        {
            this.dtProvider = dtProvider;
        }

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
                    throw new ConfigurationException(string.Format(msg, value, CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern), ex);
                }
            }
        }

        [ReflectorProperty("randomOffSetInMinutesFromTime", Required = false)]
        public Int32 RandomOffSetInMinutesFromTime
        {
            get { return randomOffSetInMinutesFromTime; }
            set
            {
                randomOffSetInMinutesFromTime = value;
                if (randomOffSetInMinutesFromTime < 0 || randomOffSetInMinutesFromTime >= 60)
                    throw new ConfigurationException("randomOffSetInMinutesFromTime must be in the range 0 - 59");
            }
        }


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

        [ReflectorProperty("buildCondition", Required = false)]
        public BuildCondition BuildCondition = BuildCondition.IfModificationExists;

        [ReflectorArray("weekDays", Required = false)]
        public DayOfWeek[] WeekDays = (DayOfWeek[])DayOfWeek.GetValues(typeof(DayOfWeek));

        private void SetNextIntegrationDateTime()
        {

            if (integrationTime.Minutes + RandomOffSetInMinutesFromTime >= 60)
                throw new ConfigurationException(String.Format("Scheduled time {0}:{1} + randomOffSetInMinutesFromTime {2} would exceed the hour, this is not allowed", integrationTime.Hours, integrationTime.Minutes, RandomOffSetInMinutesFromTime));

            DateTime now = dtProvider.Now;
            nextBuild = new DateTime(now.Year, now.Month, now.Day, integrationTime.Hours, integrationTime.Minutes, 0, 0);

            if (randomOffSetInMinutesFromTime > 0)
            {
                Int32 randomNumber = randomizer.Next(randomOffSetInMinutesFromTime);
                nextBuild = nextBuild.AddMinutes(randomNumber);
            }

            if (now >= nextBuild || now.Date == previousBuild.Date)
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

        public virtual void IntegrationCompleted()
        {
            if (triggered)
            {
                previousBuild = dtProvider.Now;
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
            DateTime now = dtProvider.Now;
            if (now > NextBuild && IsValidWeekDay(now.DayOfWeek))
            {
                triggered = true;
                return new IntegrationRequest(BuildCondition, Name, null);
            }
            return null;
        }


        void IConfigurationValidation.Validate(IConfiguration configuration, object parent, IConfigurationErrorProcesser errorProcesser)
        {
            string projectName = "(Unknown)";

            if (parent is Project)
            {
                Project parentProject = parent as Project;

                projectName = parentProject.Name;
            }

            if (integrationTime.Minutes + RandomOffSetInMinutesFromTime >= 60)
                errorProcesser.ProcessError("Scheduled time {0}:{1} + randomOffSetInMinutesFromTime {2} would exceed the hour, this is not allowed. Conflicting project {3}", integrationTime.Hours, integrationTime.Minutes, RandomOffSetInMinutesFromTime, projectName);
        }

    }
}