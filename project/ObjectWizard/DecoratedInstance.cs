using System;

namespace ObjectWizard
{
	public class DecoratedInstance : TypeDecoratorable
	{
		private readonly object instance;
		private DecoratedType decorator;

		public DecoratedInstance(object instance)
		{
			this.instance = instance;
		}

		public TypeDecoratorable Decorate(Type type)
		{
			decorator = new DecoratedType(type);
			return decorator;
		}

		public object Instance
		{
			get { return instance; }
		}

		public DecoratedType Decorator
		{
			get { return decorator; }
		}
	}
}
