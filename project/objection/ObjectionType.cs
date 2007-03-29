using System;

namespace Objection
{
	public class ObjectionType : DecoratableByType
	{
		private readonly Type type;
		private ObjectionType decorator;

		public ObjectionType(Type type)
		{
			this.type = type;
		}

		public DecoratableByType Decorate(Type type)
		{
			decorator = new ObjectionType(type);
			return decorator;
		}

		public Type Type
		{
			get { return type; }
		}

		public ObjectionType Decorator
		{
			get { return decorator; }
		}
	}
}
