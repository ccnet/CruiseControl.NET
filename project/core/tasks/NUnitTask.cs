using System;
using System.Runtime.InteropServices;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using System.Diagnostics;
    using System.IO;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;

    /// <summary>
    /// <para type="tip">
    ///  See <link>Using CruiseControl.NET with NUnit</link> for more details.
    /// </para>
    /// <para>
    ///  This task enables you to instruct CCNet to run the unit tests contained within a collection of assemblies. The results of the unit
    ///  tests will be automatically included in the CCNet build results. This can be useful if you have some unit tests that you want to
    ///  run as part of the integration process, but you don't need as part of your developer build process. For example, if you have a set
    ///  of integration tests that you want to run in a separate build process, it is easy to set up a project to use this task.
    /// </para>
    /// <para>
    ///  If you are using the <link>Visual Studio Task</link> and you want to run unit tests then you probably want to use this task.
    ///  Alternatively you can run NUnit using post-build tasks in your Visual Studio project properties.
    /// </para>
    /// <para type="warning">
    ///  We recommend not using this task, and using your builder to run your tests if possible. This way if the tests fail and you don't
    ///  know why, it is a lot easier to try and replicate the problem on another machine.
    /// </para>
    /// <para type="warning">
    ///  When using this task,do NOT merge an xml file from bin folder of your app with the merge task, or the results will be save twice in
    ///  the buildlog file.
    /// </para>
    /// </summary>
    /// <title>NUnit Task</title>
    /// <version>1.0</version>
    /// <example>
    /// <code>
    /// &lt;nunit&gt;
    /// &lt;path&gt;D:\dev\ccnet\ccnet\tools\nunit\nunit-console.exe&lt;/path&gt;
    /// &lt;assemblies&gt;
    /// &lt;assembly&gt;D:\dev\Refactoring\bin\Debug\Refactoring.exe&lt;/assembly&gt;
    /// &lt;assembly&gt;D:\dev\Refactoring\bin\Debug\Refactoring.Core.dll&lt;/assembly&gt;
    /// &lt;/assemblies&gt;
    /// &lt;excludedCategories&gt;
    /// &lt;excludedCategory&gt;LongRunning&lt;/excludedCategory&gt;
    /// &lt;/excludedCategories&gt;
    /// &lt;/nunit&gt;
    /// </code>
    /// </example>
    [ReflectorType("nunit")]
    public class NUnitTask
        : TaskBase
    {
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string DefaultPath = @"nunit-console";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const int DefaultTimeout = 600;
        private const string DefaultOutputFile = "nunit-results.xml";
        private readonly ProcessExecutor executor;

        /// <summary>
        /// Initializes a new instance of the <see cref="NUnitTask" /> class.	
        /// </summary>
        /// <remarks></remarks>
        public NUnitTask()
            : this(new ProcessExecutor())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NUnitTask" /> class.	
        /// </summary>
        /// <param name="exec">The exec.</param>
        /// <remarks></remarks>
        public NUnitTask(ProcessExecutor exec)
        {
            executor = exec;
            this.Assemblies = new string[0];
            this.NUnitPath = DefaultPath;
            this.OutputFile = DefaultOutputFile;
            this.Timeout = DefaultTimeout;
            this.Priority = ProcessPriorityClass.Normal;
            this.ExcludedCategories = new string[0];
            this.IncludedCategories = new string[0];
        }

        #region Public fields
        #region Assemblies
        /// <summary>
        /// List of the paths to the assemblies containing the NUnit tests to be run.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("assemblies")]
        public string[] Assemblies { get; set; }
        #endregion

        #region NUnitPath
        /// <summary>
        /// Path of <b>nunit-console.exe</b> application. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>nunit-console</default>
        [ReflectorProperty("path", Required = false)]
        public string NUnitPath { get; set; }
        #endregion

        #region OutputFile
        /// <summary>
        /// The file that NUnit will write the test results to.
        /// </summary>
        /// <version>1.0</version>
        /// <default>nunit-results.xml</default>
        [ReflectorProperty("outputfile", Required = false)]
        public string OutputFile { get; set; }
        #endregion

        #region Timeout
        /// <summary>
        /// The number of seconds that the nunit process will run before timing out.
        /// </summary>
        /// <version>1.0</version>
        /// <default>600</default>
        [ReflectorProperty("timeout", Required = false)]
        public int Timeout { get; set; }
        #endregion

        #region Priority
        /// <summary>
        /// The priority class of the spawned process.
        /// </summary>
        /// <version>1.5</version>
        /// <default>Normal</default>
        [ReflectorProperty("priority", Required = false)]
        public ProcessPriorityClass Priority { get; set; }
        #endregion

        #region ExcludedCategories
        /// <summary>
        /// List of the test categories to be excluded from the NUnit run. The tests need to have the CategoryAttribute set. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("excludedCategories", Required = false)]
        public string[] ExcludedCategories { get; set; }
        #endregion

        #region IncludedCategories
        /// <summary>
        /// List of the test categories to be included in the NUnit run. The tests need to have the CategoryAttribute set. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("includedCategories", Required = false)]
        public string[] IncludedCategories { get; set; }
        #endregion
        #endregion

        /// <summary>
        /// Executes the specified result.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override bool Execute(IIntegrationResult result)
        {
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description : "Executing NUnit");

            string outputFile = result.BaseFromArtifactsDirectory(OutputFile);

            ProcessResult nunitResult = executor.Execute(NewProcessInfo(outputFile, result));
            DeleteNUnitAgentProcesses();

            result.AddTaskResult(new ProcessTaskResult(nunitResult, true));
            if (File.Exists(outputFile))
            {
                result.AddTaskResult(new FileTaskResult(outputFile));
            }
            else
            {
                Log.Warning(string.Format(System.Globalization.CultureInfo.CurrentCulture,"NUnit test output file {0} was not created", outputFile));
            }
            return !nunitResult.Failed;
        }

        private ProcessInfo NewProcessInfo(string outputFile, IIntegrationResult result)
        {
            NUnitArgument nunitArgument = new NUnitArgument(Assemblies, outputFile);
            nunitArgument.ExcludedCategories = ExcludedCategories;
            nunitArgument.IncludedCategories = IncludedCategories;
            string args = nunitArgument.ToString();

            Log.Debug(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Running unit tests: {0} {1}", NUnitPath, args));

            ProcessInfo info = new ProcessInfo(NUnitPath, args, result.WorkingDirectory, Priority);

            System.Collections.IDictionary properties = result.IntegrationProperties;
            // pass user defined the environment variables
            foreach (EnvironmentVariable item in EnvironmentVariables)
                info.EnvironmentVariables[item.name] = item.value;

            // Pass the integration environment variables to devenv.
            foreach (string key in properties.Keys)
            {
                info.EnvironmentVariables[key] = StringUtil.IntegrationPropertyToString(properties[key]);
            }

            info.TimeOut = Timeout * 1000;
            return info;
        }


        private void DeleteNUnitAgentProcesses()
        {
            var processes = Process.GetProcessesByName("nunit-agent");
            foreach (var process in processes)
            {
                if (!HasParentProcess(process.Id))
                {
                    Log.Warning("Detected leaked nunit-agent.exe process with Id={0}.", process.Id);
                    try
                    {
                        process.Kill();
                        process.WaitForExit(2000);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
        }

        private static bool HasParentProcess(int pid)
        {
            ExecutionEnvironment ee = new ExecutionEnvironment();
            if (ee.IsRunningOnWindows)
                return HasParentProcessWindows(pid);

            return HasParentProcessNonWindows(pid);
        }


        private static bool HasParentProcessNonWindows(int pid)
        {
            //todo make implementation for non windows
            // there do not appears to be a managed way of getting the parent procesid
            return false;
        }

        #region HasParentProcessWindows
        private static bool HasParentProcessWindows(int pid)
        {
            IntPtr handleToSnapshot = IntPtr.Zero;
            try
            {
                PROCESSENTRY32 procEntry = new PROCESSENTRY32();
                procEntry.dwSize = (UInt32)Marshal.SizeOf(typeof(PROCESSENTRY32));
                handleToSnapshot = CreateToolhelp32Snapshot(SnapshotFlags.Process, (uint)0);
                if (Process32First(handleToSnapshot, ref procEntry))
                {
                    do
                    {
                        if (pid == procEntry.th32ProcessID)
                        {
                            try
                            {
                                Process parentProc = Process.GetProcessById((int)procEntry.th32ParentProcessID);
                                return true;
                            }
                            catch (ArgumentException)
                            {
                                return false;
                            }
                        }
                    } while (Process32Next(handleToSnapshot, ref procEntry));
                }
                else
                {
                    Log.Error("Failed to load process infomration with error code {0}.", Marshal.GetLastWin32Error());
                }
            }
            catch (Exception ex)
            {
                Log.Error("Can't get list of processes. Exception: {0}", ex.Message);
            }
            finally
            {
                // Must clean up the snapshot object!
                CloseHandle(handleToSnapshot);
            }
            return false;
        }


        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateToolhelp32Snapshot(SnapshotFlags dwFlags, uint th32ProcessID);

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern bool Process32First([In]IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern bool Process32Next([In]IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle([In] IntPtr hObject);


        [Flags]
        private enum SnapshotFlags : uint
        {
            HeapList = 0x00000001,
            Process = 0x00000002,
            Thread = 0x00000004,
            Module = 0x00000008,
            Module32 = 0x00000010,
            Inherit = 0x80000000,
            All = 0x0000001F
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct PROCESSENTRY32
        {
            const int MAX_PATH = 260;
            internal UInt32 dwSize;
            internal UInt32 cntUsage;
            internal UInt32 th32ProcessID;
            internal IntPtr th32DefaultHeapID;
            internal UInt32 th32ModuleID;
            internal UInt32 cntThreads;
            internal UInt32 th32ParentProcessID;
            internal Int32 pcPriClassBase;
            internal UInt32 dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            internal string szExeFile;
        }


        #endregion
    }
}
