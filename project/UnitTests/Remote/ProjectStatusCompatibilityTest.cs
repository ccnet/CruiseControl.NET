using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
	[TestFixture]
	public class ProjectStatusCompatibilityTest
	{
		[Test]
		public void SaveAndLoadProjectStatus()
		{
			ProjectStatus projectStatus = new ProjectStatus("project", "category", ProjectActivity.Building, IntegrationStatus.Failure,
                                                            ProjectIntegratorState.Running, "http://localhost/ccnet", DateTime.Now, "1.0", "1.0", DateTime.Now, "building", "", 0);

			BinaryFormatter binaryFormatter = new BinaryFormatter();
			using (MemoryStream stream = new MemoryStream())
			{
				binaryFormatter.Serialize(stream, projectStatus);
				stream.Seek(0, SeekOrigin.Begin);
				binaryFormatter.Deserialize(stream);
			}
		}

		[Test, Explicit]	// backwards compatibility with CCNet 1.0 is no longer supported.
		public void Load10ProjectStatus()
		{
			DeserializeProjectStatus("ProjectStatus.1.0.bin");
		}

		[Test]
		public void Load11ProjectStatus()
		{
			DeserializeProjectStatus("ProjectStatus.1.1.bin");
		}

		[Test]
		public void Load111ProjectStatus()
		{
			DeserializeProjectStatus("ProjectStatus.1.1.1.bin");
		}

		[Test]
		public void Load12ProjectStatus()
		{
			DeserializeProjectStatus("ProjectStatus.1.2.bin");
		}

		[Test]
		public void Load121ProjectStatus()
		{
			DeserializeProjectStatus("ProjectStatus.1.2.1.bin");
		}

		private void DeserializeProjectStatus(string filename)
		{
			using (Stream stream = ResourceUtil.LoadResource(GetType(), filename))
			{
				new BinaryFormatter().Deserialize(stream);
			}
		}
	}
}
