using Exortech.NetReflector;
using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// The name of a user.
    /// </summary>
    /// <title>User Name</title>
    /// <version>1.5</version>
    /// <example>
    /// <code>
    /// &lt;userName name="me" /&gt;
    /// </code>
    /// </example>
    [ReflectorType("userName")]
    public class UserName
    {
        private string userName;

        public UserName() { }
        public UserName(string name)
        {
            userName = name;
        }

        /// <summary>
        /// The name of the user.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("name")]
        public string Name
        {
            get { return userName; }
            set { userName = value; }
        }
    }
}
