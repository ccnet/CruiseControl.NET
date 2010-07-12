using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace ThoughtWorks.CruiseControl.Core.tasks
{
    /// <summary>
    /// <title>FAKE Task</title>
    /// <para>Runs a FAKE - F# Make script.</para>
    /// <version>1.6</version>
    /// <para>
    /// "FAKE - F# Make" is a build automation system. Due to its integration in F#, all benets of the .NET Framework and
    /// functional programming can be used, including the extensive class library,
    /// powerful debuggers and integrated development environments like
    /// Visual Studio 2008 or SharpDevelop, which provide syntax highlighting and code completion.
    /// 
    /// The Google group can be found at: http://groups.google.com/group/fsharpMake
    /// More information on: http://bitbucket.org/forki/fake/wiki/Home
    /// </para>
    /// </summary>
    [ReflectorType("fake")]
    public class FakeTask : BaseExecutableTask
    {
        #region Overrides of TaskBase

        /// <summary>
        /// Execute the actual task functionality.
        /// </summary>
        /// <param name="result">The result to use.</param>
        /// <returns><c>true</c> if the task was successful; <c>false</c> otherwise.</returns>
        protected override bool Execute(IIntegrationResult result)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Overrides of BaseExecutableTask

        protected override string GetProcessFilename()
        {
            throw new NotImplementedException();
        }

        protected override string GetProcessArguments(IIntegrationResult result)
        {
            throw new NotImplementedException();
        }

        protected override string GetProcessBaseDirectory(IIntegrationResult result)
        {
            throw new NotImplementedException();
        }

        protected override ProcessPriorityClass GetProcessPriorityClass()
        {
            throw new NotImplementedException();
        }

        protected override int GetProcessTimeout()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
