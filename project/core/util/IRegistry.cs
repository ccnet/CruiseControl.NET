using System;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// 	
    /// </summary>
	public interface IRegistry
	{
        /// <summary>
        /// Gets the local machine sub key value.	
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		string GetLocalMachineSubKeyValue(string path, string name);
        /// <summary>
        /// Gets the expected local machine sub key value.	
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		string GetExpectedLocalMachineSubKeyValue(string path, string name);
	}
}
