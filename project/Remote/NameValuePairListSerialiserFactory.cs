using Exortech.NetReflector;
using Exortech.NetReflector.Util;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// Generate a serialiser for deserialising a list of name/value pairs.
    /// </summary>
    public class NameValuePairListSerialiserFactory
        : ISerialiserFactory
    {
        #region Public methods
        #region Create()
        /// <summary>
        /// Create the serialiser.
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public IXmlMemberSerialiser Create(ReflectorMember memberInfo, ReflectorPropertyAttribute attribute)
        {
            return new NameValuePairSerialiser(memberInfo, attribute, true);
        }
        #endregion
        #endregion
    }
}
