using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
	[TestFixture]
	public class DataTaskResultTest : CustomAssertion
	{
		[Test]
		public void DataSetIsValid()
		{
			string data = "foo";
			DataTaskResult result = new DataTaskResult(data);
			Assert.AreEqual(data, result.Data);
		}
	}
}