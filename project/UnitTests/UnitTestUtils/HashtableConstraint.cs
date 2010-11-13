using System;
using System.Collections;
using NMock.Constraints;

namespace ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils
{
	public class HashtableConstraint : IConstraint
	{
		private readonly Hashtable expected;
		private string message = "";

		public HashtableConstraint(Hashtable expected)
		{
			this.expected = expected;
		}

		public bool Eval(object val)
		{
			if (!(val is Hashtable))
			{
				message = "Expected Hashtable but was " + val.GetType().FullName;
				return false;
			}

			Hashtable obtained = val as Hashtable;

			if (expected.Keys.Count != obtained.Keys.Count)
			{
				message = string.Format(System.Globalization.CultureInfo.CurrentCulture,"Expected {0} keys but found {1}", expected.Keys.Count, obtained.Keys.Count);
				return false;
			}

			foreach (object expectedKey in expected.Keys)
			{
				if (!obtained.ContainsKey(expectedKey))
				{
					message = "Expected to contain key " + expectedKey;
					return false;
				}

				if (expected[expectedKey] is IConstraint)
				{
					IConstraint ic = (IConstraint) expected[expectedKey];
					bool constraint = ic.Eval(obtained[expectedKey]);
					message = ic.Message;
					return constraint;
				}

				if (expected[expectedKey] is Array)
				{
					ArrayConstraint ac = new ArrayConstraint((object[]) expected[expectedKey]);
					bool constraint = ac.Eval(obtained[expectedKey]);
					message = ac.Message;
					return constraint;
				}

				if (!object.Equals(expected[expectedKey], obtained[expectedKey]))
				{
					message =
						string.Format(System.Globalization.CultureInfo.CurrentCulture,"Expected {0} to be {1} but was {2}", expectedKey, expected[expectedKey], obtained[expectedKey]);
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
