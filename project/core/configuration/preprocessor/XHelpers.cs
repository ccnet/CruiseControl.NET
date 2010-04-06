using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor
{
    /// <summary>
    /// Extension methods for System.Xml.Linq
    /// </summary>
    public static class XHelpers
    {
        /// <summary>
        /// Does the element have an attribute of the given name?
        /// </summary>
        /// <param name="element">element to check</param>
        /// <param name="attr_name">attribute to check for</param>
        /// <returns>true/false</returns>
        public static bool HasAttribute(this XElement element, XName attr_name)
        {
            return element.Attribute( attr_name ) != null;
        }

        /// <summary>
        /// Returns the first sibling that follows the given element.
        ///  </summary>
        /// <param name="element">Element whose sibling is returned</param>
        /// <returns>First following sibling element, or null if no siblings exist</returns>
        public static XElement NextSiblingElement(this XElement element)
        {
            return element.ElementsAfterSelf().FirstOrDefault();
        }

        /// <summary>
        /// Extracts file and line/position information from the given object, in displayable form,
        /// for use in error messages.
        /// </summary>
        /// <param name="obj">Xml object for whom to return the context</param>
        /// <returns>A string of the form "File: [file_path] line XXX, pos YYY"</returns>
        public static string ErrorContext(this XObject obj)
        {
            string obj_info = "";
            if ( obj is XAttribute )
            {
                obj_info = String.Format( "Attribute '{0}'", ( ( XAttribute ) obj ).Name );
            }
            else if ( obj is XElement )
            {
                obj_info = String.Format( "Element '{0}'", ( ( XElement ) obj ).Name );
            }
            IXmlLineInfo line_info = obj;
            string line_and_pos = "line and position unknown";
            if ( line_info.HasLineInfo() )
            {
                line_and_pos = String.Format( "line {0}, pos {1}", line_info.LineNumber,
                                              line_info.LinePosition );
            }
            return String.Format( "File: {0} ({1}:{2})", obj.BaseUri, line_and_pos, obj_info );
        }

        /// <summary>
        /// Returns the given element's named attribute value as a string 
        /// </summary>
        /// <param name="element">Element whose value is returned</param>
        /// <param name="attr_name">Name of attribute to return</param>
        /// <returns>Attribute value, or empty string if no such attribute exists</returns>
        public static string GetAttributeValue(this XElement element, XName attr_name)
        {
            XAttribute attr = element.Attribute( attr_name );
            return attr == null ? "" : attr.Value;
        }

        /// <summary>
        /// Returns the concatenated text values of the given nodeset
        /// </summary>
        /// <param name="nodes">Nodeset whose values are returned</param>
        /// <returns>Untrimmed, concatenated text values of the given nodeset</returns>
        public static string GetTextValue(this IEnumerable< XNode > nodes)
        {
            return String.Concat( nodes.Select< XNode, String >( _ValueOf ).ToArray() );
        }

        private static string _ValueOf(XNode node)
        {
            switch ( node.NodeType )
            {
                case XmlNodeType.Element:
                    return ( ( XElement ) node ).Value;
                case XmlNodeType.Text:
                    return ( ( XText ) node ).Value;
                case XmlNodeType.Comment:
                case XmlNodeType.ProcessingInstruction:
                    return String.Empty;
                default:
                    throw new InvalidOperationException(
                        String.Format( "{0} Unhandled node type {1}",
                                       node.ErrorContext(),
                                       node.NodeType ) );
            }
        }
    }    

    public static class XmlNs
    {
        public static readonly XNamespace PreProcessor = XNamespace.Get("urn:ccnet.config.builder");
    }
}
