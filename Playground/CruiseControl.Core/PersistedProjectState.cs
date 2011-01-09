namespace CruiseControl.Core
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Xaml;

    /// <summary>
    /// Defines the state that gets persisted for a project.
    /// </summary>
    public class PersistedProjectState
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PersistedProjectState"/> class.
        /// </summary>
        public PersistedProjectState()
        {
            this.Values = new List<ProjectValue>();
        }
        #endregion

        #region Public properties
        #region LastIntegration
        /// <summary>
        /// Gets or sets the last integration.
        /// </summary>
        /// <value>
        /// The last integration.
        /// </value>
        [DefaultValue(null)]
        public IntegrationSummary LastIntegration { get; set; }
        #endregion

        #region Values
        /// <summary>
        /// Gets the values.
        /// </summary>
        public IList<ProjectValue> Values { get; private set; }
        #endregion
        #endregion

        #region Public methods
        #region SetValue()
        /// <summary>
        /// Sets a value in the state.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void SetValue(ProjectItem item, string name, string value)
        {
            var localValue = this.FindValue(item, name);
            if (localValue == null)
            {
                if (value != null)
                {
                    this.Values.Add(new ProjectValue(item.UniversalName, name, value));
                }
            }
            else
            {
                if (value == null)
                {
                    this.Values.Remove(localValue);
                }
                else
                {
                    localValue.Value = value;
                }
            }
        }
        #endregion

        #region GetValue()
        /// <summary>
        /// Gets a value from the state.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="name">The name.</param>
        /// <returns>
        /// The value if it exists; <c>null</c> otherwise.
        /// </returns>
        public string GetValue(ProjectItem item, string name)
        {
            var value = this.FindValue(item, name);
            return value == null ? null : value.Value;
        }
        #endregion

        #region Save()
        /// <summary>
        /// Saves to the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void Save(Stream stream)
        {
            XamlServices.Save(stream, this);
        }
        #endregion
        #endregion

        #region Private methods
        #region FindValue()
        /// <summary>
        /// Attempts to find a value.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="name">The name.</param>
        /// <returns>
        /// The <see cref="ProjectValue"/> if found; <c>null</c> otherwise.
        /// </returns>
        private ProjectValue FindValue(ProjectItem item, string name)
        {
            var value = this.Values
                .SingleOrDefault(v => v.Owner == item.UniversalName && v.Name == name);
            return value;
        }
        #endregion
        #endregion
    }
}
