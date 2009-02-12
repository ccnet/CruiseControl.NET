using System.Drawing;
using System.IO;
using System.Reflection;

namespace ThoughtWorks.CruiseControl.CCTrayLib
{
    public static class IconUtil
    {
        /// <summary>
        /// If you just do new Icon(Stream) it downgrades any 24-bit icons to 8bit colour.  
        /// This function preserves the colour depth of 24bpp icons.
        /// </summary>
        public static Icon LoadFromStream(Stream stream)
        {
            using (Bitmap bmp = (Bitmap)Image.FromStream(stream))
                return Icon.FromHandle(bmp.GetHicon());
        }

        public static Icon LoadFromResource(string resourceName)
        {
            return LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName));
        }
    }
}
