using System;
using NUnit.Framework;

using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.util.test
{
	[TestFixture]
	public class ProcessInfoTest : CustomAssertion
	{
		[Test]
		public void IfStandardInputContentIsSetThenStandardInputIsRedirected()
		{
			ProcessInfo info = new ProcessInfo("temp");
			info.StandardInputContent = "Some content";

			Assert(info.StartInfo.RedirectStandardInput);
			Assert(!info.StartInfo.UseShellExecute);
		}

		[Test]
		public void IfStandardInputContentIsNotSetThenStandardInputIsNotRedirected()
		{
			ProcessInfo info = new ProcessInfo("temp");

			Assert(!info.StartInfo.RedirectStandardInput);
		}
	}
}
