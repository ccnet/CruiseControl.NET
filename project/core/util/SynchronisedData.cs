//-----------------------------------------------------------------------
// <copyright file="SynchronisedData.cs" company="CruiseControl.NET">
//     Copyright (c) 2009 CruiseControl.NET. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ThoughtWorks.CruiseControl.Core.Util
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;

    #region LoadDataHandler
    /// <summary>
    /// A delegate for loading data.
    /// </summary>
    /// <returns>The data that has been loaded.</returns>
    public delegate object LoadDataHandler();
    #endregion

    #region SynchronisedData
    /// <summary>
    /// A data item that is only loaded once.
    /// </summary>
    /// <remarks>
    /// The caller that initialises an instance of this class is responsible for loading the data
    /// (using the <see cref="LoadData"/>() method). Any other caller that accesses an instance of
    /// this class should call <see cref="WaitForLoad"/>() before attempting to access the data.
    /// </remarks>
    public class SynchronisedData
        : IDisposable
    {
        #region Private fields
        #region manualEvent
        /// <summary>
        /// The <see cref="ManualResetEvent"/> for handling synchronisation.
        /// </summary>
        private ManualResetEvent manualEvent = new ManualResetEvent(false);
        #endregion
        #endregion

        #region Public properties
        #region Data
        /// <summary>
        /// Gets the synchronised data.
        /// </summary>
        /// <value>The data that is being synchronised.</value>
        public object Data { get; private set; }
        #endregion
        #endregion

        #region Public methods
        #region WaitForLoad()
        /// <summary>
        /// Waits for the data to be loaded.
        /// </summary>
        /// <param name="milliseconds">The milliseconds.</param>
        /// <returns>True if the data has been loaded, false otherwise.</returns>
        public bool WaitForLoad(int milliseconds)
        {
            var result = this.manualEvent.WaitOne(milliseconds, false);
            return result;
        }
        #endregion

        #region LoadData()
        /// <summary>
        /// Loads the data and marks this instance as loaded.
        /// </summary>
        /// <param name="handler">The handler for loading the data.</param>
        public void LoadData(LoadDataHandler handler)
        {
            try
            {
                // Load the data
                this.Data = handler();
            }
            finally
            {
                // Always set the event - otherwise other threads could be waiting forever!
                this.manualEvent.Set();
            }
        }
        #endregion

        #region Dispose()
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Make sure the manual event is reset - just in case something has gone badly wrong!
            this.manualEvent.Set();
        }
        #endregion
        #endregion
    }
    #endregion
}
