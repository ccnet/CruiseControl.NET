using System.Collections.Generic;
using System.Xml.Linq;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor.ElementProcessors
{
    internal class IgnoreProcessor : ElementProcessor
    {
        public IgnoreProcessor(XName element_name_to_ignore, PreprocessorEnvironment env)
            : base( element_name_to_ignore, env )
        {
        }

        public override IEnumerable< XNode > Process(XNode node)
        {
            return new XNode[] {};
        }
    }
}
