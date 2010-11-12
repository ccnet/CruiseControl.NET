//-----------------------------------------------------------------------
// <copyright file="NetworkCredentialsSerializer.cs" company="CruiseControl.NET">
//     Copyright (c) 2009 CruiseControl.NET. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ThoughtWorks.CruiseControl.Core.Util
{
    using System;
    using System.Net;
    using System.Xml;
    using Exortech.NetReflector;
    using Exortech.NetReflector.Util;

    /// <summary>
    /// Serialises/deserialises network credentials.
    /// </summary>
    public class NetworkCredentialsSerializer
        : XmlMemberSerialiser
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkCredentialsSerializer"/> class.
        /// </summary>
        /// <param name="member">The member to use.</param>
        /// <param name="attribute">The attribute defining the serialiser.</param>
        public NetworkCredentialsSerializer(ReflectorMember member, ReflectorPropertyAttribute attribute)
            : base(member, attribute)
        {
        }
        #endregion

        #region Public methods
        #region Read()
        /// <summary>
        /// Reads the specified node.
        /// </summary>
        /// <param name="node">The node to read.</param>
        /// <param name="table">The underlying serialiser table.</param>
        /// <returns>The deserialised network credentials.</returns>
        public override object Read(XmlNode node, NetReflectorTypeTable table)
        {
            NetworkCredential ret = null;

            XmlElement elem = (XmlElement)node;

            if (elem != null)
            {
                if (!elem.HasAttribute("userName") || String.IsNullOrEmpty(elem.GetAttribute("userName").Trim()))
                {
                    Log.Warning("No 'userName' specified!");
                    return ret;
                }

                if (!elem.HasAttribute("password"))
                {
                    Log.Warning("No 'password' specified!");
                    return ret;
                }

                if (elem.HasAttribute("domain") && !String.IsNullOrEmpty(elem.GetAttribute("domain").Trim()))
                {
                    ret = new NetworkCredential(elem.GetAttribute("userName").Trim(), elem.GetAttribute("password").Trim(), elem.GetAttribute("domain").Trim());
                }
                else
                {
                   ret = new NetworkCredential(elem.GetAttribute("userName").Trim(), elem.GetAttribute("password").Trim());
                }
            }

            return ret;
        }
        #endregion

        #region Write()
        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer to use.</param>
        /// <param name="target">The target to write.</param>
        public override void Write(XmlWriter writer, object target)
        {
            NetworkCredential credentials = target as NetworkCredential;
            if (credentials != null)
            {
                writer.WriteStartElement("networkCredentials");
                writer.WriteAttributeString("userName", credentials.UserName);
                writer.WriteAttributeString("password", credentials.Password);
                writer.WriteAttributeString("domain", credentials.Domain);
                writer.WriteEndElement();
            }
        }
        #endregion
        #endregion
    }
}
