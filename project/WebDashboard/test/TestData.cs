using System;

namespace ThoughtWorks.CruiseControl.Web.Test
{
	public class TestData
	{
		public static string LogFileContents
		{
			get 
			{
				return @"<cruisecontrol>
    <modifications>
        <modification type=""change"">
            <filename>config.xml</filename>
            <project>E:\Projects\cvs\cruisecontrol\main\sample_project\config.xml</project>
            <date>05/07/2002 01:03:43</date>
            <user>User</user>
            <email>
            </email>
            <comment />
        </modification>
        <modification type=""change"">
            <filename>currentbuild.txt</filename>
            <project>E:\Projects\cvs\cruisecontrol\main\sample_project\currentbuild.txt</project>
            <date>05/07/2002 01:03:55</date>
            <user>User</user>
            <email>
            </email>
            <comment />
        </modification>
        <modification type=""change"">
            <filename>HelloWorld</filename>
            <project>E:\Projects\cvs\cruisecontrol\main\sample_project\HelloWorld</project>
            <date>05/07/2002 01:03:25</date>
            <user>User</user>
            <email>
            </email>
            <comment />
        </modification>
        <modification type=""change"">
            <filename>log.xml</filename>
            <project>E:\Projects\cvs\cruisecontrol\main\sample_project\log.xml</project>
            <date>05/07/2002 01:03:24</date>
            <user>User</user>
            <email>
            </email>
            <comment />
        </modification>
        <modification type=""change"">
            <filename>log20020507010320.xml</filename>
            <project>E:\Projects\cvs\cruisecontrol\main\sample_project\logs\log20020507010320.xml</project>
            <date>05/07/2002 01:03:25</date>
            <user>User</user>
            <email>
            </email>
            <comment />
        </modification>
    </modifications>
    <info>
        <property name=""lastbuild"" value=""20020507010320"" />
        <property name=""label"" value=""1.1"" />
        <property name=""interval"" value=""10"" />
    </info>
    <build error=""E:\Projects\cvs\cruisecontrol\main\sample_project\build.xml:75: srcdir &quot;E:\Projects\cvs\cruisecontrol\main\sample_project\test&quot; does not exist!"" time=""3 seconds"">

        <message priority=""debug""><![CDATA[Setting project property: ant.java.version -> 1.3]]></message>

        <message priority=""debug""><![CDATA[Detected Java version: 1.3 in: E:\JBuilder4\jdk1.3\jre]]></message>

        <message priority=""debug""><![CDATA[Detected OS: Windows 2000]]></message>

        <message priority=""debug""><![CDATA[ +User task: tar     org.apache.tools.ant.taskdefs.Tar]]></message>

        <message priority=""debug""><![CDATA[ +User task: fail     org.apache.tools.ant.taskdefs.Exit]]></message>

        <message priority=""debug""><![CDATA[ +User task: uptodate     org.apache.tools.ant.taskdefs.UpToDate]]></message>

        <message priority=""debug""><![CDATA[ +User task: jpcoverage     org.apache.tools.ant.taskdefs.optional.sitraka.Coverage]]></message>

        <message priority=""debug""><![CDATA[ +User task: dependset     org.apache.tools.ant.taskdefs.DependSet]]></message>

        <message priority=""debug""><![CDATA[ +User task: vsscheckin     org.apache.tools.ant.taskdefs.optional.vss.MSVSSCHECKIN]]></message>

        <message priority=""debug""><![CDATA[ +User task: java     org.apache.tools.ant.taskdefs.Java]]></message>

        <message priority=""debug""><![CDATA[ +User task: execon     org.apache.tools.ant.taskdefs.ExecuteOn]]></message>

        <message priority=""debug""><![CDATA[ +User task: echo     org.apache.tools.ant.taskdefs.Echo]]></message>

        <message priority=""debug""><![CDATA[ +User task: native2ascii     org.apache.tools.ant.taskdefs.optional.Native2Ascii]]></message>

        <message priority=""debug""><![CDATA[ +User task: jjtree     org.apache.tools.ant.taskdefs.optional.javacc.JJTree]]></message>

        <message priority=""debug""><![CDATA[ +User task: chmod     org.apache.tools.ant.taskdefs.Chmod]]></message>

        <message priority=""debug""><![CDATA[ +User task: javadoc2     org.apache.tools.ant.taskdefs.Javadoc]]></message>

        <message priority=""debug""><![CDATA[ +User task: deltree     org.apache.tools.ant.taskdefs.Deltree]]></message>

        <message priority=""debug""><![CDATA[ +User task: cvs     org.apache.tools.ant.taskdefs.Cvs]]></message>

        <message priority=""debug""><![CDATA[ +User task: ccmcreatetask     org.apache.tools.ant.taskdefs.optional.ccm.CCMCreateTask]]></message>

        <message priority=""debug""><![CDATA[ +User task: jlink     org.apache.tools.ant.taskdefs.optional.jlink.JlinkTask]]></message>

        <message priority=""debug""><![CDATA[ +User task: tstamp     org.apache.tools.ant.taskdefs.Tstamp]]></message>

        <message priority=""debug""><![CDATA[ +User task: icontract     org.apache.tools.ant.taskdefs.optional.IContract]]></message>

        <message priority=""debug""><![CDATA[ +User task: pathconvert     org.apache.tools.ant.taskdefs.PathConvert]]></message>

        <message priority=""debug""><![CDATA[ +User task: unjar     org.apache.tools.ant.taskdefs.Expand]]></message>

        <message priority=""debug""><![CDATA[ +User task: patch     org.apache.tools.ant.taskdefs.Patch]]></message>

        <message priority=""debug""><![CDATA[ +User task: sound     org.apache.tools.ant.taskdefs.optional.sound.SoundTask]]></message>

        <message priority=""debug""><![CDATA[ +User task: starteam     org.apache.tools.ant.taskdefs.optional.scm.AntStarTeamCheckOut]]></message>

        <message priority=""debug""><![CDATA[ +User task: cccheckout     org.apache.tools.ant.taskdefs.optional.clearcase.CCCheckout]]></message>

        <message priority=""debug""><![CDATA[ +User task: mail     org.apache.tools.ant.taskdefs.SendEmail]]></message>

        <message priority=""debug""><![CDATA[ +User task: p4reopen     org.apache.tools.ant.taskdefs.optional.perforce.P4Reopen]]></message>

        <message priority=""debug""><![CDATA[ +User task: gzip     org.apache.tools.ant.taskdefs.GZip]]></message>

        <message priority=""debug""><![CDATA[ +User task: p4label     org.apache.tools.ant.taskdefs.optional.perforce.P4Label]]></message>

        <message priority=""debug""><![CDATA[ +User task: wlrun     org.apache.tools.ant.taskdefs.optional.ejb.WLRun]]></message>

        <message priority=""debug""><![CDATA[ +User task: jpcovreport     org.apache.tools.ant.taskdefs.optional.sitraka.CovReport]]></message>

        <message priority=""debug""><![CDATA[ +User task: mimemail     org.apache.tools.ant.taskdefs.optional.net.MimeMail]]></message>

        <message priority=""debug""><![CDATA[ +User task: copy     org.apache.tools.ant.taskdefs.Copy]]></message>

        <message priority=""debug""><![CDATA[ +User task: filter     org.apache.tools.ant.taskdefs.Filter]]></message>

        <message priority=""debug""><![CDATA[ +User task: jar     org.apache.tools.ant.taskdefs.Jar]]></message>

        <message priority=""debug""><![CDATA[ +User task: unzip     org.apache.tools.ant.taskdefs.Expand]]></message>

        <message priority=""debug""><![CDATA[ +User task: rename     org.apache.tools.ant.taskdefs.Rename]]></message>

        <message priority=""debug""><![CDATA[ +User task: mmetrics     org.apache.tools.ant.taskdefs.optional.metamata.MMetrics]]></message>

        <message priority=""debug""><![CDATA[ +User task: propertyfile     org.apache.tools.ant.taskdefs.optional.PropertyFile]]></message>

        <message priority=""debug""><![CDATA[ +User task: copyfile     org.apache.tools.ant.taskdefs.Copyfile]]></message>

        <message priority=""debug""><![CDATA[ +User task: ccmcheckintask     org.apache.tools.ant.taskdefs.optional.ccm.CCMCheckinDefault]]></message>

        <message priority=""debug""><![CDATA[ +User task: p4submit     org.apache.tools.ant.taskdefs.optional.perforce.P4Submit]]></message>

        <message priority=""debug""><![CDATA[ +User task: gunzip     org.apache.tools.ant.taskdefs.GUnzip]]></message>

        <message priority=""debug""><![CDATA[ +User task: antcall     org.apache.tools.ant.taskdefs.CallTarget]]></message>

        <message priority=""debug""><![CDATA[ +User task: taskdef     org.apache.tools.ant.taskdefs.Taskdef]]></message>

        <message priority=""debug""><![CDATA[ +User task: mkdir     org.apache.tools.ant.taskdefs.Mkdir]]></message>

        <message priority=""debug""><![CDATA[ +User task: sql     org.apache.tools.ant.taskdefs.SQLExec]]></message>

        <message priority=""debug""><![CDATA[ +User task: replace     org.apache.tools.ant.taskdefs.Replace]]></message>

        <message priority=""debug""><![CDATA[ +User task: ear     org.apache.tools.ant.taskdefs.Ear]]></message>

        <message priority=""debug""><![CDATA[ +User task: ant     org.apache.tools.ant.taskdefs.Ant]]></message>

        <message priority=""debug""><![CDATA[ +User task: vsshistory     org.apache.tools.ant.taskdefs.optional.vss.MSVSSHISTORY]]></message>

        <message priority=""debug""><![CDATA[ +User task: style     org.apache.tools.ant.taskdefs.XSLTProcess]]></message>

        <message priority=""debug""><![CDATA[ +User task: p4sync     org.apache.tools.ant.taskdefs.optional.perforce.P4Sync]]></message>

        <message priority=""debug""><![CDATA[ +User task: fixcrlf     org.apache.tools.ant.taskdefs.FixCRLF]]></message>

        <message priority=""debug""><![CDATA[ +User task: sequential     org.apache.tools.ant.taskdefs.Sequential]]></message>

        <message priority=""debug""><![CDATA[ +User task: vssget     org.apache.tools.ant.taskdefs.optional.vss.MSVSSGET]]></message>

        <message priority=""debug""><![CDATA[ +User task: genkey     org.apache.tools.ant.taskdefs.GenerateKey]]></message>

        <message priority=""debug""><![CDATA[ +User task: p4edit     org.apache.tools.ant.taskdefs.optional.perforce.P4Edit]]></message>

        <message priority=""debug""><![CDATA[ +User task: zip     org.apache.tools.ant.taskdefs.Zip]]></message>

        <message priority=""debug""><![CDATA[ +User task: condition     org.apache.tools.ant.taskdefs.ConditionTask]]></message>

        <message priority=""debug""><![CDATA[ +User task: antstructure     org.apache.tools.ant.taskdefs.AntStructure]]></message>

        <message priority=""debug""><![CDATA[ +User task: pvcs     org.apache.tools.ant.taskdefs.optional.pvcs.Pvcs]]></message>

        <message priority=""debug""><![CDATA[ +User task: javah     org.apache.tools.ant.taskdefs.optional.Javah]]></message>

        <message priority=""debug""><![CDATA[ +User task: p4counter     org.apache.tools.ant.taskdefs.optional.perforce.P4Counter]]></message>

        <message priority=""debug""><![CDATA[ +User task: javac     org.apache.tools.ant.taskdefs.Javac]]></message>

        <message priority=""debug""><![CDATA[ +User task: test     org.apache.tools.ant.taskdefs.optional.Test]]></message>

        <message priority=""debug""><![CDATA[ +User task: p4revert     org.apache.tools.ant.taskdefs.optional.perforce.P4Revert]]></message>

        <message priority=""debug""><![CDATA[ +User task: xmlvalidate     org.apache.tools.ant.taskdefs.optional.XMLValidateTask]]></message>

        <message priority=""debug""><![CDATA[ +User task: ccmreconfigure     org.apache.tools.ant.taskdefs.optional.ccm.CCMReconfigure]]></message>

        <message priority=""debug""><![CDATA[ +User task: cab     org.apache.tools.ant.taskdefs.optional.Cab]]></message>

        <message priority=""debug""><![CDATA[ +User task: typedef     org.apache.tools.ant.taskdefs.Typedef]]></message>

        <message priority=""debug""><![CDATA[ +User task: mparse     org.apache.tools.ant.taskdefs.optional.metamata.MParse]]></message>

        <message priority=""debug""><![CDATA[ +User task: ccuncheckout     org.apache.tools.ant.taskdefs.optional.clearcase.CCUnCheckout]]></message>

        <message priority=""debug""><![CDATA[ +User task: ejbjar     org.apache.tools.ant.taskdefs.optional.ejb.EjbJar]]></message>

        <message priority=""debug""><![CDATA[ +User task: jpcovmerge     org.apache.tools.ant.taskdefs.optional.sitraka.CovMerge]]></message>

        <message priority=""debug""><![CDATA[ +User task: available     org.apache.tools.ant.taskdefs.Available]]></message>

        <message priority=""debug""><![CDATA[ +User task: p4have     org.apache.tools.ant.taskdefs.optional.perforce.P4Have]]></message>

        <message priority=""debug""><![CDATA[ +User task: rpm     org.apache.tools.ant.taskdefs.optional.Rpm]]></message>

        <message priority=""debug""><![CDATA[ +User task: vsslabel     org.apache.tools.ant.taskdefs.optional.vss.MSVSSLABEL]]></message>

        <message priority=""debug""><![CDATA[ +User task: cvspass     org.apache.tools.ant.taskdefs.CVSPass]]></message>

        <message priority=""debug""><![CDATA[ +User task: move     org.apache.tools.ant.taskdefs.Move]]></message>

        <message priority=""debug""><![CDATA[ +User task: wljspc     org.apache.tools.ant.taskdefs.optional.jsp.WLJspc]]></message>

        <message priority=""debug""><![CDATA[ +User task: junitreport     org.apache.tools.ant.taskdefs.optional.junit.XMLResultAggregator]]></message>

        <message priority=""debug""><![CDATA[ +User task: javacc     org.apache.tools.ant.taskdefs.optional.javacc.JavaCC]]></message>

        <message priority=""debug""><![CDATA[ +User task: signjar     org.apache.tools.ant.taskdefs.SignJar]]></message>

        <message priority=""debug""><![CDATA[ +User task: csc     org.apache.tools.ant.taskdefs.optional.dotnet.CSharp]]></message>

        <message priority=""debug""><![CDATA[ +User task: p4change     org.apache.tools.ant.taskdefs.optional.perforce.P4Change]]></message>

        <message priority=""debug""><![CDATA[ +User task: cccheckin     org.apache.tools.ant.taskdefs.optional.clearcase.CCCheckin]]></message>

        <message priority=""debug""><![CDATA[ +User task: property     org.apache.tools.ant.taskdefs.Property]]></message>

        <message priority=""debug""><![CDATA[ +User task: iplanet-ejbc     org.apache.tools.ant.taskdefs.optional.ejb.IPlanetEjbcTask]]></message>

        <message priority=""debug""><![CDATA[ +User task: unwar     org.apache.tools.ant.taskdefs.Expand]]></message>

        <message priority=""debug""><![CDATA[ +User task: touch     org.apache.tools.ant.taskdefs.Touch]]></message>

        <message priority=""debug""><![CDATA[ +User task: ilasm     org.apache.tools.ant.taskdefs.optional.dotnet.Ilasm]]></message>

        <message priority=""debug""><![CDATA[ +User task: ccmcheckin     org.apache.tools.ant.taskdefs.optional.ccm.CCMCheckin]]></message>

        <message priority=""debug""><![CDATA[ +User task: get     org.apache.tools.ant.taskdefs.Get]]></message>

        <message priority=""debug""><![CDATA[ +User task: blgenclient     org.apache.tools.ant.taskdefs.optional.ejb.BorlandGenerateClient]]></message>

        <message priority=""debug""><![CDATA[ +User task: vsscheckout     org.apache.tools.ant.taskdefs.optional.vss.MSVSSCHECKOUT]]></message>

        <message priority=""debug""><![CDATA[ +User task: stylebook     org.apache.tools.ant.taskdefs.optional.StyleBook]]></message>

        <message priority=""debug""><![CDATA[ +User task: javadoc     org.apache.tools.ant.taskdefs.Javadoc]]></message>

        <message priority=""debug""><![CDATA[ +User task: netrexxc     org.apache.tools.ant.taskdefs.optional.NetRexxC]]></message>

        <message priority=""debug""><![CDATA[ +User task: antlr     org.apache.tools.ant.taskdefs.optional.ANTLR]]></message>

        <message priority=""debug""><![CDATA[ +User task: record     org.apache.tools.ant.taskdefs.Recorder]]></message>

        <message priority=""debug""><![CDATA[ +User task: untar     org.apache.tools.ant.taskdefs.Untar]]></message>

        <message priority=""debug""><![CDATA[ +User task: delete     org.apache.tools.ant.taskdefs.Delete]]></message>

        <message priority=""debug""><![CDATA[ +User task: ejbc     org.apache.tools.ant.taskdefs.optional.ejb.Ejbc]]></message>

        <message priority=""debug""><![CDATA[ +User task: ddcreator     org.apache.tools.ant.taskdefs.optional.ejb.DDCreator]]></message>

        <message priority=""debug""><![CDATA[ +User task: copydir     org.apache.tools.ant.taskdefs.Copydir]]></message>

        <message priority=""debug""><![CDATA[ +User task: war     org.apache.tools.ant.taskdefs.War]]></message>

        <message priority=""debug""><![CDATA[ +User task: ccupdate     org.apache.tools.ant.taskdefs.optional.clearcase.CCUpdate]]></message>

        <message priority=""debug""><![CDATA[ +User task: depend     org.apache.tools.ant.taskdefs.optional.depend.Depend]]></message>

        <message priority=""debug""><![CDATA[ +User task: parallel     org.apache.tools.ant.taskdefs.Parallel]]></message>

        <message priority=""debug""><![CDATA[ +User task: ccmcheckout     org.apache.tools.ant.taskdefs.optional.ccm.CCMCheckout]]></message>

        <message priority=""debug""><![CDATA[ +User task: renameext     org.apache.tools.ant.taskdefs.optional.RenameExtensions]]></message>

        <message priority=""debug""><![CDATA[ +User task: exec     org.apache.tools.ant.taskdefs.ExecTask]]></message>

        <message priority=""debug""><![CDATA[ +User task: apply     org.apache.tools.ant.taskdefs.Transform]]></message>

        <message priority=""debug""><![CDATA[ +User task: junit     org.apache.tools.ant.taskdefs.optional.junit.JUnitTask]]></message>

        <message priority=""debug""><![CDATA[ +User task: wlstop     org.apache.tools.ant.taskdefs.optional.ejb.WLStop]]></message>

        <message priority=""debug""><![CDATA[ +User task: maudit     org.apache.tools.ant.taskdefs.optional.metamata.MAudit]]></message>

        <message priority=""debug""><![CDATA[ +User task: jdepend     org.apache.tools.ant.taskdefs.optional.jdepend.JDependTask]]></message>

        <message priority=""debug""><![CDATA[ +User task: sleep     org.apache.tools.ant.taskdefs.Sleep]]></message>

        <message priority=""debug""><![CDATA[ +User task: rmic     org.apache.tools.ant.taskdefs.Rmic]]></message>

        <message priority=""debug""><![CDATA[ +User datatype: fileset     org.apache.tools.ant.types.FileSet]]></message>

        <message priority=""debug""><![CDATA[ +User datatype: patternset     org.apache.tools.ant.types.PatternSet]]></message>

        <message priority=""debug""><![CDATA[ +User datatype: filterset     org.apache.tools.ant.types.FilterSet]]></message>

        <message priority=""debug""><![CDATA[ +User datatype: filelist     org.apache.tools.ant.types.FileList]]></message>

        <message priority=""debug""><![CDATA[ +User datatype: description     org.apache.tools.ant.types.Description]]></message>

        <message priority=""debug""><![CDATA[ +User datatype: path     org.apache.tools.ant.types.Path]]></message>

        <message priority=""debug""><![CDATA[ +User datatype: mapper     org.apache.tools.ant.types.Mapper]]></message>

        <message priority=""debug""><![CDATA[Setting project property: java.runtime.name -> Java(TM) 2 Runtime Environment, Standard Edition]]></message>

        <message priority=""debug""><![CDATA[Setting project property: sun.boot.library.path -> E:\JBuilder4\jdk1.3\jre\bin]]></message>

        <message priority=""debug""><![CDATA[Setting project property: java.vm.version -> 1.3.0-C]]></message>

        <message priority=""debug""><![CDATA[Setting project property: java.vm.vendor -> Sun Microsystems Inc.]]></message>

        <message priority=""debug""><![CDATA[Setting project property: java.vendor.url -> http://java.sun.com/]]></message>

        <message priority=""debug""><![CDATA[Setting project property: path.separator -> ;]]></message>

        <message priority=""debug""><![CDATA[Setting project property: java.vm.name -> Java HotSpot(TM) Client VM]]></message>

        <message priority=""debug""><![CDATA[Setting project property: file.encoding.pkg -> sun.io]]></message>

        <message priority=""debug""><![CDATA[Setting project property: java.vm.specification.name -> Java Virtual Machine Specification]]></message>

        <message priority=""debug""><![CDATA[Setting project property: user.dir -> E:\Projects\cvs\cruisecontrol\main\sample_project]]></message>

        <message priority=""debug""><![CDATA[Setting project property: java.runtime.version -> 1.3.0-C]]></message>

        <message priority=""debug""><![CDATA[Setting project property: java.awt.graphicsenv -> sun.awt.Win32GraphicsEnvironment]]></message>

        <message priority=""debug""><![CDATA[Setting project property: os.arch -> x86]]></message>

        <message priority=""debug""><![CDATA[Setting project property: java.io.tmpdir -> E:\Temp\]]></message>

        <message priority=""debug""><![CDATA[Setting project property: line.separator -> 
    ]]></message>

        <message priority=""debug""><![CDATA[Setting project property: java.vm.specification.vendor -> Sun Microsystems Inc.]]></message>

        <message priority=""debug""><![CDATA[Setting project property: java.awt.fonts -> ]]></message>

        <message priority=""debug""><![CDATA[Setting project property: os.name -> Windows 2000]]></message>

        <message priority=""debug""><![CDATA[Setting project property: java.library.path -> E:\JBuilder4\jdk1.3\bin;.;D:\WINNT\System32;D:\WINNT;E:\JBuilder4\jdk1.3\bin;E:\Program Files\Microsoft.NET\FrameworkSDK\Bin\;D:\Program Files\Microsoft Visual Studio.NET\Vc7\bin\;D:\Program Files\Microsoft Visual Studio.NET\Common7\IDE\;D:\WINNT\Microsoft.NET\Framework\v1.0.2914\;D:\Program Files\Microsoft Visual Studio.NET\Vc7\bin\;D:\Program Files\Microsoft Visual Studio.NET\Common7\IDE\;D:\WINNT\system32;D:\WINNT;D:\WINNT\System32\Wbem;E:\PROGRA~1\ULTRAEDT;e:\Program Files\Perforce;E:\Program Files\F-Secure\Ssh Trial;E:\ssh-1.2.14-win32bin;e:\PROGRA~1\COSMOS~1\Shared\bin;E:\PROGRA~1\IBM\IMNNQ;E:\Program Files\SQLLIB\BIN;E:\Program Files\SQLLIB\FUNCTION;E:\Program Files\SQLLIB\SAMPLES\REPL;E:\Program Files\SQLLIB\HELP;E:\Program Files\Embarcadero\Nov2001Shared;E:\Program Files\Embarcadero\RSQL601;E:\Program Files\Microsoft Visual Studio\Common\Tools\WinNT;E:\Program Files\Microsoft Visual Studio\Common\MSDev98\Bin;E:\Program Files\Microsoft Visual Studio\Common\Tools;E:\Program Files\Microsoft Visual Studio\VC98\bin;]]></message>

        <message priority=""debug""><![CDATA[Setting project property: java.specification.name -> Java Platform API Specification]]></message>

        <message priority=""debug""><![CDATA[Setting project property: java.class.version -> 47.0]]></message>

        <message priority=""debug""><![CDATA[Setting project property: os.version -> 5.0]]></message>

        <message priority=""debug""><![CDATA[Setting project property: user.home -> D:\Documents and Settings\non]]></message>

        <message priority=""debug""><![CDATA[Setting project property: user.timezone -> ]]></message>

        <message priority=""debug""><![CDATA[Setting project property: java.awt.printerjob -> sun.awt.windows.WPrinterJob]]></message>

        <message priority=""debug""><![CDATA[Setting project property: file.encoding -> Cp1252]]></message>

        <message priority=""debug""><![CDATA[Setting project property: java.specification.version -> 1.3]]></message>

        <message priority=""debug""><![CDATA[Setting project property: java.class.path -> E:\JBuilder4\jdk1.3\jre\lib\rt.jar;E:\Projects\cvs\cruisecontrol\main\classes;E:\Projects\cvs\cruisecontrol\main\lib\activation.jar;E:\Projects\cvs\cruisecontrol\main\lib\ant.jar;E:\Projects\cvs\cruisecontrol\main\lib\jakarta-oro-2.0.3.jar;E:\Projects\cvs\cruisecontrol\main\lib\jdom.jar;E:\Projects\cvs\cruisecontrol\main\lib\jmxri.jar;E:\Projects\cvs\cruisecontrol\main\lib\jmxtools.jar;E:\Projects\cvs\cruisecontrol\main\lib\junit.jar;E:\Projects\cvs\cruisecontrol\main\lib\log4j.jar;E:\Projects\cvs\cruisecontrol\main\lib\mail.jar;E:\Projects\cvs\cruisecontrol\main\lib\optional.jar;E:\Projects\cvs\cruisecontrol\main\lib\starteam-sdk-interfaceonly.jar;E:\Projects\cvs\cruisecontrol\main\lib\xalan.jar;E:\Projects\cvs\cruisecontrol\main\lib\xerces.jar]]></message>

        <message priority=""debug""><![CDATA[Setting project property: user.name -> non]]></message>

        <message priority=""debug""><![CDATA[Setting project property: java.vm.specification.version -> 1.0]]></message>

        <message priority=""debug""><![CDATA[Setting project property: java.home -> E:\JBuilder4\jdk1.3\jre]]></message>

        <message priority=""debug""><![CDATA[Setting project property: user.language -> sv]]></message>

        <message priority=""debug""><![CDATA[Setting project property: java.specification.vendor -> Sun Microsystems Inc.]]></message>

        <message priority=""debug""><![CDATA[Setting project property: awt.toolkit -> sun.awt.windows.WToolkit]]></message>

        <message priority=""debug""><![CDATA[Setting project property: java.vm.info -> mixed mode]]></message>

        <message priority=""debug""><![CDATA[Setting project property: java.version -> 1.3.0]]></message>

        <message priority=""debug""><![CDATA[Setting project property: java.ext.dirs -> E:\JBuilder4\jdk1.3\jre\lib\ext]]></message>

        <message priority=""debug""><![CDATA[Setting project property: sun.boot.class.path -> E:\JBuilder4\jdk1.3\jre\lib\rt.jar;E:\JBuilder4\jdk1.3\jre\lib\i18n.jar;E:\JBuilder4\jdk1.3\jre\lib\sunrsasign.jar;E:\JBuilder4\jdk1.3\jre\classes]]></message>

        <message priority=""debug""><![CDATA[Setting project property: java.vendor -> Sun Microsystems Inc.]]></message>

        <message priority=""debug""><![CDATA[Setting project property: file.separator -> \]]></message>

        <message priority=""debug""><![CDATA[Setting project property: java.vendor.url.bug -> http://java.sun.com/cgi-bin/bugreport.cgi]]></message>

        <message priority=""debug""><![CDATA[Setting project property: sun.io.unicode.encoding -> UnicodeLittle]]></message>

        <message priority=""debug""><![CDATA[Setting project property: sun.cpu.endian -> little]]></message>

        <message priority=""debug""><![CDATA[Setting project property: user.region -> SE]]></message>

        <message priority=""debug""><![CDATA[Setting project property: sun.cpu.isalist -> pentium i486 i386]]></message>

        <message priority=""debug""><![CDATA[Setting ro project property: ant.version -> Ant version 1.4 compiled on September 3 2001]]></message>

        <message priority=""debug""><![CDATA[Setting ro project property: label -> 1.1]]></message>

        <message priority=""debug""><![CDATA[Setting ro project property: cctimestamp -> 20020507010355]]></message>

        <message priority=""debug""><![CDATA[Setting ro project property: ant.file -> E:\Projects\cvs\cruisecontrol\main\sample_project\build.xml]]></message>

        <message priority=""debug""><![CDATA[parsing buildfile E:\Projects\cvs\cruisecontrol\main\sample_project\build.xml with URI = file:E:/Projects/cvs/cruisecontrol/main/sample_project/build.xml]]></message>

        <message priority=""debug""><![CDATA[Setting ro project property: ant.project.name -> HelloWorld]]></message>

        <message priority=""debug""><![CDATA[Adding reference: HelloWorld -> org.apache.tools.ant.Project@580be3]]></message>

        <message priority=""debug""><![CDATA[Setting project property: basedir -> E:\Projects\cvs\cruisecontrol\main\sample_project]]></message>

        <message priority=""debug""><![CDATA[Project base dir set to: E:\Projects\cvs\cruisecontrol\main\sample_project]]></message>

        <message priority=""debug""><![CDATA[   +Task: property]]></message>

        <message priority=""debug""><![CDATA[Setting project property: build.dir -> classes]]></message>

        <message priority=""debug""><![CDATA[   +Task: property]]></message>

        <message priority=""debug""><![CDATA[Setting project property: dist.dir -> dist]]></message>

        <message priority=""debug""><![CDATA[   +Task: property]]></message>

        <message priority=""debug""><![CDATA[Setting project property: logdir -> logs]]></message>

        <message priority=""debug""><![CDATA[   +Task: property]]></message>

        <message priority=""debug""><![CDATA[Setting project property: source.dir -> src]]></message>

        <message priority=""debug""><![CDATA[   +Task: property]]></message>

        <message priority=""debug""><![CDATA[Setting project property: test.dir -> test]]></message>

        <message priority=""debug""><![CDATA[   +Task: property]]></message>

        <message priority=""debug""><![CDATA[Setting project property: junit.results -> test-results]]></message>

        <message priority=""debug""><![CDATA[   +Task: property]]></message>

        <message priority=""debug""><![CDATA[Setting project property: ccdir -> E:\Projects\cvs\cruisecontrol\main]]></message>

        <message priority=""debug""><![CDATA[   +DataType: path]]></message>

        <message priority=""debug""><![CDATA[Adding reference: classpath -> ]]></message>

        <message priority=""debug""><![CDATA[ +Target: init]]></message>

        <message priority=""debug""><![CDATA[   +Task: tstamp]]></message>

        <message priority=""debug""><![CDATA[   +Task: mkdir]]></message>

        <message priority=""debug""><![CDATA[   +Task: mkdir]]></message>

        <message priority=""debug""><![CDATA[   +Task: mkdir]]></message>

        <message priority=""debug""><![CDATA[ +Target: clean]]></message>

        <message priority=""debug""><![CDATA[   +Task: delete]]></message>

        <message priority=""debug""><![CDATA[   +Task: delete]]></message>

        <message priority=""debug""><![CDATA[   +Task: mkdir]]></message>

        <message priority=""debug""><![CDATA[   +Task: mkdir]]></message>

        <message priority=""debug""><![CDATA[ +Target: compile]]></message>

        <message priority=""debug""><![CDATA[   +Task: javac]]></message>

        <message priority=""debug""><![CDATA[ +Target: test]]></message>

        <message priority=""debug""><![CDATA[   +Task: delete]]></message>

        <message priority=""debug""><![CDATA[   +Task: mkdir]]></message>

        <message priority=""debug""><![CDATA[   +Task: echo]]></message>

        <message priority=""debug""><![CDATA[   +Task: junit]]></message>

        <message priority=""debug""><![CDATA[Implicitly adding /E:/Projects/cvs/cruisecontrol/main/lib/junit.jar to classpath]]></message>

        <message priority=""debug""><![CDATA[Implicitly adding /E:/Projects/cvs/cruisecontrol/main/lib/ant.jar to classpath]]></message>

        <message priority=""debug""><![CDATA[Implicitly adding /E:/Projects/cvs/cruisecontrol/main/lib/optional.jar to classpath]]></message>

        <message priority=""debug""><![CDATA[ +Target: jar]]></message>

        <message priority=""debug""><![CDATA[   +Task: jar]]></message>

        <message priority=""debug""><![CDATA[ +Target: modificationcheck]]></message>

        <message priority=""debug""><![CDATA[   +Task: taskdef]]></message>

        <message priority=""debug""><![CDATA[   +Task: echo]]></message>

        <message priority=""debug""><![CDATA[ +Target: all]]></message>

        <message priority=""debug""><![CDATA[   +Task: echo]]></message>

        <message priority=""debug""><![CDATA[ +Target: masterbuild]]></message>

        <message priority=""debug""><![CDATA[ +Target: cleanbuild]]></message>

        <message priority=""debug""><![CDATA[Build sequence for target `masterbuild' is [init, compile, test, jar, masterbuild]]]></message>

        <message priority=""debug""><![CDATA[Complete build sequence is [init, compile, test, jar, masterbuild, clean, cleanbuild, modificationcheck, all]]]></message>

        <target name=""init"" time=""0 seconds"">

            <task location=""E:\Projects\cvs\cruisecontrol\main\sample_project\build.xml:55: "" name=""Tstamp"" time=""0 seconds"">

                <message priority=""debug""><![CDATA[Setting project property: DSTAMP -> 20020507]]></message>

                <message priority=""debug""><![CDATA[Setting project property: TSTAMP -> 0104]]></message>

                <message priority=""debug""><![CDATA[Setting project property: TODAY -> May 7 2002]]></message>


            </task>

            <task location=""E:\Projects\cvs\cruisecontrol\main\sample_project\build.xml:56: "" name=""Mkdir"" time=""0 seconds"" />

            <task location=""E:\Projects\cvs\cruisecontrol\main\sample_project\build.xml:57: "" name=""Mkdir"" time=""0 seconds"" />

            <task location=""E:\Projects\cvs\cruisecontrol\main\sample_project\build.xml:58: "" name=""Mkdir"" time=""0 seconds"" />


        </target>

        <target name=""compile"" time=""0 seconds"">

            <task location=""E:\Projects\cvs\cruisecontrol\main\sample_project\build.xml:75: "" name=""Javac"" time=""0 seconds"">

                <message priority=""debug""><![CDATA[FileSet: Setup file scanner in dir E:\Projects\cvs\cruisecontrol\main\sample_project\src with patternSet{ includes: [**/*.java] excludes: [] }]]></message>

                <message priority=""debug""><![CDATA[java\hello\HelloWorld.java added as E:\Projects\cvs\cruisecontrol\main\sample_project\classes\java\hello\HelloWorld.class doesn't exist.]]></message>

                <message priority=""debug""><![CDATA[test\hello\HelloWorldTest.java added as E:\Projects\cvs\cruisecontrol\main\sample_project\classes\test\hello\HelloWorldTest.class doesn't exist.]]></message>


            </task>


        </target>

        <properties>

            <property name=""cctimestamp"" value=""20020507010355"" />

            <property name=""java.runtime.name"" value=""Java(TM) 2 Runtime Environment, Standard Edition"" />

            <property name=""sun.boot.library.path"" value=""E:\JBuilder4\jdk1.3\jre\bin"" />

            <property name=""java.vm.version"" value=""1.3.0-C"" />

            <property name=""ant.version"" value=""Ant version 1.4 compiled on September 3 2001"" />

            <property name=""ant.java.version"" value=""1.3"" />

            <property name=""java.vm.vendor"" value=""Sun Microsystems Inc."" />

            <property name=""java.vendor.url"" value=""http://java.sun.com/"" />

            <property name=""path.separator"" value="";"" />

            <property name=""java.vm.name"" value=""Java HotSpot(TM) Client VM"" />

            <property name=""file.encoding.pkg"" value=""sun.io"" />

            <property name=""java.vm.specification.name"" value=""Java Virtual Machine Specification"" />

            <property name=""user.dir"" value=""E:\Projects\cvs\cruisecontrol\main\sample_project"" />

            <property name=""java.runtime.version"" value=""1.3.0-C"" />

            <property name=""basedir"" value=""E:\Projects\cvs\cruisecontrol\main\sample_project"" />

            <property name=""java.awt.graphicsenv"" value=""sun.awt.Win32GraphicsEnvironment"" />

            <property name=""os.arch"" value=""x86"" />

            <property name=""java.io.tmpdir"" value=""E:\Temp\"" />

            <property name=""line.separator"" value="" "" />

            <property name=""java.vm.specification.vendor"" value=""Sun Microsystems Inc."" />

            <property name=""java.awt.fonts"" value="""" />

            <property name=""os.name"" value=""Windows 2000"" />

            <property name=""DSTAMP"" value=""20020507"" />

            <property name=""build.dir"" value=""classes"" />

            <property name=""ant.project.name"" value=""HelloWorld"" />

            <property name=""TODAY"" value=""May 7 2002"" />

            <property name=""java.library.path"" value=""E:\JBuilder4\jdk1.3\bin;.;D:\WINNT\System32;D:\WINNT;E:\JBuilder4\jdk1.3\bin;E:\Program Files\Microsoft.NET\FrameworkSDK\Bin\;D:\Program Files\Microsoft Visual Studio.NET\Vc7\bin\;D:\Program Files\Microsoft Visual Studio.NET\Common7\IDE\;D:\WINNT\Microsoft.NET\Framework\v1.0.2914\;D:\Program Files\Microsoft Visual Studio.NET\Vc7\bin\;D:\Program Files\Microsoft Visual Studio.NET\Common7\IDE\;D:\WINNT\system32;D:\WINNT;D:\WINNT\System32\Wbem;E:\PROGRA~1\ULTRAEDT;e:\Program Files\Perforce;E:\Program Files\F-Secure\Ssh Trial;E:\ssh-1.2.14-win32bin;e:\PROGRA~1\COSMOS~1\Shared\bin;E:\PROGRA~1\IBM\IMNNQ;E:\Program Files\SQLLIB\BIN;E:\Program Files\SQLLIB\FUNCTION;E:\Program Files\SQLLIB\SAMPLES\REPL;E:\Program Files\SQLLIB\HELP;E:\Program Files\Embarcadero\Nov2001Shared;E:\Program Files\Embarcadero\RSQL601;E:\Program Files\Microsoft Visual Studio\Common\Tools\WinNT;E:\Program Files\Microsoft Visual Studio\Common\MSDev98\Bin;E:\Program Files\Microsoft Visual Studio\Common\Tools;E:\Program Files\Microsoft Visual Studio\VC98\bin;"" />

            <property name=""java.specification.name"" value=""Java Platform API Specification"" />

            <property name=""java.class.version"" value=""47.0"" />

            <property name=""ccdir"" value=""E:\Projects\cvs\cruisecontrol\main"" />

            <property name=""os.version"" value=""5.0"" />

            <property name=""dist.dir"" value=""dist"" />

            <property name=""ant.file"" value=""E:\Projects\cvs\cruisecontrol\main\sample_project\build.xml"" />

            <property name=""user.home"" value=""D:\Documents and Settings\non"" />

            <property name=""user.timezone"" value="""" />

            <property name=""java.awt.printerjob"" value=""sun.awt.windows.WPrinterJob"" />

            <property name=""logdir"" value=""logs"" />

            <property name=""file.encoding"" value=""Cp1252"" />

            <property name=""java.specification.version"" value=""1.3"" />

            <property name=""junit.results"" value=""test-results"" />

            <property name=""TSTAMP"" value=""0104"" />

            <property name=""user.name"" value=""non"" />

            <property name=""java.class.path"" value=""E:\JBuilder4\jdk1.3\jre\lib\rt.jar;E:\Projects\cvs\cruisecontrol\main\classes;E:\Projects\cvs\cruisecontrol\main\lib\activation.jar;E:\Projects\cvs\cruisecontrol\main\lib\ant.jar;E:\Projects\cvs\cruisecontrol\main\lib\jakarta-oro-2.0.3.jar;E:\Projects\cvs\cruisecontrol\main\lib\jdom.jar;E:\Projects\cvs\cruisecontrol\main\lib\jmxri.jar;E:\Projects\cvs\cruisecontrol\main\lib\jmxtools.jar;E:\Projects\cvs\cruisecontrol\main\lib\junit.jar;E:\Projects\cvs\cruisecontrol\main\lib\log4j.jar;E:\Projects\cvs\cruisecontrol\main\lib\mail.jar;E:\Projects\cvs\cruisecontrol\main\lib\optional.jar;E:\Projects\cvs\cruisecontrol\main\lib\starteam-sdk-interfaceonly.jar;E:\Projects\cvs\cruisecontrol\main\lib\xalan.jar;E:\Projects\cvs\cruisecontrol\main\lib\xerces.jar"" />

            <property name=""java.vm.specification.version"" value=""1.0"" />

            <property name=""java.home"" value=""E:\JBuilder4\jdk1.3\jre"" />

            <property name=""user.language"" value=""sv"" />

            <property name=""java.specification.vendor"" value=""Sun Microsystems Inc."" />

            <property name=""awt.toolkit"" value=""sun.awt.windows.WToolkit"" />

            <property name=""test.dir"" value=""test"" />

            <property name=""java.vm.info"" value=""mixed mode"" />

            <property name=""java.version"" value=""1.3.0"" />

            <property name=""java.ext.dirs"" value=""E:\JBuilder4\jdk1.3\jre\lib\ext"" />

            <property name=""label"" value=""1.1"" />

            <property name=""sun.boot.class.path"" value=""E:\JBuilder4\jdk1.3\jre\lib\rt.jar;E:\JBuilder4\jdk1.3\jre\lib\i18n.jar;E:\JBuilder4\jdk1.3\jre\lib\sunrsasign.jar;E:\JBuilder4\jdk1.3\jre\classes"" />

            <property name=""java.vendor"" value=""Sun Microsystems Inc."" />

            <property name=""file.separator"" value=""\"" />

            <property name=""java.vendor.url.bug"" value=""http://java.sun.com/cgi-bin/bugreport.cgi"" />

            <property name=""source.dir"" value=""src"" />

            <property name=""sun.cpu.endian"" value=""little"" />

            <property name=""sun.io.unicode.encoding"" value=""UnicodeLittle"" />

            <property name=""user.region"" value=""SE"" />

            <property name=""sun.cpu.isalist"" value=""pentium i486 i386"" />


        </properties>
    </build>
</cruisecontrol>";
			}
		}

		public static string StyleSheetContents
		{
			get 
			{
				return @"<?xml version=""1.0""?>
<xsl:stylesheet xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"" version=""1.0"">

    <xsl:output method=""html""/>

    <xsl:variable name=""modification.list"" select=""cruisecontrol/modifications/modification""/>

    <xsl:template match=""/"">
        <table align=""center"" cellpadding=""2"" cellspacing=""0"" border=""0"" width=""98%"">
            <!-- Modifications -->
            <tr>
                <td class=""modifications-sectionheader"" colspan=""4"">
                    &#160;Modifications since last build:&#160;
                    (<xsl:value-of select=""count($modification.list)""/>)
                </td>
            </tr>

            <xsl:apply-templates select=""$modification.list"">
                <xsl:sort select=""date"" order=""descending"" data-type=""text"" />
            </xsl:apply-templates>
            
        </table>
    </xsl:template>

    <!-- Modifications template -->
    <xsl:template match=""modification"">
        <tr>
            <xsl:if test=""position() mod 2=0"">
                <xsl:attribute name=""class"">modifications-oddrow</xsl:attribute>
            </xsl:if>
            <xsl:if test=""position() mod 2!=0"">
                <xsl:attribute name=""class"">modifications-evenrow</xsl:attribute>
            </xsl:if>

            <td class=""modifications-data""><xsl:value-of select=""@type""/></td>
            <td class=""modifications-data""><xsl:value-of select=""user""/></td>
            <td class=""modifications-data""><xsl:value-of select=""project""/>/<xsl:value-of select=""filename""/></td>
            <td class=""modifications-data""><xsl:value-of select=""comment""/></td>
        </tr>
    </xsl:template>


</xsl:stylesheet>";
			}
		}
	}
}
