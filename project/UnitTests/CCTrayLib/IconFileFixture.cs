using System.Drawing;
using System.IO;
using ThoughtWorks.CruiseControl.CCTrayLib;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib
{
	/// <summary>
	/// Summary description for IconFileFixture.
	/// </summary>
	public class IconFileFixture
	{
		protected Icon _originalIcon; 
		protected string _file;

		public virtual void Init()
		{
			_originalIcon = new StatusIcon("ThoughtWorks.CruiseControl.CCTrayLib.Yellow.ico").Icon;
			_file = "./yellow.ico";
			using (FileStream stream = File.Create(_file))
			{
				_originalIcon.Save(stream);    
			}
		}

		public virtual void DeleteFile()
		{
		    File.Delete(_file); 
		}

	    public string FileName
	    {
	        get { return _file; }
	    }
	}
}
