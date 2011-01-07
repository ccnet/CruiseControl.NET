namespace CruiseControl.Core.Tasks.Conditions
{
    using System;
    using System.ComponentModel;
    using System.Windows.Markup;
    using CruiseControl.Core.Interfaces;
    using Ninject;

    /// <summary>
    /// Checks if a file exists.
    /// </summary>
    [ContentProperty("FileName")]
    public class FileExists
        : TaskCondition
    {
        #region Public properties
        #region FileName
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName { get; set; }
        #endregion

        #region FileSystem
        /// <summary>
        /// Gets or sets the file system.
        /// </summary>
        /// <value>
        /// The file system.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Inject]
        public IFileSystem FileSystem { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region Validate()
        /// <summary>
        /// Validates this condition.
        /// </summary>
        /// <param name="validationLog">The validation log.</param>
        public override void Validate(IValidationLog validationLog)
        {
            base.Validate(validationLog);
            if (string.IsNullOrEmpty(this.FileName))
            {
                validationLog.AddError("FileName has not been set");
            }
        }
        #endregion

        #region Evaluate()
        /// <summary>
        /// Evaluates whether this condition is valid.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///   <c>true</c> if this instance is valid; <c>false</c> otherwise.
        /// </returns>
        public override bool Evaluate(TaskExecutionContext context)
        {
            var fileFound = this.FileSystem.CheckIfFileExists(this.FileName);
            return fileFound;
        }
        #endregion
        #endregion
    }
}
