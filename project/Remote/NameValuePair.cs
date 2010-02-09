using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
#if !NoReflector
using Exortech.NetReflector;
#endif

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <title>Named Value</title>
    /// <version>1.4.4</version>
    /// <summary>
    /// <para>
    /// A value with an associated name. This is a generic configuration item that is used in other configuration
    /// elements.
    /// </para>
    /// </summary>
    /// <example>
    /// <code >
    /// &lt;namedValue name="The Name" value="something" /&gt;
    /// </code>
    /// </example>
    [Serializable]
#if !NoReflector
    [ReflectorType("namedValue")]
#endif
    public class NameValuePair
        : IEquatable<NameValuePair>
    {
        #region Private fields
        private string name;
        private string namedValue;
        #endregion

        #region Constructors
        /// <summary>
        /// Starts a new blank <see cref="NameValuePair"/>.
        /// </summary>
        public NameValuePair() { }

        /// <summary>
        /// Starts a new <see cref="NameValuePair"/> with a name and value.
        /// </summary>
        public NameValuePair(string name, string value)
        {
            this.name = name;
            this.namedValue = value;
        }
        #endregion

        #region Public properties
        #region Name
        /// <summary>
        /// The name of the value.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>n/a</default>
        [XmlAttribute("name")]
#if !NoReflector
        [ReflectorProperty("name")]
#endif
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        #endregion

        #region Value
        /// <summary>
        /// The actual value.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>n/a</default>
        [XmlAttribute("value")]
#if !NoReflector
        [ReflectorProperty("value", Required = false)]
#endif
        public string Value
        {
            get { return namedValue; }
            set { namedValue = value; }
        }
        #endregion
        #endregion

        #region Public methods
        #region ToDictionary()
        /// <summary>
        /// Converts a list of <see cref="NameValuePair"/> to a dictionary of strings.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ToDictionary(List<NameValuePair> values)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            if (values != null)
            {
                foreach (NameValuePair value in values)
                {
                    dictionary.Add(value.name, value.Value);
                }
            }
            return dictionary;
        }
        #endregion

        #region FromDictionary()
        /// <summary>
        /// Converts a list of <see cref="NameValuePair"/> from a dictionary of strings.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static List<NameValuePair> FromDictionary(Dictionary<string, string> values)
        {
            List<NameValuePair> pairs = new List<NameValuePair>();
            if (values != null)
            {
                foreach (string key in values.Keys)
                {
                    pairs.Add(new NameValuePair(key, values[key]));
                }
            }
            return pairs;
        }
        #endregion

        #region FindNamedValue()
        /// <summary>
        /// Attempts to find a named value.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="name">The name of the value to find.</param>
        /// <returns></returns>
        public static string FindNamedValue(List<NameValuePair> values, string name)
        {
            string actualValue = null;
            if (values != null)
            {
                foreach (NameValuePair value in values)
                {
                    if (string.Equals(value.name, name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        actualValue = value.Value;
                        break;
                    }
                }
            }
            return actualValue;
        }
        #endregion

        #region Copy()
        /// <summary>
        /// Copies all the values from a dictionary into a list.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="list"></param>
        public static void Copy(Dictionary<string, string> dictionary, List<NameValuePair> list)
        {
            if (dictionary != null)
            {
                foreach (var pair in dictionary)
                {
                    list.Add(new NameValuePair(pair.Key, pair.Value));
                }
            }
        }

        /// <summary>
        /// Copies all the values from one list to another.
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="source"></param>
        public static void Copy(List<NameValuePair> source, List<NameValuePair> destination)
        {
            if (source != null)
            {
                foreach (var pair in source)
                {
                    destination.Add(new NameValuePair(pair.Name, pair.Value));
                }
            }
        }
        #endregion

        #region GetHashCode()
        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like
        /// a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.name == null ? 0 : this.name.GetHashCode();
        }
        #endregion

        #region Equals()
        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as NameValuePair);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(NameValuePair other)
        {
            if (other == null)
            {
                return false;
            }

            return other.name == this.name;
        }
        #endregion
        #endregion
    }
}
