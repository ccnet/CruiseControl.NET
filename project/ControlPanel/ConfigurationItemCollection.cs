using System;
using System.Collections;
using System.Reflection;

using NUnit.Framework;
using Exortech.NetReflector;

using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Label;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.ControlPanel
{
	public class ConfigurationItemCollection : DictionaryBase
	{
		public void LoadPropertiesOf(object obj) 
		{
			if (obj == null) return;
			foreach (PropertyInfo property in obj.GetType().GetProperties()) 
			{
				IReflectorAttribute attribute = ReflectorDecoratedItem.GetReflectorAttributeIfThereIsOne(property);
				if (attribute != null)
				{
					Type type = property.PropertyType;

					if (type == typeof(string)) 
					{
						Add(new TextItem(attribute.Name, obj, property));
					} 
					else if (type == typeof(int) ||
						type == typeof(double) ||
						type == typeof(bool))
					{
						Add(new PrimitiveItem(attribute.Name, obj, property));
					}
					else if (typeof(ICollection).IsAssignableFrom(type))
					{
						// can't handle collections yet
					}
					else
					{
						Add(new ReflectorDecoratedItem(attribute.Name, obj, property, GetTypesThatImplement(property.PropertyType)));
					}
				}
			}
		}

		// TODO: get netreflectortypetable to expose an enumerator - Jeremy
		public Type [] GetTypesThatImplement(Type someInterface) 
		{
			ArrayList types = new ArrayList();
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (Type type in assembly.GetTypes()) 
				{
					if (someInterface.IsAssignableFrom(type) && 
						ReflectorDecoratedItem.GetReflectorAttributeIfThereIsOne(type) != null &&
						type.Name.IndexOf("Mock") == -1)
					{
						types.Add(type);
					}
				}
			}
			return (Type []) types.ToArray(typeof(Type));
		}

		public new IEnumerator GetEnumerator() 
		{
			return InnerHashtable.Values.GetEnumerator();
		}
		
		public void Add(ConfigurationItem item) 
		{
			InnerHashtable[item.Name] = item;
		}

		public ConfigurationItem this[string name]
		{
			get { return (ConfigurationItem) InnerHashtable[name]; }
		}
	}
}
