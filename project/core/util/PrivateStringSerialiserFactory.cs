namespace ThoughtWorks.CruiseControl.Core.Util
{
    using Exortech.NetReflector;
    using Exortech.NetReflector.Util;

    /// <summary>
    /// Factory class for <see cref="PrivateStringSerialiser"/>.
    /// </summary>
    public class PrivateStringSerialiserFactory
        : ISerialiserFactory
    {
        #region Public methods
        #region Create()
        /// <summary>
        /// Creates the specified serialiser.
        /// </summary>
        /// <param name="memberInfo">The member info.</param>
        /// <param name="attribute">The attribute defining the serialiser.</param>
        /// <returns>A new instance of <see cref="UriSerializer"/>.</returns>
        public IXmlMemberSerialiser Create(ReflectorMember memberInfo, ReflectorPropertyAttribute attribute)
        {
            return new PrivateStringSerialiser(memberInfo, attribute);
        }
        #endregion
        #endregion
    }
}
