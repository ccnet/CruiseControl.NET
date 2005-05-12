using System;
using System.IO;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	public class IntegrationResultMother
	{
		public static IntegrationResult Create(bool succeeded)
		{
			return Create(succeeded, DateTime.Now);
		}

		public static IntegrationResult Create(IntegrationStatus status)
		{
			return Create(status, DateTime.Now);
		}

		public static IntegrationResult Create(bool succeeded, DateTime date)
		{
			IntegrationStatus status = (succeeded) ? IntegrationStatus.Success : IntegrationStatus.Failure;
			return Create(status, date);
		}

		public static IntegrationResult Create(IntegrationStatus status, DateTime date)
		{
			IntegrationResult result = new IntegrationResult("test", Path.GetTempPath());
			result.Status = status;
			result.StartTime = date;
			result.EndTime = date;
			return result;			
		}

		public static IntegrationResult CreateSuccessful()
		{
			return Create(true, DateTime.Now);
		}

		public static IntegrationResult CreateSuccessful(string label)
		{
			IntegrationResult result = CreateSuccessful();
			result.Label = label;
			return result;
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

		public static IntegrationResult CreateSuccessful(DateTime startDate)
		{
			return Create(true, startDate);
		}

		public static IntegrationResult CreateSuccessful(Modification[] modifications)
		{
			IntegrationResult result = Create(true, DateTime.Now);
			result.Modifications = modifications;
			return result;
		}

		public static IntegrationResult CreateFailed()
		{
			return Create(false, DateTime.Now);
		}

		public static IntegrationResult CreateFailed(string label)
		{
			IntegrationResult result = CreateFailed();
			result.Label = label;
			return result;
		}

		public static IntegrationResult CreateFixed()
		{
			IntegrationResult result = CreateSuccessful();
			result.LastIntegrationStatus = IntegrationStatus.Failure;
			return result;
		}

		public static IntegrationResult CreateExceptioned()
		{
			IntegrationResult result = new IntegrationResult();
			result.Status = IntegrationStatus.Exception;
			return result;
		}

		public static IntegrationResult CreateStillSuccessful()
		{
			IntegrationResult result = CreateSuccessful();
			result.LastIntegrationStatus = IntegrationStatus.Success;
			return result;
		}

	}
}
