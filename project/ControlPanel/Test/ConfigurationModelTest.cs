using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using NUnit.Framework;
using Exortech.NetReflector;

using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Label;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.ControlPanel.Test
{

	[TestFixture]
	public class ConfigurationModelTest : Assertion
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
			AssertEquals("marathon.net", item.ValueAsString);
			AssertEquals(null, item.AvailableValues);
			AssertEquals(false, item.CanHaveChildren);

			item.ValueAsString = "nfit";

			AssertEquals("nfit", project.Name);
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

			AssertEquals("cvs", item.ValueAsString);
			AssertEquals("clearCase,cvs,defaultsourcecontrol,filesystem,multi,p4,pvcs,starteam,svn,vss", 
				string.Join(",", item.AvailableValues));

			AssertEquals("c:/bin/cvs.exe", item.Items["executable"].ValueAsString);
			AssertEquals("/cvsroot/marathonnet", item.Items["cvsroot"].ValueAsString);
			AssertEquals(true, item.CanHaveChildren);

			// assign something

			item.Items["executable"].ValueAsString = "foobar";

			AssertEquals("foobar", cvs.Executable);

			item.ValueAsString = "p4";
			item.Items["view"].ValueAsString = "crap";

			AssertEquals(typeof(P4), project.SourceControl.GetType());
			AssertEquals("crap", ((P4) project.SourceControl).View);
		}

		[Test]
		public void LoadNullAsAnAvailableValueForLabellers()
		{
			DefaultLabeller labeller = new DefaultLabeller();
			labeller.LabelPrefix = "foo";

			project.Labeller = labeller;

			model.Load(configuration);

			ConfigurationItem item = model.Projects[0].Items["labeller"];

			AssertEquals("defaultlabeller", item.ValueAsString);
			AssertEquals(",defaultlabeller", string.Join(",", item.AvailableValues));

			AssertEquals("", model.Projects[0].Items["sourcecontrol"].ValueAsString);

			item.ValueAsString = "";
			AssertEquals(null, project.Labeller);

			item.ValueAsString = "defaultlabeller";
			AssertEquals(typeof(DefaultLabeller), project.Labeller.GetType());
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

			AssertEquals(2, projectNode.Nodes.Count);

			// check 
			AssertEquals("marathon.net", projectNode.Text);
			AssertEquals("build", projectNode.Nodes[0].Text);
			AssertEquals("labeller", projectNode.Nodes[1].Text);

			// subitems
			AssertEquals("weburl", projectNode.Items[0].Name);
			AssertEquals("name", projectNode.Items[1].Name);
		}

		[Test]
		public void LoadFromFile()
		{
			using (TempFiles files = new TempFiles()) 
			{
				files.Add("ccnet.config", _configFileContents);

				model.Load(files.MapPath("ccnet.config"));

				AssertEquals("MyProject", model.Projects[0].Name);
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

				Assert(files.ContentsOf("ccnet.out.config").IndexOf("<webURL>http://localhost/CruiseControl.NET/</webURL>") != -1);

				model = new ConfigurationModel();
				model.Load(files.MapPath("ccnet.out.config"));
				
				AssertEquals("cvs", model.Projects[0].Items["sourcecontrol"].ValueAsString);
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

				Assert(files.ContentsOf("ccnet.config").IndexOf("<webURL>http://localhost/CruiseControl.NET/</webURL>") != -1);

				files.Delete("ccnet.config");
				model.Projects[0].Save();

				Assert(files.ContentsOf("ccnet.config").IndexOf("<webURL>http://localhost/CruiseControl.NET/</webURL>") != -1);
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
					Fail("should have failed");
				} 
				catch (ConfigurationException e) 
				{
					AssertEquals("couldn't save because state is invalid :", e.Message);
				}

				AssertEquals("old contents", files.ContentsOf("ccnet.out.config"));
			}
		}

		private void Print(ConfigurationItemCollection items, string indent) 
		{
			foreach (ConfigurationItem item in items)
			{
				Console.WriteLine(indent + item.Name + "=" + item.ValueAsString);
				Print(item.Items, indent + "    ");
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
