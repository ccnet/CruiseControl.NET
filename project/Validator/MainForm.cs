using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Exortech.NetReflector;
using Manoli.Utils.CSharpFormat;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Security;
using ThoughtWorks.CruiseControl.Core.Config.Preprocessor;

namespace Validator
{
    public partial class MainForm
        : Form, INetReflectorConfigurationReader
    {
        private string myFileName;
        private Stopwatch myStopwatch = new Stopwatch();
        private HtmlElement myBodyEl;
        private const string CONFIG_ASSEMBLY_PATTERN = "ccnet.*.plugin.dll";
        private NetReflectorTypeTable myTypeTable;
        private NetReflectorReader myConfigReader;
        private PersistWindowState myWindowState;
        private List<string> myFileHistory = new List<string>();
        private bool isConfigValid = true;

        public MainForm()
        {
            InitializeComponent();
            InitialiseConfigReader();
            InitialiseDocuments();
            InitialisePersistence();
        }

        private void InitialiseDocuments()
        {
            InitialiseBrowser(validationResults, "Validator.Template.htm");
        }

        private void InitialiseBrowser(WebBrowser browser, string template)
        {
            browser.AllowNavigation = false;
            browser.AllowWebBrowserDrop = false;
            using (Stream xmlStream = this.GetType().Assembly.GetManifestResourceStream(template))
            {
                CompletionClosure.LoadSynchronously(browser, xmlStream);
            }
        }

        private class CompletionClosure
        {
            public WebBrowserDocumentCompletedEventHandler Handler;
            public volatile bool done;
            public CompletionClosure()
            {
                done = false;
                Handler = new WebBrowserDocumentCompletedEventHandler(b_DocumentCompleted);
            }
            void b_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
            {
                done = true;
            }
            public static void LoadSynchronously(WebBrowser b, Stream page)
            {
                CompletionClosure cc = new CompletionClosure();
                b.DocumentCompleted += cc.Handler;
                try
                {
                    b.DocumentStream = page;
                    while (!cc.done)
                    {
                        Application.DoEvents();
                        System.Threading.Thread.Sleep(0);
                    }
                }
                finally
                {
                    b.DocumentCompleted -= cc.Handler;
                }
            }
        }

        /// <summary>
        /// The log file to use.
        /// </summary>
        public string LogFile { get; set; }

        private void InitialiseConfigReader()
        {
            myTypeTable = new NetReflectorTypeTable();
            Assembly thisAssembly = typeof(IProject).Assembly;
            myTypeTable.Add(thisAssembly);
            foreach (AssemblyName referencedAssembly in thisAssembly.GetReferencedAssemblies())
            {
                myTypeTable.Add(Assembly.Load(referencedAssembly));
            }

            var pluginLocation = System.Configuration.ConfigurationManager.AppSettings["PluginLocation"];
            if (!string.IsNullOrEmpty(pluginLocation))
            {
                if (Directory.Exists(pluginLocation))
                {
                    myTypeTable.Add(pluginLocation, CONFIG_ASSEMBLY_PATTERN);
                }
            }

            myTypeTable.Add(Directory.GetCurrentDirectory(), CONFIG_ASSEMBLY_PATTERN);
            myTypeTable.InvalidNode += delegate(InvalidNodeEventArgs args)
            {
                throw new NetReflectorException(args.Message);
            };
            myConfigReader = new NetReflectorReader(myTypeTable);
        }

        private void InitialisePersistence()
        {
            myWindowState = new PersistWindowState();
            myWindowState.Parent = this;
            // set registry path in HKEY_CURRENT_USER
            myWindowState.RegistryPath = @"Software\ThoughtWorks\CCValidator";
            myWindowState.LoadState += new WindowStateEventHandler(OnLoadState);
            myWindowState.SaveState += new WindowStateEventHandler(OnSaveState);
        }

        private void OnLoadState(object sender, WindowStateEventArgs e)
        {
            SetConfigView((ConfigViewMode)Enum.Parse(typeof(ConfigViewMode),
                (string)e.Key.GetValue("ConfigViewMode", ConfigViewMode.Vertical.ToString())));

            for (int loop = 0; loop < 5; loop++)
            {
                string file = e.Key.GetValue("History" + loop.ToString(), null) as string;
                if (file != null) AddFileToHistory(file);
            }
        }

        private void OnSaveState(object sender, WindowStateEventArgs e)
        {
            // save additional state information to registry
            ConfigViewMode mode = ConfigViewMode.Vertical;
            if (horizontalToolStripMenuItem.Checked) mode = ConfigViewMode.Horizontal;
            if (offToolStripMenuItem.Checked) mode = ConfigViewMode.Off;
            e.Key.SetValue("ConfigViewMode", mode.ToString());

            int index = 0;
            foreach (string file in myFileHistory)
            {
                e.Key.SetValue("History" + (index++).ToString(), file);
            }
        }

        private void AddFileToHistory(string fileName)
        {
            if (myFileHistory.Contains(fileName)) myFileHistory.Remove(fileName);
            if (myFileHistory.Count >= 5) myFileHistory.RemoveAt(0);
            myFileHistory.Add(fileName);
            historyMenu.DropDownItems.Clear();
            int position = 0;
            foreach (string file in myFileHistory)
            {
                ToolStripItem item = new ToolStripButton(string.Format("&{0} {1}", myFileHistory.Count - position++, file));
                item.ToolTipText = file;
                item.Click += delegate(object sender, EventArgs e)
                {
                    ToolStripButton button = sender as ToolStripButton;
                    myFileName = button.ToolTipText;
                    AddFileToHistory(myFileName);
                    StartConfigurationLoad();
                };
                item.AutoSize = true;
                historyMenu.DropDownItems.Insert(0, item);
            }

            // The following forces the menu items to be wide enough to display the entire file name
            // Otherwise it only displays as wide as the second-to-last item
            historyMenu.DropDownItems.Add(string.Empty);
            historyMenu.DropDownItems.RemoveAt(historyMenu.DropDownItems.Count - 1);
        }

        private void exitMenuButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void openMenuButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.DefaultExt = "config";
            dialog.DereferenceLinks = true;
            dialog.Filter = "Config files (*.config)|*.config|All files (*.*)|*.*";
            dialog.FilterIndex = 1;
            dialog.Multiselect = false;
            dialog.ShowHelp = false;
            dialog.ShowReadOnly = false;
            dialog.Title = "Select Configuration";
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                myFileName = dialog.FileName;
                AddFileToHistory(myFileName);
                StartConfigurationLoad();
            }
        }

        /// <summary>
        /// Validate a configuration file.
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns></returns>
        public bool ValidateConfig(string configFile)
        {
            isConfigValid = true;
            myFileName = configFile;
            StartConfigurationLoad();
            return isConfigValid;
        }

        /// <summary>
        /// Begin loading the configuration.
        /// </summary>
        private void StartConfigurationLoad()
        {
            DisplayProgressMessage("Loading configuration, please wait...", 0);
            myStopwatch.Reset();
            myStopwatch.Start();
            var loader = new DefaultConfigurationFileLoader(this);
            myBodyEl = validationResults.Document.Body;
            myBodyEl.InnerHtml = string.Empty;
            this.configurationHierarchy.Initialise(myFileName);
            try
            {
                // Attempt to load the configuration
                loader.Load(new FileInfo(myFileName));
            }
            catch (ConfigurationException error)
            {
                // There is an error with the configuration
                var message = "Configuration contains invalid XML: " + error.Message;
                myBodyEl.AppendChild(
                    GenerateElement("div",
                    new HtmlAttribute("class", "error"),
                    GenerateElement("div", message)));
                LogMessage(message);
                isConfigValid = false;
            }
            catch (PreprocessorException error)
            {
                // There was an error with pre-processing
                var message = "Preprocessing failed loading the XML: " + error.Message;
                myBodyEl.AppendChild(
                    GenerateElement("div",
                    new HtmlAttribute("class", "error"),
                    GenerateElement("div", message)));
                LogMessage(message);
                isConfigValid = false;
            }
            catch (Exception error)
            {
                // Catch-all exception block
                StringBuilder message = new StringBuilder();
                message.Append("An unexpected error has occurred while loading the configuration!" +
                    Environment.NewLine +
                    "Please report this error to the CCNet user group (http://groups.google.com/group/ccnet-user). This will help us to improve this application.");
                Exception currentError = error;
                while (currentError != null)
                {
                    message.AppendFormat("{0}{1} [{2}]", Environment.NewLine, currentError.Message, currentError.GetType().Name);
                    message.AppendFormat("{0}{1}", Environment.NewLine, currentError.StackTrace);
                    currentError = currentError.InnerException;
                    if (currentError != null)
                    {
                        message.AppendFormat("{0}{1} Inner Exception {1}", Environment.NewLine, new string('=', 10));
                    }
                }
                MessageBox.Show(this, message.ToString(), "Unexpected error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isConfigValid = false;
            }
        }
        private void DisplayFileName()
        {
            HtmlElement nameEl = GenerateElement("div",
                new HtmlAttribute("class", "fileName"),
                GenerateElement("b", "Configuration file: "),
                myFileName);
            myBodyEl.AppendChild(nameEl);
            LogMessage(string.Format("Validating {0}", myFileName));
        }

        private HtmlElement GenerateElement(string tagName, params object[] contents)
        {
            HtmlElement element = validationResults.Document.CreateElement(tagName);
            foreach (object contentEl in contents)
            {
                if (contentEl is HtmlElement)
                {
                    element.AppendChild(contentEl as HtmlElement);
                }
                else if (contentEl is HtmlAttribute)
                {
                    HtmlAttribute attrbite = contentEl as HtmlAttribute;
                    element.SetAttribute(attrbite.Name, attrbite.Value);
                }
                else
                {
                    HtmlElement spanEl = validationResults.Document.CreateElement("span");
                    spanEl.InnerText = contentEl.ToString();
                    element.AppendChild(spanEl);
                }
            }
            return element;
        }

        public IConfiguration Read(XmlDocument document, IConfigurationErrorProcesser errorProcesser)
        {
            DisplayFileName();
            DisplayConfig();

            DisplayProgressMessage("Validating configuration, please wait...", 10);
            ValidateData(document);

            LoadCompleted();
            return null;
        }

        private void DisplayConfig()
        {
            using (StreamReader sr = new StreamReader(myFileName))
            {
                xmlDisplay.IsReadOnly = false;
                xmlDisplay.Text = sr.ReadToEnd();
                xmlDisplay.IsReadOnly = true;
            }
        }

        private void DisplayProgressMessage(string message, int progress)
        {
            progressLabel.Text = message;
            progressBar.Value = progress;
            Application.DoEvents();
        }

        private void ValidateData(XmlDocument document)
        {
            var rootElement = document.DocumentElement.Name == "cruisecontrol" ? document.DocumentElement : null;
            if (rootElement == null)
            {
                var message = "Configuration is missing root <cruisecontrol> element";
                myBodyEl.AppendChild(
                    GenerateElement("div",
                    new HtmlAttribute("class", "error"),
                    GenerateElement("div", message)));
                LogMessage(message);
                isConfigValid = false;
            }
            else
            {
                HtmlElement tableEl = GenerateElement("table",
                    new HtmlAttribute("class", "results"),
                    GenerateHeader());
                Configuration configuration = new Configuration();
                if (!string.IsNullOrEmpty(rootElement.NamespaceURI))
                {
                    var parts = rootElement.NamespaceURI.Split('/');
                    var version = parts[parts.Length - 2] + "." + parts[parts.Length - 1];
                    myBodyEl.AppendChild(GenerateElement("div", "Target Version: " + version));
                }
                else
                {
                    myBodyEl.AppendChild(GenerateElement("div", "No version information"));
                }

                XmlNodeList nodes = rootElement.SelectNodes("*");
                int row = 0;
                double increment = (double)80 / nodes.Count;
                List<ConfigurationItem> items = new List<ConfigurationItem>();
                foreach (XmlNode childElement in nodes)
                {
                    DisplayProgressMessage("Validating elements, please wait...", Convert.ToInt32(10 + row * increment));
                    object config = ValidateElement(tableEl, childElement, row++, configuration);
                    if (config != null)
                    {
                        items.Add(new ConfigurationItem(childElement.Name, config));
                    }
                    else
                    {
                        isConfigValid = false;
                    }
                }

                myBodyEl.AppendChild(tableEl);
                isConfigValid &= InternalValidation(configuration);

                DisplayProcessedConfiguration(items);
                this.configurationHierarchy.Finalise();
            }
        }

        private void DisplayProcessedConfiguration(List<ConfigurationItem> config)
        {
            StringWriter buffer = new StringWriter();
            XmlTextWriter writer = new XmlTextWriter(buffer);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartElement("cruisecontrol");
            foreach (ConfigurationItem item in config)
            {
                new ReflectorTypeAttribute(item.Name).Write(writer, item.Configuration);
            }
            writer.WriteEndElement();

            processedDisplay.IsReadOnly = false;
            processedDisplay.Text = buffer.ToString();
            processedDisplay.IsReadOnly = true;
        }


        private object ValidateElement(HtmlElement tableEl, XmlNode node, int row, Configuration configuration)
        {
            HtmlAttribute rowClass = new HtmlAttribute("class", (row % 2) == 1 ? "even" : "odd");
            object loadedItem = null;
            try
            {
                loadedItem = myConfigReader.Read(node);
                this.configurationHierarchy.Add(loadedItem);
                if (loadedItem is IProject)
                {
                    IProject project = loadedItem as IProject;
                    configuration.AddProject(project);
                    tableEl.AppendChild(
                        GenerateElement("tr",
                            rowClass,
                            GenerateElement("td", project.Name),
                            GenerateElement("td", "Project"),
                            GenerateElement("td", "Yes")));
                    LogMessage(string.Format("Loaded project '{0}'", project.Name));
                }
                else if (loadedItem is IQueueConfiguration)
                {
                    IQueueConfiguration queueConfig = loadedItem as IQueueConfiguration;
                    configuration.QueueConfigurations.Add(queueConfig);
                    tableEl.AppendChild(
                        GenerateElement("tr",
                            rowClass,
                            GenerateElement("td", queueConfig.Name),
                            GenerateElement("td", "Queue"),
                            GenerateElement("td", "Yes")));
                    LogMessage(string.Format("Loaded queue '{0}'", queueConfig.Name));
                }
                else if (loadedItem is ISecurityManager)
                {
                    ISecurityManager securityManager = loadedItem as ISecurityManager;
                    configuration.SecurityManager = securityManager as ISecurityManager;
                    LogMessage("Loaded security manager");
                }
                else
                {
                    tableEl.AppendChild(
                        GenerateElement("tr",
                            rowClass,
                            GenerateElement("td", (node as XmlElement).GetAttribute("name")),
                            GenerateElement("td", node.Name),
                            GenerateElement("td", "No")));
                    var message = "Unknown configuration type: " + loadedItem.GetType().Name;
                    tableEl.AppendChild(
                        GenerateElement("tr",
                            rowClass,
                            GenerateElement("td",
                                new HtmlAttribute("colspan", "3"),
                                GenerateElement("div",
                                    new HtmlAttribute("class", "error"),
                                    message))));
                    LogMessage(message);
                    isConfigValid = false;
                }
            }
            catch (Exception error)
            {
                string errorMsg = error.Message;
                int index = errorMsg.IndexOf("Xml Source");
                if (index >= 0) errorMsg = errorMsg.Substring(0, index - 1);
                tableEl.AppendChild(
                    GenerateElement("tr",
                        rowClass,
                        GenerateElement("td", (node as XmlElement).GetAttribute("name")),
                        GenerateElement("td", node.Name),
                        GenerateElement("td", "No")));
                tableEl.AppendChild(
                    GenerateElement("tr",
                        rowClass,
                        GenerateElement("td",
                            new HtmlAttribute("colspan", "3"),
                            GenerateElement("div",
                                new HtmlAttribute("class", "error"),
                                errorMsg))));
                isConfigValid = false;
                LogMessage(error.Message);
            }

            return loadedItem;
        }

        private HtmlElement GenerateHeader()
        {
            HtmlElement element = GenerateElement("tr",
                GenerateElement("th", "Item"),
                GenerateElement("th", "Type"),
                GenerateElement("th", "Valid"));
            return element;
        }

        private void LoadCompleted()
        {
            // The following line is needed to make the browser display the styles correctly!
            myBodyEl.InnerHtml = myBodyEl.InnerHtml;
            myStopwatch.Stop();
            var message = string.Format("Configuration loaded ({0:0.00}s)",
                Convert.ToDouble(myStopwatch.ElapsedMilliseconds) / 1000);
            DisplayProgressMessage(message, 100);
            LogMessage(message);
        }

        private void reloadMenuButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(myFileName))
            {
                MessageBox.Show(this, "Reload can only be used after a file has loaded",
                    "Functionality not available", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                StartConfigurationLoad();
            }
        }

        private void printMenuButton_Click(object sender, EventArgs e)
        {
            validationResults.ShowPrintPreviewDialog();
        }

        private void vericalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetConfigView(ConfigViewMode.Vertical);
        }

        private void horizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetConfigView(ConfigViewMode.Horizontal);
        }

        private void offToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetConfigView(ConfigViewMode.Off);
        }

        private void SetConfigView(ConfigViewMode mode)
        {
            vericalToolStripMenuItem.Checked = (mode == ConfigViewMode.Vertical);
            horizontalToolStripMenuItem.Checked = (mode == ConfigViewMode.Horizontal);
            offToolStripMenuItem.Checked = (mode == ConfigViewMode.Off);
            resultsDisplay.Panel2Collapsed = (mode == ConfigViewMode.Off);
            resultsDisplay.Orientation = (mode == ConfigViewMode.Horizontal) ? Orientation.Horizontal : Orientation.Vertical;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm about = new AboutForm();
            about.ShowDialog(this);
        }

        private struct ConfigurationItem
        {
            public string Name;
            public object Configuration;

            public ConfigurationItem(string name, object config)
            {
                Name = name;
                Configuration = config;
            }
        }

        private bool InternalValidation(Configuration configuration)
        {
            var errorProcesser = new ValidationErrorProcesser(validationResults);
            DisplayProgressMessage("Validating internal integrity, please wait...", 90);

            HtmlElement nameEl = GenerateElement("div",
                new HtmlAttribute("class", "titleLine"),
                GenerateElement("b", "Internal validation"));
            myBodyEl.AppendChild(nameEl);
            bool isValid = true;
            int row = 0;

            foreach (IProject project in configuration.Projects)
            {
                if (project is IConfigurationValidation)
                {
                    errorProcesser.ItemName = string.Format("project '{0}'", project.Name);
                    isValid &= RunValidationCheck(configuration, project as IConfigurationValidation, errorProcesser.ItemName, ref row, errorProcesser);
                }
            }

            foreach (IQueueConfiguration queue in configuration.QueueConfigurations)
            {
                if (queue is IConfigurationValidation)
                {
                    errorProcesser.ItemName = string.Format("queue '{0}'", queue.Name);
                    isValid &= RunValidationCheck(configuration, queue as IConfigurationValidation, errorProcesser.ItemName, ref row, errorProcesser);
                }
            }

            if (configuration.SecurityManager is IConfigurationValidation)
            {
                errorProcesser.ItemName = "security manager";
                isValid &= RunValidationCheck(configuration, configuration.SecurityManager as IConfigurationValidation, errorProcesser.ItemName, ref row, errorProcesser);
            }

            if (isValid && errorProcesser.Passed)
            {
                var message = "Internal validation passed";
                myBodyEl.AppendChild(
                    GenerateElement("div",
                    message));
                LogMessage(message);
            }
            return isValid;
        }

        private bool RunValidationCheck(Configuration configuration, IConfigurationValidation validator, string name, ref int row, IConfigurationErrorProcesser errorProcesser)
        {
            bool isValid = true;

            try
            {
                validator.Validate(configuration, null, errorProcesser);
            }
            catch (Exception error)
            {
                var message = string.Format("Internal validation failed for {0}: {1}",
                            name,
                            error.Message);
                HtmlAttribute rowClass = new HtmlAttribute("class", (row % 2) == 1 ? "even" : "odd");
                myBodyEl.AppendChild(
                    GenerateElement("div",
                        rowClass,
                        GenerateElement("div",
                        new HtmlAttribute("class", "error"),
                        message)));
                LogMessage(message);
                isValid = false;
                row++;
            }
            return isValid;
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            openMenuButton_Click(sender, e);
        }

        private void buttonReload_Click(object sender, EventArgs e)
        {
            reloadMenuButton_Click(sender, e);
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            printMenuButton_Click(sender, e);
        }

        public void LogMessage(string message)
        {
            if (!string.IsNullOrEmpty(LogFile))
            {
                File.AppendAllText(LogFile,
                    string.Format("{0:o} {1}", DateTime.Now, message) + Environment.NewLine);
            }
        }
    }
}
