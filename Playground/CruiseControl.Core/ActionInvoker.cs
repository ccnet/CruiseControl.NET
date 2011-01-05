namespace CruiseControl.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using CruiseControl.Common.Messages;

    /// <summary>
    /// Invokes actions on the various configuration elements using universal names.
    /// </summary>
    public class ActionInvoker
    {
        #region Private fields
        private Server serverConfiguration;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionInvoker"/> class.
        /// </summary>
        /// <param name="server">The server.</param>
        public ActionInvoker(Server server)
        {
            this.serverConfiguration = server;
        }
        #endregion

        #region Public methods
        #region Initialise()
        /// <summary>
        /// Initialises this instance.
        /// </summary>
        public void Initialise()
        {
            foreach (var channel in this.serverConfiguration.ClientChannels)
            {
                channel.Initialise(this);
            }
        }
        #endregion

        #region Invoke()
        /// <summary>
        /// Invokes an action on an item.
        /// </summary>
        /// <param name="name">The universal name of the item.</param>
        /// <param name="action">The action name.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        /// The return message from the action.
        /// </returns>
        public BaseMessage Invoke(string name, string action, BaseMessage message)
        {
            var item = this.LocateItem(name);
            var method = FindAction(item.GetType(), action);
            var input = method.GetParameters()
                .FirstOrDefault(p => p.ParameterType.IsSubclassOf(typeof(BaseMessage)));
            if (!input.ParameterType.IsAssignableFrom(message.GetType()))
            {
                throw new InvalidOperationException("Invalid input message");
            }

            var output = method.Invoke(item, new[] {message}) as BaseMessage;
            return output;
        }
        #endregion

        #region List()
        /// <summary>
        /// Lists the available actions on an item.
        /// </summary>
        /// <param name="name">The universal name of the item.</param>
        public RemoteActionDefinition[] List(string name)
        {
            var item = this.LocateItem(name);
            var itemType = item.GetType();
            var actions = new List<RemoteActionDefinition>();
            foreach (var method in itemType.GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                var actionAttributes = method.GetCustomAttributes(
                    typeof(RemoteActionAttribute), false);
                if (actionAttributes.Length > 0)
                {
                    // TODO: Validate security here

                    var description = ExtractDescription(method);
                    var definition = new RemoteActionDefinition(
                        method.Name,
                        description);
                    actions.Add(definition);
                }
            }

            return actions.ToArray();
        }
        #endregion

        #region Query()
        /// <summary>
        /// Queries the details on an item action.
        /// </summary>
        /// <param name="name">The universal name of the item.</param>
        /// <param name="action">The action name.</param>
        public RemoteActionDefinition Query(string name, string action)
        {
            var item = this.LocateItem(name);
            var method = FindAction(item.GetType(), action);
            var description = ExtractDescription(method);
            var input = method.GetParameters()
                .FirstOrDefault(p => p.ParameterType.IsSubclassOf(typeof(BaseMessage)));
            var definition = new RemoteActionDefinition(
                method.Name,
                description)
                                 {
                                     InputMessage = input == null ? null : input.ParameterType.FullName,
                                     OutputMessage = method.ReturnType.FullName
                                 };
            return definition;
        }
        #endregion

        #region CleanUp()
        /// <summary>
        /// Cleans up.
        /// </summary>
        public void CleanUp()
        {
            foreach (var channel in this.serverConfiguration.ClientChannels)
            {
                channel.CleanUp();
            }
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
            var item = this.serverConfiguration.Locate(name);
            if (item == null)
            {
                throw new InvalidOperationException("Unable to find item with name '" + name + "'");
            }

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
            if (method == null)
            {
                throw new InvalidOperationException("Unable to find action");
            }

            // TODO: Validate security here

            return method;
        }
        #endregion
        #endregion
    }
}
