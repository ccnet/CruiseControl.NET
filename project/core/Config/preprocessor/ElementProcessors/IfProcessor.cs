using System.Collections.Generic;
using System.Xml.Linq;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor.ElementProcessors
{
    internal class IfProcessor : ConditionalProcessor
    {
        public IfProcessor(PreprocessorEnvironment env)
            : base(env._Settings.Namespace.GetName("if"), env)
        {
        }

        public override IEnumerable< XNode > Process(XNode node)
        {
            XElement element = _AssumeElement( node );
            Validation.RequireAttributes( element, AttrName.Expr );
            var expr = ( string ) element.Attribute( AttrName.Expr );
            return _ProcessConditional( element,
                                        _Env.EvalBool( _ProcessText( expr ).GetTextValue() ) );
        }
    }
}
