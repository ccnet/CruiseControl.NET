namespace ThoughtWorks.CruiseControl.Remote
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Exortech.NetReflector;
    using Exortech.NetReflector.Util;
    using System.Xml;

    /// <summary>
    /// Serialise/deserialise a name/value pair.
    /// </summary>
    public class NameValuePairSerialiser
        : XmlMemberSerialiser
    {
        #region Private fields
        private bool isList/* = false*/;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="NameValuePairSerialiser"/>.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="attribute"></param>
        /// <param name="isList"></param>
        public NameValuePairSerialiser(ReflectorMember info, ReflectorPropertyAttribute attribute, bool isList)
            : base(info, attribute)
        {
            this.isList = isList;
        }
        #endregion

        #region Public properties
        #region IsList
        /// <summary>
        /// Gets a value indicating whether this instance is list.
        /// </summary>
        /// <value><c>true</c> if this instance is list; otherwise, <c>false</c>.</value>
        public bool IsList
        {
            get { return this.isList; }
        }
        #endregion
        #endregion

        #region Public methods
        #region Read()
        /// <summary>
        /// Read a node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="table"></param>
        /// <returns></returns>
		public override object Read(XmlNode node, NetReflectorTypeTable table)
        {
            if (isList)
            {
                return ReadList(node);
            }
            else
            {
                var value = ReadValue(node as XmlElement);
                return value;
            }
        }

    	#endregion

        #region Write()
        /// <summary>
        /// Write a node.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="target"></param>
        public override void Write(XmlWriter writer, object target)
        {
            if (isList)
            {
                var list = target as NameValuePair[];
                if (list != null)
                {
                    writer.WriteStartElement(base.Attribute.Name);
                    foreach (var value in list)
                    {
                        WriteValue(writer, value, "value");
                    }
                    writer.WriteEndElement();
                }
            }
            else
            {
                var value = (target as NameValuePair);
                if (value != null)
                {
                    WriteValue(writer, value, base.Attribute.Name);
                }
            }
        }
        #endregion
        #endregion

        #region Private methods
        #region ReadList()
        /// <summary>
        /// Read a list from a node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private object ReadList(XmlNode node)
        {
            var valueList = new List<NameValuePair>();

            if (node != null)
            {
                // Validate the attributes
                if (node.Attributes.Count > 0)
                {
                    throw new NetReflectorException(string.Concat("A name/value pair list cannot directly contain attributes.", Environment.NewLine, "XML: ", node.OuterXml));
                }

                // Check each element
                var subNodes = node.SelectNodes("*");
                if (subNodes != null)
                {
                    foreach (XmlElement valueElement in subNodes)
                    {
                        if (valueElement.Name == "value")
                        {
                            var newValue = ReadValue(valueElement);

                            valueList.Add(newValue);
                        }
                        else
                        {
                            // Unknown sub-item
                            throw new NetReflectorException(string.Concat(valueElement.Name, " is not a valid sub-item.",
                                                                          Environment.NewLine, "XML: ", valueElement.OuterXml));
                        }
                    }
                }
            }
            return valueList.ToArray();
        }

        #endregion

        #region ReadValue()
        /// <summary>
        /// Reads a value.
        /// </summary>
        /// <param name="valueElement"></param>
        /// <returns></returns>
        private NameValuePair ReadValue(XmlElement valueElement)
        {
            // Make sure there are no child elements
            var fileSubNodes = valueElement.SelectNodes("*");
            if (fileSubNodes != null && fileSubNodes.Count > 0)
            {
                throw new NetReflectorException(string.Concat("value elements cannot contain any sub-items.", Environment.NewLine, "XML: ", valueElement.OuterXml));
            }

            // Read the value
            var newValue = new NameValuePair();
            newValue.Value = valueElement.InnerText;

            // Read the name
            newValue.Name = valueElement.GetAttribute("name"); ;
            return newValue;
        }
        #endregion

        #region WriteValue()
        /// <summary>
        /// Writes a value.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="elementName"></param>
        /// <param name="value"></param>
        private void WriteValue(XmlWriter writer, NameValuePair value, string elementName)
        {
            writer.WriteStartElement(elementName);
            if (!string.IsNullOrEmpty(value.Name)) writer.WriteAttributeString("name", value.Name);
            writer.WriteString(value.Value);
            writer.WriteEndElement();
        }
        #endregion
        #endregion
    }
}
