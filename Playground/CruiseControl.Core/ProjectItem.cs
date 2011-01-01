namespace CruiseControl.Core
{
    using System.ComponentModel;

    /// <summary>
    /// Signifies that an item is within a project.
    /// </summary>
    public abstract class ProjectItem
    {
        #region Private fields
        private Project project;
        #endregion

        #region Public properties
        #region Name
        /// <summary>
        /// Gets or sets the name of the source control.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        #endregion

        #region UniversalName
        /// <summary>
        /// Gets the universal name of this item.
        /// </summary>
        /// <value>
        /// The universal name.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual string UniversalName
        {
            get
            {
                var name = (this.Project == null ? "urn:ccnet:" : this.Project.UniversalName) +
                           ":" + this.NameOrType;
                return name;
            }
        }
        #endregion

        #region Project
        /// <summary>
        /// Gets or sets the project.
        /// </summary>
        /// <value>
        /// The project.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Project Project
        {
            get { return this.project; }
            set
            {
                this.project = value;
                this.OnProjectChanged();
            }
        }
        #endregion
        #endregion

        #region Protected methods
        #region NameOrType
        /// <summary>
        /// Gets the name or type.
        /// </summary>
        /// <value>
        /// The name or type of this task.
        /// </value>
        protected string NameOrType
        {
            get { return this.Name ?? this.GetType().Name; }
        }
        #endregion
        #endregion

        #region Protected methods
        #region OnProjectChanged()
        /// <summary>
        /// Called when the project has been changed.
        /// </summary>
        protected virtual void OnProjectChanged()
        {
        }
        #endregion
        #endregion
    }
}
