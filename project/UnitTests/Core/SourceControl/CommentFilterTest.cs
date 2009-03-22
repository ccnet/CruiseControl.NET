using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
    [TestFixture]
	public class CommentFilterTest
	{
        [Test]
        [ExpectedException(typeof(NetReflectorException), "Missing Xml node (pattern) for required member (ThoughtWorks.CruiseControl.Core.Sourcecontrol.CommentFilter.Pattern).\r\n" +
            "Xml: <commentFilter />")]
        public void ShouldNotPopulateWithoutPattern()
        {
            NetReflector.Read(@"<commentFilter/>");
        }

        [Test]
        public void ShouldPopulateFromMinimalSimpleXml()
        {
            CommentFilter filter = (CommentFilter) NetReflector.Read(@"<commentFilter pattern="".*""/>");
            Assert.AreEqual(".*", filter.Pattern, "Wrong filter value found.");
        }

        [Test]
        public void ShouldPopulateFromMinimalComplexXml()
        {
            CommentFilter filter = (CommentFilter) NetReflector.Read(@"<commentFilter> <pattern>.*</pattern> </commentFilter>");
            Assert.AreEqual(".*", filter.Pattern, "Wrong filter value found.");
        }

        [Test]
		public void ShouldRejectModificationsWithNullComments()
		{
			Modification modification = new Modification();
            CommentFilter filter = new CommentFilter();
			filter.Pattern = ".*";
			Assert.IsFalse(filter.Accept(modification), "Should not have matched but did.");
		}

        [Test]
        public void ShouldAcceptModificationsWithMatchingComments()
        {
            Modification modification = new Modification();
            modification.Comment = "This is a comment.";
            CommentFilter filter = new CommentFilter();
            filter.Pattern = ".* is a .*";
            Assert.IsTrue(filter.Accept(modification), "Should have matched but did not.");
        }

        [Test]
        public void ShouldRejectModificationsWithMatchingComments()
        {
            Modification modification = new Modification();
            modification.Comment = "This is a comment.";
            CommentFilter filter = new CommentFilter();
            filter.Pattern = ".* is not a .*";
            Assert.IsFalse(filter.Accept(modification), "Should not have matched but did.");
        }
    }
}

