using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// An action to perform on a CruiseControl.NET server.
    /// </summary>
    /// <title>CruiseServer Control Action</title>
    /// <version>1.5</version>
    /// <example>
    /// <code>
    /// &lt;controlAction type="StartProject" project="CCNet" /&gt;
    /// </code>
    /// </example>
    [ReflectorType("controlAction")]
    public class CruiseServerControlTaskAction
    {
        #region Public properties
        #region Project
        /// <summary>
        /// The project to run the command on.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("project", Required = false)]
        public string Project { get; set; }
        #endregion

        #region Type
        /// <summary>
        /// The type of command.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("type")]
        public CruiseServerControlTaskActionType Type { get; set; }
        #endregion
        #endregion
    }
}
