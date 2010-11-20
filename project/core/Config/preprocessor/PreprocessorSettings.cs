using System.Collections.Generic;
using System.Xml.Linq;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor
{
    public class PreprocessorSettings
    {
        /// <summary>
        /// Are explicit symbol declarations required?
        /// If true, it is an error to attempt to evaluate a symbol
        /// which has not been defined by the input file via the 
        /// <c>define</c> or <c>default</c> statement.
        /// </summary>
        public bool ExplicitDeclarationRequired;

        /// <summary>
        /// Name/value pairs of variables which are turned into preprocessor
        /// definitions at global scope.  If <c>ExplicitDeclarationRequired</c> is true,
        /// these definitions may not be used.
        /// </summary>
        public IDictionary< string, string > InitialDefinitions;

        /// <summary>
        /// Should constant names be case-sensitive?
        /// </summary>
        public bool NamesAreCaseSensitve;

        /// <summary>
        /// If true, operating system environment variables
        /// may be referenced as if they were declared by the 
        /// input file.  If ExplicitDeclarationRequired is true,
        /// this flag may not be used.
        /// </summary>
        public bool UseOsEnvironment = true;

        /// <summary>
        /// XML Namespace to be used for built-in commands.
        /// </summary>
        public XNamespace Namespace = XmlNs.PreProcessor;

        /// <summary>
        /// Ignore semantically-insignificant whitespace
        /// </summary>
        public bool IgnoreWhitespace;
    }
}
