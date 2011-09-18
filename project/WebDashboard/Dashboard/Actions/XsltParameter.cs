﻿using System;
using System.Xml.Serialization;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// Define an XSL-T parameter.
    /// </summary>
    /// <title>xsltParameter</title>
    [Serializable]
    [ReflectorType("xsltParameter")]
    public class XsltParameter
    {
        #region Private fields
        private string name;
        private string namedValue;
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
    }
}
