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

		public StatusIcon(){}

		public StatusIcon(String resourceName)
		{
			icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName));
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
                    return new StatusIcon(new Icon(stream), true);
                }
            }
            catch (ArgumentException)
            {
                try
                {
                    // Attempt to load the icon from an image file instead
                    using (FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read))
                    {
                        using (Bitmap image = new Bitmap(stream))
                        {
                            return new StatusIcon(Icon.FromHandle(image.GetHicon()), true);
                        }
                    }
                }
                catch (SystemException ex)
                {
                    throw new IconNotFoundException(file, ex);
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
