using System;
using System.Collections;
using NMock;

namespace ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils
{
	public class Mockery
	{
		IList mocks = new ArrayList();
		
		public IMock NewStrictMock(Type type)
		{
			DynamicMock mock = new DynamicMock(type);
			mock.Strict = true;
			mocks.Add(mock);
			return mock;
		}
		
		public void Verify()
		{
			foreach (IMock mock in mocks)
			{
				mock.Verify();
			}
		}

        public IMock NewDynamicMock(Type type)
        {
            DynamicMock mock = new DynamicMock(type);
            mock.Strict = false;
            mocks.Add(mock);
            return mock;
        }



	}
}