using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core.Config
{
    /// <summary>
    /// Defines the configuration settings for a queue.
    /// </summary>
    public interface IQueueConfiguration
    {
        /// <summary>
        /// The name of the queue.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Defines how duplicates should be handled.
        /// </summary>
        QueueDuplicateHandlingMode HandlingMode { get; set; }
    }
}
