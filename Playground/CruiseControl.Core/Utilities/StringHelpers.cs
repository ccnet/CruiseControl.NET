namespace CruiseControl.Core.Utilities
{
    /// <summary>
    /// Helper methods for working with strings.
    /// </summary>
    public static class StringHelpers
    {
        #region Public methods
        #region StripQuotes()
        /// <summary>
        /// Strips the quotes.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The value without any starting or ending quotes.
        /// </returns>
        public static string StripQuotes(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            var endQuote = (value.EndsWith("\"") ? 1 : 0);
            var startQuote = (value.StartsWith("\"") ? 1 : 0);
            if ((endQuote + startQuote) == 0)
            {
                return value;
            }

            var newValue = value.Substring(startQuote, value.Length - endQuote - startQuote);
            return newValue;
        }
        #endregion
        #endregion
    }
}
