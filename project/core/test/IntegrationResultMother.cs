using System;
using System.IO;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Test
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

		public static IntegrationResult CreateSuccessful(DateTime startDate)
		{
			return Create(true, startDate);
		}

		public static IntegrationResult CreateFailed()
		{
			return Create(false, DateTime.Now);
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
