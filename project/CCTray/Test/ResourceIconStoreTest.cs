using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTray.Test;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTray.Test
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
			AssertSame(expectedIcon, actualIcon);
	    }
	}
}
