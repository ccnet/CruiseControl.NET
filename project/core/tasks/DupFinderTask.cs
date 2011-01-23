//-----------------------------------------------------------------------
// <copyright file="DupFinderTask.cs" company="Craig Sutherland">
//     Copyright (c) 2009 CruiseControl.NET. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Globalization;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Xml;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;

    /// <summary>
    /// <para>
    /// Check for duplicates using dupfinder (http://duplicatefinder.codeplex.com/).
    /// </para>
    /// </summary>
    /// <title>Duplicate Finder Task</title>
    /// <version>1.5</version>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;dupfinder&gt;
    /// &lt;fileMask&gt;*.cs&lt;/fileMask&gt;
    /// &lt;inputDir&gt;Code&lt;/inputDir&gt;
    /// &lt;/dupfinder&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;dupfinder&gt;
    /// &lt;dynamicValues /&gt;
    /// &lt;fileMask&gt;*.cs&lt;/fileMask&gt;
    /// &lt;includeCode&gt;False&lt;/includeCode&gt;
    /// &lt;inputDir&gt;Code&lt;/inputDir&gt;
    /// &lt;recurse&gt;False&lt;/recurse&gt;
    /// &lt;shortenNames&gt;False&lt;/shortenNames&gt;
    /// &lt;threshold&gt;5&lt;/threshold&gt;
    /// &lt;timeout&gt;600&lt;/timeout&gt;
    /// &lt;width&gt;2&lt;/width&gt;
    /// &lt;/dupfinder&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <heading>Extended Functionality</heading>
    /// <para>
    /// This task offers some extended functionality over what the base dupfinder executable offers. This extended
    /// functionality is primarily intended to add extra value to the web dashboard display. The extended options are:
    /// </para>
    /// <para>
    /// <b>&lt;shortenNames&gt;</b>: This will remove the &lt;inputDir&gt; value from the file names. This means the
    /// filenames only contain the relative path to the file, which makes it easier to see where the file is.
    /// </para>
    /// <para>
    /// <b>&lt;includeCode&gt;</b>: This will include the lines of code that were duplicated into the output. These can
    /// then be seen in the web dashboard. This meakes it easy to see the code that has been duplicated.
    /// </para>
    /// <para>
    /// These features work by post-processing the XML output from dupfinder. That is, once dupfinder has finished, the
    /// task loads the XML file, finds all the elements that need changing and changes them as required. For the code
    /// inclusion, it will also open the relevant code files and extract the lines of code as needed.
    /// </para>
    /// </remarks>
    [ReflectorType("dupfinder")]
    public class DupFinderTask
        : BaseExecutableTask
    {
        #region Private consts
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private constant")]
        private const string DefaultExecutable = "dupfinder";

        /// <summary>Default priority class</summary>
        private const ProcessPriorityClass DefaultPriority = ProcessPriorityClass.Normal;
        #endregion

        #region Private fields
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private field")]
        private string executable;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DupFinderTask"/> class.
        /// </summary>
        public DupFinderTask()
            : this(new ProcessExecutor())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DupFinderTask"/> class.
        /// </summary>
        /// <param name="executor">The executor to use.</param>
        public DupFinderTask(ProcessExecutor executor)
        {
            this.executor = executor;
            this.TimeOut = 600;
            this.Threshold = 5;
            this.Width = 2;
            this.Priority = ProcessPriorityClass.Normal;
        }
        #endregion

        #region Public properties
        #region Executable
        /// <summary>
        /// The executable to use.
        /// </summary>
        /// <version>1.5</version>
        /// <default>dupfinder</default>
        [ReflectorProperty("executable", Required = false)]
        public string Executable { get; set; }
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

        #region InputDir
        /// <summary>
        /// The input directory to scan. If relative, this will be relative to the project working directory.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("inputDir", Required = true)]
        public string InputDir { get; set; }
        #endregion

        #region FileMask
        /// <summary>
        /// The file mask to use.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("fileMask", Required = true)]
        public string FileMask { get; set; }
        #endregion

        #region Focus
        /// <summary>
        /// The name of the file to focus on.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("focus", Required = false)]
        public string Focus { get; set; }
        #endregion

        #region TimeOut
        /// <summary>
        /// The time-out period in seconds.
        /// </summary>
        /// <version>1.5</version>
        /// <default>600</default>
        [ReflectorProperty("timeout", Required = false)]
        public int TimeOut { get; set; }
        #endregion

        #region Threshold
        /// <summary>
        /// The threshold is the number of consecutive lines that have to be the same before it is considered a
        /// duplicate.
        /// </summary>
        /// <version>1.5</version>
        /// <default>5</default>
        [ReflectorProperty("threshold", Required = false)]
        public int Threshold { get; set; }
        #endregion

        #region Width
        /// <summary>
        /// The first line of a duplicate must contain at least this many non-white-space characters.
        /// </summary>
        /// <version>1.5</version>
        /// <default>2</default>
        [ReflectorProperty("width", Required = false)]
        public int Width { get; set; }
        #endregion

        #region Recurse
        /// <summary>
        /// To find files that match the filemask in current directory and subdirectories.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        [ReflectorProperty("recurse", Required = false)]
        public bool Recurse { get; set; }
        #endregion

        #region ShortenFileNames
        /// <summary>
        /// Whether to shorten filenames.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        [ReflectorProperty("shortenNames", Required = false)]
        public bool ShortenFileNames { get; set; }
        #endregion

        #region IncludeCode
        /// <summary>
        /// Whether to include the code that has been duplicated.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        [ReflectorProperty("includeCode", Required = false)]
        public bool IncludeCode { get; set; }
        #endregion

        #region LinesToExclude
        /// <summary>
        /// The lines to exclude.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("excludeLines", Required = false)]
        public string[] LinesToExclude { get; set; }
        #endregion

        #region FilesToExclude
        /// <summary>
        /// The files to exclude.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("excludeFiles", Required = false)]
        public string[] FilesToExclude { get; set; }
        #endregion

        #region logger
        /// <summary>
        /// Gets or sets the logger to use.
        /// </summary>
        /// <value>The logger.</value>
        public ILogger logger { get; private set; }
        #endregion
        #endregion

        #region Protected methods
        #region Execute()
        /// <summary>
        /// Run the task.
        /// </summary>
        /// <param name="result">The result to use.</param>
        /// <returns>
        /// True if the task was successful, false otherwise.
        /// </returns>
        protected override bool Execute(IIntegrationResult result)
        {
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description : "Executing DupFinder");
            this.logger = this.logger ?? new DefaultLogger();

            this.executable = string.IsNullOrEmpty(this.Executable) ? DefaultExecutable : this.Executable;
            if (!Path.IsPathRooted(this.executable))
            {
                this.executable = result.BaseFromWorkingDirectory(this.executable);
                this.logger.Debug("Executable changed to " + this.executable);
            }

            // Run the executable
            this.logger.Info("Executing DupFinder");
            var info = this.CreateProcessInfo(result);
            var processResult = this.TryToRun(info, result);
            if (processResult.TimedOut)
            {
                result.AddTaskResult(MakeTimeoutBuildResult(info));
            }

            if (this.ShortenFileNames || this.IncludeCode)
            {
                // Load the results into an XML document
                var document = new XmlDocument();
                document.LoadXml(processResult.StandardOutput);

                if (this.IncludeCode)
                {
                    this.logger.Info("Including duplicate code lines");
                    this.ImportCode(document);
                }

                if (this.ShortenFileNames)
                {
                    this.logger.Info("Shortening filenames");
                    this.RemoveInputDir(document);
                }

                // Generate a new result
                processResult = new ProcessResult(
                    document.OuterXml,
                    processResult.StandardError,
                    processResult.ExitCode,
                    processResult.TimedOut,
                    processResult.Failed);
            }

            // Add the result
            result.AddTaskResult(new ProcessTaskResult(processResult, false));
            return processResult.Succeeded;
        }
        #endregion

        #region GetProcessFilename()
        /// <summary>
        /// Retrieve the executable to use.
        /// </summary>
        /// <returns>The filename of the process to execute.</returns>
        protected override string GetProcessFilename()
        {
            var path = this.executable ?? this.Executable;
            path = string.IsNullOrEmpty(path) ? DefaultExecutable : path;
            path = this.QuoteSpaces(path);
            return path;
        }
        #endregion

        #region GetProcessBaseDirectory()
        /// <summary>
        /// Retrieve the base directory.
        /// </summary>
        /// <param name="result">The result to use.</param>
        /// <returns>The base directory to use.</returns>
        protected override string GetProcessBaseDirectory(IIntegrationResult result)
        {
            var path = this.QuoteSpaces(this.InputDir);
            return path;
        }
        #endregion

        #region GetProcessTimeout()
        /// <summary>
        /// Get the time-out period.
        /// </summary>
        /// <returns>The time-out period in milliseconds.</returns>
        protected override int GetProcessTimeout()
        {
            return this.TimeOut * 1000;
        }
        #endregion

        #region GetProcessArguments()
        /// <summary>
        /// Retrieve the arguments
        /// </summary>
        /// <param name="result">The result to use.</param>
        /// <returns>The arguments to pass to the process.</returns>
        protected override string GetProcessArguments(IIntegrationResult result)
        {
            var buffer = new ProcessArgumentBuilder();
            buffer.AppendIf(this.Recurse, "-r");
            buffer.AppendArgument("-t" + this.Threshold.ToString(CultureInfo.CurrentCulture));
            buffer.AppendArgument("-w" + this.Width.ToString(CultureInfo.CurrentCulture));
            buffer.AppendArgument("-oConsole");

            // Add the focus
            if (!string.IsNullOrEmpty(this.Focus))
            {
                buffer.AppendArgument("-f" + this.QuoteSpaces(this.Focus));
            }

            // Add the lines to exclude
            foreach (var line in this.LinesToExclude ?? new string[0])
            {
                buffer.AppendArgument("-x" + this.QuoteSpaces(line));
            }

            // Add the lines to exclude
            foreach (var line in this.FilesToExclude ?? new string[0])
            {
                buffer.AppendArgument("-e" + this.QuoteSpaces(line));
            }

            buffer.AppendArgument(this.FileMask);
            return buffer.ToString();
        }
        #endregion

        #region GetProcessPriorityClass()
        /// <summary>
        /// Gets the requested priority class value for this Task.
        /// </summary>
        protected override ProcessPriorityClass GetProcessPriorityClass()
        {
            return this.Priority;
        }
        #endregion

        #region RemoveInputDir()
        /// <summary>
        /// Removes the input directory from the filenames.
        /// </summary>
        /// <param name="document">The document containing the data.</param>
        protected void RemoveInputDir(XmlDocument document)
        {
            var duplicateNodes = document.SelectNodes("//Duplicate");
            var length = this.InputDir.Length + 1;
            foreach (XmlElement duplicate in duplicateNodes)
            {
                var filename = duplicate.GetAttribute("FileName");
                if (!string.IsNullOrEmpty(filename) && filename.StartsWith(this.InputDir))
                {
                    // Store the original, just in case it is needed
                    duplicate.SetAttribute("OriginalFileName", filename);
                    duplicate.SetAttribute("FileName", filename.Substring(length));
                }
            }
        }
        #endregion

        #region ImportCode()
        /// <summary>
        /// Imports the duplicated code lines.
        /// </summary>
        /// <param name="document">The document to use.</param>
        protected void ImportCode(XmlDocument document)
        {
            var duplicatesNodes = document.SelectNodes("//Duplicates");
            var fileNames = new Dictionary<string, List<XmlElement>>();

            // Get all the file names so each file only needs to be loaded once
            this.logger.Debug("Generating file list");
            foreach (XmlElement node in duplicatesNodes)
            {
                // Check to see if any of the filenames have already been added
                var isFound = false;
                foreach (XmlElement duplicate in node.ChildNodes)
                {
                    var fileName = duplicate.GetAttribute("FileName");
                    if (fileNames.ContainsKey(fileName))
                    {
                        fileNames[fileName].Add(duplicate);
                        isFound = true;
                        break;
                    }
                }

                // Add the first filename if the name has  not already been added
                if (!isFound)
                {
                    var first = node.FirstChild as XmlElement;
                    var elementList = new List<XmlElement>();
                    elementList.Add(first);
                    fileNames.Add(first.GetAttribute("FileName"), elementList);
                }
            }

            // Load the duplicated code
            this.logger.Debug("Importing duplicate code lines");
            foreach (var file in fileNames)
            {
                // Sort all the lines so the lines can be processed in order
                file.Value.Sort(this.CompareFileNodes);

                using (var inputFile = this.IOSystemActual.OpenInputStream(file.Key))
                {
                    using (var reader = new StreamReader(inputFile))
                    {
                        var lines = new Dictionary<int, string>();
                        var lineNumber = 1;
                        string currentLine = null;
                        foreach (var node in file.Value)
                        {
                            // Calculate the lines to read
                            var parent = node.ParentNode as XmlElement;
                            var firstLine = Convert.ToInt32(node.GetAttribute("LineNumber"), CultureInfo.CurrentCulture);
                            var blockLength = Convert.ToInt32(parent.GetAttribute("Length"), CultureInfo.CurrentCulture);
                            var lastLine = firstLine + blockLength;

                            // Move to the first line
                            while (lineNumber < firstLine)
                            {
                                currentLine = reader.ReadLine();
                                lineNumber++;
                            }

                            // Read in the lines
                            while (lineNumber <= lastLine)
                            {
                                currentLine = reader.ReadLine();
                                lines.Add(lineNumber, currentLine);
                                lineNumber++;
                            }

                            // Finally, add the lines of code to the XML
                            var codeNode = document.CreateElement("code");
                            parent.AppendChild(codeNode);
                            for (var loop = firstLine; loop <= lastLine; loop++)
                            {
                                var lineNode = document.CreateElement("line");
                                codeNode.AppendChild(lineNode);
                                lineNode.InnerText = lines[loop];
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region CompareFileNodes()
        /// <summary>
        /// Compares two file nodes.
        /// </summary>
        /// <param name="firstNode">The first node.</param>
        /// <param name="secondNode">The second node.</param>
        /// <returns>
        /// Condition Less than 0 firstNode is less than secondNode. 0 firstNode equals secondNode.
        /// Greater than 0 firstNode is greater than secondNode.
        /// </returns>
        protected int CompareFileNodes(XmlElement firstNode, XmlElement secondNode)
        {
            var firstLine = Convert.ToInt32(firstNode.GetAttribute("LineNumber"), CultureInfo.CurrentCulture);
            var secondLine = Convert.ToInt32(secondNode.GetAttribute("LineNumber"), CultureInfo.CurrentCulture);
            return firstLine - secondLine;
        }
        #endregion

        #region QuoteSpaces()
        /// <summary>
        /// Adds quotes to a string if it contains spaces.
        /// </summary>
        /// <param name="value">The string to check.</param>
        /// <returns>The string with quotes if needed.</returns>
        protected string QuoteSpaces(string value)
        {
            if (value.Contains(" ") && !value.StartsWith("\"") && !value.EndsWith("\""))
            {
                return "\"" + value + "\"";
            }
            else
            {
                return value;
            }
        }
        #endregion
        #endregion
    }
}
