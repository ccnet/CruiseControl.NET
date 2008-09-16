using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Config
{
    /// <summary>
    /// Provides a default implementation of the queue functionality.
    /// </summary>
    [ReflectorType("queue")]
    public class DefaultQueueConfiguration
        : IQueueConfiguration
    {
        private string _name;
        private QueueDuplicateHandlingMode _handlingMode = QueueDuplicateHandlingMode.UseFirst;
        private string _lockQueueNames;

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
            _name = name;
        }

        /// <summary>
        /// The name of the queue.
        /// </summary>
        [ReflectorProperty("name", Required = true)]
        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Defines how duplicates should be handled.
        /// </summary>
        [ReflectorProperty("duplicates", Required = false)]
        public virtual QueueDuplicateHandlingMode HandlingMode
        {
            get { return _handlingMode; }
            set { _handlingMode = value; }
        }

        [ReflectorProperty("lockqueues", Required = false)]
        public virtual string LockQueueNames
        {
            get { return _lockQueueNames; }
            set { _lockQueueNames = value; }
        }
    }
}
