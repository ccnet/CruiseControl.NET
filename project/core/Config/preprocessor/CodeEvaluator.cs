using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.JScript;
using Convert = System.Convert;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor
{
    public class Evaluator
    {
        // Fields
        private const string JSCRIPT_SOURCE =
            @"import System.IO;
            import System.Collections;  
            package Evaluator
            {
               class Evaluator
               {
                  public function Eval(expr : String) : Object 
                  { 
                     var obj = eval(expr,'unsafe'); 
                     if ( obj instanceof Array )
                     {
                        var dotnet_arr : String[];
                        dotnet_arr = new String[ obj.length ];
                        var i : int;
                        for( i = 0; i < obj.length; ++i )
                        {
                            dotnet_arr[ i ] = '' + obj[ i ];
                        }         
                        return dotnet_arr;               
                     }
                     return '' + obj;
                  }
                  public function StringArray( arr ) : String[]
                  {
                    return arr;
                  }

               }
            }";

        private static readonly object _evaluator;
        private static readonly Type _evaluator_type;

        private static readonly JScriptCodeProvider _provider = new JScriptCodeProvider();


        // Methods
        static Evaluator()
        {
            _CheckMono();
            var parameters = new CompilerParameters {GenerateInMemory = true};
            CompilerParameters options = parameters;
            CompilerResults results = _provider.CompileAssemblyFromSource( options,
                                                                           new[] {JSCRIPT_SOURCE} );
            if ( results.Errors.Count > 0 )
            {
                string message = "";
                foreach ( CompilerError error in results.Errors )
                {
                    message = message + error;
                    message = message + "\n";
                }
                throw new ApplicationException( message );
            }
            _evaluator_type = results.CompiledAssembly.GetType( "Evaluator.Evaluator" );
            _evaluator = Activator.CreateInstance( _evaluator_type );
        }

        public static bool EvalToBool(string statement)
        {
            return bool.Parse( EvalToString( statement ) );
        }

        public static double EvalToDouble(string statement)
        {
            return double.Parse( EvalToString( statement ) );
        }

        public static int EvalToInteger(string statement)
        {
            return int.Parse( EvalToString( statement ) );
        }

        public static object EvalToObject(string statement)
        {
            return _evaluator_type.InvokeMember( "Eval", BindingFlags.InvokeMethod, null, _evaluator,
                                                 new object[] {statement} );
        }

        public static string EvalToString(string statement)
        {
            return EvalToObject( statement ).ToString();
        }

        public static T EvalToType< T >(string expression)
        {
            _CheckMono();
            return ( T ) Convert.ChangeType( EvalToObject( expression ), typeof ( T ) );
        }

        public static string StringAsLiteral(string s)
        {
            var expression = new CodePrimitiveExpression( s );
            var sb = new StringBuilder();
            var options = new CodeGeneratorOptions();
            using ( TextWriter writer = new StringWriter( sb ) )
            {
                _provider.GenerateCodeFromExpression( expression, writer, options );
            }
            return ( "(" + sb + ")" );
        }

        private static void _CheckMono()
        {
            if ( Type.GetType( "Mono.Runtime" ) != null )
                throw new ApplicationException(
                    "This feature requires JSCRIPT.NET and is not supported under Mono" );
        }
    }
}
