using System.Collections.Generic;
using System.Xml.Linq;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor.ElementProcessors
{
    internal abstract class ConditionalProcessor : ElementProcessor
    {
        protected ConditionalProcessor(XName targetElementName, PreprocessorEnvironment env)
            : base( targetElementName, env )
        {
        }

        /// <summary>
        ///  Common logic for if/ifdef/ifndef/else constructs
        /// </summary>
        /// <param name="conditionalElement"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected IEnumerable< XNode > _ProcessConditional(XElement conditionalElement,
                                                           bool condition)
        {
            return condition
                       ? _ProcessNodes( conditionalElement.Nodes() )
                       : _ProcessNextElse( conditionalElement );
        }

        private IEnumerable< XNode > _ProcessNextElse(XElement element)
        {
            XElement next_element = element.NextSiblingElement();
            if ( next_element != null && next_element.Name == _Env._Settings.Namespace.GetName("else" ) )
            {
                return _ProcessNodes( next_element.Nodes() );
            }
            return new XNode[] {};
        }
    }
}
