
using System;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// Handy XMl routines
    /// </summary>
	public static class XmlUtil
	{
        /// <summary>
        /// Creates the document.	
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public static XmlDocument CreateDocument(string xml)
		{
			XmlDocument document = new XmlDocument();
			document.LoadXml(xml);
			return document;
		}

        /// <summary>
        /// Creates the document element.	
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public static XmlElement CreateDocumentElement(string xml)
		{
			return CreateDocument(xml).DocumentElement;
		}

        /// <summary>
        /// Gets the first element.	
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public static XmlElement GetFirstElement(XmlDocument doc, string name)
		{
			XmlNodeList list = doc.GetElementsByTagName(name);
			if (list == null)
			{
				return null;
			}
			return (XmlElement) list.Item(0);
		}

        /// <summary>
        /// Gets the single element.	
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public static XmlElement GetSingleElement(XmlDocument doc, string name)
		{
			XmlNodeList list = doc.GetElementsByTagName(name);
			if (list == null)
			{
				return null;
			}
			if (list.Count > 1)
			{
				throw new CruiseControlException(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Expected single element '{0}', got multiple ({1})", name, list.Count));
			}
			return (XmlElement) list.Item(0);
		}

        /// <summary>
        /// Gets the single element value.	
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public static string GetSingleElementValue(XmlDocument doc, string name)
		{
			return GetSingleElementValue(doc, name,string.Empty);
		}

        /// <summary>
        /// Gets the single element value.	
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public static string GetSingleElementValue(XmlDocument doc, string name, string defaultValue)
		{
			XmlElement element = GetSingleElement(doc, name);
			if (element == null)
			{
				return defaultValue;
			}
			else
			{
				return element.InnerText;
			}
		}

        /// <summary>
        /// Generates the outer XML.	
        /// </summary>
        /// <param name="xmlContent">Content of the XML.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public static string GenerateOuterXml(string xmlContent)
		{
			return CreateDocument(xmlContent).OuterXml;
		}

        /// <summary>
        /// Generates the indented outer XML.	
        /// </summary>
        /// <param name="xmlContent">Content of the XML.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public static string GenerateIndentedOuterXml(string xmlContent)
		{
			StringWriter buffer = new StringWriter();
			XmlTextWriter writer = new XmlTextWriter(buffer);
			writer.Formatting = Formatting.Indented;
			writer.WriteNode(new XmlTextReader(new StringReader(xmlContent)), false);
			return buffer.ToString();
		}

        /// <summary>
        /// Selects the value.	
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="xpath">The xpath.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public static string SelectValue(XmlNode node, string xpath, string defaultValue)
		{
			if (node == null || node.InnerXml == null || node.InnerXml == String.Empty)
			{
				return defaultValue;
			}
			node = node.SelectSingleNode(xpath);
			if (node == null)
			{
				throw new ArgumentException("No node found at: " + xpath);
			}
			return node.InnerText;
		}

        /// <summary>
        /// Selects the value.	
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="xpath">The xpath.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public static string SelectValue(XmlDocument document, string xpath, string defaultValue)
		{
			XmlNode node = document.SelectSingleNode(xpath);
			return SelectValue(node, xpath, defaultValue);
		}

        /// <summary>
        /// Selects the node.	
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="xpath">The xpath.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public static XmlNode SelectNode(string xml, string xpath)
		{
			return CreateDocument(xml).SelectSingleNode(xpath);
		}

        /// <summary>
        /// Selects the required value.	
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="xpath">The xpath.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public static string SelectRequiredValue(XmlDocument document, string xpath)
		{
			XmlNode node = document.SelectSingleNode(xpath);
			if (node == null || node.InnerXml == null || node.InnerXml == String.Empty)
			{
				throw new CruiseControlException("Document missing required value at xpath: " + xpath);
			}
			return node.InnerText;
		}

        /// <summary>
        /// Selects the required value.	
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="xpath">The xpath.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public static string SelectRequiredValue(string xml, string xpath)
		{
			return SelectRequiredValue(CreateDocument(xml), xpath);
		}

        /// <summary>
        /// Encode a string so it is safe to use as XML "character data".
        /// </summary>
        /// <param name="text">the text to encode</param>
        /// <returns>the encoded text</returns>
        /// <remarks>
        /// This method damages the resulting string, because the sequence "]]&gt;" is forbidden inside
        /// a CDATA section and cannot be escaped or encoded.  Since we can't protect it, we insert a
        /// space between the brackets so it isn't recognized by an XML parser.  C'est la guerre.
        /// </remarks>
		public static string EncodeCDATA(string text)
		{
			Regex CDataCloseTag = new Regex(@"\]\]>");
			return CDataCloseTag.Replace(text, @"] ]>");
		}

        /// <summary>
        /// Strings the serialize.	
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public static string StringSerialize(object obj)
		{
			XmlSerializer serializer = new XmlSerializer(obj.GetType());
			StringWriter writer1 = new StringWriter();
			serializer.Serialize(writer1, obj);

			StringReader reader = new StringReader(writer1.ToString());
			StringWriter writer2 = new StringWriter();

			// This is because .NET's XML Serialization is a but bunk and puts a dodgy first line in the xml
			reader.ReadLine();
			writer2.WriteLine(@"<?xml version=""1.0""?>");
			writer2.Write(reader.ReadToEnd());

			return writer2.ToString();
		}

        /// <summary>
        /// Adds the child.	
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public static XmlElement AddChild(XmlNode parent, string name, string value)
		{
			if (value == null) return null;

			XmlElement child = AddChild(parent, name);
			child.InnerText = value;
			parent.AppendChild(child);
			return child;
		}

        /// <summary>
        /// Adds the child.	
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public static XmlElement AddChild(XmlNode parent, string name)
		{
			XmlDocument document = (parent is XmlDocument) ? (XmlDocument) parent : parent.OwnerDocument;
			XmlElement node = document.CreateElement(name);
			parent.AppendChild(node);
			return node;
		}

        /// <summary>
        /// Writes the non null element string.	
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <remarks></remarks>
		public static void WriteNonNullElementString(XmlWriter writer, string name, string value)
		{
			if (value != null)
			{
				writer.WriteElementString(name, value);
			}
		}

        /// <summary>
        /// Verifies the XML is well formed.	
        /// </summary>
        /// <param name="actual">The actual.</param>
        /// <remarks></remarks>
		public static void VerifyXmlIsWellFormed(string actual)
		{
			XmlDocument document = new XmlDocument();
			document.LoadXml(actual);
		}

        /// <summary>
        /// Encode a string so it is safe to use as XML "parsed character data".
        /// </summary>
        /// <param name="input">the text to encode</param>
        /// <returns>the encoded text</returns>
        public static string EncodePCDATA(string input)
        {
            string result;
            result = Regex.Replace(input, "&", "&amp;");    // Do this one first so "&"s in replacements pass through unchanged.
            result = Regex.Replace(result, "<", "&lt;");
            result = Regex.Replace(result, ">", "&gt;");
            result = Regex.Replace(result, "-", "&#x2d;");   // Because XML comments are "--comment--".
            return result;
        }

    }
}