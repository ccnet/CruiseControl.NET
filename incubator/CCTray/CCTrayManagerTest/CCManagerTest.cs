using System;
using System.Windows.Forms;
using NUnit.Framework;
using CCManager;

namespace CCManager.Test
{
	[TestFixture]
	public class CCManagerTest
	{
		CCTray tray;
		[SetUp]
		public void SetUp()
		{
			tray = new CCTray();
		}
		
		[Test]
		public void NoConnection()
		{
			Assertion.Assert("Start should not be enabled, check that CC.NET is not running", !tray.contextMenu.MenuItems[1].Enabled);
			Assertion.Assert("Stop should not be enabled, check that CC.NET is not running", !tray.contextMenu.MenuItems[2].Enabled);
		}
		
		[TearDown]
		public void TearDown()
		{
			tray.exit(null, null);
		}
	}
}