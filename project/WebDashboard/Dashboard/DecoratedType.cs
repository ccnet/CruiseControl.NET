using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class DecoratedType : TypeDecoratorable
	{
		private readonly Type type;
		private DecoratedType decoratoratable;

		public DecoratedType(Type type)
		{
			this.type = type;
		}

		public TypeDecoratorable Decorate(Type type)
		{
			decoratoratable = new DecoratedType(type);
			return decoratoratable;
		}

		public Type Type
		{
			get { return type; }
		}

		public DecoratedType Decorator
		{
			get { return decoratoratable; }
		}
	}
}
