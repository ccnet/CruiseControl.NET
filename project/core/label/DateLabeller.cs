using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Label
{
	/// <summary>Generates label numbers according to Ccp standards.</summary>
	[ReflectorType("dateLabeller")]
	public class DateLabeller : ILabeller
	{
		private readonly DateTimeProvider dateTimeProvider;

		public DateLabeller() : this(new DateTimeProvider())
		{}

		public DateLabeller(DateTimeProvider dateTimeProvider)
		{
			this.dateTimeProvider = dateTimeProvider;
		}

		public string Generate(IIntegrationResult integrationResult)
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
			return new Version(now.Year, now.Month, now.Day, revision).ToString();
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

		public void Run(IIntegrationResult result)
		{
			result.Label = Generate(result);
		}
	}
}