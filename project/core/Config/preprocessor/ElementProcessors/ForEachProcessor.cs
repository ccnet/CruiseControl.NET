using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor.ElementProcessors
{
    internal class ForEachProcessor : ElementProcessor
    {
        public ForEachProcessor(PreprocessorEnvironment env)
            : base(env._Settings.Namespace.GetName("for-each"), env)
        {
        }

        public override IEnumerable< XNode > Process(XNode node)
        {
            XElement element = _AssumeElement( node );

            string iter_name =
                _ProcessText( element.GetAttributeValue( AttrName.IteratorName ) ).GetTextValue();
            string iterator_expr =
                _ProcessText( element.GetAttributeValue( AttrName.IteratorExpr ) ).GetTextValue();
            /* Compute the set of values to iterate over. */
            IEnumerable< XNode > iter_values = _Env.EvalExpr( iterator_expr );
            /* Iterate over each value separately and collect the results */
            return
                iter_values.SelectMany(
                    iter_val =>
                    _ProcessIteration( element, iter_val, iter_name ) );
        }

        private IEnumerable< XNode > _ProcessIteration(XElement element, XNode iter_val,
                                                       string iter_name)
        {
            XNode val = iter_val;
            IEnumerable< XNode > results = _Env.Call( () =>
                                                          {
                                                              /* Bind the iterator symbolic name to the current value */
                                                              if ( val is XContainer )
                                                                  /* Nodeset value */
                                                              {
                                                                  _Env.DefineNodesetSymbol(
                                                                      iter_name,
                                                                      ( ( XContainer ) val ).
                                                                          Nodes() );
                                                              }
                                                              else /* Text value */
                                                              {
                                                                  _Env.DefineTextSymbol( iter_name,
                                                                                         val.
                                                                                             ToString
                                                                                             () );
                                                              }
                                                              /* Process loop body */
                                                              return _ProcessNodes( element.Nodes() );
                                                          } );
            return results;
        }
    }
}
