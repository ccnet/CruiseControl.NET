using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Security;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Triggers;

namespace Validator
{
    /// <summary>
    /// Displays the configuration in an hierarchy.
    /// </summary>
    public partial class ConfigurationHierarchy 
        : UserControl
    {
        #region Private fields
        private Dictionary<string, TreeNode> queues = new Dictionary<string, TreeNode>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationHierarchy"/> class.
        /// </summary>
        public ConfigurationHierarchy()
        {
            InitializeComponent();
        }
        #endregion

        #region Public methods
        #region Initialise()
        /// <summary>
        /// Initialises the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void Initialise(string fileName)
        {
            this.hierarchy.BeginUpdate();
            try
            {
                this.hierarchy.Nodes.Clear();
                var parent = new ConfigurationDetails(fileName);
                this.AddNode(this.hierarchy, "Configuration", "configuration", parent);
            }
            finally
            {
                this.hierarchy.EndUpdate();
            }

            this.queues.Clear();
            this.itemDetails.SelectedObject = null;
        }
        #endregion

        #region Finalise()
        /// <summary>
        /// Finalises the display.
        /// </summary>
        public void Finalise()
        {
            this.hierarchy.Nodes[0].Expand();
            foreach (var queue in this.queues.Values)
            {

                queue.Text += " (" + queue.Nodes.Count.ToString("#,##0", Thread.CurrentThread.CurrentUICulture) + ")";
            }
        }
        #endregion

        #region Add()
        /// <summary>
        /// Adds the specified configuration item.
        /// </summary>
        /// <param name="configurationItem">The configuration item.</param>
        public void Add(object configurationItem)
        {
            if (configurationItem is IProject)
            {
                this.AddProject(configurationItem as IProject);
            }
            else if (configurationItem is IQueueConfiguration)
            {
                this.AddQueue(configurationItem as IQueueConfiguration);
            }
            else if (configurationItem is ISecurityManager)
            {
                this.AddSecurity(configurationItem as ISecurityManager);
            }
        }
        #endregion
        #endregion

        #region Public methods
        #region AddProject()
        /// <summary>
        /// Adds a project to the hierarchy.
        /// </summary>
        /// <param name="value">The project to add.</param>
        /// <returns>The new <see cref="TreeNode"/> for the project.</returns>
        private TreeNode AddProject(IProject value)
        {
            this.hierarchy.BeginUpdate();
            try
            {
                // Get the owning queue node
                TreeNode queueNode;
                var queueName = string.IsNullOrEmpty(value.QueueName) ? value.Name : value.QueueName;
                if (this.queues.ContainsKey(queueName))
                {
                    queueNode = this.queues[queueName];
                }
                else
                {
                    // Add a new queue
                    queueNode = this.AddQueue(new DefaultQueueConfiguration(queueName));
                }

                // Add the project node
                var projectNode = this.AddNode(queueNode, value.Name, "project", value);
                if (value is Project)
                {
                    var project = value as Project;
                    this.AddSourceControl(projectNode, project.SourceControl);
                    this.AddTriggers(projectNode, project.Triggers as MultipleTrigger);
                    this.AddTasks(projectNode, "Pre-build", project.PrebuildTasks);
                    this.AddTasks(projectNode, "Tasks", project.Tasks);
                    this.AddTasks(projectNode, "Publishers", project.Publishers);
                }

                return projectNode;
            }
            finally
            {
                this.hierarchy.EndUpdate();
            }
        }
        #endregion

        #region AddSourceControl()
        /// <summary>
        /// Adds a source control block.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="value">The value to add.</param>
        private void AddSourceControl(TreeNode parent, ISourceControl value)
        {
            if (value != null)
            {
                var node = this.AddNode(
                    parent, 
                    "Source Control: " + this.GetReflectionName(value, "(Unknown)"), 
                    "sourcecontrol", 
                    value);
            }
        }
        #endregion

        #region AddTriggers()
        /// <summary>
        /// Adds the triggers.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="triggers">The triggers.</param>
        private void AddTriggers(TreeNode parent, MultipleTrigger triggers)
        {
            if ((triggers != null) && (triggers.Triggers.Length > 0))
            {
                var triggersNode = this.AddNode(parent, "Triggers" + this.CountItems(triggers.Triggers), "triggers", null);
                foreach (var trigger in triggers.Triggers)
                {
                    var nodeName = this.GetReflectionName(trigger, "(Unknown trigger)");
                    var triggerNode = this.AddNode(
                        triggersNode,
                        nodeName,
                        "trigger",
                        trigger);

                    this.AddTriggers(triggerNode, trigger as MultipleTrigger);
                }
            }
        }
        #endregion

        #region AddTasks()
        /// <summary>
        /// Adds tasks to the hierarchy.
        /// </summary>
        /// <param name="parent">The parent node.</param>
        /// <param name="title">The title of the node.</param>
        /// <param name="tasks">The tasks to add.</param>
        /// <returns>
        /// The new <see cref="TreeNode"/> for the tasks.
        /// </returns>
        private TreeNode AddTasks(TreeNode parent, string title, ITask[] tasks)
        {
            if ((tasks != null) && (tasks.Length > 0))
            {
                var node = this.AddNode(parent, title + this.CountItems(tasks), "tasks", null);
                foreach (var task in tasks)
                {
                    this.AddTask(node, task);
                }

                return node;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region AddTask()
        /// <summary>
        /// Adds a task to the hierarchy.
        /// </summary>
        /// <param name="parent">The parent node.</param>
        /// <param name="value">The task to add.</param>
        /// <returns>
        /// The new <see cref="TreeNode"/> for the task.
        /// </returns>
        private TreeNode AddTask(TreeNode parent, ITask value)
        {
            var nodeName = this.GetReflectionName(value, "(Unknown task)");
            var node = this.AddNode(parent, nodeName, "task", value);

            if (value is TaskContainerBase)
            {
                this.AddTasks(node, "Tasks", (value as TaskContainerBase).Tasks);
            }

            return node;
        }
        #endregion

        #region AddQueue()
        /// <summary>
        /// Adds a queue to the hierarchy.
        /// </summary>
        /// <param name="value">The queue to add.</param>
        /// <returns>The new <see cref="TreeNode"/> for the queue.</returns>
        private TreeNode AddQueue(IQueueConfiguration value)
        {
            this.hierarchy.BeginUpdate();
            try
            {
                // Check if the node already exists or not
                TreeNode queueNode;
                if (this.queues.ContainsKey(value.Name))
                {
                    queueNode = this.queues[value.Name];
                }
                else
                {
                    // Add the new node and store it
                    queueNode = this.AddNode(this.hierarchy.Nodes[0], value.Name, "queue", value);
                    this.queues.Add(value.Name, queueNode);
                }

                // If the node already exists, then the project will use it so this will be a safe call
                // The only time this method can be called twice if the actual queue configuration is set
                queueNode.Tag = value;
                return queueNode;                
            }
            finally
            {
                this.hierarchy.EndUpdate();
            }
        }
        #endregion

        #region AddSecurity()
        /// <summary>
        /// Adds a security manager to the hierarchy.
        /// </summary>
        /// <param name="value">The security manager to add.</param>
        /// <returns>The new <see cref="TreeNode"/> for the security manager.</returns>
        private TreeNode AddSecurity(ISecurityManager value)
        {
            this.hierarchy.BeginUpdate();
            try
            {
                var managerNode = this.AddNode(
                    this.hierarchy.Nodes[0], 
                    "Security Manager: " + this.GetReflectionName(value, "(Unknown)"), 
                    "security", 
                    value);
                managerNode.Tag = value;

                if (value is InternalSecurityManager)
                {
                    // HACK: Since ISM is the only manager that currently has users and permissions in the 
                    // configuration we can use this, but we should change it so it is more generic in future
                    var manager = value as InternalSecurityManager;
                    if (manager.Users.Length > 0)
                    {
                        var usersNode = this.AddNode(
                            managerNode, 
                            "Users" + this.CountItems(manager.Users), 
                            "users", 
                            null);
                        foreach (var user in manager.Users)
                        {
                            this.AddNode(
                                usersNode, 
                                this.GetReflectionName(user, string.Empty) + ": " + (string.IsNullOrEmpty(user.DisplayName) ? user.UserName : user.DisplayName), 
                                "user", 
                                user);
                        }

                        var permissionsNode = this.AddNode(
                            managerNode, 
                            "Permissions" + this.CountItems(manager.Permissions), 
                            "permissions", 
                            null);
                        foreach (var permission in manager.Permissions)
                        {
                            this.AddPermission(permissionsNode, permission);
                        }
                    }
                }
                return managerNode;
            }
            finally
            {
                this.hierarchy.EndUpdate();
            }
        }
        #endregion

        #region AddPermission()
        /// <summary>
        /// Adds a new permission permission.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="permission">The permission.</param>
        /// <returns>The new <see cref="TreeNode"/>.</returns>
        private TreeNode AddPermission(TreeNode parent, IPermission permission)
        {
            var node = this.AddNode(
                parent,
                this.GetReflectionName(permission, string.Empty) + ": " + permission.Identifier,
                "permission",
                permission);

            return node;
        }
        #endregion

        #region AddNode()
        /// <summary>
        /// Adds a new node to a treeview.
        /// </summary>
        /// <param name="parent">The parent node.</param>
        /// <param name="text">The node text.</param>
        /// <param name="imageKey">The image key.</param>
        /// <param name="item">The associated item.</param>
        /// <returns>The new <see cref="TreeNode"/>.</returns>
        private TreeNode AddNode(TreeView parent, string text, string imageKey, object item)
        {
            return this.AddNode(parent.Nodes, text, imageKey, item);
        }

        /// <summary>
        /// Adds a new child node.
        /// </summary>
        /// <param name="parent">The parent node.</param>
        /// <param name="text">The node text.</param>
        /// <param name="imageKey">The image key.</param>
        /// <param name="item">The associated item.</param>
        /// <returns>The new <see cref="TreeNode"/>.</returns>
        private TreeNode AddNode(TreeNode parent, string text, string imageKey, object item)
        {
            return this.AddNode(parent.Nodes, text, imageKey, item);
        }

        /// <summary>
        /// Adds a new node.
        /// </summary>
        /// <param name="parent">The parent collection.</param>
        /// <param name="text">The node text.</param>
        /// <param name="imageKey">The image key.</param>
        /// <param name="item">The associated item.</param>
        /// <returns>The new <see cref="TreeNode"/>.</returns>
        private TreeNode AddNode(TreeNodeCollection parent, string text, string imageKey, object item)
        {
            var index = this.localImages.Images.IndexOfKey(imageKey);
            var node = new TreeNode(text, index, index);
            node.Tag = item;
            parent.Add(node);
            return node;
        }
        #endregion

        #region hierarchy_AfterSelect()
        /// <summary>
        /// Handles the AfterSelect event of the hierarchy control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TreeViewEventArgs"/> instance containing the event data.</param>
        private void hierarchy_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag == null)
            {
                this.itemDetails.SelectedObject = null;
            }
            else
            {
                this.itemDetails.SelectedObject = new ConfigurationTypeDescriptor(e.Node.Tag);
            }
        }
        #endregion

        #region GetReflectionName()
        /// <summary>
        /// Gets the name of an item from reflection.
        /// </summary>
        /// <param name="value">The value to get the name from.</param>
        /// <param name="defaultName">The default name.</param>
        /// <returns>The name from reflection if found, the default name otherwise.</returns>
        private string GetReflectionName(object value, string defaultName)
        {
            var valueName = defaultName;
            var reflection = value.GetType().GetCustomAttributes(typeof(ReflectorTypeAttribute), true);
            if (reflection.Length > 0)
            {
                valueName = (reflection[0] as ReflectorTypeAttribute).Name;
            }

            return valueName;
        }
        #endregion

        #region CountItems()
        /// <summary>
        /// Generates a formatted count of the items.
        /// </summary>
        /// <param name="items">The items to count.</param>
        /// <returns>The formatted string.</returns>
        private string CountItems(object[] items)
        {
            return " (" + items.Length.ToString("#,##0", Thread.CurrentThread.CurrentUICulture) + ")";
        }
        #endregion
        #endregion

        #region Private classes
        #region ConfigurationDetails
        /// <summary>
        /// Details on the configuration.
        /// </summary>
        private class ConfigurationDetails
        {
            public ConfigurationDetails(string filename)
            {
                this.Filename = filename;
                var info = new FileInfo(filename);
                this.Size = info.Length;
                this.LastModified = info.LastWriteTime;
            }

            /// <summary>
            /// Gets the name of the source file.
            /// </summary>
            /// <value>The source filename.</value>
            [Description("The location of source file that the configuration was loaded from.")]
            [Category("File")]
            public string Filename { get; set; }

            /// <summary>
            /// Gets the size of the file.
            /// </summary>
            /// <value>The file size in bytes.</value>
            [Description("The size (in bytes) of the source file.")]
            [Category("File")]
            public long Size { get; set; }

            /// <summary>
            /// Gets the date and time the file was last modified.
            /// </summary>
            /// <value>The modification date/time.</value>
            [Description("The the date and tie the source file was last modified.")]
            [Category("File")]
            [DisplayName("Last Modified")]
            public DateTime LastModified { get; set; }
        }
        #endregion
        #endregion
    }
}
