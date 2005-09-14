using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace ThoughtWorks.CruiseControl.CCTrayLib
{
	[Serializable]
	public class StatusIcon
	{
		private Icon icon;

		public StatusIcon()
		{
		}

		public StatusIcon(String resourceName)
		{
			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
			icon = Icon.FromHandle(((Bitmap) Image.FromStream(stream)).GetHicon());
		}

		public StatusIcon(Icon i)
		{
			icon = i;
		}

		public static StatusIcon LoadFromFile(string file)
		{
			try
			{
				using (FileStream iconFile = File.Open(file, FileMode.Open))
				{
					return new StatusIcon(new Icon(iconFile));
				}
			}
			catch (SystemException ex)
			{
				throw new IconNotFoundException(file, ex);
			}
		}

		public Icon Icon
		{
			get { return icon; }
		}

	}
}