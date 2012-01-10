using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor.ElementProcessors
{
    /// <summary>
    /// Processor for the "for" looping element
    /// </summary>
    internal class ForProcessor : ElementProcessor
    {
        public ForProcessor(PreprocessorEnvironment env)
            : base(env._Settings.Namespace.GetName("for"), env)
        {
        }

        public override IEnumerable< XNode > Process(XNode node)
        {
            XElement element = _AssumeElement( node );
            Validation.RequireAttributes( element, AttrName.CounterName, AttrName.InitExpr,
                                          AttrName.TestExpr, AttrName.CountExpr );
            // Get the attributes representing the counter name, initialization expression, termination test
            // expression, and counter-increment expression.  General, this translates to a traditional C-style
            // for-loop semantics: for( counter_name = init_expr; test_expr; count_expr );
            string counter_name =
                _ProcessText( element.GetAttributeValue( AttrName.CounterName ) ).GetTextValue();
            string init_expr =
                _ProcessText( element.GetAttributeValue( AttrName.InitExpr ) ).GetTextValue();
            string test_expr = element.GetAttributeValue( AttrName.TestExpr );
            string count_expr = element.GetAttributeValue( AttrName.CountExpr );
            bool use_scope = true;
            if (element.HasAttribute(AttrName.UseScope))
            {
                use_scope = (bool)element.Attribute(AttrName.UseScope);
            }
            var generated_nodes = new List<XNode>();

            string current_expr = init_expr;
            int count = _ExprAsInt( current_expr );
            bool run = true;
            while ( run )
            {
                /* Necessary due to "modified closure" lambda interaction rules */
                int count1 = count;
                XNode[] nodes = null;
                if (use_scope)
                    nodes = _Env.Call(() =>
                                      {
                                          return _GetSubNodes(element, count_expr, counter_name, test_expr, ref count1, ref run);
                                      });
                else
                    nodes = _GetSubNodes(element, count_expr, counter_name, test_expr, ref count1, ref run);

                /* Necessary due to "modified closure" lambda interaction rules */
                count = count1;
                generated_nodes.AddRange( nodes );
            }
            return generated_nodes;
        }

        private XNode[] _GetSubNodes(XElement element, string count_expr, string counter_name, string test_expr, ref int count1, ref bool run)
        {
            // Define the counter value in the environment
            _Env.DefineTextSymbol(counter_name,
                                   count1.ToString(
                                       NumberFormatInfo.
                                           InvariantInfo));
            // Test for loop termination condition
            if (
                !_Env.EvalBool(
                    _ProcessText(test_expr).GetTextValue()))
            {
                /* terminate */
                run = false;
                return new XNode[] { };
            }

            // Evaluate the count expression (must be done in the current environment stack frame)
            count1 =
                _ExprAsInt(
                    _ProcessText(count_expr).GetTextValue());

            // Process the loop body
            return _ProcessNodes(element.Nodes()).ToArray();
        }

        private int _ExprAsInt(string expr)
        {
            int val;
            if ( !Int32.TryParse( _Env.EvalExprAsString( expr ), out val ) )
            {
                throw new InvalidCastException(
                    String.Format( CultureInfo.CurrentCulture, "Expression '{0}' does not evaluate to an integer", expr ) );
            }
            return val;
        }
    }
}
