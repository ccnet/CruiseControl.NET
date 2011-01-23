namespace CruiseControl.Core
{
    using System;

    /// <summary>
    /// Exposes a method as an external action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RemoteActionAttribute
        : Attribute
    {
    }
}