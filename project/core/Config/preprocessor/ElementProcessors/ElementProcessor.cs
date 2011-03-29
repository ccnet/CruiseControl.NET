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

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementProcessor" /> class.	
        /// </summary>
        /// <param name="targetElementName">The target_element_name.</param>
        /// <param name="env">The env.</param>
        /// <remarks></remarks>
        protected ElementProcessor(XName targetElementName, PreprocessorEnvironment env)
        {
            TargetElementName = targetElementName;
            _Env = env;
        }

        /// <summary>
        /// Gets or sets the _ env.	
        /// </summary>
        /// <value>The _ env.</value>
        /// <remarks></remarks>
        protected PreprocessorEnvironment _Env { get; private set; }

        #region IElementProcessor Members

        /// <summary>
        /// Processes the specified node.	
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public abstract IEnumerable< XNode > Process(XNode node);

        /// <summary>
        /// Gets or sets the name of the target element.	
        /// </summary>
        /// <value>The name of the target element.</value>
        /// <remarks></remarks>
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


        /// <summary>
        /// _s the process text.	
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected IEnumerable< XNode > _ProcessText(string value)
        {
            IEnumerable< XNode > result = _Env.EvalTextSymbols( value );
            //return result.GetTextValue() != value ? _ProcessNodes( result ).Cast< XNode >() : result;


            var textvalue = result.GetTextValue();

            if (textvalue != value)
            {
                return _ProcessNodes(result);
            }
            else
            {
                return result;
            }
        }


        /// <summary>
        /// _s the define from attributes.	
        /// </summary>
        /// <param name="element">The element.</param>
        /// <remarks></remarks>
        protected void _DefineFromAttributes(XElement element)
        {
            foreach ( XAttribute attr in element.Attributes() )
            {
                _Env.DefineTextSymbol( attr.Name.LocalName, attr.Value );
            }
        }

        /// <summary>
        /// _s the assume element.	
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        /// <remarks></remarks>
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
