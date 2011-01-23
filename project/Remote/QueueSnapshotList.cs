using System;
using System.Collections;

namespace ThoughtWorks.CruiseControl.Remote
{
	/// <summary>
	/// An enumerable list of named integration queues as stored in the snapshot.
	/// </summary>
    /// <remarks>
    /// This class is required for backwards compatibility with 1.4.4 or earlier versions.
    /// </remarks>
	[Serializable]
	internal class QueueSnapshotList
        : IEnumerable
    {
        #region Private fields
        private ArrayList queueSnapshots;
        #endregion

        #region Constructors
        /// <summary>
		/// Initializes a new instance of the <see cref="QueueSnapshotList"/> class.
		/// </summary>
		public QueueSnapshotList()
		{
			queueSnapshots = new ArrayList();
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
			return queueSnapshots.GetEnumerator();
		}
        #endregion
        #endregion
    }
}
