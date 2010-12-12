namespace ThoughtWorks.CruiseControl.Core.Tasks.Conditions
{
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;

    /// <title>File Exists Condition</title>
    /// <version>1.6</version>
    /// <summary>
    /// Checks if a file exists.
    /// </summary>
    /// <example>
    /// <code title="Basic example">
    /// <![CDATA[
    /// <fileExistsCondition file="readme.txt" />
    /// ]]>
    /// </code>
    /// <code title="Example in context">
    /// <![CDATA[
    /// <conditional>
    /// <conditions>
    /// <fileExistsCondition>
    /// <file>readme.txt</file>
    /// </fileExistsCondition>
    /// </conditions>
    /// <tasks>
    /// <!-- Tasks to perform if condition passed -->
    /// </tasks>
    /// <elseTasks>
    /// <!-- Tasks to perform if condition failed -->
    /// </elseTasks>
    /// </conditional>
    /// ]]>
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>
    /// This condition has been kindly supplied by Lasse Sjørup. The original project is available from
    /// <link>http://ccnetconditional.codeplex.com/</link>.
    /// </para>
    /// </remarks>
    [ReflectorType("fileExistsCondition")]
    public class FileExistsTaskCondition
        : ConditionBase
    {
        #region Public properties
        #region FileName
        /// <summary>
        /// The file to check for.
        /// </summary>
        /// <version>1.6</version>
        /// <default>n/a</default>
        /// <remarks>
        /// If the file is relative then it will be relative to the working directory.
        /// </remarks>
        [ReflectorProperty("file", Required = true)]
        public string FileName { get; set; }
        #endregion

        #region FileSystem
        /// <summary>
        /// Gets or sets the file system.
        /// </summary>
        /// <value>The file system.</value>
        public IFileSystem FileSystem { get; set; }
        #endregion
        #endregion

        #region Protected methods
        #region Evaluate()
        /// <summary>
        /// Performs the actual evaluation.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>
        /// <c>true</c> if the condition is true; <c>false</c> otherwise.
        /// </returns>
        protected override bool Evaluate(IIntegrationResult result)
        {
            var fileName = result.BaseFromWorkingDirectory(this.FileName);
            this.LogDescriptionOrMessage("Checking for file '" + fileName + "'");
            var fileSystem = this.FileSystem ?? new SystemIoFileSystem();
            var exists = fileSystem.FileExists(fileName);
            return exists;
        }
        #endregion
        #endregion
    }
}
