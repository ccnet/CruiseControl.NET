using System;
using System.Collections.Generic;
using System.Text;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// An action to perform on a CruiseControl.NET server.
    /// </summary>
    [ReflectorType("controlAction")]
    public class CruiseServerControlTaskAction
    {
        #region Public properties
        #region Project
        /// <summary>
        /// The project to run the command on.
        /// </summary>
        [ReflectorProperty("project", Required = false)]
        public string Project { get; set; }
        #endregion

        #region Type
        /// <summary>
        /// The type of command.
        /// </summary>
        [ReflectorProperty("type")]
        public CruiseServerControlTaskActionType Type { get; set; }
        #endregion
        #endregion
    }
}
