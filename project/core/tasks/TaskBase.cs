using Exortech.NetReflector;
using System;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote.Parameters;
using System.Xml;
using System.Text.RegularExpressions;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// An abstract base class to add parameters to a task
    /// </summary>
    public abstract class TaskBase
        : IParamatisedTask
    {
        #region Private fields
        private static Regex parameterRegex = new Regex(@"\$\[[^\]]*\]", RegexOptions.Compiled);
        private static Regex paramPartRegex = new Regex(@"(?<!\\)\|", RegexOptions.Compiled);
        private IDynamicValue[] myDynamicValues = new IDynamicValue[0];
        #endregion

        #region Public properties
        #region DynamicValues
        /// <summary>
        /// The dynamic values to use for the task.
        /// </summary>
        [ReflectorProperty("dynamicValues", Required = false)]
        public IDynamicValue[] DynamicValues
        {
            get { return myDynamicValues; }
            set { myDynamicValues = value;}
        }
        #endregion
        #endregion

        #region Public methods
        #region ApplyParameters()
        /// <summary>
        /// Applies the input parameters to the task.
        /// </summary>
        /// <param name="parameters">The parameters to apply.</param>
        /// <param name="parameterDefinitions">The original parameter definitions.</param>
        public virtual void ApplyParameters(Dictionary<string, string> parameters, IEnumerable<ParameterBase> parameterDefinitions)
        {
            if (myDynamicValues != null)
            {
                foreach (IDynamicValue value in myDynamicValues)
                {
                    value.ApplyTo(this, parameters, parameterDefinitions);
                }
            }
        }
        #endregion

        #region PreprocessParameters()
        /// <summary>
        /// Preprocesses a node prior to loading it via NetReflector.
        /// </summary>
        /// <param name="inputNode"></param>
        /// <returns></returns>
        [ReflectionPreprocessor]
        public virtual XmlNode PreprocessParameters(XmlNode inputNode)
        {
            var resultNode = inputNode;
            var doc = inputNode.OwnerDocument;
            var parameters = new List<XmlElement>();

            foreach (XmlNode nodeWithParam in inputNode.SelectNodes("descendant::text()|descendant-or-self::*[@*]/@*"))
            {
                var text = nodeWithParam.Value;
                if (parameterRegex.Match(text).Success)
                {
                    // Generate the format string
                    var parametersEl = doc.CreateElement("parameters");
                    var index = 0;
                    var format = parameterRegex.Replace(text, (match) =>
                    {
                        // Split into it's component parts
                        var parts = paramPartRegex.Split(match.Value.Substring(2, match.Value.Length - 3));

                        // Generate the value element
                        var dynamicValueEl = doc.CreateElement("namedValue");
                        dynamicValueEl.SetAttribute("name", parts[0].Replace("\\|", "|"));
                        dynamicValueEl.SetAttribute("value", parts.Length > 1 ? parts[1].Replace("\\|", "|") : string.Empty);
                        parametersEl.AppendChild(dynamicValueEl);

                        // Generate the replacement
                        var replacement = string.Format("{{{0}{1}}}",
                            index++,
                            parts.Length > 2 ? ":" + parts[2].Replace("\\|", "|") : string.Empty);
                        return replacement;
                    });

                    // Generate the dynamic value element
                    var replacementValue = string.Empty;
                    XmlElement replacementEl;
                    if (parametersEl.ChildNodes.Count > 1)
                    {
                        replacementEl = doc.CreateElement("replacementValue");
                        AddElement(replacementEl, "format", format);
                        replacementEl.AppendChild(parametersEl);
                    }
                    else
                    {
                        replacementEl = doc.CreateElement("directValue");
                        AddElement(replacementEl, "parameter", parametersEl.SelectSingleNode("namedValue/@name").InnerText);
                        replacementValue = parametersEl.SelectSingleNode("namedValue/@value").InnerText;
                        AddElement(replacementEl, "default", replacementValue);
                    }
                    parameters.Add(replacementEl);

                    // Generate the path
                    var propertyName = new StringBuilder();
                    var currentNode = nodeWithParam is XmlAttribute ? nodeWithParam : nodeWithParam.ParentNode;
                    while ((currentNode != inputNode) && (currentNode != null))
                    {
                        propertyName.Insert(0, "/" + currentNode.Name);
                        if (currentNode is XmlAttribute)
                        {
                            currentNode = (currentNode as XmlAttribute).OwnerElement;
                        }
                        else
                        {
                            currentNode = currentNode.ParentNode;
                        }
                    }
                    propertyName.Remove(0, 1);
                    AddElement(replacementEl, "property", propertyName.ToString());

                    // Set a replacement value
                    nodeWithParam.Value = replacementValue;
                }
            }

            // Add the parameters to the element
            if (parameters.Count > 0)
            {
                var parametersEl = inputNode.SelectSingleNode("dynamicValues");
                if (parametersEl == null)
                {
                    parametersEl = doc.CreateElement("dynamicValues");
                    inputNode.AppendChild(parametersEl);
                }

                foreach (var element in parameters)
                {
                    parametersEl.AppendChild(element);
                }
            }

            return resultNode;
        }
        #endregion
        #endregion

        #region Private methods
        #region AddElement()
        /// <summary>
        /// Adds an XML element.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private void AddElement(XmlElement parent, string name, string value)
        {
            var element = parent.OwnerDocument.CreateElement(name);
            element.InnerText = value;
            parent.AppendChild(element);
        }
        #endregion
        #endregion
    }
}
