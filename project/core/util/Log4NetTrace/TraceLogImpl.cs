
#region Copyright & License
//
// Copyright 2001-2006 The Apache Software Foundation
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
using System.Globalization;

using log4net.Core;
using log4net.Util;

namespace ThoughtWorks.CruiseControl.Core.Util.Log4NetTrace
{
    /// <summary>
    /// 	
    /// </summary>
	public class TraceLogImpl : LogImpl, ITraceLog
	{
		/// <summary>
		/// The fully qualified name of this declaring type not the type of any subclass.
		/// </summary>
		private readonly static Type ThisDeclaringType = typeof(TraceLogImpl);

		/// <summary>
		/// The default value for the TRACE level
		/// </summary>
		private readonly static Level s_defaultLevelTrace = new Level(20000, "TRACE");
		
		/// <summary>
		/// The current value for the TRACE level
		/// </summary>
		private Level m_levelTrace;


        /// <summary>
        /// Initializes a new instance of the <see cref="TraceLogImpl" /> class.	
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <remarks></remarks>
        public TraceLogImpl(log4net.Core.ILogger logger)
            : base(logger)
		{
		}

		/// <summary>
		/// Lookup the current value of the TRACE level
		/// </summary>
		protected override void ReloadLevels(log4net.Repository.ILoggerRepository repository)
		{
			base.ReloadLevels(repository);

			m_levelTrace = repository.LevelMap.LookupWithDefault(s_defaultLevelTrace);
		}

		#region Implementation of ITraceLog

        /// <summary>
        /// Traces the specified message.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks></remarks>
		public void Trace(object message)
		{
			Logger.Log(ThisDeclaringType, m_levelTrace, message, null);
		}

        /// <summary>
        /// Traces the specified message.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <remarks></remarks>
		public void Trace(object message, System.Exception exception)
		{
			Logger.Log(ThisDeclaringType, m_levelTrace, message, exception);
		}

        /// <summary>
        /// Traces the format.	
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        /// <remarks></remarks>
		public void TraceFormat(string format, params object[] args)
		{
			if (IsTraceEnabled)
			{
				Logger.Log(ThisDeclaringType, m_levelTrace, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
			}
		}

        /// <summary>
        /// Traces the format.	
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        /// <remarks></remarks>
		public void TraceFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (IsTraceEnabled)
			{
				Logger.Log(ThisDeclaringType, m_levelTrace, new SystemStringFormat(provider, format, args), null);
			}
		}

        /// <summary>
        /// Gets the is trace enabled.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public bool IsTraceEnabled
		{
			get { return Logger.IsEnabledFor(m_levelTrace); }
		}

		#endregion Implementation of ITraceLog
	}
}

