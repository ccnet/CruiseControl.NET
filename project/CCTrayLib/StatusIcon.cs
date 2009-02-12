using System;
using System.Drawing;
using System.IO;

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
		    icon = IconUtil.LoadFromResource(resourceName);
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
					return new StatusIcon(IconUtil.LoadFromStream(stream), true);
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
