namespace CruiseControl.Core.Utilities
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using CruiseControl.Core.Exceptions;
    using CruiseControl.Core.Interfaces;
    using NLog;

    /// <summary>
    /// The ProcessExecutor serves as a simple, injectable facade for executing external processes.  The ProcessExecutor
    /// spawns a new <see cref="RunnableProcess" /> using the properties specified in the input <see cref="ProcessInfo" />.
    /// All output from the executed process is contained within the returned <see cref="ProcessResult" />.
    /// </summary>
    public class ProcessExecutor
    {
        #region Private fields
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constants
        /// <summary>
        /// Default installation directory for the "Windows 2000 Service Pack 4 Support Tools" package.
        /// </summary>
        public const string Win2KSupportToolsDir = @"C:\Program Files\Support Tools";
        #endregion

        #region Public methods
        #region KillByPId()
        /// <summary>
        /// Kills a process by its PID.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="pid">The PID of the process.</param>
        public static void KillByPId(IFileSystem fileSystem, int pid)
        {
            var process = new Process();
            var platform = PopulateKillProcess(process, pid, Environment.OSVersion);

            if (!fileSystem.CheckIfFileExists(process.StartInfo.FileName))
            {
                logger.Error("KillByPId(): Unable to find kill command");
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    "Kill command {0} not found on {1} OS. PID:{2}",
                    process.StartInfo.FileName,
                    platform,
                    pid);
                throw new CruiseControlException(message);
            }

            // Execute the actual kill command
            logger.Info("KillByPId(): running kill command on {0}", pid);
            logger.Debug(
                "KillByPId(): command = [{0}], args = [{1}]",
                process.StartInfo.FileName,
                process.StartInfo.Arguments);
            process.Start();
            process.WaitForExit();
            process.Close();
        }
        #endregion

        #region PopulateKillProcess()
        /// <summary>
        /// Generates the kill process.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="pid">The PId of the process to kill.</param>
        /// <param name="operatingSystem">The operating system.</param>
        /// <returns>
        /// The platform for the kill.
        /// </returns>
        public static string PopulateKillProcess(Process process, int pid, OperatingSystem operatingSystem)
        {
            string platform;
            switch (operatingSystem.Platform)
            {
                case PlatformID.Win32NT:
                    platform = "Windows";
                    if ((operatingSystem.Version.Major == 5) && (operatingSystem.Version.Minor == 0))
                    {
                        logger.Debug("KillByPId(): detected platform is Windows 2000");
                        // Windows 2000 doesn't have taskkill.exe, so use kill.exe from the 
                        // "Windows 2000 Service Pack 4 Support Tools" package from Microsoft's download center
                        // (http://www.microsoft.com/Downloads/details.aspx?FamilyID=f08d28f3-b835-4847-b810-bb6539362473&displaylang=en)
                        // instead.  It may not exist, but if it doesn't, at least if can be obtained.
                        process.StartInfo.FileName = Path.Combine(Win2KSupportToolsDir, "kill.exe");
                        process.StartInfo.Arguments = string.Format(CultureInfo.CurrentCulture, "-f {0}", pid);
                        break;
                    }

                    logger.Debug("KillByPId(): detected platform is Windows (non-2000)");
                    process.StartInfo.FileName = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.System),
                        "taskkill.exe");
                    process.StartInfo.Arguments = string.Format(CultureInfo.CurrentCulture, "/pid {0} /t /f", pid);
                    break;

                case PlatformID.Unix:
                    // need to execute uname -s to find out if it is a MAC or not
                    process.StartInfo.FileName = "uname";
                    process.StartInfo.Arguments = "-s";
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.UseShellExecute = false;
                    var output = string.Empty;
                    var exitCode = -1;
                    try
                    {
                        process.Start();
                        process.WaitForExit();
                        var standardOutput = process.StandardOutput;
                        output = standardOutput.ReadToEnd();
                        exitCode = process.ExitCode;
                    }
                    catch
                    {
                        // This will always fail on windows
                    }

                    process.Close();
                    if ((exitCode == 0) && (output.Contains("Darwin")))
                    {
                        logger.Debug("KillByPId(): detected platform is Mac");
                        process.StartInfo.FileName = "/bin/kill";
                        process.StartInfo.Arguments = string.Format(
                            CultureInfo.CurrentCulture,
                            "-9 {0}",
                            pid);
                        platform = "Mac";
                    }
                    else
                    {
                        logger.Debug("KillByPId(): detected platform is *nix");
                        process.StartInfo.FileName = "/usr/bin/pkill";
                        process.StartInfo.Arguments = string.Format(
                            CultureInfo.CurrentCulture, "-9 -g {0}",
                            pid);
                        platform = "Unix";
                    }

                    break;

                default:
                    throw new CruiseControlException("Unknown Operating System.");
            }
            return platform;
        }
        #endregion

        //#region Execute()
        ///// <summary>
        ///// Executes the specified process info.	
        ///// </summary>
        ///// <param name="processInfo">The process info.</param>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //public virtual ProcessResult Execute(ProcessInfo processInfo)
        //{
        //    var projectName = Thread.CurrentThread.Name;
        //    using (var p = new RunnableProcess(
        //        processInfo,
        //        projectName,
        //        processInfo == null ? null : processInfo.PublicArguments))
        //    {
        //        p.ProcessOutput += ((sender, e) => OnProcessOutput(e));

        //        ProcessMonitor.MonitorProcessForProject(p.Process, projectName);
        //        var run = p.Run();
        //        ProcessMonitor.RemoveMonitorForProject(projectName);
        //        return run;
        //    }
        //}
        //#endregion
        #endregion

        #region Public events
        #region ProcessOutput
        /// <summary>
        /// Occurs when process has some output.
        /// </summary>
        public event EventHandler<ProcessOutputEventArgs> ProcessOutput;
        #endregion
        #endregion

        //        /// <summary>
        //        /// Kills the process currently running for project.	
        //        /// </summary>
        //        /// <param name="name">The name.</param>
        //        /// <remarks></remarks>
        //        public static void KillProcessCurrentlyRunningForProject(string name)
        //        {
        //            var monitor = ProcessMonitor.ForProject(name);
        //            if (monitor == null)
        //            {
        //                logger.Debug(
        //                    "Request to abort process currently running for project {0}, but no process is currently running.",
        //                    name);
        //            }
        //            else
        //            {
        //                monitor.KillProcess();
        //            }
        //        }

        //        /// <summary>
        //        /// Raises the <see cref="E:ProcessOutput" /> event.	
        //        /// </summary>
        //        /// <param name="eventArgs">The <see cref="ProcessOutputEventArgs" /> instance containing the event data.</param>
        //        /// <remarks></remarks>
        //        protected virtual void OnProcessOutput(ProcessOutputEventArgs eventArgs)
        //        {
        //            var handler = this.ProcessOutput;
        //            if (handler != null)
        //            {
        //                handler(this, eventArgs);
        //            }
        //        }

        //        private class RunnableProcess : IDisposable
        //        {
        //            public event EventHandler<ProcessOutputEventArgs> ProcessOutput;

        //            private readonly string projectName;
        //            private readonly ProcessInfo processInfo;
        //            private readonly Process process;
        //            private readonly string publicArgs;
        //            private readonly StringBuilder stdOutput = new StringBuilder();
        //            private readonly EventWaitHandle outputStreamClosed = new ManualResetEvent(false);
        //            private readonly StringBuilder stdError = new StringBuilder();
        //            private readonly EventWaitHandle errorStreamClosed = new ManualResetEvent(false);
        //            private readonly EventWaitHandle processExited = new ManualResetEvent(false);
        //            private Thread supervisingThread;


        //            public RunnableProcess(ProcessInfo processInfo, string projectName, string publicArgs)
        //            {
        //                this.projectName = projectName;
        //                this.processInfo = processInfo;
        //                this.publicArgs = publicArgs;
        //                process = processInfo.CreateProcess();
        //            }

        //            public ProcessResult Run()
        //            {
        //                var hasTimedOut = false;
        //                var hasExited = false;

        //                StartProcess();

        //                try
        //                {
        //                    hasExited = WaitHandle.WaitAll(new WaitHandle[]
        //                                                       {
        //                                                           errorStreamClosed, 
        //                                                           outputStreamClosed, 
        //                                                           processExited
        //                                                       }, processInfo.TimeOut, true);
        //                    hasTimedOut = !hasExited;
        //                    if (hasTimedOut)
        //                    {
        //                        logger.Warn(
        //                            "Process timed out: {0} {1}.  Process id: {2}. This process will now be killed.",
        //                            processInfo.FileName,
        //                            processInfo.PublicArguments,
        //                            process.Id);
        //                    }
        //                }
        //                catch (ThreadAbortException)
        //                {
        //                    // Thread aborted. This is the server trying to exit. Abort needs to continue.
        //                    logger.Info(
        //                        "Thread aborted while waiting for '{0} {1}' to exit. Process id: {2}. This process will now be killed.",
        //                        processInfo.FileName,
        //                        processInfo.PublicArguments,
        //                        process.Id);
        //                    throw;
        //                }
        //                catch (ThreadInterruptedException)
        //                {
        //                    // If one of the output handlers catches an exception, it will interrupt this thread to wake it.
        //                    // The finally block handles clean-up.
        //                    logger.Debug(
        //                        "Process interrupted: {0} {1}.  Process id: {2}. This process will now be killed.",
        //                        processInfo.FileName,
        //                        processInfo.PublicArguments,
        //                        process.Id);
        //                }
        //                finally
        //                {
        //                    if (!hasExited)
        //                    {
        //                        Kill();
        //                    }
        //                }

        //                var exitcode = process.ExitCode;
        //                bool failed = !processInfo.ProcessSuccessful(exitcode);

        //                return new ProcessResult(stdOutput.ToString(), stdError.ToString(), exitcode, hasTimedOut, failed);
        //            }

        //            private void StartProcess()
        //            {
        //                logger.Debug(
        //                    "Starting process [{0}] in working directory [{1}] with arguments [{2}]",
        //                    process.StartInfo.FileName,
        //                    process.StartInfo.WorkingDirectory,
        //                    this.publicArgs);
        //                process.OutputDataReceived += StandardOutputHandler;
        //                process.ErrorDataReceived += ErrorOutputHandler;
        //                process.Exited += ExitedHandler;
        //                process.EnableRaisingEvents = true;
        //                supervisingThread = Thread.CurrentThread;

        //                string filename = Path.Combine(process.StartInfo.WorkingDirectory, process.StartInfo.FileName);

        //                try
        //                {
        //                    var isNewProcess = process.Start();
        //                    if (!isNewProcess)
        //                    {
        //                        logger.Warn("Reusing existing process...");
        //                    }

        //                    // avoid useless setting of the default
        //                    if (processInfo.Priority != Process.GetCurrentProcess().PriorityClass)
        //                    {
        //                        try
        //                        {
        //                            logger.Debug(
        //                                "Setting PriorityClass on [{0}] to {1}", 
        //                                filename, 
        //                                processInfo.Priority));
        //                            process.PriorityClass = processInfo.Priority;
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            if (!process.HasExited)
        //                            {
        //                                logger.Info(
        //                                    "Unable to set PriorityClass on [{0}]: {1}", 
        //                                    filename, 
        //                                    ex);
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        logger.Debug(
        //                            "Not setting PriorityClass on [{0}] to default {1}", 
        //                            filename, 
        //                            processInfo.Priority);
        //                    }
        //                }
        //                catch (Win32Exception e)
        //                {
        //                    var msg = string.Format(
        //                        CultureInfo.CurrentCulture, 
        //                        "Unable to execute file [{0}]. The file may not exist or may not be executable.", 
        //                        filename);
        //                    throw new IOException(msg, e);
        //                }

        //                WriteToStandardInput();
        //                process.BeginOutputReadLine();
        //                process.BeginErrorReadLine();
        //            }

        //            private void Kill()
        //            {
        //                const int waitForKilledProcessTimeout = 10000;

        //                logger.Debug(
        //                    "Sending kill to process {0} and waiting {1} seconds for it to exit.",
        //                    process.Id,
        //                    waitForKilledProcessTimeout / 1000);
        //                CancelEventsAndWait();
        //                try
        //                {
        //                    KillByPId(process.Id);
        //                    if (!process.WaitForExit(waitForKilledProcessTimeout))
        //                        throw new CruiseControlException(
        //                            string.Format(CultureInfo.CurrentCulture, @"The killed process {0} did not terminate within the allotted timeout period {1}.  The process or one of its child processes may not have died.  This may create problems when trying to re-execute the process.  It may be necessary to reboot the server to recover.",
        //                                process.Id,
        //                                waitForKilledProcessTimeout));
        //                    logger.Warn(
        //                        "The process has been killed: {0}",
        //                        process.Id);
        //                }
        //                catch (InvalidOperationException)
        //                {
        //                    logger.Warn(
        //                        "Process has already exited before getting killed: {0}",
        //                        process.Id);
        //                }
        //            }

        //            private void CancelEventsAndWait()
        //            {
        //                process.EnableRaisingEvents = false;
        //                process.Exited -= ExitedHandler;

        //                process.CancelErrorRead();
        //                process.CancelOutputRead();
        //                WaitHandle.WaitAll(new WaitHandle[]
        //                                       {
        //                                           errorStreamClosed, 
        //                                           outputStreamClosed
        //                                       }, 1000, true);
        //            }

        //            private void WriteToStandardInput()
        //            {
        //                if (process.StartInfo.RedirectStandardInput)
        //                {
        //                    process.StandardInput.Write(processInfo.StandardInputContent);
        //                    process.StandardInput.Flush();
        //                    process.StandardInput.Close();
        //                }
        //            }

        //            private void ExitedHandler(object sender, EventArgs e)
        //            {
        //                logger.Debug(
        //                    "[{0} {1}] process exited event received",
        //                    projectName,
        //                    processInfo.FileName);
        //                processExited.Set();
        //            }

        //            private void StandardOutputHandler(object sender, DataReceivedEventArgs outLine)
        //            {
        //                try
        //                {
        //                    CollectOutput(outLine.Data, stdOutput, outputStreamClosed, "standard-output");

        //                    if (!string.IsNullOrEmpty(outLine.Data))
        //                    {
        //                        OnProcessOutput(
        //                            new ProcessOutputEventArgs(ProcessOutputType.StandardOutput, outLine.Data));
        //                    }
        //                }
        //                catch (Exception e)
        //                {
        //                    var message = string.Format(
        //                        CultureInfo.CurrentCulture,
        //                        "[{0} {1}] Exception while collecting standard output",
        //                        projectName,
        //                        processInfo.FileName);
        //                    logger.ErrorException(message, e);
        //                    supervisingThread.Interrupt();
        //                }
        //            }

        //            private void ErrorOutputHandler(object sender, DataReceivedEventArgs outLine)
        //            {
        //                try
        //                {
        //                    CollectOutput(outLine.Data, stdError, errorStreamClosed, "standard-error");

        //                    if (!string.IsNullOrEmpty(outLine.Data))
        //                    {
        //                        OnProcessOutput(
        //                            new ProcessOutputEventArgs(ProcessOutputType.ErrorOutput, outLine.Data));
        //                    }
        //                }
        //                catch (Exception e)
        //                {
        //                    var message = string.Format(
        //                        CultureInfo.CurrentCulture,
        //                        "[{0} {1}] Exception while collecting error output",
        //                        projectName,
        //                        processInfo.FileName);
        //                    logger.ErrorException(message, e);
        //                    supervisingThread.Interrupt();
        //                }
        //            }

        //            private void CollectOutput(string output, StringBuilder collector, EventWaitHandle streamReadComplete, string streamLabel)
        //            {
        //                if (output == null)
        //                {
        //                    logger.Debug(
        //                        "[{0} {1}] {2} stream closed -- null received in event",
        //                        projectName,
        //                        processInfo.FileName,
        //                        streamLabel);
        //                    streamReadComplete.Set();
        //                    return;
        //                }

        //                collector.AppendLine(output);
        //                logger.Debug("[{0} {1}] {2}", projectName, processInfo.FileName, output);
        //            }

        //            void IDisposable.Dispose()
        //            {
        //                outputStreamClosed.Close();
        //                errorStreamClosed.Close();
        //                processExited.Close();
        //                process.Dispose();
        //            }

        //            // TODO: Smelly. ProcessMonitor doesn't seem like the right abstraction.
        //            public Process Process
        //            {
        //                get { return process; }
        //            }

        //            protected virtual void OnProcessOutput(ProcessOutputEventArgs eventArgs)
        //            {
        //                var handler = this.ProcessOutput;
        //                if (handler != null)
        //                {
        //                    handler(this, eventArgs);
        //                }
        //            }
        //        }

        //        /// <summary>
        //        /// A Process-Monitor receives the currently active process of a specific project
        //        /// and stores a reference to it.
        //        /// It can be used to abort a running build.
        //        /// </summary>
        //        private class ProcessMonitor
        //        {
        //            private static readonly IDictionary<string, ProcessMonitor> processMonitors = new Dictionary<string, ProcessMonitor>();
        //            private static readonly object lockObject = new object();

        //            // Return an existing Processmonitor
        //            public static ProcessMonitor ForProject(string projectName)
        //            {
        //                Monitor.TryEnter(lockObject, 60000);
        //                try
        //                {
        //                    return processMonitors.ContainsKey(projectName) ? processMonitors[projectName] : null;
        //                }
        //                finally
        //                {
        //                    Monitor.Exit(lockObject);
        //                }
        //            }

        //            public static void MonitorProcessForProject(Process process, string projectName)
        //            {
        //                Monitor.TryEnter(lockObject, 60000);
        //                try
        //                {
        //                    processMonitors[projectName] = new ProcessMonitor(process, projectName);
        //                }
        //                finally
        //                {
        //                    Monitor.Exit(lockObject);
        //                }
        //            }

        //            public static void RemoveMonitorForProject(string projectName)
        //            {
        //                Monitor.TryEnter(lockObject, 60000);
        //                try
        //                {
        //                    processMonitors.Remove(projectName);
        //                }
        //                finally
        //                {
        //                    Monitor.Exit(lockObject);
        //                }
        //            }

        //            private readonly Process process;
        //            private readonly string projectName;

        //            private ProcessMonitor(Process process, string projectName)
        //            {
        //                this.process = process;
        //                this.projectName = projectName;
        //            }

        //            // Kill the process
        //            public void KillProcess()
        //            {
        //                KillByPId(process.Id);
        //                logger.Info("{0}: ------------------------------------------------------------------", projectName);
        //                logger.Info("{0}: ---------The Build Process was successfully aborted---------------", projectName);
        //                logger.Info("{0}: ------------------------------------------------------------------", projectName);
        //            }
        //        }
    }
}
