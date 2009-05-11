using System;
using System.Collections.Generic;
using System.Text;
using Exortech.NetReflector;
using Exortech.NetReflector.Util;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// Serialise/deserialise a merge file.
    /// </summary>
    public class MergeFileSerialiser
        : XmlMemberSerialiser
    {
        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="MergeFileSerialiser"/>.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="attribute"></param>
        public MergeFileSerialiser(ReflectorMember info, ReflectorPropertyAttribute attribute)
            : base(info, attribute)
		{ }
        #endregion

        #region Public methods
        #region Read()
        /// <summary>
        /// Read a node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public override object Read(XmlNode node, NetReflectorTypeTable table)
        {
            var fileList = new List<MergeFileInfo>();

            // Validate the attributes
            if (node.Attributes.Count > 0)
            {
                throw new NetReflectorException("A file list cannot directly contain attributes.\r\nXML: " + node.OuterXml);
            }

            // Check each element
            foreach (XmlElement fileElement in node.SelectNodes("*"))
            {
                if (fileElement.Name == "file")
                {
                    // Make sure there are no child elements
                    if (fileElement.SelectNodes("*").Count > 0)
                    {
                        throw new NetReflectorException("file cannot contain any sub-items.\r\nXML: " + node.OuterXml);
                    }

                    // Load the filename
                    var newFile = new MergeFileInfo();
                    newFile.FileName = fileElement.InnerText;

                    // Load the merge action
                    var typeAttribute = fileElement.GetAttribute("action");
                    if (string.IsNullOrEmpty(typeAttribute))
                    {
                        newFile.MergeAction = MergeFileInfo.MergeActionType.Merge;
                    }
                    else
                    {
                        try
                        {
                            newFile.MergeAction = (MergeFileInfo.MergeActionType)Enum.Parse(
                                typeof(MergeFileInfo.MergeActionType),
                                typeAttribute);
                        }
                        catch (Exception error)
                        {
                            throw new NetReflectorConverterException("Unknown action :'" + typeAttribute + "'\r\nXML: " + node.OuterXml, error);
                        }
                    }
                    fileList.Add(newFile);
                }
                else
                {
                    // Unknown sub-item
                    throw new NetReflectorException(fileElement.Name + " is not a valid sub-item.\r\nXML: " + node.OuterXml);
                }
            }
            return fileList.ToArray();
        }
        #endregion

        #region Write()
        /// <summary>
        /// Write a node.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="target"></param>
        public override void Write(XmlWriter writer, object target)
        {
            var list = target as MergeFileInfo[];
            if (list != null)
            {
                writer.WriteStartElement(base.Attribute.Name);
                foreach (var file in list)
                {
                    writer.WriteStartElement("file");
                    writer.WriteAttributeString("action", file.MergeAction.ToString());
                    writer.WriteString(file.FileName);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }
        #endregion
        #endregion
    }
}
