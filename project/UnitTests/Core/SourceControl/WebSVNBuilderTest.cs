using System;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class WebSVNUrlBuilderTest : CustomAssertion
	{
		const string URLXML = "http://localhost:7899/websvn/filedetails.php?rep=3&amp;path={0}&amp;rev={1}&amp;sc=1";
		const string URL = "http://localhost:7899/websvn/filedetails.php?rep=3&path={0}&rev={1}&sc=1";

		static string CreateSourceControlXml()
		{
			return string.Format( "<webUrlBuilder type=\"websvn\"><url>{0}</url></webUrlBuilder>", URLXML );	
		}

		static WebSVNUrlBuilder CreateBuilder() 
		{
			WebSVNUrlBuilder svnurl = new WebSVNUrlBuilder();
			NetReflector.Read( CreateSourceControlXml(), svnurl );
			return svnurl;
		}

		[Test]
		public void ValuePopulation()
		{
			WebSVNUrlBuilder svnurl = CreateBuilder();
			
			Assert.AreEqual( URL, svnurl.Url );
		}

		[Test]
		public void CheckSetup()
		{
			Modification[] mods = new Modification[2];
			mods[0] = new Modification();
			mods[0].FolderName = "/trunk";
			mods[0].FileName = "nant.bat";
			mods[0].ChangeNumber = 3;
			mods[1] = new Modification();
			mods[1].FolderName = "/trunk/MiniACE.Test";
			mods[1].FileName = "AssemblyInfo.cs";
			mods[1].ChangeNumber = 2;

			CreateBuilder().SetupModification(mods);

			string url0 = String.Format( URL, "/trunk/nant.bat", 3 );
			string url1 = String.Format( URL, "/trunk/MiniACE.Test/AssemblyInfo.cs", 2 );

			Assert.AreEqual( url0, mods[0].Url );
			Assert.AreEqual( url1, mods[1].Url );
		}
	}
}
