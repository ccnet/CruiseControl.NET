using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace ThoughtWorks.CruiseControl.CCTray
{
	[Serializable]
	public class StatusIcon
	{
		private Icon _icon;
		
		public StatusIcon()
		{
		}

		public StatusIcon(String resourceName)
		{	 
			Stream stream = Assembly.GetCallingAssembly().GetManifestResourceStream(resourceName);
			_icon = System.Drawing.Icon.FromHandle(((Bitmap)Image.FromStream(stream)).GetHicon());
		}
		public StatusIcon(Icon i)
		{
		    _icon = i;
		}

		public static StatusIcon LoadFromFile(string file)
		{
			FileStream iconFile = null; 
			try
			{
				iconFile = File.Open(file,FileMode.Open);
				return new StatusIcon(new Icon(iconFile));
			}
			catch(SystemException ex)
			{
				throw new IconNotFoundException(file, ex);    
			}
			finally
			{
				if(iconFile != null)
					iconFile.Close();    
			}
		}

		public Icon Icon
	    {
	        get { return _icon; }
	    }

	}
}
