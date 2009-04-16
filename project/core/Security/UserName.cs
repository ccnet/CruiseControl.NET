using Exortech.NetReflector;
using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    [ReflectorType("userName")]
    public class UserName
    {
        private string userName;

        public UserName() { }
        public UserName(string name)
        {
            userName = name;
        }

        [ReflectorProperty("name")]
        public string Name
        {
            get { return userName; }
            set { userName = value; }
        }
    }
}
