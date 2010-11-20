using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor.ElementProcessors
{
    internal class CountProcessor : ElementProcessor
    {
        public CountProcessor(PreprocessorEnvironment env)
            : base(env._Settings.Namespace.GetName("count"), env)
        {
        }

        public override IEnumerable< XNode > Process(XNode node)
        {
            XElement element = _AssumeElement( node );
            Validation.RequireAttributes( element, AttrName.Name );
            Validation.RequireAttributes( element, AttrName.Max );
            var max = ( int ) element.Attribute( AttrName.Max );
            var name = ( string ) element.Attribute( AttrName.Name );
            var ret_nodes = new List< XNode >();
            for ( int idx = 1; idx <= max; ++idx )
            {
                ret_nodes.AddRange( _ProcessCounterBody( element, name, idx ) );
            }
            return ret_nodes;
        }

        private IEnumerable< XNode > _ProcessCounterBody(XElement element, string counter_name,
                                                         int counter_val)
        {
            return _Env.Call( () =>
                                  {
                                      _Env.DefineTextSymbol( counter_name,
                                                             counter_val.ToString(
                                                                 NumberFormatInfo
                                                                     .
                                                                     InvariantInfo ) );
                                      return _ProcessNodes( element.Nodes() );
                                  } );
        }
    }
}
