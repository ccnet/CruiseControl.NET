using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.CCTrayLib.X10
{
    /// <summary>
    /// Represents a Cm11LowLeverDriver error message.
    /// </summary>
    public class Cm11LowLevelDriverError : System.EventArgs 
    {
        /// <summary>
        /// Creates an Cm11LowLevelDriverError with the supplied message.
        /// </summary>
        /// <param name="message">Message to user</param>
        public Cm11LowLevelDriverError(string message)
        {
            this.message = message;
        }

        private string message;

        /// <summary>
        /// Message to user.
        /// </summary>
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }
}
