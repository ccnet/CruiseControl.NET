namespace CruiseControl.Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Windows.Markup;
    using System.Xml.Linq;
    using CruiseControl.Common;
    using CruiseControl.Core.Interfaces;
    using Ninject;

    /// <summary>
    /// Invokes actions on the various configuration elements using universal names.
    /// </summary>
    public class ActionInvoker
        : IActionInvoker
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionInvoker"/> class.
        /// </summary>
        [Inject]
        public ActionInvoker()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionInvoker"/> class.
        /// </summary>
        /// <param name="server">The server.</param>
        public ActionInvoker(Server server)
        {
            this.Server = server;
        }
        #endregion

        #region Public properties
        #region Server
        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        /// <value>
        /// The server.
        /// </value>
        public Server Server { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region GenerateMessageFormat()
        /// <summary>
        /// Generates the message format.
        /// </summary>
        /// <param name="namespaces">The namespaces.</param>
        /// <param name="messageType">Type of the parameter.</param>
        /// <returns>
        /// The format of the message.
        /// </returns>
        public static string GenerateMessageFormat(
            IDictionary<string, string> namespaces,
            Type messageType)
        {
            var root = new XElement("definition");
            root.Add(new XAttribute("name", messageType.Name));

            // Retrieve the namespace
            string definitionNamespace;
            var typeNamespace = messageType.Namespace ?? string.Empty;
            if (!namespaces.TryGetValue(typeNamespace, out definitionNamespace))
            {
                var attribute = messageType
                    .Assembly
                    .GetCustomAttributes(typeof (XmlnsDefinitionAttribute), false)
                    .FirstOrDefault(xda => (xda as XmlnsDefinitionAttribute).ClrNamespace == typeNamespace);
                if (attribute != null)
                {
                    definitionNamespace = (attribute as XmlnsDefinitionAttribute).XmlNamespace;
                }
                else
                {
                    definitionNamespace = "clr-namespace:" + typeNamespace +
                                          ";assembly=" + messageType.Assembly.GetName().Name;
                }

                namespaces.Add(typeNamespace, definitionNamespace);
            }

            // Add the properties
            root.Add(new XAttribute("namespace", definitionNamespace));
            foreach (
                var property in
                    messageType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite))
            {
                var propertyXml = new XElement(
                    "value",
                    new XAttribute("name", property.Name),
                    new XAttribute("type", property.PropertyType.Name.ToLowerInvariant()));
                root.Add(propertyXml);
            }

            // Convert to a string
            var format = root.ToString(SaveOptions.DisableFormatting);
            return format;
        }
        #endregion

        #region Ping()
        /// <summary>
        /// Checks if the service is available.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the service is available; <c>false</c> otherwise.
        /// </returns>
        public bool Ping()
        {
            return true;
        }
        #endregion

        #region Invoke()
        /// <summary>
        /// Invokes an action on an item.
        /// </summary>
        /// <param name="urn">The URN to invoke the action.</param>
        /// <param name="arguments">The arguments for the action.</param>
        /// <returns>
        /// The return message from the action.
        /// </returns>
        public InvokeResult Invoke(string urn, InvokeArguments arguments)
        {
            var result = new InvokeResult();
            var item = this.LocateItem(urn);
            if (item == null)
            {
                result.ResultCode = RemoteResultCode.UnknownUrn;
                return result;
            }

            if (arguments == null)
            {
                result.ResultCode = RemoteResultCode.MissingArguments;
                return result;
            }

            var method = FindAction(item.GetType(), arguments.Action);
            if (method == null)
            {
                result.ResultCode = RemoteResultCode.UnknownAction;
                return result;
            }

            // TODO: Validate security

            if (string.IsNullOrEmpty(arguments.Data))
            {
                result.ResultCode = RemoteResultCode.InvalidInput;
                return result;
            }

            var parameters = method.GetParameters();
            var input = MessageSerialiser.Deserialise(arguments.Data);
            if (parameters[0].ParameterType.IsAssignableFrom(input.GetType()))
            {
                var output = method.Invoke(item, new[] { input });
                result.Data = MessageSerialiser.Serialise(output);
            }
            else
            {
                result.ResultCode = RemoteResultCode.InvalidInput;
            }

            return result;
        }
        #endregion

        #region Query()
        /// <summary>
        /// Queries the details on an item action.
        /// </summary>
        /// <param name="urn">The URN to query for actions.</param>
        /// <param name="arguments">The arguments for the query.</param>
        /// <returns>
        /// The allowed actions on the URN.
        /// </returns>
        public QueryResult Query(string urn, QueryArguments arguments)
        {
            var result = new QueryResult();
            arguments = arguments ?? new QueryArguments();

            var item = this.LocateItem(urn);
            if (item == null)
            {
                result.ResultCode = RemoteResultCode.UnknownUrn;
                return result;
            }

            var itemType = item.GetType();
            var actions = new List<RemoteActionDefinition>();
            var filterRegex = (arguments == null) || string.IsNullOrEmpty(arguments.FilterPattern) ?
                null :
                new Regex(arguments.FilterPattern);
            var messageFormats = new Dictionary<Type, string>();
            var namespaces = new Dictionary<string, string>();
            foreach (var method in itemType.GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                var actionAttributes = method.GetCustomAttributes(
                    typeof(RemoteActionAttribute), false);
                if ((actionAttributes.Length > 0) &&
                    ((filterRegex == null) || filterRegex.IsMatch(method.Name)))
                {
                    // TODO: Validate security here

                    var description = ExtractDescription(method);
                    var definition = new RemoteActionDefinition
                                         {
                                             Name = method.Name,
                                             Description = description
                                         };

                    // Generate the message formats
                    switch (arguments.DataToInclude)
                    {
                        case DataDefinitions.InputOnly:
                            definition.InputData = GenerateMessageFormat(
                                messageFormats,
                                namespaces,
                                method.GetParameters()[0].ParameterType);
                            break;

                        case DataDefinitions.OutputOnly:
                            definition.OutputData = GenerateMessageFormat(
                                messageFormats,
                                namespaces,
                                method.ReturnType);
                            break;

                        case DataDefinitions.Both:
                            definition.InputData = GenerateMessageFormat(
                                messageFormats,
                                namespaces,
                                method.GetParameters()[0].ParameterType);
                            definition.OutputData = GenerateMessageFormat(
                                messageFormats,
                                namespaces,
                                method.ReturnType);
                            break;
                    }

                    actions.Add(definition);
                }
            }

            // Generate the result
            result.Actions = actions.ToArray();
            return result;
        }
        #endregion

        #region RetrieveServerName()
        /// <summary>
        /// Retrieves the name of the server.
        /// </summary>
        /// <returns>
        /// The URN of the server.
        /// </returns>
        public string RetrieveServerName()
        {
            var name = this.Server.UniversalName;
            return name;
        }
        #endregion
        #endregion

        #region Private methods
        #region LocateItem()
        /// <summary>
        /// Locates an item.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// The item with the name.
        /// </returns>
        private object LocateItem(string name)
        {
            var item = this.Server.Locate(name);
            return item;
        }
        #endregion

        #region ExtractDescription()
        /// <summary>
        /// Extracts the description.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>
        /// The description of the action if there is one; <c>null</c> otherwise.
        /// </returns>
        private static string ExtractDescription(MethodInfo method)
        {
            var attribute = method.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false).FirstOrDefault() as DescriptionAttribute;
            return attribute == null
                       ? null
                       : attribute.Description;
        }
        #endregion

        #region FindAction()
        /// <summary>
        /// Finds an action on an item.
        /// </summary>
        /// <param name="itemType">Type of the item.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <returns>
        /// The <see cref="MethodInfo"/> of the action.
        /// </returns>
        private static MethodInfo FindAction(Type itemType, string actionName)
        {
            var method = itemType.GetMethod(
                actionName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            return method;
        }
        #endregion

        #region GenerateMessageFormat()
        /// <summary>
        /// Generates the message format.
        /// </summary>
        /// <param name="messageFormats">The message formats.</param>
        /// <param name="namespaces">The namespaces.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <returns>
        /// The format of the message.
        /// </returns>
        private static string GenerateMessageFormat(IDictionary<Type, string> messageFormats,
            IDictionary<string, string> namespaces,
            Type messageType)
        {
            string format;
            if (!messageFormats.TryGetValue(messageType, out format))
            {
                format = GenerateMessageFormat(namespaces, messageType);
                messageFormats.Add(messageType, format);
            }

            return format;
        }
        #endregion
        #endregion
    }
}
