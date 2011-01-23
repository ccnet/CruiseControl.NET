using System;
using System.IO;
using System.Reflection;

namespace ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils
{
	public class ResourceUtil
	{
		public static Stream LoadResource(Type type, string filename)
		{
			return Assembly.GetExecutingAssembly().GetManifestResourceStream(type, filename);
		}
	}
}