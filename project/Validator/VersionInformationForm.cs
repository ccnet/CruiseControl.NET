using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Exortech.NetReflector;
using System.Reflection;

namespace Validator
{
    /// <summary>
    /// Displays the loaded assemblies for NetReflector.
    /// </summary>
    public partial class VersionInformationForm 
        : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionInformationForm"/> class.
        /// </summary>
        public VersionInformationForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads the version information from the type table.
        /// </summary>
        /// <param name="typeTable">The type table.</param>
        public void LoadInformation(NetReflectorTypeTable typeTable)
        {
            // Sort the assemblies and types
            var assemblies = new Dictionary<Assembly, SortedDictionary<string, Type>>();
            foreach (IXmlTypeSerialiser type in typeTable)
            {
                SortedDictionary<string, Type> types;
                var assembly = type.Type.Assembly;
                if (assemblies.ContainsKey(assembly))
                {
                    types = assemblies[assembly];
                }
                else
                {
                    types = new SortedDictionary<string, Type>();
                    assemblies.Add(assembly, types);
                }

                types.Add(type.Attribute.Name, type.Type);
            }

            foreach (var assembly in assemblies)
            {
                // Make sure the assembly has been added
                var name = assembly.Key.GetName();
                var parentNode = new TreeNode(name.Name + " [" + name.Version.ToString() + "]");
                this.versionInformation.Nodes.Add(parentNode);
                
                // Add the types
                foreach (var type in assembly.Value)
                {
                    parentNode.Nodes.Add(new TreeNode(type.Key, 1, 1));
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the closeButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
