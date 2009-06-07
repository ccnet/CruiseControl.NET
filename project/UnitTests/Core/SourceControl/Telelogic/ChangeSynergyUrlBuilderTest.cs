using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol.Telelogic;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol.Telelogic
{
	[TestFixture]
	public class ChangeSynergyUrlBuilderTest
	{
		protected Synergy synergy;
		protected ChangeSynergyUrlBuilder synergyUrlBuilder;

		private string[] expectedUrls = new string[]
			{
				@"http://myserver:8060/servlet/com.continuus.webpt.servlet.PTweb?ACTION_FLAG=frameset_form&TEMPLATE_FLAG=TaskDetailsView&task_number=100&role=User&database=%5c%5cmyserver%5cshare%5cmydatabase&user=csuser",
				@"http://myserver:8060/servlet/com.continuus.webpt.servlet.PTweb?ACTION_FLAG=frameset_form&TEMPLATE_FLAG=TaskDetailsView&task_number=2000&role=User&database=%5c%5cmyserver%5cshare%5cmydatabase&user=csuser",
				@"http://myserver:8060/servlet/com.continuus.webpt.servlet.PTweb?ACTION_FLAG=frameset_form&TEMPLATE_FLAG=TaskDetailsView&task_number=30000&role=User&database=%5c%5cmyserver%5cshare%5cmydatabase&user=csuser"
			};

		[SetUp]
		public virtual void SetUp()
		{
			/* TODO We should test use of custom environmental variables.
             *      .NET 2.0 adds the method Environment.SetEnvironmentVariable(),
             *      which would support this approach */

			synergy = (Synergy) NetReflector.Read(SynergyMother.ConfigValues);
			Assert.IsNotNull(synergy.UrlBuilder);
			Assert.IsTrue(synergy.UrlBuilder is ChangeSynergyUrlBuilder);
			synergyUrlBuilder = synergy.UrlBuilder as ChangeSynergyUrlBuilder;
			Assert.IsNotNull(synergyUrlBuilder);
		}

		[Test]
		public virtual void Config()
		{
			Assert.AreSame(synergy.Connection.Database, synergyUrlBuilder.Database);
			Assert.AreEqual(@"http://myserver:8060", synergyUrlBuilder.Url);
			Assert.AreEqual(@"User", synergyUrlBuilder.Role);
			Assert.AreEqual("csuser", synergyUrlBuilder.Username);
			Assert.IsFalse("readonly" == synergyUrlBuilder.Password);
		}

		[Test]
		public void PasswordObfuscation()
		{
			ChangeSynergyUrlBuilder synergyUrlBuilder = new ChangeSynergyUrlBuilder();

			Assert.AreEqual("0:0,0,0,0,0,0,0,0", synergyUrlBuilder.ObfuscatePassword(0, "password"));
			Assert.AreEqual("1:112,97,115,115,119,111,114,100", synergyUrlBuilder.ObfuscatePassword(1, "password"));
			Assert.AreEqual("999:111888,96903,114885,114885,118881,110889,113886,99900", synergyUrlBuilder.ObfuscatePassword(999, "password"));
			Assert.AreEqual("1000:112000,97000,115000,115000,119000,111000,114000,100000", synergyUrlBuilder.ObfuscatePassword(1000, "password"));

			Assert.AreEqual("500:45500,42000,52000,34500,16000,40500,58500,16500,33500,53500,16000,33000,57000,24000,59500,55000,16000,35000,55500,47000,16000,53000,42500,54500,56000,18000,16000,39500,59000,25500,57000,16000,58000,52000,50500,16000,62000,32000,61000,60500,16000,50000,55500,51500,62500", synergyUrlBuilder.ObfuscatePassword(500, "[ThE Qu!Ck Br0wn Fo^ jUmp$ Ov3r the |@zy dog}"));
		}

		[Test]
		public virtual void SetupModification()
		{
			const int size = 3;
			Modification[] testModifications = CreateModifications();

			synergy.UrlBuilder.SetupModification(testModifications);

			// strip out the obfuscated password, which was already tested in the test case PasswordObfuscation
			for (int i = 0; i < size; i++)
			{
				int start = testModifications[i].Url.IndexOf("&generic_data=");
				int end = testModifications[i].Url.Length;
				testModifications[i].Url = testModifications[i].Url.Remove(start, end - start);
				Assert.AreEqual(expectedUrls[i], testModifications[i].Url);
			}
		}

		protected Modification[] CreateModifications()
		{
			Modification[] testModifications = new Modification[]
				{
					new Modification(), new Modification(), new Modification()
				};
			testModifications[0].ChangeNumber = "100";
			testModifications[1].ChangeNumber = "2000";
			testModifications[2].ChangeNumber = "30000";
			return (testModifications);
		}
	}

	[TestFixture]
	public class ChangeSynergyLoginTest : ChangeSynergyUrlBuilderTest
	{
		private string[] expectedUrls = new string[]
			{
				@"http://myserver:8060/servlet/com.continuus.webpt.servlet.PTweb?ACTION_FLAG=tokenless_form&TEMPLATE_FLAG=ConfirmLogin&role=User&database=%5c%5cmyserver%5cshare%5cmydatabase&context=%2fservlet%2fcom.continuus.webpt.servlet.PTweb%3fACTION_FLAG%3dframeset_form%26TEMPLATE_FLAG%3dTaskDetailsView%26role%3dUser%26database%3d%255c%255cmyserver%255cshare%255cmydatabase%26task_number%3d100",
				@"http://myserver:8060/servlet/com.continuus.webpt.servlet.PTweb?ACTION_FLAG=tokenless_form&TEMPLATE_FLAG=ConfirmLogin&role=User&database=%5c%5cmyserver%5cshare%5cmydatabase&context=%2fservlet%2fcom.continuus.webpt.servlet.PTweb%3fACTION_FLAG%3dframeset_form%26TEMPLATE_FLAG%3dTaskDetailsView%26role%3dUser%26database%3d%255c%255cmyserver%255cshare%255cmydatabase%26task_number%3d2000",
				@"http://myserver:8060/servlet/com.continuus.webpt.servlet.PTweb?ACTION_FLAG=tokenless_form&TEMPLATE_FLAG=ConfirmLogin&role=User&database=%5c%5cmyserver%5cshare%5cmydatabase&context=%2fservlet%2fcom.continuus.webpt.servlet.PTweb%3fACTION_FLAG%3dframeset_form%26TEMPLATE_FLAG%3dTaskDetailsView%26role%3dUser%26database%3d%255c%255cmyserver%255cshare%255cmydatabase%26task_number%3d30000"
			};

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			synergyUrlBuilder.Username = null;
			synergyUrlBuilder.Password = null;
		}

		[Test]
		public override void Config()
		{
			Assert.AreSame(synergy.Connection.Database, synergyUrlBuilder.Database);
			Assert.AreEqual(@"http://myserver:8060", synergyUrlBuilder.Url);
			Assert.AreEqual(@"User", synergyUrlBuilder.Role);
			Assert.IsNull(synergyUrlBuilder.Username);
			Assert.IsNull(synergyUrlBuilder.Password);
		}

		[Test]
		public override void SetupModification()
		{
			const int size = 3;
			Modification[] testModifications = CreateModifications();

			synergy.UrlBuilder.SetupModification(testModifications);

			// strip out the obfuscated password, which was already tested in the test case PasswordObfuscation
			for (int i = 0; i < size; i++)
			{
				Assert.AreEqual(expectedUrls[i], testModifications[i].Url);
			}
		}
	}
}