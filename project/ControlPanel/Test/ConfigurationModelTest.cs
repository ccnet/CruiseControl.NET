using System;
using System.Collections;
using System.IO;
using System.Reflection;

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
			AssertEquals("cvs,defaultsourcecontrol,filesystem,multi,p4,pvcs,starteam,svn,vss", 
				string.Join(",", item.AvailableValues));

			AssertEquals("c:/bin/cvs.exe", item.Items["executable"].ValueAsString);
			AssertEquals("/cvsroot/marathonnet", item.Items["cvsroot"].ValueAsString);

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

		// test collections

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
