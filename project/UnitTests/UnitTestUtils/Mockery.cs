using System;
using System.Collections;
using System.Collections.Generic;
using Moq;

namespace ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils
{
	public class Mockery
	{
		List<Mock> mocks = new List<Mock>();
		
		public Mock<T> NewStrictMock<T>() where T : class
		{
			Mock<T> mock = new Mock<T>(MockBehavior.Strict);
			mocks.Add(mock);
			return mock;
		}
		
		public void Verify()
		{
			foreach (Mock mock in mocks)
			{
				mock.Verify();
			}
		}

        public Mock<T> NewDynamicMock<T>() where T : class
        {
            Mock<T> mock = new Mock<T>(MockBehavior.Loose);
            mocks.Add(mock);
            return mock;
        }



	}
}