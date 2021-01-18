using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor.ElementProcessors
{
    /// <summary>
    /// Import external node processors types
    /// </summary>
    internal class ImportProcessor : ElementProcessor
    {
        public ImportProcessor(PreprocessorEnvironment env)
            : base(env._Settings.Namespace.GetName("import"), env)
        {
        }

        public override IEnumerable< XNode > Process(XNode node)
        {
            XElement element = _AssumeElement( node );
            Validation.RequireAttributes( element, AttrName.Type );
            string type_ref = element.GetAttributeValue( AttrName.Type ).TrimEnd();
            string assembly_loc = element.GetAttributeValue( AttrName.AssemblyLocation );
            Assembly assembly = null;
            if ( !String.IsNullOrEmpty( assembly_loc ) )
            {
                assembly = Assembly.LoadFrom( assembly_loc );
            }
            Type type;
            type = assembly != null ? assembly.GetType( type_ref ) : Type.GetType( type_ref );
            
            if (type == null)
            {
                throw ImportException.CreateException(
                    "Imported type '{0}' could not be found in '{1}'",
                    type_ref, assembly_loc);
            }
            
            var processor = Activator.CreateInstance( type, _Env ) as IElementProcessor;
            if ( processor == null )
            {
                throw ImportException.CreateException(
                    "Imported type '{0}' does not implement '{1}'",
                    type_ref, typeof ( IElementProcessor ).Name );
            }
            _Env._DefaultNodeProcessor._RegisterElementProcessor( processor );
            return new XNode[] {};
        }
    }
}
