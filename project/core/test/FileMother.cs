using System;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Test
{	
	public class FileMother
	{
		private const string TEMP_DIR = "tempCCNetTestFiles";
		
		public const string NANT_TEST_BUILDFILE = "test.build";

		public static string NANT_TEST_BUILDFILEPATH
		{
			get { return TempFileUtil.GetTempFilePath(TEMP_DIR, NANT_TEST_BUILDFILE); }
		}
		
		public static string NANT_TEST_LOGDIR
		{
			get { return TempFileUtil.GetTempPath(TEMP_DIR); }
		}

		public static string CreateBuildFile(string content)
		{
			TempFileUtil.CreateTempDir(TEMP_DIR);
			return TempFileUtil.CreateTempXmlFile(TEMP_DIR, NANT_TEST_BUILDFILE, content);
		}

//		public static string CreateConfigFile()
//		{
//			return CreateConfigFile(ConfigurationMother.Content());
//		}

		public static string CreateConfigFile(string content)
		{
			TempFileUtil.CreateTempDir(TEMP_DIR, false);
			return TempFileUtil.CreateTempXmlFile(TEMP_DIR, "testConfig.xml", content);
		}

		public static void CleanUp()
		{
			TempFileUtil.DeleteTempDir(TEMP_DIR);
		}
	}	
}