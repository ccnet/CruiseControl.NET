
using System;
using System.Threading;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// 	
    /// </summary>
	public class DateTimeProvider
	{
        /// <summary>
        /// Gets the now.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public virtual DateTime Now
		{
			get { return DateTime.Now; }
		}

        /// <summary>
        /// Sleeps the specified milliseconds.	
        /// </summary>
        /// <param name="milliseconds">The milliseconds.</param>
        /// <remarks></remarks>
		public virtual void Sleep(int milliseconds)
		{
			Thread.Sleep(milliseconds);
		}

        /// <summary>
        /// Sleeps the specified duration.	
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <remarks></remarks>
		public virtual void Sleep(TimeSpan duration)
		{
			Thread.Sleep(duration);
		}

        /// <summary>
        /// Gets the today.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public virtual DateTime Today
		{
			get { return DateTime.Today; }
		}
	}
}
