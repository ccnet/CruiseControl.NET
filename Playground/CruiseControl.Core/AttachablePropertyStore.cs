namespace CruiseControl.Core
{
    using System;
    using System.Collections.Generic;
    using System.Xaml;

    /// <summary>
    /// The base class for allowing attached properties.
    /// </summary>
    public abstract class AttachablePropertyStore
        : IAttachedPropertyStore
    {
        #region Private fields
        private readonly IDictionary<AttachableMemberIdentifier, object> attachedProperties =
            new Dictionary<AttachableMemberIdentifier, object>();
        #endregion

        #region Public proeprties
        #region PropertyCount
        /// <summary>
        /// Gets the count of the attachable member entries in this attachable member store.
        /// </summary>
        /// <returns>The integer count of entries in the store.</returns>
        public int PropertyCount
        {
            get { return this.attachedProperties.Count; }
        }
        #endregion
        #endregion

        #region Public properties
        #region CopyPropertiesTo()
        /// <summary>
        /// Copies all attachable member / value pairs from this attachable member store and into a destination array.
        /// </summary>
        /// <param name="array">The destination array. The array is a generic array, should be passed undimensioned, and should have components of <see cref="T:System.Xaml.AttachableMemberIdentifier"/> and object.</param>
        /// <param name="index">The source index at which to copy.</param>
        public void CopyPropertiesTo(KeyValuePair<AttachableMemberIdentifier, object>[] array, int index)
        {
            this.attachedProperties.CopyTo(array, index);
        }
        #endregion

        #region RemoveProperty()
        /// <summary>
        /// Removes the entry for the specified attachable member from this attachable member store.
        /// </summary>
        /// <param name="attachableMemberIdentifier">The XAML type system identifier for the attachable member entry to remove.</param>
        /// <returns>
        /// true if an attachable member entry for name> was found in the store and removed; otherwise, false.
        /// </returns>
        public bool RemoveProperty(AttachableMemberIdentifier attachableMemberIdentifier)
        {
            return attachedProperties.Remove(attachableMemberIdentifier);
        }
        #endregion

        #region SetProperty()
        /// <summary>
        /// Sets a value for the specified attachable member in the specified store.
        /// </summary>
        /// <param name="attachableMemberIdentifier">The XAML type system identifier for the attachable member entry to set.</param>
        /// <param name="value">The value to set.</param>
        public void SetProperty(AttachableMemberIdentifier attachableMemberIdentifier, object value)
        {
            this.attachedProperties[attachableMemberIdentifier] = value;
        }
        #endregion

        #region TryGetProperty()
        /// <summary>
        /// Attempts to get a value for the specified attachable member in the specified store.
        /// </summary>
        /// <param name="attachableMemberIdentifier">The XAML type system identifier for the attachable member entry to get.</param>
        /// <param name="value">Out parameter. Destination object for the value if <paramref name="attachableMemberIdentifier"/> exists in the store and has a value.</param>
        /// <returns>
        /// true if an attachable member entry for name was found in the store and a value was posted to <paramref name="value"/>; otherwise, false.
        /// </returns>
        public bool TryGetProperty(AttachableMemberIdentifier attachableMemberIdentifier, out object value)
        {
            return attachedProperties.TryGetValue(attachableMemberIdentifier, out value);
        }
        #endregion
        #endregion
    }
}
