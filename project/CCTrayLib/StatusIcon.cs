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
			icon = LoadIconFromStreamPreservingColourDepth(stream);
		}

		private static Icon LoadIconFromStreamPreservingColourDepth(Stream stream)
		{
			// If you just do new Icon(Stream) it downgrades any 24-bit icons to 8bit colour.  
			// This magic incantation preserves the colur depth of 24bpp icons.
			return Icon.FromHandle(((Bitmap) Image.FromStream(stream)).GetHicon());
		}

		public StatusIcon(Icon i)
		{
			icon = i;
		}

		public static StatusIcon LoadFromFile(string file)
		{
			try
			{
				using (FileStream stream = File.Open(file, FileMode.Open))
				{
					return new StatusIcon(LoadIconFromStreamPreservingColourDepth(stream));
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