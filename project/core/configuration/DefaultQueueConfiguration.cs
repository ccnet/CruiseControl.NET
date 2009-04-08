using Exortech.NetReflector;
using System;

namespace ThoughtWorks.CruiseControl.Core.Config
{
    /// <summary>
    /// Provides a default implementation of the queue functionality.
    /// </summary>
    [ReflectorType("queue")]
    public class DefaultQueueConfiguration
        : IQueueConfiguration, IConfigurationValidation
    {
        private string name;
        private QueueDuplicateHandlingMode handlingMode = QueueDuplicateHandlingMode.UseFirst;
        private string lockQueueNames;

        /// <summary>
        /// Default constructor - needed for NetReflector.
        /// </summary>
        public DefaultQueueConfiguration() { }

        /// <summary>
        /// Start a new queue configuration with a name.
        /// </summary>
        /// <param name="name"></param>
        public DefaultQueueConfiguration(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// The name of the queue.
        /// </summary>
        [ReflectorProperty("name", Required = true)]
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Defines how duplicates should be handled.
        /// </summary>
        [ReflectorProperty("duplicates", Required = false)]
        public virtual QueueDuplicateHandlingMode HandlingMode
        {
            get { return handlingMode; }
            set { handlingMode = value; }
        }

        [ReflectorProperty("lockqueues", Required = false)]
        public virtual string LockQueueNames
        {
            get { return lockQueueNames; }
            set { lockQueueNames = value; }
        }

        /// <summary>
        /// Checks the internal validation of the item.
        /// </summary>
        /// <param name="configuration">The entire configuration.</param>
        /// <param name="parent">The parent item for the item being validated.</param>
        public virtual void Validate(IConfiguration configuration, object parent)
        {
            // Ensure that the queue has at least one project in it
            bool queueFound = false;
            foreach (IProject projectDef in configuration.Projects)
            {
                if (string.Equals(this.Name, projectDef.QueueName, StringComparison.InvariantCulture))
                {
                    queueFound = true;
                    break;
                }
            }
            if (!queueFound)
            {
                throw new ConfigurationException(
                    string.Format("An unused queue definition has been found: name '{0}'", this.Name));
            }
        }
    }
}
