using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	// NOT CURRENTLY USED - Just a possible implementation of genericising the MVC - likely to be deleted
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
