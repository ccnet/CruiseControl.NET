using System;
using System.Collections;
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
		[Test]
		public void ProjectName()
		{
			Project project = new Project();
			project.Name = "marathon.net";

			ConfigurationModel model = new ConfigurationModel();
			model.Load(project);

			ConfigurationItem item = model.Items["name"];
			AssertEquals("marathon.net", item.ValueAsString);
			AssertEquals(null, item.AvailableValues);

			item.ValueAsString = "nfit";

			AssertEquals("nfit", project.Name);
		}

		private void Print(ConfigurationItemCollection items, string indent) 
		{
			foreach (ConfigurationItem item in items)
			{
				Console.WriteLine(indent + item.Name + "=" + item.ValueAsString);
				Print(item.Items, indent + "    ");
			}
		}

		[Test]
		public void SourceControl()
		{
			Cvs cvs = new Cvs();
			cvs.Executable = "c:/bin/cvs.exe";
			cvs.CvsRoot = "/cvsroot/marathonnet";

			Project project = new Project();
			project.Name = "marathon.net";
			project.SourceControl = cvs;

			ConfigurationModel model = new ConfigurationModel();
			model.Load(project);

			ConfigurationItem item = model.Items["sourcecontrol"];
			//Print(model.Items, "");

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
		public void NullAsAnAvailableValueForLabellers()
		{
			DefaultLabeller labeller = new DefaultLabeller();
			labeller.LabelPrefix = "foo";

			Project project = new Project();
			project.Labeller = labeller;

			ConfigurationModel model = new ConfigurationModel();
			model.Load(project);

			ConfigurationItem item = model.Items["labeller"];

			AssertEquals("defaultlabeller", item.ValueAsString);
			AssertEquals(",defaultlabeller", string.Join(",", item.AvailableValues));

			AssertEquals("", model.Items["sourcecontrol"].ValueAsString);

			item.ValueAsString = "";
			AssertEquals(null, project.Labeller);

			item.ValueAsString = "defaultlabeller";
			AssertEquals(typeof(DefaultLabeller), project.Labeller.GetType());
		}

		// test collections
	}
}
