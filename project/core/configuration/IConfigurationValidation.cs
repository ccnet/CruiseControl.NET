using System;

namespace ThoughtWorks.CruiseControl.Core.Config
{
    /// <summary>
    /// Allows a configuration item to perform any internal validation checks.
    /// </summary>
    public interface IConfigurationValidation
    {
        /// <summary>
        /// Checks the internal validation of the item.
        /// </summary>
        /// <param name="configuration">The entire configuration.</param>
        /// <param name="parent">The parent item for the item being validated.</param>
        void Validate(IConfiguration configuration, object parent);
    }
}
