using NUnit.Framework;

namespace ThoughtWorks.CruiseControl.Core.Builder.Test
{
	[TestFixture]
	public class DevenvTaskResultTest
	{
		[Test]
		public void CreateFailedXmlFromDevenvOutput()
		{
			string output = @"------ Build started: Project: Refactoring, Configuration: Debug .NET ------

Performing main compilation...
D:\dev\Refactoring\Movie.cs(30,2): error CS1513: } expected" + "\0" + @"

Build complete -- 1 errors, 0 warnings";

			string expected = @"<buildresults>" +
"<message>------ Build started: Project: Refactoring, Configuration: Debug .NET ------</message>" +
"<message>Performing main compilation...</message>" +
@"<message>D:\dev\Refactoring\Movie.cs(30,2): error CS1513: } expected</message>" +
"<message>Build complete -- 1 errors, 0 warnings</message>" +
"</buildresults>";

			DevenvTaskResult result = new DevenvTaskResult(output);
			Assert.AreEqual(expected, result.Data);
		}
	}
}
