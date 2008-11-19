using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace ThoughtWorks.CruiseControl.CCTrayLib
{
	[Serializable]
	public class StatusIcon : IDisposable
	{
		private Icon icon;

		[NonSerialized]
		private bool ownIcon = true;

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
			using (Bitmap bmp = (Bitmap)Image.FromStream(stream))
				return Icon.FromHandle(bmp.GetHicon());
		}

		public StatusIcon(Icon i)
			: this(i, false)
		{
		}

		public StatusIcon(Icon i, bool ownIcon)
		{
			icon = i;
			this.ownIcon = ownIcon;
		}

		public void Dispose()
		{
			if (ownIcon)
			{
				icon.Dispose();

				icon = null;
				ownIcon = false;
			}
		}

		public static StatusIcon LoadFromFile(string file)
		{
			try
			{
				using (FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read))
				{
					return new StatusIcon(LoadIconFromStreamPreservingColourDepth(stream), true);
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
