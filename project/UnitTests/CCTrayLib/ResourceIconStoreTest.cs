using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib;
using ThoughtWorks.CruiseControl.UnitTests.CCTrayLib;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib
{
	[TestFixture]
	public class ResourceIconStoreTest : AbstractIconStoreTestCase
	{
		[SetUp]
		public override void Init()
		{
			base.Init();
		}

		protected override IIconStore CreateIconStore()
		{
			return new ResourceIconStore();    
		}

	    protected override void Validate(StatusIcon expectedIcon, StatusIcon actualIcon)
	    {
			Assert.AreSame(expectedIcon, actualIcon);
	    }
	}
}
