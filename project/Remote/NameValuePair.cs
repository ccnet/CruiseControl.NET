using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// Define a name/value pair.
    /// </summary>
    [Serializable]
    [ReflectorType("namedValue")]
    public class NameValuePair
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
        /// The name.
        /// </summary>
        [XmlAttribute("name")]
        [ReflectorProperty("name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        #endregion

        #region Value
        /// <summary>
        /// The value.
        /// </summary>
        [XmlAttribute("value")]
        [ReflectorProperty("value")]
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
        #endregion
    }
}
