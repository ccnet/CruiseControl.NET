using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks.Test
{
	[TestFixture]
	public class DataTaskResultTest : CustomAssertion
	{
		[Test]
		public void DataSetIsValid()
		{
			string data = "foo";
			DataTaskResult result=new DataTaskResult(data);
			Assert.AreEqual(data,result.Data);
		}
	}
}
