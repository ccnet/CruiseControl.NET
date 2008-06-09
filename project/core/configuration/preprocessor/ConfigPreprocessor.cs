using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor
{
    /// <summary>
    /// Provides preprocessing facility for XML documents.  Preprocessing performs the following
    /// services:
    /// 1. Expansion of includes
    /// 2. Definition and expansion of constants    
    /// </summary>
    internal class ConfigPreprocessor
    {
        private ConfigPreprocessorEnvironment _current_env;        

        /// <summary>
        /// Run the given input reader through the preprocessor, writing it
        /// to the given output writer.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        /// <param name="resolver"></param>
        /// <param name="input_uri"></param>
        /// <exception cref="EvaluationException">Error occurred during the evaluation of a constant</exception>
        public ConfigPreprocessorEnvironment PreProcess(XmlReader input,
                                                         XmlWriter output,
            PreprocessorUrlResolver resolver,
            Uri input_uri
            )
        {
            XsltSettings xslt_settings = new XsltSettings(true, true);

#if DEBUG
            XslCompiledTransform xslt_preprocess = new XslCompiledTransform(true);
#else
            XslCompiledTransform xslt_preprocess = new XslCompiledTransform( false );
#endif

            using (XmlReader xslt_reader = XmlReader.Create(Utils.GetAssemblyResourceStream(
                    "ThoughtWorks.CruiseControl.Core.configuration.preprocessor.ConfigPreprocessor.xslt")))
            {
                xslt_preprocess.Load(
                    xslt_reader, xslt_settings, new XmlUrlResolver());
            }            

            // Install a special XmlResolver for the resolution of 
            // preprocessor includes (which use the xslt document() function).
            // The resolver keeps track of the URI path of the outermost document and any includes
            // and uses it to resolve relative include paths.  
            if ( resolver == null )
            {
                resolver = new PreprocessorUrlResolver();
            }

            XsltArgumentList xslt_args = new XsltArgumentList();

            Uri base_uri = input_uri ??
                           ( String.IsNullOrEmpty( input.BaseURI )
                                 ? new Uri(
                                       Path.Combine(
                                           Environment.CurrentDirectory,
                                           "nofile.xml" ) )
                                 : new Uri( input.BaseURI ) );
            _current_env = new ConfigPreprocessorEnvironment( base_uri, resolver );

            // The XSLT calls extension functions in _current_env.
            xslt_args.AddExtensionObject("environment", _current_env);
            try
            {
                xslt_preprocess.Transform( input, xslt_args, output, null );
            }
            catch( XsltException ex )
            {
                if ( ex.InnerException != null )
                {                    
                    // Get the _remoteStackTraceString of the Exception class
                    FieldInfo remoteStackTraceString = typeof(Exception).GetField("_remoteStackTraceString",
                    BindingFlags.Instance | BindingFlags.NonPublic );
                    
                    // Set the InnerException._remoteStackTraceString to the current InnerException.StackTrace
                    remoteStackTraceString.SetValue( ex.InnerException, 
                    ex.InnerException.StackTrace + Environment.NewLine );                    
                    // Throw the new exception
                    throw ex.InnerException;
                }
                throw;
            }

            // Notify listeners of all files encountered.
            if ( SubfileLoaded != null )
            {
                foreach ( string path in _current_env.Fileset )
                {             
                    SubfileLoaded( path );
                }
            }

            return _current_env;
        }        

        public event ConfigurationSubfileLoadedHandler SubfileLoaded;
    }
}