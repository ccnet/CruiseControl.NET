namespace CruiseControl.Core.Xaml
{
    using System.Xaml;
    using Ninject;

    public class CoreXamlSchemaContext
        : XamlSchemaContext
    {
        #region Private fields
        private readonly IKernel kernel;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreXamlSchemaContext"/> class.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public CoreXamlSchemaContext(IKernel kernel)
        {
            this.kernel = kernel;
        }
        #endregion

        #region Protected methods
        #region GetXamlType()
        /// <summary>
        /// Returns a <see cref="T:System.Xaml.XamlType"/> based on a XAML namespace, a string for type name, and type arguments for a possible generic type.
        /// </summary>
        /// <param name="xamlNamespace">The XAML namespace that contains the desired type.</param>
        /// <param name="name">String name of the desired type.</param>
        /// <param name="typeArguments">Initialization type arguments for a generic type.</param>
        /// <returns>
        /// The <see cref="T:System.Xaml.XamlType"/> that matches the input criteria.
        /// </returns>
        protected override XamlType GetXamlType(string xamlNamespace, string name, params XamlType[] typeArguments)
        {
            var retval = base.GetXamlType(xamlNamespace, name, typeArguments);
            return retval == null ?
                null :
                new CoreXamlType(this.kernel, retval.UnderlyingType, this);
        }
        #endregion
        #endregion
    }
}
