using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Remote.Monitor
{
    /// <summary>
    /// A general data storage bag.
    /// </summary>
    public class DataBag
    {
        #region Private fields
        private Dictionary<Type, object> dataStore = new Dictionary<Type, object>();
        #endregion

        #region Public methods
        #region Get()
        /// <summary>
        /// Gets a data item from the bag.
        /// </summary>
        /// <typeparam name="TData">The type of data item to get.</typeparam>
        /// <returns>The data item if found, null otherwise.</returns>
        public TData Get<TData>()
        {
            var dataType = typeof(TData);
            if (dataStore.ContainsKey(dataType))
            {
                return (TData)dataStore[dataType];
            }
            else
            {
                return default(TData);
            }
        }
        #endregion

        #region Set()
        /// <summary>
        /// Sets a data item in the bag.
        /// </summary>
        /// <typeparam name="TData">The type of data item to set.</typeparam>
        /// <param name="value">The value to set.</param>
        public void Set<TData>(TData value)
        {
            var dataType = typeof(TData);
            dataStore[dataType] = value;
        }
        #endregion

        #region Delete()
        /// <summary>
        /// Deletes a data item from the bag.
        /// </summary>
        /// <typeparam name="TData">The type of data item to delete.</typeparam>
        public void Delete<TData>()
        {
            var dataType = typeof(TData);
            if (dataStore.ContainsKey(dataType))
            {
                dataStore.Remove(dataType);
            }
        }
        #endregion
        #endregion
    }
}
