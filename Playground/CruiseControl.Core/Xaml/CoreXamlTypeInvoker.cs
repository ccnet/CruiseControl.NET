namespace CruiseControl.Core.Xaml
{
    using System;
    using System.Xaml;
    using System.Xaml.Schema;
    using Ninject;
    using NLog;

    public class CoreXamlTypeInvoker
        : XamlTypeInvoker
    {
        #region Private fields
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IKernel kernel;
        private readonly Type typeToResolve;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreXamlTypeInvoker"/> class.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="typeToResolve">The type to resolve.</param>
        /// <param name="xamlType">Type of the xaml.</param>
        public CoreXamlTypeInvoker(IKernel kernel, Type typeToResolve, XamlType xamlType)
            : base(xamlType)
        {
            this.kernel = kernel;
            this.typeToResolve = typeToResolve;
        }
        #endregion

        #region Public methods
        #region CreateInstance()
        /// <summary>
        /// Creates an object instance based on the construction-initiated <see cref="T:System.Xaml.XamlType"/> for this <see cref="T:System.Xaml.Schema.XamlTypeInvoker"/>.
        /// </summary>
        /// <param name="arguments">An array of objects that supply the x:ConstructorArgs for the instance. May be null for types that do not require or use x:ConstructorArgs.</param>
        /// <returns>
        /// The created instance based on the construction-initiated <see cref="T:System.Xaml.XamlType"/> for this <see cref="T:System.Xaml.Schema.XamlTypeInvoker"/>.
        /// </returns>
        /// <exception cref="T:System.MissingMethodException">Could not resolve a constructor.</exception>
        public override object CreateInstance(object[] arguments)
        {
            logger.Trace("Generating an instance of '{0}'", this.typeToResolve.FullName);
            return this.kernel.Get(this.typeToResolve);
        }
        #endregion
        #endregion
    }
}
