using System;
using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Label;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.ControlPanel.Test
{

	[TestFixture]
	public class ConfigurationModelTest : CustomAssertion
	{
		public const string _configFileContents = @"<cruisecontrol>
  <project name='MyProject'>
    <sourcecontrol type='cvs'>
      <executable>C:\program files\tortoisecvs\cvs</executable>
      <workingDirectory>c:\dev\ccnet\projects\marathon.net</workingDirectory>
    </sourcecontrol>

    <build type='nant'>
      <executable>c:\dev\ccnet\projects\marathon.net\tools\nant\nant.exe</executable>
      <baseDirectory>c:\dev\ccnet\projects\marathon.net</baseDirectory>
      <buildFile>cruise.build</buildFile>
    </build>
  </project>
</cruisecontrol>";

		private Project project;
		private ConfigurationModel model;
		private Configuration configuration;

		[SetUp]
		public void SetUp() 
		{
			project = new Project();
			project.Name = "marathon.net";

			configuration = new Configuration();
			configuration.AddProject(project);

			model = new ConfigurationModel();
		}

		[Test]
		public void LoadProjectName()
		{
			model.Load(configuration);

			ConfigurationItem item = model.Projects[0].Items["name"];
			Assert.AreEqual("marathon.net", item.ValueAsString);
			Assert.AreEqual(null, item.AvailableValues);
			Assert.AreEqual(false, item.CanHaveChildren);

			item.ValueAsString = "nfit";

			Assert.AreEqual("nfit", project.Name);
		}

		[Test]
		public void LoadSourceControl()
		{
			// setup
		    Cvs cvs = new Cvs();
			cvs.Executable = "c:/bin/cvs.exe";
			cvs.CvsRoot = "/cvsroot/marathonnet";

			project.SourceControl = cvs;

			model.Load(configuration);

			// test

			ConfigurationItem item = model.Projects[0].Items["sourcecontrol"];

			Assert.AreEqual("cvs", item.ValueAsString);
			AssertContainsInArray("clearCase",item.AvailableValues);
			AssertContainsInArray("cvs",item.AvailableValues);
			AssertContainsInArray("defaultsourcecontrol",item.AvailableValues);
			AssertContainsInArray("clearCase",item.AvailableValues);
			AssertContainsInArray("clearCase",item.AvailableValues);
			AssertContainsInArray("clearCase",item.AvailableValues);
			Assert.AreEqual("c:/bin/cvs.exe", item.Items["executable"].ValueAsString);
			Assert.AreEqual("/cvsroot/marathonnet", item.Items["cvsroot"].ValueAsString);
			Assert.AreEqual(true, item.CanHaveChildren);

			// assign something

			item.Items["executable"].ValueAsString = "foobar";

			Assert.AreEqual("foobar", cvs.Executable);

			item.ValueAsString = "p4";
			item.Items["view"].ValueAsString = "crap";

			Assert.AreEqual(typeof(P4), project.SourceControl.GetType());
			Assert.AreEqual("crap", ((P4) project.SourceControl).View);
		}

		[Test]
		public void LoadNullAsAnAvailableValueForLabellers()
		{
		    DefaultLabeller labeller = new DefaultLabeller();
			labeller.LabelPrefix = "foo";

			project.Labeller = labeller;

			model.Load(configuration);

			ConfigurationItem item = model.Projects[0].Items["labeller"];

			Assert.AreEqual("defaultlabeller", item.ValueAsString);
			Assert.AreEqual(",defaultlabeller", string.Join(",", item.AvailableValues));

			Assert.AreEqual("", model.Projects[0].Items["sourcecontrol"].ValueAsString);

			item.ValueAsString = "";
			Assert.AreEqual(null, project.Labeller);

			item.ValueAsString = "defaultlabeller";
			Assert.AreEqual(typeof(DefaultLabeller), project.Labeller.GetType());
		}

		[Test, Ignore("nyi")]
		public void TestCollections() 
		{
		}

		[Test]
		public void BuildTree()
		{
			model.Load(configuration);

			// setup the project model to have the things in it that it should
			ProjectModel projectModel = model.Projects[0];
			projectModel.Items.Clear();

			MockItem build = new MockItem("build", "nant", true);
			build.Items.Add(new MockItem("exec", "nant.exe"));
			projectModel.Items.Add(build);
			projectModel.Items.Add(new MockItem("weburl", "http://localhost/marathon"));
			projectModel.Items.Add(new MockItem("name", "marathon.net"));
			projectModel.Items.Add(new MockItem("labeller", null, true));
			
			ConfigurationTreeNode configurationNode = model.GetNavigationTreeNodes();
			ConfigurationItemTreeNode projectNode = (ConfigurationItemTreeNode) configurationNode.Nodes[0];

			Assert.AreEqual(2, projectNode.Nodes.Count);

			// check 
			Assert.AreEqual("marathon.net", projectNode.Text);
			Assert.AreEqual("build", projectNode.Nodes[0].Text);
			Assert.AreEqual("labeller", projectNode.Nodes[1].Text);

			// subitems
			Assert.AreEqual("weburl", projectNode.Items[0].Name);
			Assert.AreEqual("name", projectNode.Items[1].Name);
		}

		[Test]
		public void LoadFromFile()
		{
			using (TempFiles files = new TempFiles()) 
			{
				files.Add("ccnet.config", _configFileContents);

				model.Load(files.MapPath("ccnet.config"));

				Assert.AreEqual("MyProject", model.Projects[0].Name);
			}
		}

		[Test]
		public void SaveBackToFile()
		{
			using (TempFiles files = new TempFiles()) 
			{
				files.Add("ccnet.config", _configFileContents);

				model.Load(files.MapPath("ccnet.config"));
				model.Save(files.MapPath("ccnet.out.config"));

				Assert.IsTrue(files.ContentsOf("ccnet.out.config").IndexOf("<webURL>http://localhost/CruiseControl.NET/</webURL>") != -1);

				model = new ConfigurationModel();
				model.Load(files.MapPath("ccnet.out.config"));
				
				Assert.AreEqual("cvs", model.Projects[0].Items["sourcecontrol"].ValueAsString);
			}
		}

		[Test]
		public void SaveOnProjectSavesAsWell()
		{
			using (TempFiles files = new TempFiles()) 
			{
				files.Add("ccnet.config", _configFileContents);
				model.Load(files.MapPath("ccnet.config"));

				files.Delete("ccnet.config");
				model.Save();

				Assert.IsTrue(files.ContentsOf("ccnet.config").IndexOf("<webURL>http://localhost/CruiseControl.NET/</webURL>") != -1);

				files.Delete("ccnet.config");
				model.Projects[0].Save();

				Assert.IsTrue(files.ContentsOf("ccnet.config").IndexOf("<webURL>http://localhost/CruiseControl.NET/</webURL>") != -1);
			}
		}

		[Test]
		public void SaveBackToFileThrowsExceptionAndDoesntSaveIfCantReload()
		{
			using (TempFiles files = new TempFiles()) 
			{
				files.Add("ccnet.config", _configFileContents);
				files.Add("ccnet.out.config", "old contents");

				model.Load(files.MapPath("ccnet.config"));
				model.Projects[0].Items["build"].Value = null;
				try 
				{
					model.Save(files.MapPath("ccnet.out.config"));
					Assert.Fail("should have failed");
				} 
				catch (ConfigurationException e) 
				{
					Assert.AreEqual("couldn't save because state is invalid :", e.Message);
				}

				Assert.AreEqual("old contents", files.ContentsOf("ccnet.out.config"));
			}
		}
	}

	public class TempFiles : IDisposable
	{
		private DirectoryInfo _tmpDir;

		public TempFiles() 
		{
			_tmpDir = new DirectoryInfo("tmp");
			_tmpDir.Create();
		}

		public void Dispose()
		{
			_tmpDir.Delete(true);
		}

		public void Add(string filename, string contents) 
		{
			using (StreamWriter writer = new StreamWriter(MapPath(filename))) 
			{
				writer.Write(contents);
			}
		}

		public void Delete(string filename) 
		{
			File.Delete(MapPath(filename));
		}

		public string ContentsOf(string filename)
		{
			using (StreamReader reader = new StreamReader(MapPath(filename)))
			{
				return reader.ReadToEnd();
			}
		}

		public string MapPath(string filename)
		{
			return Path.Combine(_tmpDir.FullName, filename);
		}
	}
}
