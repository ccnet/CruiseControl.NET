using System.Drawing;
using System.IO;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib
{
	public class IconFileFixture
	{
		protected Icon originalIcon;
		protected string file;

		public virtual void Init()
		{
			originalIcon = new Icon(GetType(), "TestIcon.ico");
			file = "./yellow.ico";
			using (FileStream stream = File.Create(file))
			{
				originalIcon.Save(stream);
			}
		}

		public virtual void DeleteFile()
		{
			File.Delete(file);
		}

		public string FileName
		{
			get { return file; }
		}
	}
}