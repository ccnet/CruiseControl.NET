using System;
using System.Xml;
using tw.ccnet.core.util;

namespace tw.ccnet.core.test
{
	public class SimpleBuildFile
	{
		public const string Content = @"<?xml version=""1.0""?>

<project name=""ccnetTest"" default=""success"">

  <target name=""success"">
    <echo message=""I am success itself""/>
  </target>

  <target name=""fail"">
    <echo message=""I am failure itself""/>
    <fail message=""Intentional failure for test purposes, that is to say, purposes of testing, if you will""/>
  </target>

</project>";

		public static XmlDocument Document
		{
			get { return XmlUtil.CreateDocument(Content); }
		}
	}
}
