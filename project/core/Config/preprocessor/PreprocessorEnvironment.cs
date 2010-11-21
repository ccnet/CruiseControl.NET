/*
Purpose: definitinon of the XmlPreprocessor.PreprocessorEnvironment class
Author: Jeremy Lew
Created: 2008.03.24
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using ThoughtWorks.CruiseControl.Core.Config.Preprocessor.ElementProcessors;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor
{
    /// <summary>
    /// The execution environment for the preprocessor.  This environment stores and manages
    /// symbolic definitions and evaluations, as well as providing document include facilities.
    /// </summary>
    public class PreprocessorEnvironment
    {
        // Fields

        /// <summary>
        /// Matches symbolic_def references in strings.
        /// symbolic_def references are of the form $(sym_name)
        /// </summary>
        private static readonly Regex _symbol_ref_matcher = new Regex( @"([$!])\((.+?)\)",
                                                                       RegexOptions.Compiled );

        /// <summary>
        /// Stack that represents the call stack.  Each stack frame contains a map of
        /// symbol names to SymbolicDef objects.  Only one such definiton is permitted
        /// per stack frame.
        /// </summary>
        private readonly Stack< Dictionary< string, SymbolicDef > > _define_stack =
            new Stack< Dictionary< string, SymbolicDef > >();

        /// <summary>
        /// Keeps track of symbol names already evaluated on the current stack frame,
        /// to detect cyclic expansions which would loop infinitely
        /// </summary>
        private readonly Dictionary< SymbolicDef, int > _evaluated_symbols =
            new Dictionary< SymbolicDef, int >();


        /// <summary>
        /// Keeps track of input files processed by the preprocessor
        /// </summary>
        private readonly Dictionary< Uri, bool > _fileset = new Dictionary< Uri, bool >();

        /// <summary>
        /// Stack of included files.
        /// </summary>
        private readonly Stack< Uri > _include_stack = new Stack< Uri >();

        /// <summary>
        /// For resolving relative include paths
        /// </summary>
        private readonly XmlUrlResolver _resolver;

        /// <summary>
        /// Preprocessor settings
        /// </summary>
        internal PreprocessorSettings _Settings { get; private set;}


        // Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PreprocessorEnvironment" /> class.	
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="input_file_path">The input_file_path.</param>
        /// <param name="resolver">The resolver.</param>
        /// <remarks></remarks>
        public PreprocessorEnvironment(PreprocessorSettings settings, Uri input_file_path,
                                      XmlUrlResolver resolver)
        {
            _Settings = settings;
            _DefaultNodeProcessor = new DefaultProcessor( this );            
            // Bottom-most stack frame
            _define_stack.Push( new Dictionary< string, SymbolicDef >() );
            // Record the input file as the outer-most "include"
            _include_stack.Push( input_file_path );
            _fileset[ input_file_path ] = true;
            _resolver = resolver;
            if ( _Settings.InitialDefinitions != null )
            {
                foreach ( var pair in _Settings.InitialDefinitions )
                {
                    _DefineTextSymbol( pair.Key, pair.Value, false );
                }
            }
        }

        /// <summary>
        /// Get the of all files seen by the preprocessor.  This includes the
        /// main file being processed as well as any included files.
        /// </summary>
        public Uri[] Fileset
        {
            get
            {
                var array = new Uri[_fileset.Keys.Count];
                _fileset.Keys.CopyTo( array, 0 );
                return array;
            }
        }

        /// <summary>
        /// Processor used for recursing into nested symbolic definitions.
        /// </summary>
        internal DefaultProcessor _DefaultNodeProcessor { get; private set; }

        /// <summary>
        /// Canonicalize the given symbol name, removing JScript flags (if present) and
        /// converting to lower case if the settings dictate case-insensitivity.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string _CanonicalizeName(string name)
        {
            name = _Settings.NamesAreCaseSensitve ? name : name.ToLowerInvariant();
            if ( _HasJsFlag( name ) )
            {
                name = name.Substring( 0, name.Length - 3 );
            }
            return name;
        }

        private void _CheckAlreadyDefined(string name)
        {
            if ( _IsDefinedInCurrentFrame( name ) )
            {
                Utils.ThrowException( EvaluationException.CreateException,
                                      "Symbol '{0}' already defined", new object[] {name} );
            }
        }

        private void _DefineTextSymbol(string name, string value, bool is_explicit)
        {
            if ( string.IsNullOrEmpty( name ) )
            {
                throw DefinitionException.CreateException(
                    "An attempt was made to define a nameless variable (value = '{0}'",
                    new object[] {value} );
            }
            _CheckAlreadyDefined( name );
            var symbolic_def = new SymbolicDef
                                   {
                                       Name = name,
                                       Value = new[] {new XText( value )},
                                       IsExplicitlyDefined = is_explicit
                                   };
            _DefineSymbolOnStack( symbolic_def );
        }

        /// <summary>
        /// _s the get as node set.	
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected static IEnumerable< XNode > _GetAsNodeSet(IEnumerable values)
        {
            return
                ( from object value in values select new XText( value.ToString() ) ).Cast< XNode >()
                    .ToList();
        }

        /// <summary>
        /// _s the get as node set.	
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected static IEnumerable< XNode > _GetAsNodeSet(params string[] values)
        {
            return _GetAsNodeSet( ( IEnumerable ) values );
        }

        /// <summary>
        /// Retrieve the SymbolicDef bound to the given name.  If the name occurs multiple times
        /// in the stack, returns the highest (most-local) definition.
        /// </summary>
        /// <param name="name">symbol name</param>
        /// <returns>SymbolicDef</returns>
        /// <exception cref="EvaluationException">Symbol was not defined</exception>
        /// <exception cref="ExplicitDefinitionRequiredException">Symbol was defined in the 
        /// environment, but preprocess is in "explicit definition required" mode.</exception>
        internal SymbolicDef GetSymbolDef(string name)
        {
            if ( !_IsDefined( name ) )
            {
                Utils.ThrowException( UndefinedSymbolException.CreateException,
                                      "Reference to unknown symbol '{0}'", new object[] {name} );
            }
            SymbolicDef symbolic_def = _InternalGetSymbolDef( _CanonicalizeName( name ) );
            if ( _Settings.ExplicitDeclarationRequired && !symbolic_def.IsExplicitlyDefined )
            {
                Utils.ThrowException( ExplicitDefinitionRequiredException.CreateException,
                                      "Symbol '{0}' must be explicitly declared before usage.",
                                      new object[] {name} );
            }
            return symbolic_def;
        }

        /// <summary>
        /// Is the given name qualified with the :js 
        /// flag, signalling that it is to be emitted as a
        /// JScript string literal (and properly escaped for
        /// processing by the JScript engine)
        /// </summary>         
        /// <param name="name"></param>
        /// <returns></returns>
        private static bool _HasJsFlag(string name)
        {
            return name.EndsWith( ":js" );
        }

        /// <summary>
        /// Search for the given symbol definition in the environment.
        /// </summary>
        /// <param name="symbol_name"></param>
        /// <returns></returns>
        private SymbolicDef _InternalGetSymbolDef(string symbol_name)
        {
            symbol_name = _CanonicalizeName( symbol_name );
            // Try each stack frame.
            foreach ( var dictionary in _define_stack.ToArray() )
            {
                SymbolicDef symbolic_def;
                if ( dictionary.TryGetValue( symbol_name, out symbolic_def ) && !_evaluated_symbols.ContainsKey(symbolic_def))
                {
                    return symbolic_def;
                }
            }
            // If nothing is found on the stack, try the system environment variables.
            string environment_variable = Environment.GetEnvironmentVariable( symbol_name );
            if ( environment_variable != null )
            {
                var symbolic_def2 = new SymbolicDef
                                        {
                                            Name = symbol_name,
                                            Value = _StringToNodeSet( environment_variable )
                                        };
                return symbolic_def2;
            }
            return null;
        }


        // Get the given string as an IEnumerable of XText objects.
        private static IEnumerable< XNode > _StringToNodeSet(string value)
        {
            return new[] {new XText( value )};
        }

        /// <summary>
        /// Is the given symbol defined in this environment?
        /// </summary>
        /// <param name="symbol_name"></param>
        /// <returns></returns>
        private bool _IsDefined(string symbol_name)
        {
            return ( _InternalGetSymbolDef( symbol_name ) != null );
        }

        /// <summary>
        /// Is the symbol defined in the current stack frame?
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool _IsDefinedInCurrentFrame(string name)
        {
            return _define_stack.Peek().ContainsKey( _CanonicalizeName( name.ToLowerInvariant() ) );
        }

        /// <summary>
        /// Is the given string a JScript Expression?  Note that this 
        /// method only tests for the presence of enclosing {} characters,
        /// not that the contained text is actually well-formed JScript.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static bool _IsJScriptExpression(string input)
        {
            return ( input.StartsWith( "{" ) && input.EndsWith( "}" ) );
        }

        /// <summary>
        /// Define the given symbol on the current stack frame.
        /// </summary>        
        /// <param name="symbolic_def"></param>
        private void _DefineSymbolOnStack(SymbolicDef symbolic_def)
        {
            _define_stack.Peek().Add( _CanonicalizeName( symbolic_def.Name ), symbolic_def );
        }


        /// <summary>
        /// Add the given url to the set of processed files.
        /// </summary>
        /// <param name="url"></param>
        public void AddToFileset(Uri url)
        {
            _fileset[ url ] = true;
        }

        /// <summary>
        /// XSLT extension method, called to define a notset symbolic_def in the preprocessor
        /// environment.  The symbolic_def can be referred to symbolically in subsequent 
        /// definitions or expansions.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public IEnumerable< XNode > DefineNodesetSymbol(string name, IEnumerable< XNode > value)
        {
            _CheckAlreadyDefined( name );
            var sym = new SymbolicDef {Name = name, Value = value, IsExplicitlyDefined = true};
            _DefineSymbolOnStack( sym );
            return new XNode[] {};
        }


        /// <summary>
        /// XSLT extension method, called to define a text symbolic_def in the 
        /// preprocessor environment.  The symbolic_def can be referred to symbolically
        /// in subsequent definitions or expansions.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public IEnumerable< XNode > DefineTextSymbol(string name, string value)
        {
            _DefineTextSymbol( name, value, true );
            return new XNode[] {};
        }


        /// <summary>
        /// Evaluates the given boolean JScript.NET expression, returns the result.
        /// </summary>
        /// <returns>comparison evaluation</returns>
        public bool EvalBool(string expr)
        {
            bool flag;
            try
            {
                flag = Evaluator.EvalToType< bool >( expr );
            }
            catch ( Exception exception )
            {
                throw EvaluationException.CreateException( expr, exception );
            }
            return flag;
        }

        /// <summary>
        /// XSLT extension method
        /// Retrieve the value of the named symbolic_def.  
        /// </summary>
        /// <param name="name">name.</param>
        /// <returns>String or nodeset, depending on 
        /// how the symbolic_def was defined</returns>
        public IEnumerable< XNode > EvalSymbol(string name)
        {
            string symbol_name = _CanonicalizeName( name );
            SymbolicDef symbol_def;
            try
            {
                symbol_def = GetSymbolDef(symbol_name);
            }
            catch(UndefinedSymbolException)
            {
                // If UndefinedSymbolException was thrown and symbol_name exists in the _evaluated_symbols
                // dictionary, it means that a mutually-recursive evaluation is happening, which would cause
                // an infinite loop.  Throw an exception instead.
                if (_evaluated_symbols.Keys.Any( sym => sym.Name == symbol_name ))
                {
                    IEnumerable<string> names =
                  _evaluated_symbols.OrderBy(def => def.Value).Select(def => def.Key.Name);
                    string eval_chain = String.Join("->", names.ToArray()) + "->" + symbol_name;
                    throw CyclicalEvaluationException.CreateException(
                        "Cyclical definition detected definiton: {0}",
                        eval_chain);                    
                }
                throw;
            }
            IEnumerable< XNode > val = symbol_def.Value;          
            _evaluated_symbols.Add( symbol_def, _evaluated_symbols.Count );
            try
            {
                /* Run the preprocessor on the symbol definition to expand any nested 
                 * symbolic references.
                 * Must call ToArray() to materialize the (deferred) resultset before the finally
                 * block removes the symbol */
                val = val.SelectMany<XNode,XNode>( _DefaultNodeProcessor.Process ).ToArray();
                if ( _HasJsFlag( name ) )
                {
                    val = _StringToNodeSet( Evaluator.StringAsLiteral( val.GetTextValue().Trim() ) );
                }
                return val;
            }
            finally
            {
                _evaluated_symbols.Remove( symbol_def );
            }
        }

        /// <summary>
        /// _s the process.	
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected IEnumerable< XNode > _Process(IEnumerable< XNode > nodes)
        {
            IEnumerable< XNode > processed_val =
                nodes.SelectMany< XNode, XNode >( _DefaultNodeProcessor.Process );
            //if ( processed_val.GetTextValue().Trim() != nodes.GetTextValue().Trim())
            //{
            //    return _Process( processed_val );
            //}
            return processed_val;
        }

        /// <summary>
        /// Evaluate the given JScript.NET expression, return the result
        /// as a text node.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public IEnumerable< XNode > EvalExpr(string expr)
        {
            IEnumerable< XNode > iterator;
            try
            {
                object obj2 = Evaluator.EvalToObject( expr );
                var dummy = obj2 as IEnumerable; 
                if ( !( obj2 is string ) && ( dummy != null ) )
                {
                    return _GetAsNodeSet( dummy );
                }
                iterator = _GetAsNodeSet( new[] {obj2.ToString()} );
            }
            catch ( EvaluationException )
            {
                throw;
            }
            catch ( Exception exception )
            {
                throw EvaluationException.CreateException( expr, exception );
            }
            return iterator;
        }

        /// <summary>
        /// Evaluate the given JScript expression, returning the evaluation result
        /// as a string.
        /// </summary>
        /// <param name="expr">JScript expression</param>
        /// <exception cref="EvaluationException"/>
        /// <returns></returns>
        public string EvalExprAsString(string expr)
        {
            string str;
            try
            {
                object obj2 = Evaluator.EvalToObject( expr );
                var dummy = obj2 as IEnumerable;
                if ( !( obj2 is string ) && ( dummy != null) )
                {
                    var builder = new StringBuilder();
                    foreach ( object obj3 in dummy )
                    {
                        builder.Append( obj3 );
                    }
                    return builder.ToString();
                }
                str = obj2.ToString();
            }
            catch ( Exception exception )
            {
                throw EvaluationException.CreateException( expr, exception );
            }
            return str;
        }


        /// <summary>
        /// Is the given nodeset either empty or entirely text nodes?
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private static bool _IsPureText(IEnumerable< XNode > nodes)
        {
            return nodes.Count( node => node is XText ) == nodes.Count();
        }

        /// <summary>        
        /// Evaluate any symbol references contained in the given string. References
        /// are in the form ($sym_name).
        /// </summary>
        /// <param name="input">Text value, possibliy containing symbolic
        /// references of the form $(sym_name)</param>
        /// <returns>A nodeset containing the input with symbolic references replaced by their
        /// current values in the execution environment.  All other text is converted to text nodes
        /// at the proper locations in the nodeset. If the input string is enclosed entirely in curly braces,
        /// the input will be run through the JScript evaluator after the symbolic references are resolved.
        /// Example 1:
        /// input string: "xxx$(var1)yyy"
        /// If  the current value of $(var1) is the XML &lt;content/&gt;, the output will be the following nodeset
        /// XText("xxx),XElement("content"),XText("yyy")
        /// Example 2:
        /// input string:{$(x)+$(y)}
        /// If x is bound to the text node "1" and y is bound to the text node "2", the result will be...
        /// {1+2}
        /// ...which will be run throught the JScript processor, producing an ultimate result of 
        /// XText("3")
        /// </returns>
        public IEnumerable< XNode > EvalTextSymbols(string input)
        {
            bool is_jscript = _IsJScriptExpression( input );
            if ( is_jscript )
            {
                // Remove the curly braces.
                input = input.Substring( 1, input.Length - 2 );
            }
            IEnumerable< XNode > node_set;
            // Match symbol references using a regex.
            MatchCollection reference_matches = _symbol_ref_matcher.Matches( input );

            var nodes = new List< XNode >();
            int idx = 0;
            // Convert the input to a nodeset, replacing symbolc references with their values.
            foreach ( Match match in reference_matches )
            {
                if ( idx < match.Index )
                {
                    nodes.Add( new XText( input.Substring( idx, match.Index - idx ) ) );
                }
                string ref_name = match.Groups[ 2 ].Value;
                IEnumerable< XNode > match_val = EvalSymbol( ref_name );
                // If this is a pure text value (no content other than text nodes)
                // and is a {JScript-expression}, evaluate the expression.
                if ( _IsPureText( match_val ) )
                {
                    string match_val_text = match_val.GetTextValue().Trim();
                    if ( _IsJScriptExpression( match_val_text ) )
                    {
                        match_val =
                            EvalExpr( match_val_text.Substring( 1, match_val_text.Length - 2 ) );
                    }
                }
                nodes.AddRange( match_val );
                idx = match.Index + match.Length;
            }
            if ( idx < input.Length )
            {
                nodes.Add( new XText( input.Substring( idx ) ) );
            }
            node_set = nodes;

            return is_jscript ? EvalExpr( node_set.GetTextValue() ) : node_set;
        }

        /// <summary>
        /// XSLT extension method that determines whether the given symbol has
        /// been defined in the preprocessor environment.
        /// </summary>
        /// <param name="symbol_name"></param>
        /// <returns></returns>
        public bool IsDefined(string symbol_name)
        {
            return _IsDefined( symbol_name );
        }

        /// <summary>
        /// Pop a frame from the Call stack.
        /// </summary>
        public IEnumerable< XNode > PopCall()
        {
            _define_stack.Pop();
            return new XNode[] {};
        }

        /// <summary>
        /// Pops the current item on the include stack
        /// </summary>
        /// <returns></returns>
        public IEnumerable< XNode > PopInclude()
        {
            _include_stack.Pop();
            return new XNode[] {};
        }

        /// <summary>
        /// Convenience method to safely execute any code inside a 
        /// push/pop pair.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public T Call< T >(Func< T > func)
        {
            PushCall();
            try
            {
                return func();
            }
            finally
            {
                PopCall();
            }
        }

        /// <summary>
        /// Push a frame onto the Call stack
        /// </summary>
        public IEnumerable< XNode > PushCall()
        {
            _define_stack.Push( new Dictionary< string, SymbolicDef >() );
            return new XNode[] {};
        }

        /// <summary>
        /// Loads the XML include at the given path and returns the XContainer representing it.
        /// The location is pushed onto the include stack so that any nested include directives can 
        /// be resolved relative to the path of the including file.
        /// </summary>
        /// <param name="href"></param>
        /// <returns></returns>
        public XContainer PushInclude(string href)
        {
            // Get the current base uri
            Uri base_url = _include_stack.Peek();
            // Resolve the href argument against the current base uri.
            Uri url = _resolver.ResolveUri( base_url, href );
            // Record for posterity
            AddToFileset( url );

            try
            {
                // Try to read in the document at the resolved url
                using (
                    XmlReader reader =
                        XmlReader.Create((Stream)_resolver.GetEntity(url, null, typeof(Stream)))
                    )
                {
                    XDocument document = XDocument.Load(reader,
                                                         LoadOptions.SetLineInfo |
                                                         LoadOptions.SetBaseUri |
                                                         LoadOptions.PreserveWhitespace);
                    // Load was successful, push the current base URL onto the include stack and set the
                    // resolver's base dir.
                    _include_stack.Push(url);
                    return document;
                }
            }
            catch (FileNotFoundException fnfe)
            {
                throw MissingIncludeException.CreateException("Failed to include file: " + fnfe.Message);
            }
            catch (DirectoryNotFoundException dnfe)
            {
                throw MissingIncludeException.CreateException("Failed to include file: " + dnfe.Message);
            }
            catch (System.Net.WebException we)
            {
                throw MissingIncludeException.CreateException("Failed to include file '{0}': {1}", url.AbsoluteUri, we.Message);
            }
        }

        /// <summary>
        /// XSLT extension method to halt processing based on a structural error
        /// in the markup.
        /// </summary>
        /// <param name="message"></param>
        public void ThrowInvalidMarkup(string message)
        {
            throw new InvalidMarkupException( message );
        }

        // Properties
    }
}
