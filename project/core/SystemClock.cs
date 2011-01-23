
using System;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// 	
    /// </summary>
    public class SystemClock : IClock
    {
        /// <summary>
        /// Gets the now.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public DateTime Now
        {
            get { return DateTime.Now; }
        }
    }
}