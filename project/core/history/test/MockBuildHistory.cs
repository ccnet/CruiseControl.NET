using System;

namespace tw.ccnet.core.history.test
{
	public class MockBuildHistory : IBuildHistory
	{
		private IntegrationResult _result;
		private IntegrationResult _savedResult;
		private bool _exists = false;
		private bool _saved = false;

		public IntegrationResult IntegrationResult
		{
			set { _result = value; }
		}

		public IntegrationResult SavedIntegrationResult
		{
			get { return _savedResult; }
		}

		public bool Exist
		{
			set { _exists = value; }
		}

		public IntegrationResult Load()
		{
			return _result;
		}

		public void Save(IntegrationResult result)
		{
			_saved = true;
			_savedResult = result;
		}

		public bool Saved
		{
			get { return _saved; }
		}

		public bool Exists()
		{
			return _exists;
		}

		public static MockBuildHistory Create()
		{
			return new MockBuildHistory();
		}

		public static MockBuildHistory Create(IntegrationResult result, bool exist)
		{
			MockBuildHistory history = Create();
			history.IntegrationResult = result;
			history.Exist = exist;
			return history;
		}
	}
}
