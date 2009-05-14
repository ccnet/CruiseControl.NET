using System;
using System.Collections.Generic;
using System.Text;
using Exortech.NetReflector;
using Exortech.NetReflector.Util;
using System.Xml;
using ThoughtWorks.CruiseControl.Core.Util;

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

        	if (node != null)
        	{
        		// Validate the attributes
        		if (node.Attributes.Count > 0)
					throw new NetReflectorException(string.Concat("A file list cannot directly contain attributes.", Environment.NewLine, "XML: ", node.OuterXml));

        		// Check each element
        		var subNodes = node.SelectNodes("*");
        		if (subNodes != null)
        		{
        			foreach (XmlElement fileElement in subNodes)
        			{
        				if (fileElement.Name == "file")
        				{
        					// Make sure there are no child elements
        					var fileSubNodes = fileElement.SelectNodes("*");
							if (fileSubNodes != null && fileSubNodes.Count > 0)
								throw new NetReflectorException(string.Concat("file cannot contain any sub-items.", Environment.NewLine, "XML: ", fileElement.InnerXml));

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
        							newFile.MergeAction = (MergeFileInfo.MergeActionType) Enum.Parse(
        							                                                      	typeof (MergeFileInfo.MergeActionType),
        							                                                      	typeAttribute);
        						}
        						catch (Exception error)
        						{
        							throw new NetReflectorConverterException(string.Concat(
        							                                         	"Unknown action :'", typeAttribute, Environment.NewLine,
																				"'XML: " + fileElement.InnerXml), error);
        						}
        					}

        					Log.Debug(string.Concat("MergeFilesTask: Add '", newFile.FileName, "' to '", newFile.MergeAction.ToString(), "' file list."));
        					fileList.Add(newFile);
        				}
        				else
        				{
        					// Unknown sub-item
        					throw new NetReflectorException(string.Concat(fileElement.Name, " is not a valid sub-item.",
																		  Environment.NewLine, "XML: ", fileElement.InnerXml));
        				}
        			}
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
