namespace CruiseControl.Core.Structure
{
    using System.Collections.Generic;
    using CruiseControl.Core.Interfaces;

    /// <summary>
    /// A project that runs a number of predefined sections based around working with code
    /// from a source code repository.
    /// </summary>
    public class ScmProject
        : Project
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ScmProject"/> class.
        /// </summary>
        public ScmProject()
        {
            this.InitialiseProperties();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScmProject"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ScmProject(string name)
            : base(name)
        {
            this.InitialiseProperties();
        }
        #endregion

        #region Public properties
        #region PreBuild
        /// <summary>
        /// Gets the pre-build tasks.
        /// </summary>
        public IList<Task> PreBuild { get; private set; }
        #endregion

        #region Publishers
        /// <summary>
        /// Gets the post-build publishers.
        /// </summary>
        public IList<Task> Publishers { get; private set; }
        #endregion
        #endregion

        #region Public methods
        #region Validate()
        /// <summary>
        /// Validates this project after it has been loaded.
        /// </summary>
        /// <param name="validationLog"></param>
        public override void Validate(IValidationLog validationLog)
        {
            base.Validate(validationLog);
            foreach (var task in this.PreBuild)
            {
                task.Validate(validationLog);
            }

            foreach (var task in this.Publishers)
            {
                task.Validate(validationLog);
            }
        }
        #endregion
        #endregion

        #region Private methods
        #region InitialiseProperties()
        /// <summary>
        /// Initialises the properties.
        /// </summary>
        private void InitialiseProperties()
        {
            this.PreBuild = new List<Task>();
            this.Publishers = new List<Task>();
        }
        #endregion
        #endregion
    }
}
