using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace ThoughtWorks.CruiseControl.CCTray
{
	public class StatusIcon
	{
		private Icon _icon;
		public static readonly StatusIcon NOW_BUILDING = new StatusIcon("ThoughtWorks.CruiseControl.CCTray.Yellow.ico");
		public static readonly  StatusIcon EXCEPTION = new StatusIcon("ThoughtWorks.CruiseControl.CCTray.Gray.ico");
		public static readonly  StatusIcon SUCCESS = new StatusIcon("ThoughtWorks.CruiseControl.CCTray.Green.ico");
		public static readonly StatusIcon FAILURE = new StatusIcon("ThoughtWorks.CruiseControl.CCTray.Red.ico");
		public static readonly StatusIcon UNKNOWN = new StatusIcon("ThoughtWorks.CruiseControl.CCTray.Gray.ico");

		private StatusIcon(String fileName)
		{	 Stream stream = Assembly.GetCallingAssembly().GetManifestResourceStream(fileName);
			_icon = System.Drawing.Icon.FromHandle(((Bitmap)Image.FromStream(stream)).GetHicon());
		}

	    public Icon Icon
	    {
	        get { return _icon; }
	    }

	}
}
