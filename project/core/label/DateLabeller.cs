using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using System.Globalization;

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

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DateLabeller"/> class.
        /// </summary>
        public DateLabeller()
            : this(new DateTimeProvider())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateLabeller"/> class.
        /// </summary>
        /// <param name="dateTimeProvider">The date time provider.</param>
        public DateLabeller(DateTimeProvider dateTimeProvider)
        {
            this.dateTimeProvider = dateTimeProvider;
            this.YearFormat = "0000";
            this.MonthFormat = "00";
            this.DayFormat = "00";
            RevisionFormat = "000";
        }
        #endregion

        /// <summary>
        /// The format for the year part.
        /// </summary>
        /// <version>1.0</version>
        /// <default>0000</default>
        [ReflectorProperty("yearFormat", Required = false)]
        public string YearFormat { get; set; }

        /// <summary>
        /// The format for the month part.
        /// </summary>
        /// <version>1.0</version>
        /// <default>00</default>
        [ReflectorProperty("monthFormat", Required = false)]
        public string MonthFormat { get; set; }

        /// <summary>
        /// The format for the day part.
        /// </summary>
        /// <version>1.0</version>
        /// <default>000</default>
        [ReflectorProperty("dayFormat", Required = false)]
        public string DayFormat { get; set; }

        /// <summary>
        /// The format for the revision part.
        /// </summary>
        /// <version>1.0</version>
        /// <default>000</default>
        [ReflectorProperty("revisionFormat", Required = false)]
        public string RevisionFormat { get; set; }

        /// <summary>
        /// Generates the specified integration result.	
        /// </summary>
        /// <param name="integrationResult">The integration result.</param>
        /// <returns></returns>
        /// <remarks></remarks>
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
            return string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0}.{1}.{2}.{3}",
                   now.Year.ToString(YearFormat, CultureInfo.CurrentCulture), now.Month.ToString(MonthFormat, CultureInfo.CurrentCulture), now.Day.ToString(DayFormat, CultureInfo.CurrentCulture), revision.ToString(RevisionFormat, CultureInfo.CurrentCulture));
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