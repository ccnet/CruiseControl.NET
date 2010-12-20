namespace CruiseControl.Core.Xaml
{
    using System;
    using System.Xaml;
    using System.Xaml.Schema;

    public class DynamicPropertyXamlMemberInvoker
        : XamlMemberInvoker
    {
        #region Private fields
        private readonly Type actualType;
        #endregion

        #region Constructors
        public DynamicPropertyXamlMemberInvoker(XamlMember member, Type actualType)
        {
            this.actualType = actualType;
        }
        #endregion

        #region Public methods
        #region GetValue()
        public override object GetValue(object instance)
        {
            var value = base.GetValue(instance);
            return value;
        }
        #endregion

        #region SetValue()
        public override void SetValue(object instance, object value)
        {
            base.SetValue(instance, value);
        }
        #endregion
        #endregion
    }
}
