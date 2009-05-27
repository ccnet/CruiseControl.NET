using System;
using System.Collections;

namespace ThoughtWorks.CruiseControl.Remote
{
	/// <summary>
	/// An enumerable list of the contents of a particular integration queue as stored in the snapshot.
	/// </summary>
    /// <remarks>
    /// This class is required for backwards compatibility with 1.4.4 or earlier versions.
    /// </remarks>
    [Serializable]
	public class QueuedRequestSnapshotList 
        : IEnumerable
    {
        #region Private fields
        private ArrayList queuedRequests;
        #endregion

        #region Constructors
        /// <summary>
		/// Initializes a new instance of the <see cref="QueuedRequestSnapshotList"/> class.
		/// </summary>
		public QueuedRequestSnapshotList()
		{
			queuedRequests = new ArrayList();
        }
        #endregion

        #region Public methods
        #region GetEnumerator()
        /// <summary>
        /// Get the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return queuedRequests.GetEnumerator();
        }
        #endregion
        #endregion
    }
}
