using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor.ElementProcessors
{
    internal class CreateElementProcessor : ElementProcessor
    {
        public CreateElementProcessor(PreprocessorEnvironment env)
            : base(env._Settings.Namespace.GetName("element"), env)
        {
        }

        public override IEnumerable< XNode > Process(XNode node)
        {
            XElement element = _AssumeElement( node );
            Validation.RequireAttributes( element, AttrName.Name );

            // Attribute name can have symbolic expansions or expressions
            string el_name =
                _ProcessText( ( string ) element.Attribute( AttrName.Name ) ).GetTextValue();

            var new_element = new XElement( el_name );
            // Process any attribute-generation child elements, and add them as XAttributes
            var attr_elements = element.Elements( _Env._Settings.Namespace.GetName( "attribute" ) );
            var attrs = attr_elements.Select< XElement, XAttribute >( _ProcessAttribute );
            new_element.Add( attrs );
            // Process any other nodes.
            IEnumerable< XNode > remaining_nodes =
                element.Nodes().Where( n => !attr_elements.Contains( n as XElement ) );
            new_element.Add( _ProcessNodes( remaining_nodes ) );
            return new[] {new_element};
        }
   
        private XAttribute _ProcessAttribute(XElement attr_element)
        {
            Validation.RequireAttributes( attr_element, AttrName.Name );

            // Both attribute names and values can have symbolic expansions or expression evals
            string attr_name =
                _ProcessText( ( string ) attr_element.Attribute( AttrName.Name ) ).GetTextValue();
            string attr_val = _ProcessText( attr_element.Value ).GetTextValue();
            return new XAttribute( attr_name, attr_val );
        }
    }
}
