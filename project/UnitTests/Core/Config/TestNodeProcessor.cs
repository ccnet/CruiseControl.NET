using System.Collections.Generic;
using System.Xml.Linq;
using ThoughtWorks.CruiseControl.Core.Config.Preprocessor;
using ThoughtWorks.CruiseControl.Core.Config.Preprocessor.ElementProcessors;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Config.Preprocessor
{
    public class TestElementProcessor : ElementProcessor
    {
        public TestElementProcessor(PreprocessorEnvironment env)
            : base( XmlNs.PreProcessor.GetName( "test" ), env )
        {
        }

        public override IEnumerable< XNode > Process(XNode node)
        {
            return new[] {new XText( "Hello From TestElementProcessor!" )};
        }
    }
}
