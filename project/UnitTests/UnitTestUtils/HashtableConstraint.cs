using System.Collections;
using NMock.Constraints;

namespace ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils
{
	public class HashtableConstraint : IConstraint
	{
		private readonly Hashtable expected;
		string message = "";

		public HashtableConstraint(Hashtable expected)
		{
			this.expected = expected;
		}

		public bool Eval(object val)
		{
			if (! (val is Hashtable))
			{
				message = "Expected Hashtable but was " + val.GetType().FullName;
				return false;
			}

			Hashtable other = val as Hashtable;
			if (expected.Keys.Count != other.Keys.Count)
			{
				message = string.Format("Expected {0} keys but found {1}", expected.Keys.Count, other.Keys.Count);
				return false;
			}

			foreach (object expectedKey in expected.Keys)
			{
				if (! other.ContainsKey(expectedKey))
				{
					message = "Expected to have key " + expectedKey.ToString();
					return false;
				}
				if (! expected[expectedKey].Equals(other[expectedKey]))
				{
					message = string.Format("Expected {0} to be {1} but was {2}", expectedKey.ToString(), expected[expectedKey].ToString(), other[expectedKey].ToString());
					return false;
				}
			}
				
			return true;
		}

		public object ExtractActualValue(object actual)
		{
			return actual;
		}

		public string Message
		{
			get { return message; }
		}
	}
}
