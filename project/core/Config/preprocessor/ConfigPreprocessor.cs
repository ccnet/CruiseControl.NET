using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor
{
    /// <summary>
    /// The preprocessor
    /// </summary>
    public class ConfigPreprocessor
    {
        private readonly PreprocessorSettings _settings;
        private PreprocessorEnvironment _env;

        /// <summary>
        /// Occurs when [subfile loaded].	
        /// </summary>
        /// <remarks></remarks>
        public event ConfigurationSubfileLoadedHandler SubfileLoaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigPreprocessor" /> class.	
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <remarks></remarks>
        public ConfigPreprocessor(PreprocessorSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigPreprocessor" /> class.	
        /// </summary>
        /// <remarks></remarks>
        public ConfigPreprocessor() : this( new PreprocessorSettings
                                          {
                                              ExplicitDeclarationRequired = false,
                                              InitialDefinitions =
                                                  new Dictionary< string, string >(),
                                              NamesAreCaseSensitve = false,
                                              UseOsEnvironment = true
                                          } )
        {
        }


        /// <summary>
        /// Pres the process.	
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        /// <param name="resolver">The resolver.</param>
        /// <param name="input_uri">The input_uri.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public PreprocessorEnvironment PreProcess(XmlReader input, XmlWriter output,
                                                  XmlUrlResolver resolver, Uri input_uri)
        {
            // The base URI is needed to resolve includes of relative paths, as well as to generate
            // error messages.
            // If none is given explicitly, try to use the XmlReader's BaseUri.  
            // If that doesn't exist either, use the current working directory and a fake filename.
            Uri base_uri = input_uri ??
                           ( String.IsNullOrEmpty( input.BaseURI )
                                 ? new Uri(
                                       Path.Combine(
                                           Environment.CurrentDirectory,
                                           "nofile.xml" ) )
                                 : new Uri( input.BaseURI ) );
            // Create the environment
            _env = new PreprocessorEnvironment( _settings, base_uri, resolver );
            // Load the input document
            XDocument doc = XDocument.Load( input, LoadOptions.PreserveWhitespace | LoadOptions.SetBaseUri | LoadOptions.SetLineInfo );
            // Process the input document's nodes and write the results to the output stream
            foreach ( XNode out_node in
                doc.Nodes().SelectMany(
                    node => _env._DefaultNodeProcessor.Process( node ) ).Where( node => !(node is XText)) )
            {
                out_node.WriteTo( output );
            }

            // Notify listeners of all files encountered.
            if (SubfileLoaded != null)
            {
                foreach (Uri path in _env.Fileset)
                {
                    SubfileLoaded( path );
                }
            }
            return _env;
        }
    }

    internal class AttrName
    {
        private AttrName()
        {}

        public static XName AssemblyLocation = "assembly-location";
        public static XName CounterName = "counter-name";
        public static XName CountExpr = "count-expr";
        public static XName Expr = "expr";
        public static XName Href = "href";
        public static XName InitExpr = "init-expr";
        public static XName IteratorExpr = "iterator-expr";
        public static XName IteratorName = "iterator-name";
        public static XName Max = "max";
        public static XName Name = "name";
        public static XName TestExpr = "test-expr";
        public static XName Type = "type";
    }
}
