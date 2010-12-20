namespace CruiseControl.Core.Xaml
{
    using System;
    using System.Reflection;
    using System.Xaml;
    using System.Xaml.Schema;

    public class DynamicPropertyXamlMember
        : XamlMember
    {
        #region Private fields
        private readonly DynamicPropertyXamlMemberInvoker invoker;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicPropertyXamlMember"/> class.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="schemaContext">The schema context.</param>
        /// <param name="actualType">The actual type.</param>
        public DynamicPropertyXamlMember(PropertyInfo propertyInfo, XamlSchemaContext schemaContext, Type actualType)
            : base(propertyInfo, schemaContext)
        {
            this.invoker = new DynamicPropertyXamlMemberInvoker(this, actualType);
        }
        #endregion

        #region Protected methods
        #region LookupInvoker()
        /// <summary>
        /// Returns a <see cref="T:System.Xaml.Schema.XamlMemberInvoker"/> associated with this <see cref="T:System.Xaml.XamlMember"/>.
        /// </summary>
        /// <returns>
        ///   <see cref="T:System.Xaml.Schema.XamlMemberInvoker"/> information for this <see cref="T:System.Xaml.XamlMember"/>, or null.
        /// </returns>
        protected override XamlMemberInvoker LookupInvoker()
        {
            return this.invoker;
        }
        #endregion
        #endregion
    }
}
