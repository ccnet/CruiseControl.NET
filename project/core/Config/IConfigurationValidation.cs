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
        /// <param name="errorProcesser">The error processer to use.</param>
        void Validate(IConfiguration configuration, ConfigurationTrace parent, IConfigurationErrorProcesser errorProcesser);
    }
}
