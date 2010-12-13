namespace CruiseControl.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Markup;

    /// <summary>
    /// A project.
    /// </summary>
    [ContentProperty("Tasks")]
    public class Project
        : ServerItem
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class.
        /// </summary>
        public Project()
        {
            this.Tasks = new List<Task>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="tasks">The tasks.</param>
        public Project(string name, params Task[] tasks)
            : base(name)
        {
            this.Tasks = new List<Task>(tasks);
        }
        #endregion

        #region Public properties
        #region Tasks
        /// <summary>
        /// Gets or sets the tasks.
        /// </summary>
        /// <value>The tasks.</value>
        public IList<Task> Tasks { get; private set; }
        #endregion

        #region Host
        /// <summary>
        /// Gets or sets the host for the project.
        /// </summary>
        /// <value>The host.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ServerItem Host { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region Trigger()
        /// <summary>
        /// Triggers an integration.
        /// </summary>
        public virtual void Trigger()
        {
            var integrate = this.Host == null;
            var context = new IntegrationContext(this);
            if (!integrate)
            {
                this.Host.AskToIntegrate(context);

                // TODO: Make the timeout period configurable
                integrate = context.Wait(TimeSpan.FromDays(1));
            }

            if (integrate)
            {
                this.Integrate(context);
            }
        }
        #endregion

        #region Integrate()
        /// <summary>
        /// Performs an integration.
        /// </summary>
        /// <param name="context">The context.</param>
        public virtual void Integrate(IntegrationContext context)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region AskToIntegrate()
        /// <summary>
        /// Asks if an item can integrate.
        /// </summary>
        /// <param name="context">The context to use.</param>
        public override void AskToIntegrate(IntegrationContext context)
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion
    }
}
