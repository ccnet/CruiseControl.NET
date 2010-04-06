using System.Collections.Generic;
using System.Xml.Linq;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor.ElementProcessors
{
    internal class ProcessingInstructionProcessor : ElementProcessor
    {
        public ProcessingInstructionProcessor(PreprocessorEnvironment env)
            : base(env._Settings.Namespace.GetName("processing-instruction"), env)
        {
        }

        public override IEnumerable< XNode > Process(XNode node)
        {
            XElement element = _AssumeElement( node );
            Validation.RequireAttributes( element, AttrName.Name );
            // PI name and value can have symbolic expansions or expressions
            string pi_name =
                _ProcessText( ( string ) element.Attribute( AttrName.Name ) ).GetTextValue();
            string pi_val = _ProcessText( element.Value ).GetTextValue();
            var pi = new XProcessingInstruction( pi_name, pi_val );
            return new[] {pi};
        }
    }
}
