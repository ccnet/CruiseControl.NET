/**
 * Purpose: definitinon of the XmlPreprocessor.symbolic_def class
 * Author: Jeremy Lew
 * Created: 2008.03.24
 */
using System.Collections.Generic;
using System.Xml.Linq;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor
{
    /// <summary>
    /// Represents a name bound to a constant value in
    /// the context of the preprocessor environment.
    /// </summary>
    internal class SymbolicDef
    {
        /// <summary>
        /// Was the constant defined explicitly in the input document?
        /// </summary>
        public bool IsExplicitlyDefined;

        /// <summary>
        /// Constant name
        /// </summary>
        public string Name;

        /// <summary>
        /// Constant value
        /// </summary>
        public IEnumerable< XNode > Value;
    }
}
