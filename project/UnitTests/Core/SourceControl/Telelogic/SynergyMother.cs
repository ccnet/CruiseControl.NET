namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol.Telelogic
{
	public sealed class SynergyMother
	{
		private SynergyMother()
		{}

		public const string ConfigValues = @"
                    <synergy>
                        <connection>
                            <executable>ccm.cmd</executable>
                            <host>myserver</host>
                            <timeout>600</timeout>
                            <database>\\myserver\share\mydatabase</database>
                            <!-- we can obfuscate the user/pass by storing the values in an environmental variable-->
                            <username>jdoe</username>
                            <password>password</password>
                            <role>developer</role>
                            <workingDirectory></workingDirectory>
                            <homeDirectory>D:\cmsynergy\jdoe</homeDirectory>
                            <clientDatabaseDirectory>D:\cmsynergy\uidb</clientDatabaseDirectory>
                            <polling>true</polling>
                        </connection>
                        <project>
                            <delimiter>~</delimiter>
                            <release>Product/1.0</release>
                            <projectSpecification>Product~1</projectSpecification>
                            <taskFolder>1234</taskFolder>
                            <template>true</template>
                            <purpose>Custom Purpose</purpose>
                            <baseline>true</baseline>
                            <reconcile>
                                <path>Product\bin</path>
                                <path>Product\temp.txt</path>
                            </reconcile>
                        </project>
                        <changeSynergy>
                            <url>http://myserver:8060</url>
                            <role>User</role>
                            <username>csuser</username>
                            <password>readonly</password>
                        </changeSynergy>
                    </synergy>";

		public static string NewTaskInfo
		{
			get
			{
				return "15 #### 15 #### Wednesday, December 22, 2004 3:01:34 PM #### jdoe #### lorem ipsum dolerem  #### \r\n" +
					"21 #### 21 #### Wednesday, December 22, 2004 3:13:43 PM #### jdoe #### jdoe's Insulated Development projects #### \r\n" +
					"22 #### 22 #### Tuesday, January 18, 2005 12:28:58 PM #### root #### Insulated Development projects for release PRODUCT/1.0 #### \r\n" +
					"99 #### 99 #### Wednesday, February 02, 2005 12:12:19 PM #### root #### IGNORE THIS Sample Task  #### \r\n" +
					"17 #### 17 #### Friday, March 11, 2005 3:50:45 PM #### jdoe #### the quick brown fox jumped over the lazy dog  #### \r\n" +
					"1 #### 1 #### Wednesday, December 22, 2004 3:01:34 PM #### jdoe #### 0123456789 ~!@#$%^&*()_= #### \r\n";
			}
		}

		public static string NewDcmTaskInfo
		{
			get
			{
				return "mydb#15 #### 15 #### Wednesday, December 22, 2004 3:01:34 PM #### jdoe #### lorem ipsum dolerem  #### \r\n" +
					"mydb#21 #### 21 #### Wednesday, December 22, 2004 3:13:43 PM #### jdoe #### jdoe's Insulated Development projects #### \r\n" +
					"mydb#22 #### 22 #### Tuesday, January 18, 2005 12:28:58 PM #### root #### Insulated Development projects for release PRODUCT/1.0 #### \r\n" +
					"mydb#99 #### 99 #### Wednesday, February 02, 2005 12:12:19 PM #### root #### IGNORE THIS Sample Task  #### \r\n" +
					"mydb#17 #### 17 #### Friday, March 11, 2005 3:50:45 PM #### jdoe #### the quick brown fox jumped over the lazy dog  #### \r\n" +
					"mydb#1 #### 1 #### Wednesday, December 22, 2004 3:01:34 PM #### jdoe #### 0123456789 ~!@#$%^&*()_= #### \r\n";
			}
		}

		public static string NewObjects
		{
			get
			{
				return "sourcecontrol-3 integrate jdoe dir MyProject 4 15\r\n" +
					"\tMyProject\\CruiseControl.NET\\project\\core\\sourcecontrol-3@MyProject-SomeDevProject\r\n" +
					"\tMyProject\\CruiseControl.NET\\project\\core\\sourcecontrol-3@MyProject-MyProject_Int\r\n" +
					"Synergy.cs-1 integrate jdoe ms_cs MyProject 2 17,21\r\n" +
					"\tMyProject\\CruiseControl.NET\\project\\core\\sourcecontrol\\Synergy.cs-1@MyProject-MyProject_Int\r\n" +
					"\tMyProject\\CruiseControl.NET\\project\\core\\sourcecontrol\\Synergy.cs-1@MyProject-SomeDevProject\r\n" +
					"SynergyCommandBuilder.cs-1.1.1 integrate jdoe ms_cs MyProject 1 22\r\n" +
					"\tMyProject\\CruiseControl.NET\\project\\core\\sourcecontrol\\SynergyCommandBuilder.cs-1.1.1@MyProject-MyProject_Int\r\n" +
					"SynergyConnectionInfo.cs-2 integrate jdoe ms_cs MyProject 1 22\r\n" +
					"\tMyProject\\CruiseControl.NET\\project\\core\\sourcecontrol\\SynergyConnectionInfo.cs-2@SomeDevProject\r\n" +
					"\tMyProject\\CruiseControl.NET\\project\\core\\sourcecontrol\\SynergyConnectionInfo.cs-2@MyProject-MyProject_Int\r\n" +
					"\tMyProject\\CruiseControl.NET\\project\\core\\sourcecontrol\\SynergyConnectionInfo.cs-2@MyProject-MyProject_Int2\r\n" +
					"SynergyHistoryParser.cs-2.2.1 integrate jdoe ms_cs MyProject 1 1\r\n" +
					"\tMyProject\\CruiseControl.NET\\project\\core\\sourcecontrol\\SynergyHistoryParser.cs-2.2.1@MyProject-MyProject_Int\r\n" +
					"SynergyProjectInfo.cs-1 integrate jdoe ms_cs MyProject 22 17\r\n" +
					"\tMyProject\\CruiseControl.NET\\project\\core\\sourcecontrol\\SynergyProjectInfo.cs-1@MyProject-MyProject_Int\r\n" +
					"NotUsed-10 integrate jdoe dir MyProject 555 9999\r\n" +
					"\tObject is not used in scope.\r\n";
			}
		}

		public static string NewDCMObjects
		{
			get
			{
				return "sourcecontrol-3 integrate jdoe dir MyProject mydb#4 mydb#15\r\n" +
					"\tMyProject\\CruiseControl.NET\\project\\core\\sourcecontrol-3@MyProject-SomeDevProject\r\n" +
					"\tMyProject\\CruiseControl.NET\\project\\core\\sourcecontrol-3@MyProject-MyProject_Int\r\n" +
					"Synergy.cs-1 integrate jdoe ms_cs MyProject mydb#2 mydb#17,mydb#21\r\n" +
					"\tMyProject\\CruiseControl.NET\\project\\core\\sourcecontrol\\Synergy.cs-1@MyProject-MyProject_Int\r\n" +
					"\tMyProject\\CruiseControl.NET\\project\\core\\sourcecontrol\\Synergy.cs-1@MyProject-SomeDevProject\r\n" +
					"SynergyCommandBuilder.cs-1.1.1 integrate jdoe ms_cs MyProject mydb#1 mydb#22\r\n" +
					"\tMyProject\\CruiseControl.NET\\project\\core\\sourcecontrol\\SynergyCommandBuilder.cs-1.1.1@MyProject-MyProject_Int\r\n" +
					"SynergyConnectionInfo.cs-2 integrate jdoe ms_cs MyProject mydb#1 mydb#22\r\n" +
					"\tMyProject\\CruiseControl.NET\\project\\core\\sourcecontrol\\SynergyConnectionInfo.cs-2@SomeDevProject\r\n" +
					"\tMyProject\\CruiseControl.NET\\project\\core\\sourcecontrol\\SynergyConnectionInfo.cs-2@MyProject-MyProject_Int\r\n" +
					"\tMyProject\\CruiseControl.NET\\project\\core\\sourcecontrol\\SynergyConnectionInfo.cs-2@MyProject-MyProject_Int2\r\n" +
					"SynergyHistoryParser.cs-2.2.1 integrate jdoe ms_cs MyProject mydb#1 mydb#1\r\n" +
					"\tMyProject\\CruiseControl.NET\\project\\core\\sourcecontrol\\SynergyHistoryParser.cs-2.2.1@MyProject-MyProject_Int\r\n" +
					"SynergyProjectInfo.cs-1 integrate jdoe ms_cs MyProject mydb#22 mydb#17\r\n" +
					"\tMyProject\\CruiseControl.NET\\project\\core\\sourcecontrol\\SynergyProjectInfo.cs-1@MyProject-MyProject_Int\r\n" +
					"NotUsed-10 integrate jdoe dir MyProject mydb#555 mydb#9999\r\n" +
					"\tObject is not used in scope.\r\n";
			}
		}
	}
}