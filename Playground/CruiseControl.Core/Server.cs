namespace CruiseControl.Core
{
    using System.Collections.Generic;
    using System.Windows.Markup;
    using System;
    using System.ComponentModel;

    [ContentProperty("Children")]
    public class Server
        : IServerItemContainer
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Server"/> class.
        /// </summary>
        public Server()
        {
            this.Children = new List<ServerItem>();
        }
        #endregion

        #region Public properties
        #region Version
        /// <summary>
        /// Gets or sets the configuration version.
        /// </summary>
        /// <value>The version.</value>
        [TypeConverter(typeof(VersionTypeConverter))]
        public Version Version { get; set; }
        #endregion

        #region Children
        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>The children.</value>
        public IList<ServerItem> Children { get; private set; }
        #endregion
        #endregion
    }
}
