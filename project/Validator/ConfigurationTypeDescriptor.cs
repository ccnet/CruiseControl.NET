namespace Validator
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.ComponentModel;
    using System.Reflection;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;

    /// <summary>
    /// Provides an implementation of <see cref="ICustomTypeDescriptor"/> to retrieve the configuration properties.
    /// </summary>
    public class ConfigurationTypeDescriptor
        : ICustomTypeDescriptor
    {
        #region Private fields
        private readonly PropertyDescriptorCollection properties;
        private readonly object value;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationTypeDescriptor"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ConfigurationTypeDescriptor(object value)
        {
            this.value = value;
            var descriptors = new List<PropertyDescriptor>();

            if (value != null)
            {
                var type = this.value.GetType();

                // Get all the fields
                var fields = type.GetFields();
                foreach (var field in fields)
                {
                    var name = this.GetReflectionName(field);
                    if (name != null)
                    {
                        descriptors.Add(new FieldPropertyDescriptor(name, field, this.value));
                    }
                }

                // Get all the properties
                var properties = type.GetProperties();
                foreach (var property in properties)
                {
                    var name = this.GetReflectionName(property);
                    if (name != null)
                    {
                        descriptors.Add(new PropertyPropertyDescriptor(name, property, this.value));
                    }
                }
            }

            this.properties = new PropertyDescriptorCollection(descriptors.ToArray(), true);
        }
        #endregion

        #region Public methods
        #region GetAttributes()
        /// <summary>
        /// Returns a collection of custom attributes for this instance of a component.
        /// </summary>
        /// <returns>
        /// An <see cref="T:AttributeCollection"/> containing the attributes for this object.
        /// </returns>
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this.value, true);
        }
        #endregion

        #region GetClassName()
        /// <summary>
        /// Returns the class name of this instance of a component.
        /// </summary>
        /// <returns>
        /// The class name of the object, or null if the class does not have a name.
        /// </returns>
        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this.value, true);
        }
        #endregion

        #region GetComponentName()
        /// <summary>
        /// Returns the name of this instance of a component.
        /// </summary>
        /// <returns>
        /// The name of the object, or null if the object does not have a name.
        /// </returns>
        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this.value, true);
        }
        #endregion

        #region GetConverter()
        /// <summary>
        /// Returns a type converter for this instance of a component.
        /// </summary>
        /// <returns>
        /// A <see cref="T:TypeConverter"/> that is the converter for this object, or null if there is no <see cref="T:System.ComponentModel.TypeConverter"/> for this object.
        /// </returns>
        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this.value, true);
        }
        #endregion

        #region GetDefaultEvent()
        /// <summary>
        /// Returns the default event for this instance of a component.
        /// </summary>
        /// <returns>
        /// An <see cref="T:EventDescriptor"/> that represents the default event for this object, or null if this object does not have events.
        /// </returns>
        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this.value, true);
        }
        #endregion

        #region GetDefaultProperty()
        /// <summary>
        /// Returns the default property for this instance of a component.
        /// </summary>
        /// <returns>
        /// A <see cref="T:PropertyDescriptor"/> that represents the default property for this object, or null if this object does not have
        /// properties.
        /// </returns>
        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }
        #endregion

        #region GetEditor()
        /// <summary>
        /// Returns an editor of the specified type for this instance of a component.
        /// </summary>
        /// <param name="editorBaseType">A <see cref="T:Type"/> that represents the editor for this object.</param>
        /// <returns>
        /// An <see cref="T:Object"/> of the specified type that is the editor for this object, or null if the editor cannot be found.
        /// </returns>
        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this.value, editorBaseType, true);
        }
        #endregion

        #region GetEvents()
        /// <summary>
        /// Returns the events for this instance of a component using the specified attribute array as a filter.
        /// </summary>
        /// <param name="attributes">An array of type <see cref="T:Attribute"/> that is used as a filter.</param>
        /// <returns>
        /// An <see cref="T:EventDescriptorCollection"/> that represents the filtered events for this component instance.
        /// </returns>
        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this.value, attributes, true);
        }

        /// <summary>
        /// Returns the events for this instance of a component.
        /// </summary>
        /// <returns>
        /// An <see cref="T:EventDescriptorCollection"/> that represents the events for this component instance.
        /// </returns>
        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this.value, true);
        }
        #endregion

        #region GetProperties()
        /// <summary>
        /// Returns the properties for this instance of a component using the attribute array as a filter.
        /// </summary>
        /// <param name="attributes">An array of type <see cref="T:Attribute"/> that is used as a filter.</param>
        /// <returns>
        /// A <see cref="T:PropertyDescriptorCollection"/> that represents the filtered properties for this component instance.
        /// </returns>
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return this.GetProperties();
        }

        /// <summary>
        /// Returns the properties for this instance of a component.
        /// </summary>
        /// <returns>
        /// A <see cref="T:PropertyDescriptorCollection"/> that represents the properties for this component instance.
        /// </returns>
        public PropertyDescriptorCollection GetProperties()
        {
            return this.properties;
        }
        #endregion

        #region GetPropertyOwner()
        /// <summary>
        /// Returns an object that contains the property described by the specified property descriptor.
        /// </summary>
        /// <param name="pd">A <see cref="T:PropertyDescriptor"/> that represents the property whose owner is to be found.</param>
        /// <returns>
        /// An <see cref="T:Object"/> that represents the owner of the specified property.
        /// </returns>
        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this.value;
        }
        #endregion

        #region ToString()
        /// <summary>
        /// Returns a <see cref="String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.GetReflectionName(this.value);
        }
        #endregion
        #endregion

        #region Private methods
        #region GetReflectionName()
        /// <summary>
        /// Gets the name of a field or property from reflection.
        /// </summary>
        /// <param name="value">The field or property to get the name from.</param>
        /// <returns>The name from reflection if found, null otherwise.</returns>
        private string GetReflectionName(MemberInfo value)
        {
            string valueName = null;
            var attributes = value.GetCustomAttributes(true);
            foreach (var attribute in attributes)
            {
                var reflection = attribute as ReflectorPropertyAttribute;
                if (reflection != null)
                {
                    valueName = reflection.Name;
                    break;
                }
            }

            return valueName;
        }
        /// <summary>
        /// Gets the name of an item from reflection.
        /// </summary>
        /// <param name="value">The value to get the name from.</param>
        /// <returns>The name from reflection if found, string.Empty otherwise.</returns>
        private string GetReflectionName(object value)
        {
            string valueName = string.Empty;
            var reflection = value.GetType().GetCustomAttributes(typeof(ReflectorTypeAttribute), true);
            if (reflection.Length > 0)
            {
                valueName = (reflection[0] as ReflectorTypeAttribute).Name;
            }

            return valueName;
        }
        #endregion
        #endregion

        #region Public classes
        #region PropertyDescriptorBase
        /// <summary>
        /// Provides a base implementation of <see cref="PropertyDescriptor"/>.
        /// </summary>
        public abstract class PropertyDescriptorBase
            : PropertyDescriptor
        {
            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="FieldPropertyDescriptor"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="attributes">The attributes.</param>
            /// <param name="value">The object containing the values.</param>
            public PropertyDescriptorBase(string name, Attribute[] attributes, object value)
                : base(name, attributes)
            {
                this.Value = value;
            }
            #endregion

            #region Public properties
            #region Value
            /// <summary>
            /// Gets the instance containing the values.
            /// </summary>
            /// <value>The field information.</value>
            public object Value { get; private set; }
            #endregion

            #region IsReadOnly
            /// <summary>
            /// Gets a value indicating whether this property is read-only.
            /// </summary>
            /// <value></value>
            /// <returns>true if the property is read-only; otherwise, false.
            /// </returns>
            public override bool IsReadOnly
            {
                get { return true; }
            }
            #endregion
            #endregion

            #region Public methods
            #region ResetValue()
            /// <summary>
            /// Resets the value for this property of the component to the default value.
            /// </summary>
            /// <param name="component">The component with the property value that is to be reset to the default value.</param>
            public override void ResetValue(object component)
            {
            }
            #endregion

            #region CanResetValue()
            /// <summary>
            /// Returns whether resetting an object changes its value.
            /// </summary>
            /// <param name="component">The component to test for reset capability.</param>
            /// <returns>
            /// true if resetting the component changes its value; otherwise, false.
            /// </returns>
            public override bool CanResetValue(object component)
            {
                return false;
            }
            #endregion

            #region ShouldSerializeValue()
            /// <summary>
            /// Determines a value indicating whether the value of this property needs to be persisted.
            /// </summary>
            /// <param name="component">The component with the property to be examined for persistence.</param>
            /// <returns>
            /// true if the property should be persisted; otherwise, false.
            /// </returns>
            public override bool ShouldSerializeValue(object component)
            {
                return false;
            }
            #endregion

            #region SetValue()
            /// <summary>
            /// Sets the value of the component to a different value.
            /// </summary>
            /// <param name="component">The component with the property value that is to be set.</param>
            /// <param name="value">The new value.</param>
            public override void SetValue(object component, object value)
            {
            }
            #endregion

            #region WrapValue()
            /// <summary>
            /// Wraps the value.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <returns>The wrapped value if necessary, otherwise the base value.</returns>
            public object WrapValue(object value)
            {
                if (value == null)
                {
                    return value;
                }
                else
                {
                    var type = value.GetType();
                    if (type.IsPrimitive || (type == typeof(string)) || (type == typeof(PrivateString)) || type.IsArray)
                    {
                        return value;
                    }
                    else
                    {
                        return new ConfigurationTypeDescriptor(value);
                    }
                }
            }
            #endregion
            #endregion
        }
        #endregion

        #region FieldPropertyDescriptor
        /// <summary>
        /// Implements <see cref="PropertyDescriptor"/> for fields.
        /// </summary>
        public class FieldPropertyDescriptor 
            : PropertyDescriptorBase
        {
            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="FieldPropertyDescriptor"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="field">The field.</param>
            /// <param name="value">The object containing the values.</param>
            public FieldPropertyDescriptor(string name, FieldInfo field, object value)
                : base(name, (Attribute[])field.GetCustomAttributes(typeof(Attribute), true), value)
            {
                this.Field = field;
            }
            #endregion

            #region Public properties
            #region Field
            /// <summary>
            /// Gets the underlying field.
            /// </summary>
            /// <value>The field information.</value>
            public FieldInfo Field { get; private set; }
            #endregion

            #region ComponentType
            /// <summary>
            /// Fets the type of the component this property is bound to.
            /// </summary>
            /// <value></value>
            /// <returns>
            /// A <see cref="T:Type"/> that represents the type of component this property is bound to. When the 
            /// <see cref="M:PropertyDescriptor.GetValue(System.Object)"/> or 
            /// <see cref="M:PropertyDescriptor.SetValue(System.Object,System.Object)"/> methods are invoked, the object specified might be an
            /// instance of this type.
            /// </returns>
            public override Type ComponentType
            {
                get { return this.Field.DeclaringType; }
            }
            #endregion

            #region PropertyType
            /// <summary>
            /// Gets the type of the property.
            /// </summary>
            /// <value></value>
            /// <returns>
            /// A <see cref="T:System.Type"/> that represents the type of the property.
            /// </returns>
            public override Type PropertyType
            {
                get { return this.Field.FieldType; }
            }
            #endregion
            #endregion

            #region Public methods
            #region GetValue()
            /// <summary>
            /// Gets the current value of the property on a component.
            /// </summary>
            /// <param name="component">The component with the property for which to retrieve the value.</param>
            /// <returns>
            /// The value of a property for a given component.
            /// </returns>
            public override object GetValue(object component)
            {
                return this.WrapValue(this.Field.GetValue(this.Value));
            }
            #endregion
            #endregion
        }
        #endregion

        #region PropertyPropertyDescriptor
        /// <summary>
        /// Implements <see cref="PropertyDescriptor"/> for properties.
        /// </summary>
        public class PropertyPropertyDescriptor
            : PropertyDescriptorBase
        {
            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="PropertyPropertyDescriptor"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="property">The property.</param>
            /// <param name="value">The object containing the values.</param>
            public PropertyPropertyDescriptor(string name, PropertyInfo property, object value)
                : base(name, (Attribute[])property.GetCustomAttributes(typeof(Attribute), true), value)
            {
                this.Property = property;
            }
            #endregion

            #region Public properties
            #region Property
            /// <summary>
            /// Gets the underlying property.
            /// </summary>
            /// <value>The property information.</value>
            public PropertyInfo Property { get; private set; }
            #endregion

            #region ComponentType
            /// <summary>
            /// Fets the type of the component this property is bound to.
            /// </summary>
            /// <value></value>
            /// <returns>
            /// A <see cref="T:Type"/> that represents the type of component this property is bound to. When the 
            /// <see cref="M:PropertyDescriptor.GetValue(System.Object)"/> or 
            /// <see cref="M:PropertyDescriptor.SetValue(System.Object,System.Object)"/> methods are invoked, the object specified might be an
            /// instance of this type.
            /// </returns>
            public override Type ComponentType
            {
                get { return this.Property.DeclaringType; }
            }
            #endregion

            #region PropertyType
            /// <summary>
            /// Gets the type of the property.
            /// </summary>
            /// <value></value>
            /// <returns>
            /// A <see cref="T:System.Type"/> that represents the type of the property.
            /// </returns>
            public override Type PropertyType
            {
                get { return this.Property.PropertyType; }
            }
            #endregion
            #endregion

            #region Public methods
            #region GetValue()
            /// <summary>
            /// Gets the current value of the property on a component.
            /// </summary>
            /// <param name="component">The component with the property for which to retrieve the value.</param>
            /// <returns>
            /// The value of a property for a given component.
            /// </returns>
            public override object GetValue(object component)
            {
                return this.WrapValue(this.Property.GetValue(this.Value, new object[0]));
            }
            #endregion
            #endregion
        }
        #endregion
        #endregion
    }
}
