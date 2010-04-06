namespace ThoughtWorks.CruiseControl.Core.Tasks.Conditions
{
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;

    /// <title>Folder Exists Condition</title>
    /// <version>1.6</version>
    /// <summary>
    /// Checks if a folder exists.
    /// </summary>
    /// <example>
    /// <code title="Basic example">
    /// <![CDATA[
    /// <folderExistsCondition folder="documentation" />
    /// ]]>
    /// </code>
    /// <code title="Example in context">
    /// <![CDATA[
    /// <conditional>
    /// <conditions>
    /// <folderExistsCondition>
    /// <folder>documentation</folder>
    /// </folderExistsCondition>
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
    /// This condition has been kindly supplied by Lasse Sjorup. The original project is available from
    /// <link>http://ccnetconditional.codeplex.com/</link>.
    /// </para>
    /// </remarks>
    [ReflectorType("folderExistsCondition")]
    public class FolderExistsTaskCondition
        : ConditionBase
    {
        #region Public properties
        #region FolderName
        /// <summary>
        /// The folder to check for.
        /// </summary>
        /// <version>1.6</version>
        /// <default>n/a</default>
        /// <remarks>
        /// If the folder is relative then it will be relative to the working directory.
        /// </remarks>
        [ReflectorProperty("folder", Required = true)]
        public string FolderName { get; set; }
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
            var folderName = result.BaseFromWorkingDirectory(this.FolderName);
            this.LogDescriptionOrMessage("Checking for folder '" + folderName.ToString() + "'");
            var fileSystem = this.FileSystem ?? new SystemIoFileSystem();
            var exists = fileSystem.DirectoryExists(folderName);
            return exists;
        }
        #endregion
        #endregion
    }
}
