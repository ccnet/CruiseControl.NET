using Exortech.NetReflector;
using Exortech.NetReflector.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// Generate a serialiser for deserialising merge files.
    /// </summary>
    public class MergeFileSerialiserFactory
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
            return new MergeFileSerialiser(memberInfo, attribute);
        }
        #endregion
        #endregion
    }
}
