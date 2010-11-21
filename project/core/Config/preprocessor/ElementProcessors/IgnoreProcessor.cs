using System.Collections.Generic;
using System.Xml.Linq;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor.ElementProcessors
{
    internal class IgnoreProcessor : ElementProcessor
    {
        public IgnoreProcessor(XName elementNameToIgnore, PreprocessorEnvironment env)
            : base( elementNameToIgnore, env )
        {
        }

        public override IEnumerable< XNode > Process(XNode node)
        {
            return new XNode[] {};
        }
    }
}
