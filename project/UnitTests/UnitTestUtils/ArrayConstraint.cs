using System;
using NMock.Constraints;

namespace ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils
{
	public class ArrayConstraint : IConstraint
	{
		private readonly object[] expected;
		private string message = "";

		public ArrayConstraint(object[] expected)
		{
			this.expected = expected;
		}

		public bool Eval(object val)
		{
			if (!(val is Array))
			{
				message = "Expected Array but was " + val.GetType().FullName;
				return false;
			}

			object[] obtained = (object[]) val;

			if (expected.Length != obtained.Length)
			{
				message = string.Format("Expected {0} values but found {1}", expected.Length, obtained.Length);
				return false;
			}

			for (int i = 0; i < expected.Length; i++)
			{
				if (expected[i] is IConstraint)
				{
					IConstraint ic = (IConstraint) expected[i];
					bool constraint = ic.Eval(obtained[i]);
					message = ic.Message;
					return constraint;
				}

				if (expected[i] is Array)
				{
					ArrayConstraint ac = new ArrayConstraint((object[]) expected[i]);
					bool constraint = ac.Eval(obtained[i]);
					message = ac.Message;
					return constraint;
				}

				if (!expected[i].Equals(obtained[i]))
				{
					message = string.Format("Expected {0} but was {1}", expected[i], obtained[i]);
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
