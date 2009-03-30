using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Config.Preprocessor;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Config
{
    [TestFixture]
    public class PreprocessorTest
    {
    	private static readonly string FAKE_ROOT = Path.DirectorySeparatorChar == '\\' ? "c:\\temp folder\\" : "/tmp/";

        private readonly PreprocessorUrlResolver _resolver =
            new PreprocessorUrlResolver();

        [Test]
        public void TestDefineConst()
        {
            using (XmlReader input = GetInput("TestDefineConst.xml"))
            {
                using (XmlWriter output = GetOutput())
                {
                    ConfigPreprocessor preprocessor = new ConfigPreprocessor();
                    ConfigPreprocessorEnvironment env =
                        preprocessor.PreProcess(input, output, _resolver, null );
                    Assert.AreEqual(
                        env._GetConstantDef( "var1" ).Value, "value1" );                    
                }
            }
        }        

        [Test]
        public void TestUseConst()
        {
            XmlDocument doc = _Preprocess( "TestUseConst.xml" );
            AssertNodeValue(doc, "//hello/@attr1", "value1");
        }

        [Test]
        public void TestUseNestedConst()
        {
            XmlDocument doc = _Preprocess( "TestUseNestedConst.xml" );
            AssertNodeValue(doc, "//hello/@attr1", "value1+value2");            
        }

        [Test]
        public void TestUseNestedConst2()
        {
            XmlDocument doc = _Preprocess( "TestUseNestedConst2.xml" );
            AssertNodeValue(doc, "//project/@queue", "myqueue");
            AssertNodeValue(doc, "//project/name", "myproject Release");
            AssertNodeValue(doc, "//project/webURL", "localhost/?_action_ViewProjectReport=true&server=local&project=myproject%20Release");
            AssertNodeValue(doc, "//project/tasks/buildArgs", "/noconsolelogger /p:output-path=C:\\workspace\\myproject\\myproject-release.zip");
        }

        [Test]
        public void TestUseMacro()
        {
            XmlDocument doc = _Preprocess( "TestExpandNodeset.xml" );
            AssertNodeValue( doc.CreateNavigator(), "//a/@name", "fooval" );
            AssertNodeValue( doc.CreateNavigator(), "//b/@name", "barval" );
        }

        [Test]
        public void TestUseMacroWithXmlArgs()
        {
            XmlDocument doc = _Preprocess( "TestExpandNodesetWithParams.xml" );
            AssertNodeExists( doc.CreateNavigator(), "/c/hello/a/b" );
            AssertNodeExists( doc.CreateNavigator(), "/c/hi/c/d" );
        }       

        [Test]
        public void TestParamRef()
        {
            XmlDocument doc = _Preprocess( "TestParamRef.xml" ); 
            AssertNodeValue(doc.CreateNavigator(), "//root/m1", "param1val;param2val");
        }

        [Test]
        public void TestInclude()
        {
            using (XmlReader input = GetInput("TestIncluder.xml"))
            {
                ConfigPreprocessorEnvironment env;
                using ( XmlWriter output = GetOutput() )
                {
                    ConfigPreprocessor preprocessor = new ConfigPreprocessor();
                    env = preprocessor.PreProcess( input, output, new TestResolver( FAKE_ROOT + "TestIncluder.xml" ), new Uri(FAKE_ROOT + "TestIncluder.xml" ) );
                }
                AssertNodeExists( ReadOutputDoc().CreateNavigator(),
                                  "/includer/included/included2" );

                Assert.AreEqual( env.Fileset.Length, 3 );
                Assert.AreEqual(GetTestPath("TestIncluder.xml"), env.Fileset[0]);
                Assert.AreEqual(GetTestPath(String.Format(
					"Subfolder{0}TestIncluded.xml", Path.DirectorySeparatorChar)), env.Fileset[1]);
                Assert.AreEqual(GetTestPath("TestIncluded2.xml"), env.Fileset[2]);
            }
        }

        [Test]
        public void TestIncludeStack()
        {
            using (XmlReader input = GetInput("TestIncludeStack1.xml"))
            {
                ConfigPreprocessorEnvironment env;
                using (XmlWriter output = GetOutput())
                {
                    ConfigPreprocessor preprocessor = new ConfigPreprocessor();
                    env = preprocessor.PreProcess(input, output,
                        new TestResolver(FAKE_ROOT + "TestIncludeStack1.xml"),
                        new Uri(FAKE_ROOT + "TestIncludeStack1.xml"));
                }
                AssertNodeExists(ReadOutputDoc().CreateNavigator(), "/TestIncludeStack1/TestIncludeStack2/TestIncludeStack3");
                AssertNodeExists(ReadOutputDoc().CreateNavigator(), "/TestIncludeStack1/TestIncludeStack4");

                Assert.AreEqual(env.Fileset.Length, 4);

                Assert.AreEqual(GetTestPath("TestIncludeStack1.xml"), env.Fileset[0]);
                Assert.AreEqual(GetTestPath(String.Format(
					"Subfolder{0}TestIncludeStack2.xml", Path.DirectorySeparatorChar)), env.Fileset[1]);
                Assert.AreEqual(GetTestPath("TestIncludeStack3.xml"), env.Fileset[2]);
                Assert.AreEqual(GetTestPath("TestIncludeStack4.xml"), env.Fileset[3]);
            }
        }

        [Test]
        public void TestScope()
        {            
            XmlDocument doc = _Preprocess( "TestScope.xml" );
            AssertNodeValue( doc, "/root/test[1]", "val1" );
            AssertNodeValue( doc, "/root/test[2]", "val2" );
            AssertNodeValue( doc, "/root/inner/test[1]", "val1" );
            AssertNodeValue( doc, "/root/inner/test[2]", "val2_redef" );
        }

        [Test,ExpectedException(typeof(EvaluationException))]
        [Ignore("Ignoring this test because cycle checking is disabled until we can make it work correctly.")]
        public void TestCycle()
        {            
            _Preprocess( "TestCycle.xml" );            
        }

        [Test]
        public void TestCycle2()
        {
            _Preprocess("TestCycle2.xml");
        }

        [Test]
        public void TestSample()
        {            
            _Preprocess( "Sample.xml" );            
        }

        [Test]
        public void TestSampleProject()
        {            
            _Preprocess( "SampleProject.xml" );
        }

        [Test]
        public void TestIncludeFileWithSpacesInName()
        {
            using (XmlReader input = GetInput("Test Include File With Spaces.xml"))
            {
                ConfigPreprocessorEnvironment env;
                using (XmlWriter output = GetOutput())
                {
                    ConfigPreprocessor preprocessor = new ConfigPreprocessor();
                    env = preprocessor.PreProcess(input, output,
                        new TestResolver(FAKE_ROOT + "Test Include File With Spaces.xml"),
                        new Uri(FAKE_ROOT + "Test Include File With Spaces.xml"));
                }
                XmlDocument doc = ReadOutputDoc();
                AssertNodeValue(doc, "/element", "value");
                Assert.AreEqual(env.Fileset.Length, 1);
                Assert.AreEqual(GetTestPath("Test Include File With Spaces.xml"), env.Fileset[0]);
            }
        }

        [Test]
        public void TestIncludeVariable()
        {
            using (XmlReader input = GetInput("TestIncludeVariable.xml"))
            {
                ConfigPreprocessorEnvironment env;
                using (XmlWriter output = GetOutput())
                {
                    ConfigPreprocessor preprocessor = new ConfigPreprocessor();
                    env = preprocessor.PreProcess(input, output,
                        new TestResolver(FAKE_ROOT + "TestIncludeVariable.xml"),
                        new Uri(FAKE_ROOT + "TestIncludeVariable.xml"));
                }
                AssertNodeExists(ReadOutputDoc().CreateNavigator(),
                                  "/includeVariable/included/included2");

                Assert.AreEqual(env.Fileset.Length, 3);
                Assert.AreEqual(GetTestPath("TestIncludeVariable.xml"), env.Fileset[0]);
                Assert.AreEqual(GetTestPath(@"Subfolder\TestIncluded.xml"), env.Fileset[1]);
                Assert.AreEqual(GetTestPath("TestIncluded2.xml"), env.Fileset[2]);
            }
        }
        
        private static XmlDocument _Preprocess(string filename)
        {
            using( XmlReader input = GetInput( filename ) )
            {
                using ( XmlWriter output = GetOutput() )
                {
                    ConfigPreprocessor preprocessor = new ConfigPreprocessor();
                    preprocessor.PreProcess(
                        input, output, new TestResolver( FAKE_ROOT + filename  ), new Uri( FAKE_ROOT + filename  )  );
                }
                return ReadOutputDoc();
            }
        }

        private static string GetTestPath( string relative_path )
        {
            return Path.Combine(FAKE_ROOT, relative_path);
        }

        private static void AssertNodeValue(IXPathNavigable doc, string xpath, string expected_val)
        {
            XPathNavigator nav = doc.CreateNavigator();
            XPathNavigator node = nav.SelectSingleNode(xpath, nav);
            Assert.IsNotNull(node, "Node '{0}' not found", xpath);
            Assert.AreEqual( node.Value, expected_val);
        }

        private static void AssertNodeValue(XPathNavigator nav,
                                             string xpath, string expected_val)
        {
            XPathNavigator node = nav.SelectSingleNode(xpath, nav);
            Assert.IsNotNull(node, "Node '{0}' not found", xpath);
            Assert.AreEqual( node.Value, expected_val);
        }

        private static void AssertNodeExists(XPathNavigator nav,
                                             string xpath)
        {
            XPathNavigator node = nav.SelectSingleNode(xpath, nav);
            Assert.IsNotNull(node, "Node '{0}' not found", xpath);
        }

        private static XmlReader GetInput(string filename)
        {
            return
                XmlReader.Create( GetManifestResourceStream( filename ) );            
        }

		internal static Stream GetManifestResourceStream(string filename)
		{
			var result = Assembly.GetExecutingAssembly().
				GetManifestResourceStream(
				"ThoughtWorks.CruiseControl.UnitTests.Core.Config.TestAssets." +
				filename);
			Assert.IsNotNull(result, "Unable to load data from assembly : " + filename);
			return result;
		}

        private static XmlDocument ReadOutputDoc()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(OutPath);            
            return doc;
        }


        private static XmlWriter GetOutput()
        {
            return Utils.CreateWriter(OutPath);
        }

		private static string OutPath
		{
			get { return Path.Combine(Path.GetTempPath(), "out.xml"); }
		}
    }

    internal class TestResolver : PreprocessorUrlResolver
    {
        /// <summary>
        /// The base string of the URIs, to remove later.
        /// </summary>
        private readonly string _original_base_path;
        public TestResolver(string base_path)
        {
            _original_base_path = Path.GetDirectoryName( base_path ) +
                                  Path.DirectorySeparatorChar;
        }

        /// <summary>
        /// Return the resource represented by the specified URI as a Stream.
        /// </summary>
        /// <param name="absoluteUri">The URI for the resource.</param>
        /// <param name="role">Ignroed.</param>
        /// <param name="ofObjectToReturn">Ignored.</param>
        /// <returns>A Stream connected to the resource.</returns>
        public override object GetEntity (
            Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            string relativeUri = Resolve( absoluteUri );
            // Ignore the path and load the file from the manifest resources.
            return
                PreprocessorTest.GetManifestResourceStream( 
               relativeUri );
        }        

        public override ICredentials Credentials
        {
            set {  }
        }

        /// <summary>
        /// Resolve an absolute URI to a resource name.
        /// </summary>
        /// <param name="absoluteUri"></param>
        /// <returns></returns>
        private string Resolve( Uri absoluteUri )
        {
            return absoluteUri.LocalPath.Substring(_original_base_path.Length).Replace(Path.DirectorySeparatorChar, '.');
        }
    }
}
