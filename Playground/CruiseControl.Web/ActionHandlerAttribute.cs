namespace CruiseControl.Web
{
    using System;
    using System.ComponentModel.Composition;

    /// <summary>
    /// Defines a class as being an action handler.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ActionHandlerAttribute
        : ExportAttribute
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionHandlerAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="targets">The targets.</param>
        public ActionHandlerAttribute(string name, ActionHandlerTargets targets)
        {
            this.Name = name;
            this.Targets = targets;
        }
        #endregion

        #region Public properties
        #region Name
        /// <summary>
        /// Gets the name of the action.
        /// </summary>
        /// <value>The action name.</value>
        public string Name { get; private set; }
        #endregion

        #region Targets
        /// <summary>
        /// Gets or sets the target levels.
        /// </summary>
        /// <value>The targets.</value>
        public ActionHandlerTargets Targets { get; private set; }
        #endregion
        #endregion
    }
}