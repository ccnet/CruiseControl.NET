using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public class TypeSpecificationWithType : ITypeSpecification
	{
		private readonly Type type;

		public Type Type
		{
			get { return type; }
		}

		public TypeSpecificationWithType(Type type)
		{
			this.type = type;
		}
	}
}
