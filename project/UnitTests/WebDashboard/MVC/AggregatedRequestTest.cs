using System.Collections.Specialized;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.MVC
{
	[TestFixture]
	public class AggregatedRequestTest
	{
		private DynamicMock request1Mock;
		private DynamicMock request2Mock;
		private AggregatedRequest aggregatedRequest;

		[SetUp]
		public void Setup()
		{
			request1Mock = new DynamicMock(typeof(IRequest));
			request2Mock = new DynamicMock(typeof(IRequest));
			aggregatedRequest = new AggregatedRequest((IRequest) request1Mock.MockInstance, (IRequest) request2Mock.MockInstance);
		}

		private void VerifyAll()
		{
			request1Mock.Verify();
			request2Mock.Verify();
		}

		[Test]
		public void ShouldGetParameterFromFirstRequestIfInFirstRequest()
		{
			// Setup
			request1Mock.ExpectAndReturn("FindParameterStartingWith", "value", "prefix");
			request2Mock.ExpectNoCall("FindParameterStartingWith", typeof(string));

			// Execute
			string value = aggregatedRequest.FindParameterStartingWith("prefix");

			// Verify
			Assert.AreEqual("value", value);
			VerifyAll();
		}

		[Test]
		public void ShouldGetParameterFromSecondRequestIfNotInFirstRequest()
		{
			// Setup
			request1Mock.ExpectAndReturn("FindParameterStartingWith", "", "prefix");
			request2Mock.ExpectAndReturn("FindParameterStartingWith", "value", "prefix");

			// Execute
			string value = aggregatedRequest.FindParameterStartingWith("prefix");

			// Verify
			Assert.AreEqual("value", value);
			VerifyAll();
		}

		[Test]
		public void ShouldGetTextFromFirstRequestIfInFirstRequest()
		{
			// Setup
			request1Mock.ExpectAndReturn("GetText", "value", "id");
			request2Mock.ExpectNoCall("GetText", typeof(string));

			// Execute
			string value = aggregatedRequest.GetText("id");

			// Verify
			Assert.AreEqual("value", value);
			VerifyAll();
		}

		[Test]
		public void ShouldGetTextFromSecondRequestIfNotInFirstRequest()
		{
			// Setup
			request1Mock.ExpectAndReturn("GetText", "", "id");
			request2Mock.ExpectAndReturn("GetText", "value", "id");

			// Execute
			string value = aggregatedRequest.GetText("id");

			// Verify
			Assert.AreEqual("value", value);
			VerifyAll();
		}

		// Checked defaults to 'false' for IRequest
		[Test]
		public void ShouldReturnTrueForGetCheckedIfTrueForFirstRequest()
		{
			// Setup
			request1Mock.ExpectAndReturn("GetChecked", true, "id");
			request2Mock.ExpectNoCall("GetChecked", typeof(string));

			// Execute
			bool value = aggregatedRequest.GetChecked("id");

			// Verify
			Assert.AreEqual(true, value);
			VerifyAll();
		}

		[Test]
		public void ShouldReturnTrueForGetCheckedIfTrueForSecondRequest()
		{
			// Setup
			request1Mock.ExpectAndReturn("GetChecked", false, "id");
			request2Mock.ExpectAndReturn("GetChecked", true, "id");

			// Execute
			bool value = aggregatedRequest.GetChecked("id");

			// Verify
			Assert.AreEqual(true, value);
			VerifyAll();
		}

		[Test]
		public void ShouldReturnFalseForGetCheckedIfFalseForBothRequests()
		{
			// Setup
			request1Mock.ExpectAndReturn("GetChecked", false, "id");
			request2Mock.ExpectAndReturn("GetChecked", false, "id");

			// Execute
			bool value = aggregatedRequest.GetChecked("id");

			// Verify
			Assert.AreEqual(false, value);
			VerifyAll();
		}

		[Test]
		public void ShouldReturnFirstValueForGetIntIfFirstValueNotDefaultValue()
		{
			// Setup
			request1Mock.ExpectAndReturn("GetInt", 77, "id", -100);
			request2Mock.ExpectNoCall("GetInt", typeof(string), typeof(int));

			// Execute
			int value = aggregatedRequest.GetInt("id", -100);

			// Verify
			Assert.AreEqual(77, value);
			VerifyAll();
		}

		[Test]
		public void ShouldReturnSecondValueForGetIntIfFirstValueDefaultValue()
		{
			// Setup
			request1Mock.ExpectAndReturn("GetInt", -100, "id", -100);
			request2Mock.ExpectAndReturn("GetInt", 88, "id", -100);

			// Execute
			int value = aggregatedRequest.GetInt("id", -100);

			// Verify
			Assert.AreEqual(88, value);
			VerifyAll();
		}

		[Test]
		public void ShouldReturnDefaultValueForGetIntIfBothValuesAreDefaultValue()
		{
			// Setup
			request1Mock.ExpectAndReturn("GetInt", -100, "id", -100);
			request2Mock.ExpectAndReturn("GetInt", -100, "id", -100);

			// Execute
			int value = aggregatedRequest.GetInt("id", -100);

			// Verify
			Assert.AreEqual(-100, value);
			VerifyAll();
		}

		[Test]
		public void ShouldReturnCombinedParamsWithFirstRequestTakingPriority()
		{
			// Setup
			NameValueCollection request1Params = new NameValueCollection();
			NameValueCollection request2Params = new NameValueCollection();
			request1Params["only1"] = "value1";
			request2Params["only2"] = "value2";
			request1Params["both"] = "value01";
			request2Params["both"] = "value02";

			request1Mock.ExpectAndReturn("Params", request1Params);
			request2Mock.ExpectAndReturn("Params", request2Params);

			// Execute
			NameValueCollection returnedParams = aggregatedRequest.Params;

			Assert.AreEqual(3, returnedParams.Keys.Count);
			Assert.AreEqual("value1", returnedParams["only1"]);
			Assert.AreEqual("value2", returnedParams["only2"]);
			Assert.AreEqual("value01", returnedParams["both"]);
			Assert.AreEqual(1, returnedParams.GetValues("both").Length);
			VerifyAll();
		}
	}
}
