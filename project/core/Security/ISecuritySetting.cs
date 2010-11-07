namespace ThoughtWorks.CruiseControl.Core.Security
{
    public interface ISecuritySetting
    {
        /// <summary>
        /// A unique identifier for an authentication instance.
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// The security manager that loaded this setting.
        /// </summary>
        ISecurityManager Manager { get; set; }
    }
}
