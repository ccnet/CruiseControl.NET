using Exortech.NetReflector;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// Utility class for setting dynamic values.
    /// </summary>
    public static class DynamicValueUtility
    {
        /// <summary>
        /// Attempts to find a property on an objec using reflection attributes.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static PropertyValue FindProperty(object value, string property)
        {
            PropertyValue actualProperty = null;
            object currentValue = value;
            MemberInfo currentProperty = null;
            int lastIndex = 0;

            if (value != null)
            {
                PropertyPart[] parts = SplitPropertyName(property);
                int position = 0;
                while ((position < parts.Length) && (currentValue != null))
                {
                    actualProperty = null;
                    if (currentProperty != null)
                    {
                        currentValue = GetValue(currentProperty, currentValue);
                        currentProperty = null;
                    }

                    if (currentValue is IEnumerable)
                    {
                        currentValue = FindTypedValue(currentValue as IEnumerable, parts[position].Name);
                    }
                    else
                    {
                        currentProperty = FindActualProperty(currentValue, parts[position].Name);
                        if (!string.IsNullOrEmpty(parts[position].KeyName))
                        {
                            IEnumerable values = GetValue(currentProperty, currentValue) as IEnumerable;
                            currentValue = FindKeyedValue(values, parts[position].KeyName, parts[position].KeyValue);
                            currentProperty = null;
                        }
                        else if (parts[position].Index >= 0)
                        {
                            // If the current item is expecting an indexed value then treat the source as an array
                            Array values = GetValue(currentProperty, currentValue) as Array;
                            if (values != null)
                            {
                                lastIndex = parts[position].Index;
                                if (lastIndex < values.GetLength(0))
                                {
                                    // Generate the property here, it will get wiped later if there are further parts
                                    actualProperty = new PropertyValue(currentValue, currentProperty, lastIndex);
                                    currentValue = values.GetValue(lastIndex);
                                    currentProperty = null;
                                }
                            }
                            currentProperty = null;
                        }
                    }

                    position++;
                }
                if (currentProperty != null)
                {
                    actualProperty = new PropertyValue(currentValue, currentProperty, lastIndex);
                }
            }

            return actualProperty;
        }

        /// <summary>
        /// Attempts to find a reflector property.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="reflectorProperty"></param>
        /// <returns></returns>
        public static MemberInfo FindActualProperty(object value, string reflectorProperty)
        {
            MemberInfo actualProperty = null;

            if (value != null)
            {
                // Iterate through all the properties
                List<MemberInfo> allProperties = new List<MemberInfo>(value.GetType().GetProperties());
                allProperties.AddRange(value.GetType().GetFields());
                foreach (MemberInfo property in allProperties)
                {
                    // Iterate through all the attributes
                    object[] attributes = property.GetCustomAttributes(true);
                    foreach (object attribute in attributes)
                    {
                        // Get the name of the property
                        string name = null;
                        if (attribute is ReflectorPropertyAttribute)
                        {
                            name = (attribute as ReflectorPropertyAttribute).Name;
                        }

                        // Check to see whether the property has been found
                        if (name == reflectorProperty)
                        {
                            actualProperty = property;
                            break;
                        }
                    }

                    if (actualProperty != null) break;
                }
            }

            return actualProperty;
        }

        /// <summary>
        /// Finds a keyed value.
        /// </summary>
        /// <param name="values">The enumeration containing the values.</param>
        /// <param name="keyName">The name of the key.</param>
        /// <param name="keyValue">The value of the key.</param>
        /// <returns>The matching value, if found, null otherwise.</returns>
        public static object FindTypedValue(IEnumerable values, string typeName)
        {
            object actualValue = null;

            foreach (object value in values)
            {
                // Get the attributes
                object[] attributes = value.GetType().GetCustomAttributes(true);
                foreach (object attribute in attributes)
                {
                    if (attribute is ReflectorTypeAttribute)
                    {
                        string name = (attribute as ReflectorTypeAttribute).Name;
                        if (name == typeName)
                        {
                            actualValue = value;
                            break;
                        }
                    }
                }
            }

            return actualValue;
        }

        /// <summary>
        /// Finds a keyed value.
        /// </summary>
        /// <param name="values">The enumeration containing the values.</param>
        /// <param name="keyName">The name of the key.</param>
        /// <param name="keyValue">The value of the key.</param>
        /// <returns>The matching value, if found, null otherwise.</returns>
        public static object FindKeyedValue(IEnumerable values, string keyName, string keyValue)
        {
            object actualValue = null;

            if (values != null)
            {
                foreach (object value in values)
                {
                    // First, attempt to find a property that matches
                    MemberInfo property = FindActualProperty(value, keyName);
                    if (property != null)
                    {
                        // Then see if the property has a value
                        object propertyValue = GetValue(property, value);
                        if (propertyValue != null)
                        {
                            // Finally, check if the values match
                            if (string.Equals(keyValue, propertyValue.ToString()))
                            {
                                actualValue = value;
                                break;
                            }
                        }
                    }
                }
            }

            return actualValue;
        }

        /// <summary>
        /// Splits a property name into its component parts.
        /// </summary>
        /// <param name="propertyName">The property to split.</param>
        /// <returns>An array of component parts.</returns>
        public static PropertyPart[] SplitPropertyName(string propertyName)
        {
            // Split into the dot separated parts
            string[] parts = propertyName.Split('.');
            List<PropertyPart> results = new List<PropertyPart>();

            foreach (string part in parts)
            {
                PropertyPart newPart = new PropertyPart();

                // Check if each part has a key
                int keyIndex = part.IndexOf('[');
                if (keyIndex >= 0)
                {
                    // If so, find the different parts
                    newPart.Name = part.Substring(0, keyIndex);
                    string key = part.Substring(keyIndex + 1);
                    int equalsIndex = key.IndexOf('=');
                    if (equalsIndex >= 0)
                    {
                        newPart.KeyName = key.Substring(0, equalsIndex);
                        newPart.KeyValue = key.Substring(equalsIndex + 1);
                        newPart.KeyValue = newPart.KeyValue.Remove(newPart.KeyValue.Length - 1);
                    }
                    else
                    {
                        int index;
                        if (int.TryParse(key.Substring(0, key.Length - 1), out index))
                        {
                            newPart.Index = index;
                        }
                    }
                }
                else
                {
                    // Otherwise the whole is the name
                    newPart.Name = part;
                }
                results.Add(newPart);
            }

            return results.ToArray();
        }

        private static object GetValue(MemberInfo member, object source)
        {
            object value = null;

            if (member is PropertyInfo)
            {
                value = (member as PropertyInfo).GetValue(source, new object[0]);
            }
            else
            {
                value = (member as FieldInfo).GetValue(source);
            }

            return value;
        }

        /// <summary>
        /// Defines a property value.
        /// </summary>
        public class PropertyValue
        {
            private object mySource;
            private MemberInfo myProperty;
            private int myArrayIndex;

            internal PropertyValue(object source, MemberInfo property, int arrayIndex)
            {
                this.mySource = source;
                this.myProperty = property;
                this.myArrayIndex = arrayIndex;
            }

            /// <summary>
            /// The source of the property.
            /// </summary>
            public object Source
            {
                get { return mySource; }
            }

            /// <summary>
            /// The property.
            /// </summary>
            public MemberInfo Property
            {
                get { return myProperty; }
            }

            /// <summary>
            /// The current value of the property.
            /// </summary>
            public object Value
            {
                get
                {
                    if (myProperty is PropertyInfo)
                    {
                        return (myProperty as PropertyInfo).GetValue(mySource, new object[0]);
                    }
                    else
                    {
                        return (myProperty as FieldInfo).GetValue(mySource);
                    }
                }
            }

            /// <summary>
            /// Changes the value of the property.
            /// </summary>
            /// <param name="value">The new value to set.</param>
            public void ChangeProperty(object value)
            {
                object actualValue = value;
                if (myProperty is PropertyInfo)
                {
                    ChangePropertyValue(value, actualValue);
                }
                else
                {
                    actualValue = ChangeFieldValue(value, actualValue);
                }
            }

            /// <summary>
            /// Change the value when the source is a property.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="actualValue"></param>
            private void ChangePropertyValue(object value, object actualValue)
            {
                object[] index = new object[0];
                PropertyInfo property = (myProperty as PropertyInfo);

                // If it is an array then reset the index and use the array item type instead
                if (property.PropertyType.IsArray)
                {
                    index = new object[] {
                                myArrayIndex
                            };
                    if (property.PropertyType.GetElementType() != value.GetType())
                    {
                        actualValue = Convert.ChangeType(value, property.PropertyType.GetElementType());
                    }
                }
                else
                {
                    if (property.PropertyType != value.GetType())
                    {
                        actualValue = Convert.ChangeType(value, property.PropertyType);
                    }
                }
                property.SetValue(mySource, actualValue, index);
            }

            /// <summary>
            /// Change the value when the source is a field.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="actualValue"></param>
            /// <returns></returns>
            private object ChangeFieldValue(object value, object actualValue)
            {
                FieldInfo property = (myProperty as FieldInfo);

                // If it is an array then just set the array value instead of changing the entire value
                if (property.FieldType.IsArray)
                {
                    if (property.FieldType.GetElementType() != value.GetType())
                    {
                        actualValue = Convert.ChangeType(value, property.FieldType);
                    }
                    Array array = property.GetValue(mySource) as Array;
                    array.SetValue(actualValue, myArrayIndex);
                }
                else
                {
                    if (property.FieldType != value.GetType())
                    {
                        actualValue = Convert.ChangeType(value, property.FieldType);
                    }
                    property.SetValue(mySource, actualValue);
                }
                return actualValue;
            }
        }

        /// <summary>
        /// Defines a part of a property.
        /// </summary>
        public class PropertyPart
        {
            /// <summary>
            /// The name of the property.
            /// </summary>
            public string Name;

            /// <summary>
            /// The name of the key
            /// </summary>
            public string KeyName;

            /// <summary>
            /// The value of the key
            /// </summary>
            public string KeyValue;

            /// <summary>
            /// The index of the item in the array.
            /// </summary>
            public int Index = -1;
        }
    }
}
