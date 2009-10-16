//-----------------------------------------------------------------------
// <copyright file="NetworkCredentialSerializerFactory.cs" company="CruiseControl.NET">
//     Copyright (c) 2009 CruiseControl.NET. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ThoughtWorks.CruiseControl.Core.Util
{
    using Exortech.NetReflector;
    using Exortech.NetReflector.Util;

    /// <summary>
    /// Factory class for generating a <see cref="NetworkCredentialsSerializer"/>.
    /// </summary>
    public class NetworkCredentialSerializerFactory
        : ISerialiserFactory
    {
        #region Public methods
        #region Create()
        /// <summary>
        /// Creates the specified serialiser.
        /// </summary>
        /// <param name="memberInfo">The member info.</param>
        /// <param name="attribute">The attribute defining the serialiser.</param>
        /// <returns>A new instance of <see cref="NetworkCredentialsSerializer"/>.</returns>
        public IXmlMemberSerialiser Create(ReflectorMember memberInfo, ReflectorPropertyAttribute attribute)
        {
            return new NetworkCredentialsSerializer(memberInfo, attribute);
        }
        #endregion
        #endregion
    }
}
