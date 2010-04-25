namespace ThoughtWorks.CruiseControl.UnitTests.IntegrationTests
{
    using System;

    /// <summary>
    /// An exception has occurred in the test harness.
    /// </summary>
    public class HarnessException
        : Exception
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="HarnessException"/> class.
        /// </summary>
        public HarnessException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HarnessException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public HarnessException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HarnessException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public HarnessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        #endregion
    }
}
