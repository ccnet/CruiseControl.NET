using System;
using System.Runtime.Serialization;

namespace ThoughtWorks.CruiseControl.CCTrayLib
{
	[Serializable]
	public class IconNotFoundException : ApplicationException
	{
		public IconNotFoundException ()
		{
		}

		public IconNotFoundException (string error) : base (error)
		{
		}

		public IconNotFoundException (string error, Exception e) : base (error, e)
		{
		}

		protected IconNotFoundException(SerializationInfo info, StreamingContext context)
			: base (info, context)
		{
		}
	}
}