using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Label
{
	/// <summary>
    /// The Date Labeller is used to generate labels in the format "yyyy.mm.dd.build". Using the Date Labeller makes it easy for the user to identify and communicate the date that a particular build occurred.
    /// </summary>
    /// <title>Date Labeller</title>
    /// <version>1.0</version>
    /// <remarks>
    /// <para>
    /// The revision is increased on every build done at the same day, so if you do 2 builds on 2009/01/20, the first will be have label 2009.01.20.001,  and the second will be 2009.01.20.002 
    /// </para>
    /// <para>
    /// This labeller has been contributed by Andy Johnstone
    /// </para>
    /// </remarks>
    /// <example>
    /// <code title="Minimal Example">
    /// &lt;labeller type="dateLabeller" /&gt;
    /// </code>
    /// </example>
	[ReflectorType("dateLabeller")]
	public class DateLabeller
        : LabellerBase
	{
		private readonly DateTimeProvider dateTimeProvider;

        /// <summary>
        /// The format for the year part.
        /// </summary>
        /// <version>1.0</version>
        /// <default>0000</default>
        [ReflectorProperty("yearFormat", Required = false)]
        public string YearFormat = "0000";

        /// <summary>
        /// The format for the month part.
        /// </summary>
        /// <version>1.0</version>
        /// <default>00</default>
        [ReflectorProperty("monthFormat", Required = false)]
        public string MonthFormat = "00";

        /// <summary>
        /// The format for the day part.
        /// </summary>
        /// <version>1.0</version>
        /// <default>000</default>
        [ReflectorProperty("dayFormat", Required = false)]
        public string DayFormat = "00";

        /// <summary>
        /// The format for the revision part.
        /// </summary>
        /// <version>1.0</version>
        /// <default>000</default>
        [ReflectorProperty("revisionFormat", Required = false)]
        public string RevisionFormat = "000";


		public DateLabeller() : this(new DateTimeProvider())
		{}

		public DateLabeller(DateTimeProvider dateTimeProvider)
		{
			this.dateTimeProvider = dateTimeProvider;
		}

		public override string Generate(IIntegrationResult integrationResult)
		{
			DateTime now = dateTimeProvider.Now;

			Version version = ParseVersion(now, integrationResult.LastIntegration);

			int revision = version.Revision;
			if (now.Year == version.Major && now.Month == version.Minor && now.Day == version.Build)
			{
				revision += 1;
			}
			else
			{
				revision = 1;
			}
            return string.Format("{0}.{1}.{2}.{3}",
                   now.Year.ToString(YearFormat), now.Month.ToString(MonthFormat), now.Day.ToString(DayFormat), revision.ToString(RevisionFormat));
		}

		private Version ParseVersion(DateTime date, IntegrationSummary lastIntegrationSummary)
		{
			try
			{
				return new Version(lastIntegrationSummary.LastSuccessfulIntegrationLabel);
			}
			catch (SystemException)
			{
				return new Version(date.Year, date.Month, date.Day, 0);
			}
		}
	}
}