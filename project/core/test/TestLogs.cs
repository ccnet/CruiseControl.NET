using System;

namespace tw.ccnet.core.test
{
	public class TestLogs
	{
		public static string Successful
		{
			get
			{
				return @"
<buildresults project=""ccnet"">
  <message><![CDATA[Buildfile: file:///C:/dev/ccnet/ccnet.build
]]></message>
  <property />
  <property />
  <property />
  <property />
  <property />
  <property />
  <property />
  <property />
  <property />
  <property />
  <property />
  <property />
  <property />
  <property />
  <all>
    <message><![CDATA[all:
]]></message>
    <call>
      <clean>
        <message><![CDATA[clean:
]]></message>
        <delete>
          <message><![CDATA[Deleting directory C:\dev\ccnet\build]]></message>
        </delete>
      </clean>
    </call>
    <call>
      <init>
        <message><![CDATA[init:
]]></message>
        <mkdir>
          <message><![CDATA[Creating directory C:\dev\ccnet\build]]></message>
        </mkdir>
      </init>
      <compile>
        <message><![CDATA[compile:
]]></message>
        <call>
          <compile_core>
            <message><![CDATA[compile_core:
]]></message>
            <csc>
              <message><![CDATA[Compiling 16 files to C:\dev\ccnet\build\CCNetCore.dll]]></message>
            </csc>
          </compile_core>
        </call>
        <call>
          <compile_web>
            <message><![CDATA[compile_web:
]]></message>
            <csc>
              <message><![CDATA[Compiling 4 files to C:\dev\ccnet\build\ccnet.dll]]></message>
            </csc>
          </compile_web>
        </call>
      </compile>
      <unittest>
        <message><![CDATA[unittest:
]]></message>
        <exec>
          <message><![CDATA[tools/nunit-console/nunit-console.exe /assembly:build\CCNetCore.dll /xml:core.test.xml]]></message>
          <message><![CDATA[C:\dev\ccnet>tools/nunit-console/nunit-console.exe /assembly:build\CCNetCore.dll /xml:core.test.xml]]></message>
          <message><![CDATA[NUnit version 2.0.3
Copyright (C) 2002 James W. Newkirk, Michael C. Two, Alexei A. Vorontsov. 
Copyright (C) 2000-2002 Philip Craig.
All Rights Reserved.
.......

Tests run: 7, Failures: 0, Not run: 0, Time: 0.9914256 seconds



]]></message>
        </exec>
        <exec>
          <message><![CDATA[tools/nunit-console/nunit-console.exe /assembly:build\ccnet.dll /xml:web.test.xml]]></message>
          <message><![CDATA[C:\dev\ccnet>tools/nunit-console/nunit-console.exe /assembly:build\ccnet.dll /xml:web.test.xml]]></message>
          <message><![CDATA[NUnit version 2.0.3
Copyright (C) 2002 James W. Newkirk, Michael C. Two, Alexei A. Vorontsov. 
Copyright (C) 2000-2002 Philip Craig.
All Rights Reserved.
......

Tests run: 6, Failures: 0, Not run: 0, Time: 0.350504 seconds



]]></message>
        </exec>
      </unittest>
    </call>
    <call>
      <deploy>
        <message><![CDATA[deploy:
]]></message>
        <call>
          <deploy_content>
            <message><![CDATA[deploy_content:
]]></message>
            <copy />
          </deploy_content>
        </call>
        <copy />
        <copy>
          <message><![CDATA[Copying 2 files to c:\inetpub\wwwroot\ccnet\bin]]></message>
          <message><![CDATA[Copying C:\dev\ccnet\build\CCNetCore.dll to c:\inetpub\wwwroot\ccnet\bin\CCNetCore.dll]]></message>
          <message><![CDATA[Copying C:\dev\ccnet\build\ccnet.dll to c:\inetpub\wwwroot\ccnet\bin\ccnet.dll]]></message>
        </copy>
      </deploy>
    </call>
    <call>
      <acceptancetest>
        <message><![CDATA[acceptancetest:
]]></message>
        <csc>
          <message><![CDATA[Compiling 2 files to C:\dev\ccnet\build\CCNetAcceptance.dll]]></message>
        </csc>
        <copy>
          <message><![CDATA[Copying 1 files]]></message>
        </copy>
        <nunit>
          <message><![CDATA[Running tw.ccnet.acceptance.AllTests]]></message>
          <message><![CDATA[1 tests: ALL SUCCESSFUL]]></message>
        </nunit>
        <tstamp>
          <message><![CDATA[September 5, 2002 11:51:40 AM]]></message>
        </tstamp>
      </acceptancetest>
    </call>
  </all>
  <message><![CDATA[BUILD SUCCEEDED
]]></message>
  <message><![CDATA[Total time: 17 seconds
]]></message>
</buildresults>";
			}
		}

		public static string Failed
		{
			get
			{
				return @"<buildresults project=""ccnetlaunch"">
  <message><![CDATA[Buildfile: file:///C:/dev/ccnet/ccnet-config/cruise.build
]]></message>
  <fail>
    <message><![CDATA[fail:
]]></message>
    <echo>
      <message><![CDATA[I am failure itself]]></message>
    </echo>
    <fail />
  </fail>
  <message><![CDATA[BUILD FAILED
]]></message>
  <message><![CDATA[Intentional failure for test purposes, that is to say, purposes of testing, if you will
]]></message>
</buildresults>";
			}
		}
	}
}
