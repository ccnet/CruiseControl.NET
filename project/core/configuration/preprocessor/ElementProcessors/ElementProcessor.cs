using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor.ElementProcessors
{
    /// <summary>
    /// Interface that node processors must implement
    /// </summary>
    public interface IElementProcessor
    {
        /// <summary>
        /// Retrieve the name of the element that this processor processes.
        /// </summary>
        XName TargetElementName { get; }

        /// <summary>
        /// Called by the preprocessor to process a node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        IEnumerable< XNode > Process(XNode node);
    }


    /// <summary>
    /// Abstract base class for node processors
    /// </summary>
    public abstract class ElementProcessor : IElementProcessor
    {
        /// <summary>
        /// Regex matches 2 or more contiguous whitespace characters
        /// </summary>
        private readonly Regex _space_matcher = new Regex( @"\s\s\s*" );

        protected ElementProcessor(XName target_element_name, PreprocessorEnvironment env)
        {
            TargetElementName = target_element_name;
            _Env = env;
        }

        protected PreprocessorEnvironment _Env { get; private set; }

        #region IElementProcessor Members

        public abstract IEnumerable< XNode > Process(XNode node);

        public XName TargetElementName { get; protected set; }

        #endregion

        /// <summary>
        /// Perform default preprocessing on the given nodes
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        protected IEnumerable< XNode > _ProcessNodes(IEnumerable< XNode > nodes)
        {
            if ( nodes.Any() )
            {
                var return_nodes = new List< XNode >();
                foreach ( XNode node in nodes )
                {
                    return_nodes.AddRange( _Env._DefaultNodeProcessor.Process( node ) );
                }
                return return_nodes;
            }
            return new XNode[] {};
        }


        protected IEnumerable< XNode > _ProcessText(string value)
        {
            IEnumerable< XNode > result = _Env.EvalTextSymbols( value );
            return result.GetTextValue() != value ? _ProcessNodes( result ).Cast< XNode >() : result;
        }


        protected void _DefineFromAttributes(XElement element)
        {
            foreach ( XAttribute attr in element.Attributes() )
            {
                _Env.DefineTextSymbol( attr.Name.LocalName, attr.Value );
            }
        }

        protected static XElement _AssumeElement(XNode node)
        {
            try
            {
                return ( XElement ) node;
            }
            catch ( InvalidCastException )
            {
                throw new InvalidOperationException( "Expected an XML element, got a " +
                                                     node.NodeType );
            }
        }
    }
}
