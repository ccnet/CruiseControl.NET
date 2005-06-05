using System;
using System.Drawing;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Presentation
{
	[TestFixture]
	public class TrayIconTest : IIconProvider
	{
		private Icon icon;

		[Test]
		public void CanSubscribeToAnIconProvider()
		{
			TrayIcon trayIcon = new TrayIcon();
			Assert.IsNull( trayIcon.Icon );

			icon = ResourceProjectStateIconProvider.GRAY.Icon;
			trayIcon.BindToIconProvider( this );

			Assert.AreSame( icon, trayIcon.Icon );
		}

		[Test]
		public void UpdatesIconWhenTheIconProviderChangesItsIcon()
		{
			// my original plan was to use WinForms databinding 
			// for this task.  Alas NotifyIconEx is a Component
			// not a Control: apparently you need to be a control
			// in order to support databinding...
			TrayIcon trayIcon = new TrayIcon();

			icon = ResourceProjectStateIconProvider.GRAY.Icon;
			trayIcon.BindToIconProvider( this );

			Assert.AreSame( icon, trayIcon.Icon );

			icon = ResourceProjectStateIconProvider.RED.Icon;

			if (IconChanged != null)
				IconChanged( this, EventArgs.Empty );

			Assert.AreSame( icon, trayIcon.Icon );

		}

		public Icon Icon
		{
			get { return icon; }
		}
		public event EventHandler IconChanged;
	}
}