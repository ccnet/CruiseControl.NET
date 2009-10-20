//-----------------------------------------------------------------------
// <copyright file="DupFinderTask.cs" company="Craig Sutherland">
//     Copyright (c) 2009 CruiseControl.NET. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;
    using System.Xml;
    using System.Collections.Generic;

    /// <summary>
    /// Check for duplicates using dupfinder (http://duplicatefinder.codeplex.com/).
    /// </summary>
    [ReflectorType("dupfinder")]
    public class DupFinderTask
        : BaseExecutableTask
    {
        #region Private consts
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private constant")]
        private const string DefaultExecutable = "dupfinder";
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
        }
        #endregion

        #region Public properties
        #region Executable
        /// <summary>
        /// Gets or sets the executable to use.
        /// </summary>
        /// <value>The executable.</value>
        [ReflectorProperty("executable", Required = false)]
        public string Executable { get; set; }
        #endregion

        #region InputDir
        /// <summary>
        /// Gets or sets the input directory to scan.
        /// </summary>
        [ReflectorProperty("inputDir", Required = true)]
        public string InputDir { get; set; }
        #endregion

        #region FileMask
        /// <summary>
        /// Gets or sets the file mask to use.
        /// </summary>
        [ReflectorProperty("fileMask", Required = true)]
        public string FileMask { get; set; }
        #endregion

        #region Focus
        /// <summary>
        /// Gets or sets the name of the file to focus on.
        /// </summary>
        [ReflectorProperty("focus", Required = false)]
        public string Focus { get; set; }
        #endregion

        #region TimeOut
        /// <summary>
        /// Gets or sets the time-out period in seconds.
        /// </summary>
        [ReflectorProperty("timeout", Required = false)]
        public int TimeOut { get; set; }
        #endregion

        #region Threshold
        /// <summary>
        /// Gets or sets the threshold is the number of consecutive lines that have to be the same before it is considered a duplicate.
        /// </summary>
        [ReflectorProperty("threshold", Required = false)]
        public int Threshold { get; set; }
        #endregion

        #region Width
        /// <summary>
        /// Gets or sets the first line of a duplicate must contain at least this many non-white-space characters.
        /// </summary>
        [ReflectorProperty("width", Required = false)]
        public int Width { get; set; }
        #endregion

        #region Recurse
        /// <summary>
        /// Gets or sets a value indicating whether to find files that match the filemask in current directory and subdirectories.
        /// </summary>
        /// <value><c>true</c> if recurse; otherwise, <c>false</c>.</value>
        [ReflectorProperty("recurse", Required = false)]
        public bool Recurse { get; set; }
        #endregion

        #region ShortenFileNames
        /// <summary>
        /// Gets or sets a value indicating whether to shorten filenames.
        /// </summary>
        /// <value><c>true</c> if shortening is required; otherwise, <c>false</c>.</value>
        [ReflectorProperty("shortenNames", Required = false)]
        public bool ShortenFileNames { get; set; }
        #endregion

        #region IncludeCode
        /// <summary>
        /// Gets or sets a value indicating whether to include the code that has been duplicated.
        /// </summary>
        /// <value><c>true</c> if code is to be included; otherwise, <c>false</c>.</value>
        [ReflectorProperty("includeCode", Required = false)]
        public bool IncludeCode { get; set; }
        #endregion

        #region LinesToExclude
        /// <summary>
        /// Gets or sets the lines to exclude.
        /// </summary>
        [ReflectorProperty("excludeLines", Required = false)]
        public string[] LinesToExclude { get; set; }
        #endregion

        #region FilesToExclude
        /// <summary>
        /// Gets or sets the files to exclude.
        /// </summary>
        [ReflectorProperty("excludeFiles", Required = false)]
        public string[] FilesToExclude { get; set; }
        #endregion

        #region ioSystem
        /// <summary>
        /// Gets or sets the IO system to use.
        /// </summary>
        /// <value>The IO system.</value>
        public IFileSystem ioSystem { get; set; }
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
            var processResult = TryToRun(CreateProcessInfo(result), result);

            if (this.ShortenFileNames || this.IncludeCode)
            {
                // Load the results into an XML document
                var document = new XmlDocument();
                document.LoadXml(processResult.StandardOutput);

                if (this.IncludeCode)
                {
                    this.logger.Info("Including duplicate code lines");
                    this.ioSystem = this.ioSystem ?? new SystemIoFileSystem();
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
            return !processResult.Failed;
        }
        #endregion

        #region GetProcessFilename()
        /// <summary>
        /// Retrieve the executable to use.
        /// </summary>
        /// <returns>The filename of the process to execute.</returns>
        protected override string GetProcessFilename()
        {
            var path = this.QuoteSpaces(this.executable);
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
            buffer.AppendArgument("-t" + this.Threshold.ToString());
            buffer.AppendArgument("-w" + this.Width.ToString());
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

                using (var inputFile = this.ioSystem.OpenInputStream(file.Key))
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
                            var firstLine = Convert.ToInt32(node.GetAttribute("LineNumber"));
                            var blockLength = Convert.ToInt32(parent.GetAttribute("Length"));
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
            var firstLine = Convert.ToInt32(firstNode.GetAttribute("LineNumber"));
            var secondLine = Convert.ToInt32(secondNode.GetAttribute("LineNumber"));
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
