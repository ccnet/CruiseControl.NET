using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor.ElementProcessors
{
    internal class ExpandSymbolProcessor : ElementProcessor
    {
        public ExpandSymbolProcessor(PreprocessorEnvironment env)
            : base( env._Settings.Namespace.GetName("_Unused_Name_"), env)
        {
        }

        public override IEnumerable< XNode > Process(XNode node)
        {
            XElement element = _AssumeElement( node );
            string symbol_name = element.Name.LocalName;
            if ( !_Env.IsDefined( symbol_name ) )
                throw InvalidMarkupException.CreateException( "[{0}] Undefined symbol '{1}'   ",
                                                              node.ErrorContext(),
                                                              element.Name );
            /* Evaluate the symbol on a new stack frame, since we may be defining local symbols
             * based on the attributes */
            return _Env.Call( () =>
                                  {
                                      /* Bind attributes as local symbolic definitions */
                                      _DefineFromAttributes( element );
                                      /* Bind any nested definition elements */
                                      _ProcessNodes(
                                          element.Elements( _Env._Settings.Namespace.GetName("define") ).Select
                                              ( n => ( XNode ) n ) );

                                      // Must materialize the deferred-execution nodeset 
                                      // iterator before this method returns, otherwise 
                                      // the evaluation stack will be wrong.  Calling ToArray()
                                      // ensures this.
                                      return _Env.EvalSymbol( symbol_name ).ToArray();
                                      //return _ProcessNodes( ret );
                                  } );
        }
    }
}
