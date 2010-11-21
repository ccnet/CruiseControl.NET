#region Copyright & License
//
// Copyright 2001-2005 The Apache Software Foundation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System;

using log4net;

namespace ThoughtWorks.CruiseControl.Core.Util.Log4NetTrace
{
    /// <summary>
    /// 	
    /// </summary>
	public interface ITraceLog : ILog
	{
        /// <summary>
        /// Traces the specified message.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks></remarks>
		void Trace(object message);
        /// <summary>
        /// Traces the specified message.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <remarks></remarks>
		void Trace(object message, Exception exception);
        /// <summary>
        /// Traces the format.	
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        /// <remarks></remarks>
		void TraceFormat(string format, params object[] args);
        /// <summary>
        /// Gets the is trace enabled.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		bool IsTraceEnabled { get; }
	}
}

