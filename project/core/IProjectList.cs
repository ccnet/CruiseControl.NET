using System;
using System.Collections;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// 	
    /// </summary>
	public interface IProjectList : IEnumerable
	{
        /// <summary>
        /// Gets the <see cref="IProject" /> with the specified project name.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		IProject this[string projectName] { get; }
	}
}
