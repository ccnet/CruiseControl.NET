using System;
using System.IO;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	public class IntegrationResultMother
	{
		public const string DefaultProjectName = "test";

		public static IntegrationResult Create(IntegrationStatus status)
		{
			return Create(status, status);
		}

		public static IntegrationResult Create(IntegrationStatus status, DateTime date)
		{
			return Create(status, status, date);
		}

		public static IntegrationResult Create(IntegrationStatus status, IntegrationStatus lastStatus)
		{
			return Create(status, lastStatus, DateTime.Now);
		}

		public static IntegrationResult Create(IntegrationStatus status, IntegrationStatus lastIntegrationStatus, DateTime date)
		{
			IntegrationResult result = Create(new IntegrationSummary(lastIntegrationStatus, null, null));
			result.Status = status;
			result.StartTime = date;
			result.EndTime = date;
			result.Label = "2.0";
			result.ArtifactDirectory = Path.GetTempPath();
			return result;
		}

		public static IntegrationResult Create(IntegrationSummary integrationSummary)
		{
			return new IntegrationResult(DefaultProjectName, Path.GetTempPath(), ModificationExistRequest(), integrationSummary);
		}

		public static IntegrationResult CreateSuccessful(DateTime startDate)
		{
			return Create(IntegrationStatus.Success, startDate);
		}

		public static IntegrationResult CreateSuccessful()
		{
			return CreateSuccessful(DateTime.Now);
		}

		public static IntegrationResult CreateSuccessful(string label)
		{
			IntegrationResult result = CreateSuccessful();
			result.Label = label;
			return result;
		}

		public static IntegrationResult CreateSuccessful(Modification[] modifications)
		{
			IntegrationResult result = CreateSuccessful();
			result.Modifications = modifications;
			return result;
		}

		public static IntegrationResult CreateStillSuccessful()
		{
			return Create(IntegrationStatus.Success);
		}

		public static IntegrationResult CreateUnknown()
		{
			return Create(IntegrationStatus.Unknown, DateTime.Now);
		}

		public static IntegrationResult CreateUnknown(string label)
		{
			IntegrationResult result = CreateUnknown();
			result.Label = label;
			return result;
		}

		public static IntegrationResult CreateFailed()
		{
			return Create(IntegrationStatus.Failure, IntegrationStatus.Success);
		}

		public static IntegrationResult CreateFailed(IntegrationStatus previousIntegrationStatus)
		{
			return Create(IntegrationStatus.Failure, previousIntegrationStatus, DateTime.Now);
		}


		public static IntegrationResult CreateFailed(string label)
		{
			IntegrationResult result = CreateFailed();
			result.Label = label;
			return result;
		}

		public static IIntegrationResult CreateStillFailing()
		{
			return Create(IntegrationStatus.Failure);
		}

		public static IntegrationResult CreateFixed()
		{
			IntegrationResult result = Create(IntegrationStatus.Success, IntegrationStatus.Failure, DateTime.Now);
			return result;
		}

		public static IntegrationResult CreateExceptioned()
		{
			IntegrationResult result = new IntegrationResult();
			result.Status = IntegrationStatus.Exception;
			return result;
		}

		public static IntegrationResult CreateInitial()
		{
			return IntegrationResult.CreateInitialIntegrationResult(DefaultProjectName, "");
		}

		private static IntegrationRequest ModificationExistRequest()
		{
			return new IntegrationFixture().ModificationExistRequest();
		}
	}
}