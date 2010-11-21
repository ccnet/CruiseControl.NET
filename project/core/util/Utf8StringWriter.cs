
using System.IO;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// 	
    /// </summary>
	public class Utf8StringWriter : StringWriter
	{
        /// <summary>
        /// Gets the encoding.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public override Encoding Encoding
		{
			get { return Encoding.UTF8; }
		}
	}
}