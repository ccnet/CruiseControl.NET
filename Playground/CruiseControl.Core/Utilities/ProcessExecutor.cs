namespace CruiseControl.Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using CruiseControl.Core.Exceptions;
    using CruiseControl.Core.Interfaces;
    using Ninject;
    using NLog;

    /// <summary>
    /// The ProcessExecutor serves as a simple, injectable facade for executing external processes.  The ProcessExecutor
    /// spawns a new <see cref="RunnableProcess" /> using the properties specified in the input <see cref="ProcessInfo" />.
    /// All output from the executed process is contained within the returned <see cref="ProcessResult" />.
    /// </summary>
    public class ProcessExecutor : IProcessExecutor
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

        #region Public properties
        #region FileSystem
        /// <summary>
        /// Gets or sets the file system.
        /// </summary>
        /// <value>
        /// The file system.
        /// </value>
        [Inject]
        public IFileSystem FileSystem { get; set; }
        #endregion
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

        #region Execute()
        /// <summary>
        /// Executes the specified process info.
        /// </summary>
        /// <param name="processInfo">The process info.</param>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="itemId">The item id.</param>
        /// <param name="outputFile">The output file.</param>
        /// <returns>
        /// The result of the execution.
        /// </returns>
        public virtual ProcessResult Execute(ProcessInfo processInfo, string projectName, string itemId, string outputFile)
        {
            using (var p = new RunnableProcess(
                this.FileSystem,
                processInfo,
                projectName,
                itemId))
            {
                p.ProcessOutput += ((sender, e) => OnProcessOutput(e));

                ProcessMonitor.MonitorProcessForProject(projectName, itemId, p.Process);
                var run = p.Run(outputFile);
                ProcessMonitor.RemoveMonitorForProject(projectName, itemId);
                return run;
            }
        }

        /// <summary>
        /// Executes the specified process for a project item.
        /// </summary>
        /// <param name="processInfo">The process info.</param>
        /// <param name="item">The item.</param>
        /// <returns>
        /// The result of the execution.
        /// </returns>
        public ProcessResult Execute(ProcessInfo processInfo, ProjectItem item, TaskExecutionContext context)
        {
            var logFile = context.GeneratePathInWorkingDirectory(item.NameOrType + ".log");
            return this.Execute(processInfo, item.Project.Name, item.NameOrType, logFile);
        }
        #endregion

        #region KillProcessesForProject()
        /// <summary>
        /// Kills all the currently active processes for a project.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="name">The name of the project.</param>
        public static void KillProcessesForProject(IFileSystem fileSystem, string name)
        {
            var monitor = ProcessMonitor.RetrieveForProject(name);
            if (monitor == null)
            {
                logger.Debug(
                    "Request to abort process currently running for project {0}, but no process is currently running.",
                    name);
            }
            else
            {
                logger.Info("Killing active processes for '{0}'", name);
                monitor.KillAll(fileSystem);
            }
        }
        #endregion
        #endregion

        #region Public events
        #region ProcessOutput
        /// <summary>
        /// Occurs when process has some output.
        /// </summary>
        public event EventHandler<ProcessOutputEventArgs> ProcessOutput;
        #endregion
        #endregion

        #region Protected methods
        #region OnProcessOutput()
        /// <summary>
        /// Raises the <see cref="ProcessOutput"/> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="CruiseControl.Core.Utilities.ProcessOutputEventArgs"/> instance containing the event data.</param>
        protected virtual void OnProcessOutput(ProcessOutputEventArgs eventArgs)
        {
            var handler = this.ProcessOutput;
            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }
        #endregion
        #endregion

        #region Private classes
        #region ProcessMonitor
        /// <summary>
        /// The process monitor keeps track of any processes that are running for each project.
        /// </summary>
        private class ProcessMonitor
        {
            #region Private fields
            private static readonly IDictionary<string, ProcessMonitor> processMonitors = new Dictionary<string, ProcessMonitor>();
            private static readonly object lockObject = new object();
            private readonly string projectName;
            #endregion

            #region Constructors
            /// <summary>
            /// Prevents a default instance of the <see cref="ProcessMonitor"/> class from being created.
            /// </summary>
            /// <param name="projectName">Name of the project.</param>
            private ProcessMonitor(string projectName)
            {
                this.projectName = projectName;
                this.Processes = new Dictionary<string, Process>();
            }
            #endregion

            #region Public methods
            #region RetrieveForProject()
            /// <summary>
            /// Retrieves the active processes for a project.
            /// </summary>
            /// <param name="projectName">Name of the project.</param>
            /// <returns>
            /// The active processes for a project.
            /// </returns>
            public static ProcessMonitor RetrieveForProject(string projectName)
            {
                var lockTaken = false;
                try
                {
                    Monitor.TryEnter(lockObject, TimeSpan.FromMinutes(1), ref lockTaken);
                    ProcessMonitor monitor;
                    return processMonitors.TryGetValue(projectName, out monitor)
                               ? monitor
                               : null;
                }
                finally
                {
                    if (lockTaken)
                    {
                        Monitor.Exit(lockObject);
                    }
                }
            }
            #endregion

            #region MonitorProcessForProject()
            /// <summary>
            /// Starts monitoring a process for a project.
            /// </summary>
            /// <param name="projectName">Name of the project.</param>
            /// <param name="taskId">The task id.</param>
            /// <param name="process">The process to monitor.</param>
            public static void MonitorProcessForProject(string projectName, string taskId, Process process)
            {
                var lockTaken = false;
                try
                {
                    Monitor.TryEnter(lockObject, TimeSpan.FromMinutes(1), ref lockTaken);
                    ProcessMonitor monitor;
                    if (!processMonitors.TryGetValue(projectName, out monitor))
                    {
                        monitor = new ProcessMonitor(projectName);
                        processMonitors.Add(projectName, monitor);
                        logger.Debug("Added monitor for '{0}'", projectName);
                    }

                    monitor.Processes.Add(taskId, process);
                    logger.Debug("Added process '{0}' to monitor for '{1}", taskId, projectName);
                }
                finally
                {
                    if (lockTaken)
                    {
                        Monitor.Exit(lockObject);
                    }
                }
            }
            #endregion

            #region RemoveMonitorForProject()
            /// <summary>
            /// Removes the monitor for project.
            /// </summary>
            /// <param name="projectName">Name of the project.</param>
            /// <param name="taskId">The task id.</param>
            public static void RemoveMonitorForProject(string projectName, string taskId)
            {
                var lockTaken = false;
                try
                {
                    Monitor.TryEnter(lockObject, TimeSpan.FromMinutes(1), ref lockTaken);
                    ProcessMonitor monitor;
                    if (processMonitors.TryGetValue(projectName, out monitor))
                    {
                        monitor.Processes.Remove(taskId);
                        logger.Debug("Removed process '{0}' from monitor for '{1}'", taskId, projectName);
                        if (!monitor.Processes.Any())
                        {
                            logger.Debug("Removed monitor for '{0}'", projectName);
                            processMonitors.Remove(projectName);
                        }
                    }
                }
                finally
                {
                    if (lockTaken)
                    {
                        Monitor.Exit(lockObject);
                    }
                }
            }
            #endregion

            #region
            /// <summary>
            /// Kills all the processes.
            /// </summary>
            /// <param name="fileSystem">The file system.</param>
            public void KillAll(IFileSystem fileSystem)
            {
                foreach (var process in this.Processes.ToArray())
                {
                    logger.Info("Killing process for '{0}' in '{1}'", process.Key, this.projectName);
                    KillByPId(fileSystem, process.Value.Id);
                    RemoveMonitorForProject(projectName, process.Key);
                }
            }
            #endregion
            #endregion

            #region Private properties
            #region Processes
            /// <summary>
            /// Gets or sets the processes.
            /// </summary>
            /// <value>
            /// The processes.
            /// </value>
            private IDictionary<string, Process> Processes { get; set; }
            #endregion
            #endregion
        }
        #endregion

        #region RunnableProcess
        /// <summary>
        /// A runnable process.
        /// </summary>
        private sealed class RunnableProcess
            : IDisposable
        {
            #region Private fields
            private readonly string projectName;
            private readonly ProcessInfo processInfo;
            private readonly string itemId;
            private readonly Process process;
            private string logFile;
            private readonly EventWaitHandle outputStreamClosed = new ManualResetEvent(false);
            private readonly EventWaitHandle errorStreamClosed = new ManualResetEvent(false);
            private readonly EventWaitHandle processExited = new ManualResetEvent(false);
            private Thread supervisingThread;
            private readonly IFileSystem fileSystem;
            private TextWriter writer;
            private Stream writerStream;
            #endregion

            #region Public events
            #region ProcessOutput
            /// <summary>
            /// Occurs when process has some output.
            /// </summary>
            public event EventHandler<ProcessOutputEventArgs> ProcessOutput;
            #endregion
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="RunnableProcess"/> class.
            /// </summary>
            /// <param name="fileSystem">The file system.</param>
            /// <param name="processInfo">The process info.</param>
            /// <param name="projectName">Name of the project.</param>
            /// <param name="itemId">The item id.</param>
            public RunnableProcess(IFileSystem fileSystem, ProcessInfo processInfo, string projectName, string itemId)
            {
                this.fileSystem = fileSystem;
                this.projectName = projectName;
                this.processInfo = processInfo;
                this.itemId = itemId;
                this.process = processInfo.CreateProcess(fileSystem);
            }
            #endregion

            #region Public properties
            #region Process
            /// <summary>
            /// Gets the process.
            /// </summary>
            public Process Process
            {
                get { return process; }
            }
            #endregion
            #endregion

            #region Public methods
            #region Run()
            /// <summary>
            /// Runs this instance.
            /// </summary>
            /// <param name="outputFile">The output file.</param>
            /// <returns>
            /// The result from the process.
            /// </returns>
            public ProcessResult Run(string outputFile)
            {
                this.logFile = outputFile;
                var hasTimedOut = false;
                var hasExited = false;
                this.writerStream = this.fileSystem.OpenFileForWrite(this.logFile);
                this.writer = new StreamWriter(this.writerStream);
                this.StartProcess();
                try
                {
                    hasExited = WaitHandle.WaitAll(new WaitHandle[]
                                                               {
                                                                   errorStreamClosed, 
                                                                   outputStreamClosed, 
                                                                   processExited
                                                               }, processInfo.TimeOut, true);
                    hasTimedOut = !hasExited;
                    if (hasTimedOut)
                    {
                        logger.Warn(
                            "Process timed out: {0} {1}.  Process id: {2}. This process will now be killed.",
                            processInfo.FileName,
                            processInfo.PublicArguments,
                            process.Id);
                    }
                }
                catch (ThreadAbortException)
                {
                    // Thread aborted. This is the server trying to exit. Abort needs to continue.
                    logger.Info(
                        "Thread aborted while waiting for '{0} {1}' to exit. Process id: {2}. This process will now be killed.",
                        processInfo.FileName,
                        processInfo.PublicArguments,
                        process.Id);
                    throw;
                }
                catch (ThreadInterruptedException)
                {
                    // If one of the output handlers catches an exception, it will interrupt this thread to wake it.
                    // The finally block handles clean-up.
                    logger.Debug(
                        "Process interrupted: {0} {1}.  Process id: {2}. This process will now be killed.",
                        processInfo.FileName,
                        processInfo.PublicArguments,
                        process.Id);
                }
                finally
                {
                    if (!hasExited)
                    {
                        Kill();
                    }
                }

                var exitcode = process.ExitCode;
                var failed = !processInfo.CheckIfSuccess(exitcode);
                this.CleanUpWriter();
                return new ProcessResult(
                    this.fileSystem,
                    this.logFile,
                    exitcode,
                    hasTimedOut,
                    failed);
            }
            #endregion

            #region Dispose();
            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting
            /// unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                outputStreamClosed.Close();
                errorStreamClosed.Close();
                processExited.Close();
                this.process.Dispose();
                this.CleanUpWriter();
            }
            #endregion
            #endregion

            #region Protected methods
            #region OnProcessOutput()
            /// <summary>
            /// Raises the <see cref="ProcessOutput"/> event.
            /// </summary>
            /// <param name="eventArgs">The <see cref="CruiseControl.Core.Utilities.ProcessOutputEventArgs"/> instance containing the event data.</param>
            private void OnProcessOutput(ProcessOutputEventArgs eventArgs)
            {
                var handler = this.ProcessOutput;
                if (handler != null)
                {
                    handler(this, eventArgs);
                }
            }
            #endregion
            #endregion

            #region Private methods
            #region CleanUpWriter()
            /// <summary>
            /// Cleans up writer.
            /// </summary>
            private void CleanUpWriter()
            {
                if (this.writer != null)
                {
                    this.writer.Close();
                    this.writer = null;
                }

                if (this.writerStream != null)
                {
                    this.writerStream.Close();
                    this.writerStream = null;
                }
            }
            #endregion

            #region StartProcess()
            /// <summary>
            /// Starts the process.
            /// </summary>
            private void StartProcess()
            {
                logger.Debug(
                    "Starting process [{0}] in working directory [{1}] with arguments [{2}]",
                    process.StartInfo.FileName,
                    process.StartInfo.WorkingDirectory,
                    this.processInfo.Arguments);
                process.OutputDataReceived += StandardOutputHandler;
                process.ErrorDataReceived += ErrorOutputHandler;
                process.Exited += ExitedHandler;
                process.EnableRaisingEvents = true;
                supervisingThread = Thread.CurrentThread;

                var filename = Path.Combine(
                    process.StartInfo.WorkingDirectory,
                    process.StartInfo.FileName);
                try
                {
                    var isNewProcess = process.Start();
                    if (!isNewProcess)
                    {
                        logger.Warn("Reusing existing process...");
                    }

                    // avoid useless setting of the default
                    if (processInfo.Priority != Process.GetCurrentProcess().PriorityClass)
                    {
                        try
                        {
                            logger.Debug(
                                "Setting PriorityClass on [{0}] to {1}",
                                filename,
                                processInfo.Priority);
                            process.PriorityClass = processInfo.Priority;
                        }
                        catch (Exception ex)
                        {
                            if (!process.HasExited)
                            {
                                logger.Info(
                                    "Unable to set PriorityClass on [{0}]: {1}",
                                    filename,
                                    ex);
                            }
                        }
                    }
                    else
                    {
                        logger.Debug(
                            "Not setting PriorityClass on [{0}] to default {1}",
                            filename,
                            processInfo.Priority);
                    }
                }
                catch (Win32Exception e)
                {
                    var msg = string.Format(
                        CultureInfo.CurrentCulture,
                        "Unable to execute file [{0}]. The file may not exist or may not be executable.",
                        filename);
                    throw new IOException(msg, e);
                }

                WriteToStandardInput();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }
            #endregion

            #region ExitedHandler
            /// <summary>
            /// Handles an exit response from the process.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
            private void ExitedHandler(object sender, EventArgs e)
            {
                logger.Debug(
                    "[{0}-{2} {1}] process exited event received",
                    this.projectName,
                    this.processInfo.FileName,
                    this.itemId);
                processExited.Set();
            }
            #endregion

            #region Kill()
            /// <summary>
            /// Kills this instance.
            /// </summary>
            private void Kill()
            {
                const int waitForKilledProcessTimeout = 10000;

                logger.Debug(
                    "Sending kill to process {0} and waiting {1} seconds for it to exit.",
                    process.Id,
                    waitForKilledProcessTimeout / 1000);
                CancelEventsAndWait();
                try
                {
                    KillByPId(this.fileSystem, process.Id);
                    if (!process.WaitForExit(waitForKilledProcessTimeout))
                        throw new CruiseControlException(
                            string.Format(CultureInfo.CurrentCulture, @"The killed process {0} did not terminate within the allotted timeout period {1}.  The process or one of its child processes may not have died.  This may create problems when trying to re-execute the process.  It may be necessary to reboot the server to recover.",
                                process.Id,
                                waitForKilledProcessTimeout));
                    logger.Warn(
                        "The process has been killed: {0}",
                        process.Id);
                }
                catch (InvalidOperationException)
                {
                    logger.Warn(
                        "Process has already exited before getting killed: {0}",
                        process.Id);
                }
            }
            #endregion
            #endregion

            #region CancelEventsAndWait()
            /// <summary>
            /// Cancels the events and wait.
            /// </summary>
            private void CancelEventsAndWait()
            {
                process.EnableRaisingEvents = false;
                process.Exited -= ExitedHandler;

                process.CancelErrorRead();
                process.CancelOutputRead();
                WaitHandle.WaitAll(new WaitHandle[]
                                               {
                                                   errorStreamClosed, 
                                                   outputStreamClosed
                                               }, 1000, true);
            }
            #endregion

            #region WriteToStandardInput()
            /// <summary>
            /// Writes to standard input.
            /// </summary>
            private void WriteToStandardInput()
            {
                if (process.StartInfo.RedirectStandardInput)
                {
                    process.StandardInput.Write(processInfo.StandardInputContent);
                    process.StandardInput.Flush();
                    process.StandardInput.Close();
                }
            }
            #endregion

            #region StandardOutputHandler()
            /// <summary>
            /// Handles output from standard out.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="outLine">The <see cref="System.Diagnostics.DataReceivedEventArgs"/> instance containing the event data.</param>
            private void StandardOutputHandler(object sender, DataReceivedEventArgs outLine)
            {
                try
                {
                    CollectOutput(outLine.Data, "O", outputStreamClosed, "standard-output");

                    if (!string.IsNullOrEmpty(outLine.Data))
                    {
                        OnProcessOutput(
                            new ProcessOutputEventArgs(ProcessOutputType.StandardOutput, outLine.Data));
                    }
                }
                catch (Exception e)
                {
                    var message = string.Format(
                        CultureInfo.CurrentCulture,
                        "[{0} {1}] Exception while collecting standard output",
                        projectName,
                        processInfo.FileName);
                    logger.ErrorException(message, e);
                    supervisingThread.Interrupt();
                }
            }
            #endregion

            #region ErrorOutputHandler()
            /// <summary>
            /// Handles output from standard error.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="outLine">The <see cref="System.Diagnostics.DataReceivedEventArgs"/> instance containing the event data.</param>
            private void ErrorOutputHandler(object sender, DataReceivedEventArgs outLine)
            {
                try
                {
                    CollectOutput(outLine.Data, "E", errorStreamClosed, "standard-error");

                    if (!string.IsNullOrEmpty(outLine.Data))
                    {
                        OnProcessOutput(
                            new ProcessOutputEventArgs(ProcessOutputType.ErrorOutput, outLine.Data));
                    }
                }
                catch (Exception e)
                {
                    var message = string.Format(
                        CultureInfo.CurrentCulture,
                        "[{0} {1}] Exception while collecting error output",
                        projectName,
                        processInfo.FileName);
                    logger.ErrorException(message, e);
                    supervisingThread.Interrupt();
                }
            }
            #endregion

            #region CollectOutput()
            /// <summary>
            /// Collects the output.
            /// </summary>
            /// <param name="output">The output.</param>
            /// <param name="dataType">Type of the data.</param>
            /// <param name="streamReadComplete">The stream read complete.</param>
            /// <param name="streamLabel">The stream label.</param>
            private void CollectOutput(string output, string dataType, EventWaitHandle streamReadComplete, string streamLabel)
            {
                if (output == null)
                {
                    logger.Debug(
                        "[{0} {1}] {2} stream closed -- null received in event",
                        projectName,
                        processInfo.FileName,
                        streamLabel);
                    streamReadComplete.Set();
                    return;
                }

                this.writer.WriteLine(dataType + output);
                logger.Debug("[{0} {1}] {2}", projectName, processInfo.FileName, output);
            }
            #endregion
        }
        #endregion
        #endregion
    }
}
