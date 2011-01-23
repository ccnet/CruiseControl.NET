//-----------------------------------------------------------------------
// <copyright file="UriSerializer.cs" company="CruiseControl.NET">
//     Copyright (c) 2009 CruiseControl.NET. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ThoughtWorks.CruiseControl.Core.Util
{
    using System;
    using System.Xml;
    using Exortech.NetReflector;
    using Exortech.NetReflector.Util;

    /// <summary>
    /// A serialiser for working with URIs.
    /// </summary>
    public class UriSerializer
        : XmlMemberSerialiser
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="UriSerializer"/> class.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="attribute">The attribute.</param>
        public UriSerializer(ReflectorMember member, ReflectorPropertyAttribute attribute)
            : base(member, attribute)
        {
        }
        #endregion

        #region Public methods
        #region Read()
        /// <summary>
        /// Reads a URI.
        /// </summary>
        /// <param name="node">The node containing the URI.</param>
        /// <param name="table">The serialiser table.</param>
        /// <returns>A new instance of a <see cref="Uri"/> if the node is valid; null otherwise.</returns>
        public override object Read(XmlNode node, NetReflectorTypeTable table)
        {
            if (node == null)
            {
                // NetReflector should do this check, but doesn't
                if (this.Attribute.Required)
                {
                    throw new NetReflectorItemRequiredException(Attribute.Name + " is required");
                }
                else
                {
                    return null;
                }
            }

            Uri ret;
            if (node is XmlAttribute)
            {
                ret = new Uri(node.Value);
            }
            else
            {
                ret = new Uri(node.InnerText);
            }

            return ret;
        }
        #endregion

        #region Write()
        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer to use.</param>
        /// <param name="target">The URI to write.</param>
        public override void Write(XmlWriter writer, object target)
        {
            if (!(target is Uri)) target = ReflectorMember.GetValue(target);
            var uri = target as Uri;
            if (uri != null)
            {
                writer.WriteElementString("uri", uri.ToString());
            }
        }
        #endregion
        #endregion
    }
}
