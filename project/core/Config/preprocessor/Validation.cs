using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor
{
    /// <summary>
    /// XML Validation helpers
    /// </summary>
    internal static class Validation
    {
        /// <summary>
        /// Asserts that each of the given attributes exist on the given element.
        /// </summary>
        /// <param name="element">Element</param>
        /// <param name="attr_names">Names of attributes to check for</param>
        /// <exception cref="InvalidMarkupException">Thrown if one or more attributes are not present</exception>
        public static void RequireAttributes(XElement element, params XName[] attr_names)
        {
            IEnumerable< XName > missing_attrs =
                attr_names.Where( attr_name => element.Attribute( attr_name ) == null );
            if ( missing_attrs.Any() )
            {
                string[] attr_list = attr_names.Select( n => n.ToString() ).ToArray();
                throw InvalidMarkupException.CreateException(
                    "{0} Element '{1}' does not have required attribute(s) '{2}'",
                    element.ErrorContext(), element.Name,
                    String.Join( ",", attr_list ) );
            }
        }
    }
}
