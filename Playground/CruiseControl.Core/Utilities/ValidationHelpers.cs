namespace CruiseControl.Core.Utilities
{
    using System.Collections.Generic;
    using System.Linq;
    using CruiseControl.Core.Interfaces;

    /// <summary>
    /// Helper methods for working with validation.
    /// </summary>
    public static class ValidationHelpers
    {
        #region Public methods
        #region CheckForDuplicateItems()
        /// <summary>
        /// Checks for duplicate items.
        /// </summary>
        /// <param name="children">The children.</param>
        /// <param name="validationLog">The validation log.</param>
        /// <param name="itemName">Name of the item.</param>
        public static void CheckForDuplicateItems(
            IEnumerable<ServerItem> children, 
            IValidationLog validationLog,
            string itemName)
        {
            var childNames = new Dictionary<string, int>();
            foreach (var child in children ?? new ServerItem[0])
            {
                child.Validate(validationLog);
                int nameCount;
                var childName = child.Name ?? string.Empty;
                childNames.TryGetValue(childName, out nameCount);
                childNames[childName] = ++nameCount;
            }

            foreach (var duplicate in childNames.Where(v => v.Value > 1))
            {
                validationLog.AddError("Duplicate {1} name detected: '{0}'", duplicate.Key, itemName);
            }
        }
        #endregion
        #endregion
    }
}
