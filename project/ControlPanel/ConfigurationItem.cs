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
	public interface IConfigurationItem
	{
		string Name { get; }
		string ValueAsString { get; set; }
		ConfigurationItemCollection Items { get; }
		bool CanHaveChildren { get; }
	}

	public abstract class ConfigurationItem : IConfigurationItem
	{
		private string _name;
		private object _parent;
		protected PropertyInfo _property;
		private ConfigurationItemCollection _items = new ConfigurationItemCollection();

		public ConfigurationItem(string name, object parent, PropertyInfo property) 
		{
			_name = name;
			_parent = parent;
			_property = property;
		}

		public string Name
		{
			get { return _name; }
		}

		public virtual object Value
		{
			get { return _property.GetValue(_parent, null); }
			set { _property.SetValue(_parent, value, null); }
		}

		public abstract string ValueAsString { get; set; }

		public virtual string [] AvailableValues
		{
			get { return null; }
		}

		public ConfigurationItemCollection Items
		{
			get { return _items; }
		}

		public virtual bool CanHaveChildren
		{
			get { return false; }
		}

		public override string ToString()
		{
			return Name + "=" + ValueAsString;
		}
	}

	public class TextItem : ConfigurationItem
	{
		public TextItem(string name, object parent, PropertyInfo property) : base(name, parent, property) {}

		public override string ValueAsString
		{
			get { return (string) Value; }
			set { Value = value; }
		}
	}

	public class PrimitiveItem : ConfigurationItem
	{
		private MethodInfo _parse;

		public PrimitiveItem(string name, object parent, PropertyInfo property) 
			: base(name, parent, property) 
		{
			_parse = property.PropertyType.GetMethod("Parse", BindingFlags.Static);
		}

		public override string ValueAsString
		{
			get { return base.Value.ToString(); }
			set { Value = _parse.Invoke(null, new object []{value}); }
		}
	}

	public class ReflectorDecoratedItem : ConfigurationItem
	{
		private Type [] _availableTypes;

		public ReflectorDecoratedItem(string name, object parent, PropertyInfo property, Type [] availableTypes) 
			: base(name, parent, property) 
		{
			_availableTypes = availableTypes;
			Items.LoadPropertiesOf(Value);
		}

		public override object Value
		{
			get { return base.Value; }
			set
			{
				base.Value = value;
				Items.Clear();
				Items.LoadPropertiesOf(Value);
			}
		}

		public override string ValueAsString
		{
			get { return Value != null ? GetReflectorName(Value.GetType()) : ""; }
			set
			{
				if (value == "") 
				{
					Value = null;
					return;
				}
				foreach (Type type in _availableTypes)
				{
					if (value == GetReflectorName(type))
					{
						Value = Activator.CreateInstance(type);
						return;
					}
				}
				throw new ArgumentException("didn't find " + value);
			}
		}

		public override string[] AvailableValues
		{
			get
			{
				ArrayList values = new ArrayList();

				IReflectorAttribute attr = GetReflectorAttributeIfThereIsOne(_property);
				if (attr is ReflectorPropertyAttribute && !((ReflectorPropertyAttribute)attr).Required) 
				{
					values.Add("");
				}

				foreach (Type type in _availableTypes)
				{
					values.Add(GetReflectorName(type));
				}
				values.Sort();

				return (string []) values.ToArray(typeof(string));
			}
		}

		public static IReflectorAttribute GetReflectorAttributeIfThereIsOne(ICustomAttributeProvider provider)
		{
			IReflectorAttribute attributeToReturn = null;
			foreach (object attribute in provider.GetCustomAttributes(true))
			{
				if (attribute is IReflectorAttribute)
				{
					if (attributeToReturn != null) throw new Exception("I thought you could only have one reflector attribute");
					attributeToReturn = (IReflectorAttribute) attribute;
				}
			}
			return attributeToReturn;
		}

		private string GetReflectorName(ICustomAttributeProvider provider) 
		{
			IReflectorAttribute attribute = GetReflectorAttributeIfThereIsOne(provider);
			if (attribute != null) 
				return attribute.Name;
			else
				throw new ArgumentException(provider + "doesn't have a reflector attribute");
		}

		public override bool CanHaveChildren
		{
			get { return true; }
		}

	}
}
