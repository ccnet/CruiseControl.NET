using NUnit.Framework;
using System;

namespace ThoughtWorks.CruiseControl.Core.Config.Test
{
	[TestFixture]
	public class ConfigurationTest
	{
		[Test]
		public void CreateIntegrators()
		{
			Project project1 = new Project();
			project1.Name = "project1";
			Project project2 = new Project();
			project2.Name = "project2";

			Configuration config = new Configuration();
			config.AddProject(project1);
			config.AddProject(project2);
		}
	}
}
