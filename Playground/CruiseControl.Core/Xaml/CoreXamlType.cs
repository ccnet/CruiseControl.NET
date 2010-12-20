namespace CruiseControl.Core.Xaml
{
    using System;
    using System.Reflection;
    using System.Xaml;
    using System.Xaml.Schema;
    using Ninject;

    public class CoreXamlType
        : XamlType
    {
        #region Private fields
        private readonly CoreXamlTypeInvoker invoker;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreXamlType"/> class.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="type">The type.</param>
        /// <param name="context">The context.</param>
        public CoreXamlType(IKernel kernel, Type type, XamlSchemaContext context)
            : base(type, context)
        {
            this.invoker = new CoreXamlTypeInvoker(kernel, type, this);
        }
        #endregion

        #region Protected methods
        #region LookupInvoker()
        /// <summary>
        /// Returns a <see cref="T:System.Xaml.Schema.XamlTypeInvoker"/> that is associated with this <see cref="T:System.Xaml.XamlType"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Xaml.Schema.XamlTypeInvoker"/> information for this <see cref="T:System.Xaml.XamlType"/>; otherwise, null.
        /// </returns>
        protected override XamlTypeInvoker LookupInvoker()
        {
            return this.invoker;
        }
        #endregion

        #region LookupIsConstructible()
        /// <summary>
        /// Returns a value that indicates whether this <see cref="T:System.Xaml.XamlType"/> represents a constructible type, as per the XAML definition.
        /// </summary>
        /// <returns>
        /// true if this <see cref="T:System.Xaml.XamlType"/> represents a constructible type; otherwise, false.
        /// </returns>
        protected override bool LookupIsConstructible()
        {
            return true;
        }
        #endregion
        #endregion
    }
}
