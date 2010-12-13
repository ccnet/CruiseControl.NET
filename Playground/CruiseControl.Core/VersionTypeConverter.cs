namespace CruiseControl.Core
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    /// <summary>
    /// Handles the conversion to <see cref="Version"/> instances.
    /// </summary>
    public class VersionTypeConverter
        : TypeConverter
    {
        #region Public methods
        #region CanConvertFrom()
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            var canConvert = sourceType == typeof(string) ?
                true :
                base.CanConvertFrom(context, sourceType);
            return canConvert;
        }
        #endregion

        #region ConvertFrom()
        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return new Version((string)value);
            }

            return base.ConvertFrom(context, culture, value);
        }
        #endregion
        #endregion
    }
}
