using System.Collections.Generic;
using System.Xml.Linq;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor.ElementProcessors
{
    internal class ConfigTemplateProcessor : ElementProcessor
    {
        public ConfigTemplateProcessor(PreprocessorEnvironment env)
            : base( env._Settings.Namespace.GetName("config-template"), env )
        {
        }

        public override IEnumerable< XNode > Process(XNode node)
        {
            return _ProcessNodes( _AssumeElement( node ).Nodes() );
        }
    }
}
