
using System.IO;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class Utf8StringWriter : StringWriter
	{
		public override Encoding Encoding
		{
			get { return Encoding.UTF8; }
		}
	}
}