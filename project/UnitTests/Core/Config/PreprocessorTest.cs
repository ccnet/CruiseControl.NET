using System;
using System.Collections.Generic;
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
    	private static readonly string FAKE_ROOT = Platform.IsWindows ? "c:\\temp folder\\" : "/tmp/";

        private readonly XmlUrlResolver _resolver = new XmlUrlResolver();

        [Test]
        public void TestDocType()
        {
            _Preprocess("TestDocType.xml");            
        }

        [Test]
        public void TestAttrNodesetDefine()
        {
            XmlDocument doc = _Preprocess("TestAttrNodesetDefine.xml");
            XPathNavigator nav = doc.CreateNavigator();
            AssertNodeValue(nav, "/root/test-one/outer-content", "textcontent");
            AssertNodeExists(nav, "/root/test-two/outer-content/nodeset-content");
        }

        [Test]
        public void TestAttributeWithNoName()
        {
            Assert.Throws<InvalidMarkupException>(() =>
            {
                _Preprocess("TestInvalidAttribute2.xml");
            });
        }

        [Test]
        [Platform(Exclude = "Mono", Reason = "JScript not available")]
        public void TestBigFor()
        {
            XmlDocument doc = _Preprocess("TestBigFor.xml");
            XPathNavigator nav = doc.CreateNavigator();
            AssertNodeValue(nav, "/root/hello[1]/@attr", "0");
            AssertNodeValue(nav, "/root/hello[25000]/@attr", "24999");
        }

        [Test]
        public void TestRedefineBug()
        {
            _Preprocess("TestRedefineBug.xml");            
        }

        [Test]
        public void TestCount()
        {
            XmlDocument doc = _Preprocess("TestCount.xml");
            XPathNavigator nav = doc.CreateNavigator();
            AssertNodeValue(nav, "/root/foo[1]/@val", "1");
            AssertNodeValue(nav, "/root/foo[2]/@val", "2");
            AssertNodeValue(nav, "/root/foo[3]/@val", "3");
            AssertNodeCount(nav, "/root/foo[@val]", 3);
        }      

        [Test]
        public void TestElementsAndAttributes()
        {
            XmlDocument doc = _Preprocess("TestElementAttribute.xml");
            XPathNavigator nav = doc.CreateNavigator();
            AssertNodeValue(nav, "/myel", "myelval");
            AssertNodeValue(nav, "/myel/@myattr1", "myattrval1");
            AssertNodeValue(nav, "/myel/@myattr2", "myattrval2");
        }

        [Test]
        [Platform(Exclude = "Mono", Reason = "computername is unknown in mono, maybe fix the test file?")]
        public void TestEnvironmentVariableExpansion()
        {
            var doc = _Preprocess("TestEnvironmentVariableExpansion.xml");
            var nav = doc.CreateNavigator();
            AssertNodeValue(nav, "/root/Direct", Environment.MachineName);
            AssertNodeValue(nav, "/root/Direct/@AttributeForm", Environment.MachineName);
            AssertNodeValue(nav, "/root/Indirect", Environment.MachineName);
            AssertNodeValue(nav, "/root/Indirect/@AttributeForm", Environment.MachineName);
        }

        [Test]
        [Platform(Exclude = "Mono", Reason = "JScript not available")]
        public void TestEvals()
        {
            XmlDocument doc = _Preprocess("TestEval.xml");
            XPathNavigator nav = doc.CreateNavigator();
            AssertNodeValue(nav, "/root/eval1", "54");
            AssertNodeValue(nav, "/root/eval2", "6");
            AssertNodeValue(nav, "/root/evalliteral",
                             @"(""A STRING WITH 'THINGS' WHICH MAY NEED TO BE ""ESCAPED"""")");
            AssertNodeValue(nav, "/root/two", "2");
            AssertNodeExists(nav, "/root/three/inner");
        }


        [Test]
        public void TestExplicitDefine1()
        {
            var settings = new PreprocessorSettings();
            Environment.SetEnvironmentVariable("foo", "foo_val");
            settings.ExplicitDeclarationRequired = false;
            XmlDocument doc = _Preprocess("TestExplicitDefine.xml", settings);
            XPathNavigator nav = doc.CreateNavigator();
            AssertNodeValue(nav, "/root/@foo", "foo_val");
        }

        [Test]
        public void TestExplicitDefine2()
        {
            Assert.Throws<ExplicitDefinitionRequiredException>(() =>
            {
                var settings = new PreprocessorSettings();
                try
                {
                    Environment.SetEnvironmentVariable("foo", "foo_val");
                    settings.ExplicitDeclarationRequired = true;
                    _Preprocess("TestExplicitDefine.xml", settings);
                }
                finally
                {
                    Environment.SetEnvironmentVariable("foo", null);
                }
            });
        }

        [Test]
        [Platform(Exclude = "Mono", Reason = "JScript not available")]
        public void TestFor()
        {
            XmlDocument doc = _Preprocess("TestFor.xml");
            XPathNavigator nav = doc.CreateNavigator();
            AssertNodeValue(nav, "/root/hello[1]/@attr", "0");
            AssertNodeValue(nav, "/root/hello[2]/@attr", "1");
            AssertNodeValue(nav, "/root/hello[3]/@attr", "2");
        }

        [Test]
        [Platform(Exclude = "Mono", Reason = "JScript not available")]
        public void TestForEach()
        {
            XmlDocument doc = _Preprocess("TestForEach.xml");
            XPathNavigator nav = doc.CreateNavigator();
            AssertNodeExists(nav, "/root/one");
            AssertNodeExists(nav, "/root/two");
            AssertNodeExists(nav, "/root/three");
            AssertNodeExists(nav, "/root/four");
            AssertNodeExists(nav, "/root/five");
            AssertNodeExists(nav, "/root/six");
        }

        [Test]
        [Platform(Exclude = "Mono", Reason = "JScript not available")]
        public void TestIf()
        {
            XmlDocument doc = _Preprocess("TestIf.xml");
            XPathNavigator nav = doc.CreateNavigator();
            AssertNodeExists(nav, "/root/ifeq_succeeded");
            AssertNodeDoesNotExist(nav, "/root/ifeq_failed");
            AssertNodeExists(nav, "/root/ifeq_negative_succeeded");
            AssertNodeDoesNotExist(nav, "/root/ifeq_negative_failed");
            AssertNodeExists(nav, "/root/ifneq_succeeded");
            AssertNodeDoesNotExist(nav, "/root/ifneq_failed");
        }

        [Test]
        public void TestIfDefIfNDef()
        {
            XmlDocument doc = _Preprocess("TestIfdefIfNDef.xml");
            XPathNavigator nav = doc.CreateNavigator();
            AssertNodeExists(nav, "/root/ifdef_foo_succeeded");
            AssertNodeDoesNotExist(nav, "/root/ifdef_foo_failed");
            AssertNodeExists(nav, "/root/ifndef_foo_succeeded");
            AssertNodeDoesNotExist(nav, "/root/ifndef_foo_failed");
            AssertNodeExists(nav, "/root/ifdef_else_succeeded");
            AssertNodeDoesNotExist(nav, "/root/ifdef_else_failed");
            AssertNodeExists(nav, "/root/ifndef_else_succeeded");
            AssertNodeDoesNotExist(nav, "/root/ifndef_else_failed");
        }

        [Test]
        public void TestImporFrom()
        {
            XmlDocument doc = _Preprocess("TestImportFrom.xml");
            AssertNodeValue(doc, "/root/test", "Hello From TestElementProcessor!");
        }

        [Test]
        public void TestImport()
        {
            XmlDocument doc = _Preprocess("TestImport.xml");
            AssertNodeValue(doc, "/root/test", "Hello From TestElementProcessor!");
        }
        
        [Test]
        public void TestInitialDefine()
        {
            var settings = new PreprocessorSettings();
            var defs = new Dictionary<string, string>();
            defs["foo"] = "foo_val";
            settings.InitialDefinitions = defs;
            settings.ExplicitDeclarationRequired = false;
            XmlDocument doc = _Preprocess("TestExplicitDefine.xml", settings);
            XPathNavigator nav = doc.CreateNavigator();
            AssertNodeValue(nav, "/root/@foo", "foo_val");
        }

        [Test]
        public void TestInitialDefine2()
        {
            Assert.Throws<ExplicitDefinitionRequiredException>(() =>
            {
                var settings = new PreprocessorSettings();
                var defs = new Dictionary<string, string>();
                defs["foo"] = "foo_val";
                settings.InitialDefinitions = defs;
                settings.ExplicitDeclarationRequired = true;
                XmlDocument doc = _Preprocess("TestExplicitDefine.xml", settings);
                XPathNavigator nav = doc.CreateNavigator();
                AssertNodeValue(nav, "/root/@foo", "foo_val");
            });
        }

        [Test]
        public void TestMisplacedAttribute()
        {
            Assert.Throws<InvalidMarkupException>(() =>
            {
                _Preprocess("TestInvalidAttribute.xml");
            });
        }

        [Test]
        public void TestProcessingInstruction()
        {
            XmlDocument doc = _Preprocess("TestProcessingInstruction.xml");
            XPathNavigator nav = doc.CreateNavigator();
            AssertNodeValue(nav, "/processing-instruction('mypi')", "mypival");
        }
        
        #region existing

        [Test]
        public void TestDefineConst()
        {
            using (XmlReader input = GetInput("TestDefineConst.xml"))
            {
                using (XmlWriter output = GetOutput())
                {
                    ConfigPreprocessor preprocessor = new ConfigPreprocessor();
                    PreprocessorEnvironment env =
                        preprocessor.PreProcess(input, output, _resolver, null );
                    Assert.AreEqual( env.EvalSymbol( "var1" ).GetTextValue(), "value1" );                    
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
                PreprocessorEnvironment env;

                using ( XmlWriter output = GetOutput() )
                {
                    ConfigPreprocessor preprocessor = new ConfigPreprocessor();
                    env = preprocessor.PreProcess( input, output, new TestResolver( FAKE_ROOT + "TestIncluder.xml" ), new Uri(FAKE_ROOT + "TestIncluder.xml" ) );
                }

                AssertNodeExists( ReadOutputDoc().CreateNavigator(), "/includer/included/included2" );

                Assert.AreEqual( env.Fileset.Length, 3 );
                Assert.AreEqual( GetTestPath( "TestIncluder.xml" ), env.Fileset[ 0 ].LocalPath );
                Assert.AreEqual(
                    GetTestPath( String.Format( "Subfolder{0}TestIncluded.xml", Path.DirectorySeparatorChar ) ),
                    env.Fileset[ 1 ].LocalPath );
                Assert.AreEqual( GetTestPath( "TestIncluded2.xml" ), env.Fileset[ 2 ].LocalPath );
            }
        }

        [Test]
        public void TestIncludeStack()
        {
            using (XmlReader input = GetInput("TestIncludeStack1.xml"))
            {
                PreprocessorEnvironment env;

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

                Assert.AreEqual(GetTestPath("TestIncludeStack1.xml"), env.Fileset[0].LocalPath);
                Assert.AreEqual(GetTestPath(String.Format(
                    "Subfolder{0}TestIncludeStack2.xml", Path.DirectorySeparatorChar)), env.Fileset[1].LocalPath);
                Assert.AreEqual(GetTestPath("TestIncludeStack3.xml"), env.Fileset[2].LocalPath);
                Assert.AreEqual(GetTestPath("TestIncludeStack4.xml"), env.Fileset[3].LocalPath);
            }
        }

        [Test]
        public void TestMissingIncludeFile()
        {
            Assert.Throws<MissingIncludeException>(() =>
            {
                string filename = "TestMissingIncludeFile.xml";

                using (XmlReader input = GetInput(filename))
                {
                    using (XmlWriter output = GetOutput())
                    {
                        ConfigPreprocessor preprocessor = new ConfigPreprocessor();
                        preprocessor.PreProcess(
                            input, output,
                            new FileNotFoundTestResolver(FAKE_ROOT + filename),
                            new Uri(FAKE_ROOT + filename));
                    }
                }
            });
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

        [Test]        
        public void TestCycle()
        {
            Assert.Throws<CyclicalEvaluationException>(() =>
            {
                _Preprocess("TestCycle.xml");
            });
        }

        [Test]
        public void TestCycle2()
        {
            _Preprocess("TestCycle2.xml");
        }

        [Test]
        [Platform(Exclude = "Mono", Reason = "path is unknown")]
        public void TestSample()
        {            
            _Preprocess( "Sample.xml" );            
        }

        [Test]
        [Platform(Exclude = "Mono", Reason = "computername is unknown")]
        public void TestSampleProject()
        {            
            _Preprocess( "SampleProject.xml" );
        }

        [Test]
        public void TestIncludeFileWithSpacesInName()
        {
            using (XmlReader input = GetInput("Test Include File With Spaces.xml"))
            {
                PreprocessorEnvironment env;

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
                Assert.AreEqual( new Uri(GetTestPath("Test Include File With Spaces.xml")), env.Fileset[0]);
            }
        }

        [Test]
        public void TestIncludeVariable()
        {
            using (XmlReader input = GetInput("TestIncludeVariable.xml"))
            {
                PreprocessorEnvironment env;

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
                Assert.AreEqual(GetTestPath("TestIncludeVariable.xml"), env.Fileset[0].LocalPath);
                Assert.AreEqual(GetTestPath(String.Format( "Subfolder{0}TestIncluded.xml", Path.DirectorySeparatorChar )), env.Fileset[1].LocalPath);
                Assert.AreEqual(GetTestPath("TestIncluded2.xml"), env.Fileset[2].LocalPath);
            }
        }

        [Test]
        public void TestCCNET_1991()
        {
            var doc = _Preprocess("TestCCNET-1991.xml");
            AssertNodeValue(doc,"/cruisecontrol/project/workingDirectory",@"C:\Test\Space Error\Working");
            AssertNodeValue(doc, "/cruisecontrol/project/artifactDirectory", @"C:\Test\Space Error\Artifacts");
            AssertNodeValue(doc, "/cruisecontrol/project/publishers/xmllogger/@logDir", @"C:\Test\Space Error\BuildLogs");        
        }
        #endregion

        private static XmlDocument _Preprocess(string filename  )
        {
            return _Preprocess( filename, new PreprocessorSettings() );
        }

        private static XmlDocument _Preprocess(string filename, PreprocessorSettings settings )
        {
            using( XmlReader input = GetInput( filename ) )
            {
                using ( XmlWriter output = GetOutput() )
                {
                    ConfigPreprocessor preprocessor = new ConfigPreprocessor( settings );
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

        private static void AssertNodeCount(XPathNavigator nav, string xpath, int count)
        {
            XPathNodeIterator nodes = nav.Select(xpath, nav);
            Assert.That(nodes.Count, Is.EqualTo(count));
        }

        private static void AssertNodeValue(IXPathNavigable doc, string xpath, string expected_val)
        {
            XPathNavigator nav = doc.CreateNavigator();
            XPathNavigator node = nav.SelectSingleNode(xpath, nav);
            Assert.IsNotNull(node, "Node '{0}' not found", xpath);
            Assert.AreEqual( node.Value.Trim(), expected_val);
        }

        private static void AssertNodeValue(XPathNavigator nav, string xpath, string expected_val)
        {
            XPathNavigator node = nav.SelectSingleNode(xpath, nav);
            Assert.IsNotNull(node, "Node '{0}' not found", xpath);
            Assert.AreEqual( node.Value.Trim(), expected_val);
        }

        private static void AssertNodeExists(XPathNavigator nav, string xpath)
        {
            XPathNavigator node = nav.SelectSingleNode(xpath, nav);
            Assert.IsNotNull(node, "Node '{0}' not found", xpath);
        }

        private static void AssertNodeDoesNotExist(XPathNavigator nav, string xpath)
        {
            XPathNavigator node = nav.SelectSingleNode(xpath, nav);
            Assert.IsNull(node, "Node '{0}' found when it should not exist", xpath);
        }

        private static XmlReader GetInput(string filename)
        {
            var settings = new XmlReaderSettings();
            settings.ProhibitDtd = false;            
            return
                XmlReader.Create( GetManifestResourceStream( filename ), settings );
        }

        internal static Stream GetManifestResourceStream(string filename)
        {
            return GetManifestResourceStream(filename, true);
        }

        internal static Stream GetManifestResourceStream(string filename, bool assertResourceFound)
		{
			System.IO.Stream result = Assembly.GetExecutingAssembly().
				GetManifestResourceStream(
				"ThoughtWorks.CruiseControl.UnitTests.Core.Config.TestAssets." +
				filename.Replace("/", "."));
            if (assertResourceFound)
            {
                Assert.IsNotNull(result, "Unable to load data from assembly : " + filename);
            }
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

    internal class TestResolver : XmlUrlResolver
    {
        private readonly string _original_base_path;

        public TestResolver(string base_path) : base()
        {
            _original_base_path = Path.GetDirectoryName( base_path ) + Path.DirectorySeparatorChar;
        }

        public override ICredentials Credentials
        {
            set { }
        }

        public override object GetEntity(Uri absolute_uri, string role, Type of_object_to_return)
        {
            string relative_uri = Resolve( absolute_uri );
            // Ignore the path and load the file from the manifest resources.
            Stream stream = PreprocessorTest.GetManifestResourceStream(
                relative_uri );
            if ( stream == null )
                Utils.ThrowAppException( "Resource '{0}' not found", relative_uri );
            return stream;
        }

        protected string Resolve(Uri absolute_uri)
        {
            return absolute_uri.LocalPath.Substring( _original_base_path.Length ).Replace( '\\', '.' );
        }
    }

    internal class FileNotFoundTestResolver : TestResolver
    {
        public FileNotFoundTestResolver(string base_path) : base(base_path)
        {
        }

        public override object GetEntity(Uri absolute_uri, string role, Type of_object_to_return)
        {
            string relative_uri = Resolve(absolute_uri);
            // Ignore the path and load the file from the manifest resources.
            // Don't assert that the resource exists, we need that to fall through
            // for testing purposes.
            Stream stream = PreprocessorTest.GetManifestResourceStream(relative_uri, false);
            if (stream == null)
                throw new FileNotFoundException("Test resource not found.", absolute_uri.OriginalString);
            return stream;
        }
    }
}
