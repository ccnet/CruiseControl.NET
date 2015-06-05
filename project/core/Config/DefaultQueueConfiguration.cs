using System;
using System.Collections.Generic;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Config
{
    /// <summary>
    /// Configure the behaviour of the build queues.
    /// </summary>
    /// <title>Queue Configuration Element</title>
    /// <version>1.4.2</version>
    /// <example>
    /// <code title="Full Example">
    /// &lt;queue name="Q1" duplicates="UseFirst" lockqueues="Q2,Q3" /&gt;
    /// </code>
    /// <para>See the notes for additional examples.</para>
    /// </example>
    /// <remarks>
    /// <heading>Duplicate Handling</heading>
    /// <para>
    /// There are different settings that can be used to specify how force build requests should be handled.
    /// </para>
    /// <para>
    /// The default behaviour is to not allow force build requests to update the queue and use the first request that was added.
    /// </para>
    /// <para>
    /// The following example shows how to explicitly configure the default behavior.
    /// </para>
    /// <code>
    /// &lt;cruisecontrol&gt;
    ///   &lt;queue name="Q1" duplicates="UseFirst"/&gt;
    ///   &lt;project name="MyFirstProject" queue="Q1" queuePriority="1"&gt;
    ///     &lt;otherProjectSettings /&gt;
    ///   &lt;/project&gt;
    /// &lt;/cruisecontrol&gt;
    /// </code>
    /// <para>
    /// The following example shows how to configure a queue so that force build requests will replace existing requests of the interval trigger without changing the position of the request in the queue.
    /// </para>
    /// <code>
    /// &lt;cruisecontrol&gt;
    ///   &lt;queue name="Q1" duplicates="ApplyForceBuildsReplace"/&gt;
    ///   &lt;project name="MyFirstProject" queue="Q1" queuePriority="1"&gt;
    ///     &lt;otherProjectSettings /&gt;
    ///   &lt;/project&gt;
    /// &lt;/cruisecontrol&gt;
    /// </code>
    /// <para>
    /// The following example shows how to configure a queue so that force build requests will remove existing non forced requests of the interval trigger and re-add the force build request. This is changing the position of the request in the queue.
    /// Basically : remove the interval (non forced) builds and add the forced builds via the queue priority setting
    /// </para>
    /// <code>
    /// &lt;cruisecontrol&gt;
    ///   &lt;queue name="Q1" duplicates="ApplyForceBuildsReAdd"/&gt;
    ///   &lt;project name="MyFirstProject" queue="Q1" queuePriority="1"&gt;
    ///     &lt;otherProjectSettings /&gt;
    ///   &lt;/project&gt;
    /// &lt;/cruisecontrol&gt;
    /// </code>
    /// <para>
    /// The following example shows how to configure a queue so that force build requests will remove existing non forced requests of the interval trigger and re-add a force build request to the first item in the queue.
    /// </para>
    /// <code>
    /// &lt;cruisecontrol&gt;
    ///   &lt;queue name="Q1" duplicates="ApplyForceBuildsReAddTop"/&gt;
    ///   &lt;project name="MyFirstProject" queue="Q1" queuePriority="1"&gt;
    ///     &lt;otherProjectSettings /&gt;
    ///   &lt;/project&gt;
    /// &lt;/cruisecontrol&gt;
    /// </code>
    /// <heading>Locking</heading>
    /// <para>
    /// The following example shows how to configure two queues, Q1 and Q2, that acquire a lock against each other. That means that while the queue Q1 is building a project the queue Q2 is locked. While Q2 is building Q1 is locked. To specify more than one queue that should be locked use commas to separate the queue names within the lockqueues attribute. Of course the lockqueues attribute can be used together with the duplicates attribute explained above.
    /// </para>
    /// <code>
    /// &lt;cruisecontrol&gt;
    ///   &lt;queue name="Q1" lockqueues="Q2"/&gt;
    ///   &lt;queue name="Q2" lockqueues="Q1"/&gt;
    ///   &lt;project name="MyFirstProject" queue="Q1" queuePriority="1"&gt;
    ///     &lt;otherProjectSettings /&gt;
    ///   &lt;/project&gt;
    ///   &lt;project name="MySecondProject" queue="Q2" queuePriority="1"&gt;
    ///     &lt;otherProjectSettings /&gt;
    ///   &lt;/project&gt;
    /// &lt;/cruisecontrol&gt;
    /// </code>
    /// </remarks>
    [ReflectorType("queue")]
    public class DefaultQueueConfiguration
        : IQueueConfiguration, IConfigurationValidation
    {
        private string name;
        private QueueDuplicateHandlingMode handlingMode = QueueDuplicateHandlingMode.UseFirst;
        private string lockQueueNames;
		private int maxSize = int.MaxValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultQueueConfiguration"/> class.
        /// </summary>
        public DefaultQueueConfiguration() 
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultQueueConfiguration"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public DefaultQueueConfiguration(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// The name of the queue.
        /// </summary>
        /// <default>n/a</default>
        /// <version>1.4.2</version>
        [ReflectorProperty("name", Required = true)]
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Defines how duplicates should be handled.
        /// </summary>
        /// <default>UseFirst</default>
        /// <version>1.4.2</version>
        [ReflectorProperty("duplicates", Required = false)]
        public virtual QueueDuplicateHandlingMode HandlingMode
        {
            get { return handlingMode; }
            set { handlingMode = value; }
        }

        /// <summary>
        /// A comma sperated list of queue names that the queue should acquire a lock against.
        /// </summary>
        /// <default>none</default>
        /// <version>1.4.2</version>
        [ReflectorProperty("lockqueues", Required = false)]
        public virtual string LockQueueNames
        {
            get { return lockQueueNames; }
            set { lockQueueNames = value; }
        }
		
		/// <summary>
        /// The maximum number of items that can be exist in the queue.
        /// </summary>
        /// <default>none</default>
        /// <version>1.4.2</version>
        [ReflectorProperty("maxsize", Required = false)]
        public virtual int MaxSize
        {
            get { return maxSize; }
            set { maxSize = value; }
        }

        /// <summary>
        /// The list of projects for the queue.
        /// </summary>
        /// <default>none</default>
        /// <version>1.6</version>
        [ReflectorProperty("projects", Required = false)]
        public virtual List<Project> Projects { get; set; }

        /// <summary>
        /// Checks the internal validation of the item.
        /// </summary>
        /// <param name="configuration">The entire configuration.</param>
        /// <param name="parent">The parent item for the item being validated.</param>
        /// <param name="errorProcesser"></param>
        public virtual void Validate(IConfiguration configuration, ConfigurationTrace parent, IConfigurationErrorProcesser errorProcesser)
        {
            // Ensure that the queue has at least one project in it
            var queueFound = false;
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
                errorProcesser.ProcessError(new ConfigurationException(
                    string.Format(System.Globalization.CultureInfo.CurrentCulture,"An unused queue definition has been found: name '{0}'", this.Name)));
            }
        }
    }
}
