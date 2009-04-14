using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using ThoughtWorks.CruiseControl.Core.Util;
using ConstantDict = System.Collections.Generic.Dictionary<string,ThoughtWorks.CruiseControl.Core.Config.Preprocessor.Constant>;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor
{
    /// <summary>
    /// The evaluation environment for the preprocessor.
    /// Keeps track of constant definitions, maintains a call stack for macro
    /// evaluation.
    /// </summary>
    internal class ConfigPreprocessorEnvironment
    {
        //Stack of call-stack frames.
        private readonly Stack<ConstantDict> _env_stack = new Stack< ConstantDict >();
        //Set of unique file paths seen by preprocessor
        private readonly Dictionary<string,bool> _fileset = new Dictionary< string, bool >( );
        /// <summary>
        /// Matches constant references in strings.
        /// Constant references are of the form $(const_name)
        /// </summary>
        private static readonly Regex _const_ref_matcher =
            new Regex(@"\$\(.+?\)", RegexOptions.Compiled);
        /// <summary>
        /// Evaluation stack, for cycle detection
        /// </summary>
        private readonly Stack<string> _eval_stack = new Stack<string>();
        /// <summary>
        /// Stack of included files.
        /// </summary>
        private readonly Stack< Uri > _include_stack = new Stack< Uri >();
        /// <summary>
        /// For resolving include paths
        /// </summary>
        private readonly PreprocessorUrlResolver _resolver;
        /// <summary>
        /// 
        /// </summary>
        private readonly XmlDocument _utility_doc = new XmlDocument();

        public ConfigPreprocessorEnvironment( Uri input_file_path, PreprocessorUrlResolver resolver )
        {
            // Bottom-most stack frame
            _env_stack.Push( new ConstantDict() );
            // Record the input file as the outer-most "include"
            _include_stack.Push( input_file_path );
            _fileset[ input_file_path.LocalPath ] = true;
            _resolver = resolver;
        }                        
       
        /// <summary>
        /// Add the given url to the set of processed files.
        /// </summary>
        /// <param name="url"></param>
        
        public void AddToFileset (Uri url)
        {
            _fileset[url.LocalPath] = true;
        }

        /// <summary>
        /// Get the list of all files seen by the preprocessor
        /// </summary>
        public string[] Fileset
        {
            get
            {
                string[] docs = new string[_fileset.Keys.Count];
                _fileset.Keys.CopyTo( docs, 0 );
                return docs;
            }
        }

        #region XSLT Extension Methods

        /// <summary>
        /// XSLT extension method, called to define a text constant in the 
        /// preprocessor environment.  The constant can be referred to symbolically
        /// in subsequent definitions or expansions.
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void define_text_constant(string name, string value)
        {
            _CheckAlreadyDefined(name);
            Constant const_def = new Constant();
            const_def.Name = name;
            const_def.Value = value;
            _SetConstant(const_def);
        }

        /// <summary>
        /// XSLT extension method, called to define a notset constant in the preprocessor
        /// environment.  The constant can be referred to symbolically in subsequent 
        /// definitions or expansions.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void define_nodeset_constant(string name, XPathNodeIterator value)
        {
            _CheckAlreadyDefined(name);
            Constant const_def = new Constant();
            const_def.Name = name;
            const_def.Value = value;
            _SetConstant(const_def);
        }

        /// <summary>
        /// XSLT extension method
        /// Expand any $(const_name) constant references within the given 
        /// string.
        /// </summary>
        /// <param name="value">Text value, possibliy containing constant
        /// references of the form $(const_name)</param>
        /// <returns>The input string, with constant replaced by their
        /// values.</returns>
        public object eval_text_constants(string value)
        {
            // Cache for constants we've already encountered in this string.
            Dictionary< string, string > const_values= new Dictionary< string, string >( );
            // Replace all constant references with their assocaited values.
            value =
                _const_ref_matcher.Replace(
                    value, delegate (Match match)
                           {
                               string const_val;
                               if ( !const_values.TryGetValue( match.Value, out const_val) )
                               {
                                   string name = match.Value.Substring(2, match.Value.Length - 3);
                                   const_values[ match.Value] = const_val = eval_text_constant( name );
                               }
                               return const_val;
                           });
            return _utility_doc.CreateTextNode(value);
        }

        /// <summary>
        /// XSLT extension method
        /// Retrieve the value of the named constant.  
        /// </summary>
        /// <param name="name">name.</param>
        /// <returns>String or nodeset, depending on 
        /// how the constant was defined</returns>
        public object eval_constant(string name)
        {
            string lower_name = name.ToLowerInvariant();
            StringBuilder sb = new StringBuilder();
            // Check for cycles.
            if ( _eval_stack.Contains( lower_name ) )
            {
                string[] evals = _eval_stack.ToArray();
                Array.Reverse( evals );
                foreach ( string eval in evals )
                {
                    if ( sb.Length > 0 )
                        sb.Append( "->" );
                    sb.Append( eval );
                }
                sb.Append( "->" + lower_name );
                Utils.ThrowException(
                    EvaluationException.CreateException,
                    "Cycle detected while evaluating '{0}'.  Eval Stack: {1}",
                    name, sb );                
            }
            _eval_stack.Push( lower_name );
            Constant const_def = _GetConstantDef( name );
            if ( const_def.Value is string )
            {
                XmlDocument doc = new XmlDocument();
                using (
                    XmlWriter writer = doc.CreateNavigator().AppendChild() )
                {
                    writer.WriteElementString(
                        "root", "", const_def.Value.ToString() );
                }
                return
                    doc.DocumentElement.CreateNavigator().SelectChildren(
                        XPathNodeType.Text );
            }
            return const_def.Value;
        }

        public string eval_text_constant( string name )
        {
            string lower_name = name.ToLowerInvariant();
            _CheckForCycle( name );
            _eval_stack.Push( lower_name );
            Constant const_def = _GetConstantDef( name );
            if ( const_def.Value is string )
            {
                return const_def.Value as String;
            }
            else if ( const_def.Value is IXPathNavigable )
            {
                IXPathNavigable nav = ( IXPathNavigable ) const_def.Value;
                return nav.CreateNavigator().Value;
            }
            else
                Utils.ThrowAppException(
                    "Unexpected value type '{0}' while processing value for '{1}'",
                    const_def.Value.GetType().Name, name );
            return null;
        }

        private void _CheckForCycle (string name)
        {
            /*
             * Cycle checking has been disabled because doesn't work right at this time.  Consider this:
             * a = "$(b) $(c)"
             * b = "$(c)"
             * c = "d"
             * 
             * The stack ought to look like this over time:
             * Time Result          Stack
             * ---- ------          --------
             * 1    "$(a)"          (empty)
             * 2    "$(b) $(c)"     a
             * 3    "$(c) $(c)"     a b
             * 4    "d $(c)"        a b c
             * 5    "d $(c)"        a b
             * 6    "d $(c)"        a
             * 7    "d d"           a c
             * 8    "d d"           a
             * 9    "d d"           (empty)
             * 
             * But in fact it looks like this:
             * Time Result          Stack
             * ---- ------          --------
             * 1    "$(a)"          (empty)
             * 2    "$(b) $(c)"     a
             * 3    "$(c) $(c)"     a b
             * 4    "$(c) d"        a b c
             * 5    cycle detected expanding "$(c)"
             */
#if false
            StringBuilder sb = new StringBuilder();            
            // Check for cycles.
            if ( _eval_stack.Contains( name ) )
            {
                string[] evals = _eval_stack.ToArray();
                Array.Reverse( evals );
                foreach ( string eval in evals )
                {
                    if ( sb.Length > 0 )
                        sb.Append( "->" );
                    sb.Append( eval );
                }
                sb.Append( "->" + name );
                Utils.ThrowException(
                    EvaluationException.CreateException,
                    "Cycle detected while evaluating '{0}'.  Eval Stack: {1}",
                    name, sb );                
            }            
#endif
        }

        /// <summary>
        /// XSLT extension method.
        /// XSLT is done processing the (possibly recursive) expansion of a 
        /// constant.  We can throw away the eval stack, which has been used during
        /// the expansion to detect cycles (defining two constants in terms of 
        /// each other, which would lead to a stack overflow if we didn't catch it)
        /// </summary>
        public void unwind_eval_stack()
        {
            _eval_stack.Clear();
            
        }

        /// <summary>
        /// Push a frame onto the call stack
        /// </summary>
        public void push_stack()
        {
            _env_stack.Push( new ConstantDict() );
        }

        /// <summary>
        /// Pop a frame from the call stack.
        /// </summary>
        public void pop_stack()
        {
            _env_stack.Pop();
        }

        public XPathNavigator push_include( string href )
        {
            Uri current_include = _include_stack.Peek();
            Uri new_include = _resolver.ResolveUri( current_include, href );

            AddToFileset( new_include );

            Log.Debug(string.Format("Beginning include level {0} for \"{1}\" included by \"{2}\", resolved to \"{3}\"",
                _include_stack.Count + 1, href, current_include, new_include));
            XPathDocument doc =
                new XPathDocument(
                    ( Stream )
                    _resolver.GetEntity(
                        new_include, null, typeof ( Stream ) ) );

            // Push href onto the include stack so that any nested includes will 
            // resolve relative to it.
            _include_stack.Push( new_include );

            return doc.CreateNavigator();
        }

        public void pop_include()
        {
            Uri thisInclude = _include_stack.Peek();
            int thisLevel = _include_stack.Count;
            _include_stack.Pop();
            Log.Debug(string.Format("Ending include level {0} for \"{1}\" included by \"{2}\"",
                thisLevel, thisInclude, _include_stack.Peek()));
        }

        #endregion

         /// <summary>
        /// Is the given symbol defined in this environment?
        /// </summary>
        /// <param name="symbol_name"></param>
        /// <returns></returns>
        private bool _IsDefined( string symbol_name )
        {
            return _InternalGetSymbolDef( symbol_name ) != null;
        }

        /// <summary>
        /// Define the given symbol on the current stack frame.
        /// </summary>        
        /// <param name="constant_def"></param>
        private void _SetConstant(Constant constant_def )
        {
            _env_stack.Peek().Add( constant_def.Name.ToLowerInvariant(), constant_def );
        }    
     
        /// <summary>
        /// Retrieve the given symbol definition from the environment
        /// </summary>        
        /// <param name="name">Symbol name</param>
        /// <returns></returns>
        internal Constant _GetConstantDef( string name )
        {
            if ( !_IsDefined( name ) )
            {
                Utils.ThrowException(
                    EvaluationException.CreateException,
                    "Reference to unknown symbol {0}", name );
            }
            return _InternalGetSymbolDef( name.ToLowerInvariant() );
        }


        /// <summary>
        /// Search for the given symbol definition in the environment.
        /// </summary>
        /// <param name="symbol_name"></param>
        /// <returns></returns>
        private Constant _InternalGetSymbolDef (string symbol_name)
        {
            symbol_name = symbol_name.ToLowerInvariant();
            // Try each stack frame.
            foreach ( ConstantDict frame_defs in _env_stack.ToArray() )
            {
                Constant constant_def;
                if ( frame_defs.TryGetValue( symbol_name, out constant_def ))
                    return constant_def;
            }
            // If nothing is found on the stack, try the system environment variables.
            string env_var = Environment.GetEnvironmentVariable( symbol_name );
            if ( env_var != null )
            {
                Constant c = new Constant();
                c.Name = symbol_name;
                c.Value = env_var;
                return c;
            }
            return null;
        }


        /// <summary>
        /// Is the symbol defined in the current stack frame?
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool _IsDefinedInCurrentFrame (string name)
        {
            return _env_stack.Peek().ContainsKey( name.ToLowerInvariant() );
        }

        private void _CheckAlreadyDefined(string name)
        {
            if (_IsDefinedInCurrentFrame(name))
            {
                Utils.ThrowException(
                    EvaluationException.CreateException,
                    "Symbol '{0}' already defined", name );
            }
        }
    }

    public class EvaluationException : PreprocessorException
    {
        internal EvaluationException( string msg ) : base( msg )
        {            
        }

        internal static Exception CreateException (string msg, params object[] args)
        {
            return new EvaluationException( String.Format( msg, args ) );
        }
    }

    public class DefinitionException : PreprocessorException
    {
        internal DefinitionException( string msg ) : base( msg )
        {            
        }

        internal static Exception CreateException (string msg, params object[] args)
        {
            return new EvaluationException( String.Format( msg, args ) );
        }
    }

    public class PreprocessorException : ApplicationException
    {
        internal PreprocessorException( string msg ) : base( msg )
        {            
        }
    }

    internal delegate Exception ExceptionFactory( string msg, params object[] args);

       
}