using System;

namespace Objection
{
	public class ObjectionObject : DecoratableByType
	{
		private readonly object instance;
		private ObjectionType decorator;

		public ObjectionObject(object instance)
		{
			this.instance = instance;
		}

		public DecoratableByType Decorate(Type type)
		{
			decorator = new ObjectionType(type);
			return decorator;
		}

		public object Instance
		{
			get { return instance; }
		}

		public ObjectionType Decorator
		{
			get { return decorator; }
		}
	}
}
	