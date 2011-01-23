using System.Collections;
using System.Xml;
using NMock.Constraints;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Util
{
	public class HasChildElements : BaseConstraint
	{
		private readonly int count;

		public HasChildElements(int count)
		{
			this.count = count;
		}

		public override bool Eval(object val)
		{
			XmlElement element = XmlUtil.CreateDocumentElement(val.ToString());
			return element.ChildNodes.Count == count;
		}

		public override string Message
		{
			get { return string.Format(System.Globalization.CultureInfo.CurrentCulture,"does not have {0} child nodes.", count); }
		}
	}

	public class IsValidXml : CompoundConstraint
	{
		public override bool Eval(object val)
		{
			XmlUtil.VerifyXmlIsWellFormed(val.ToString());
			return base.Eval(val);
		}

		public override string Message
		{
			get { return "to be valid xml."; }
		}
	}

	public abstract class CompoundConstraint : BaseConstraint
	{
		private IList constraints = new ArrayList();

		public override bool Eval(object val)
		{
			foreach (BaseConstraint c in constraints)
			{
				if (! c.Eval(val))
				{
					return false;
				}
			}
			return true;
		}

		public IConstraint And(BaseConstraint constraint)
		{
			constraints.Add(constraint);
			return this;
		}
	}
}