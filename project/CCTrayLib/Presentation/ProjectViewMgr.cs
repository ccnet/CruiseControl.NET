using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;


using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
    public partial class CCProjectsViewMgr : UserControl
    {
        MainForm mainForm;
        private TabControl m_tabControl;
        private Panel CCProjectsViewMgrPanel;
        List<CCProjectsView> m_projectViews;
        public CCProjectsView m_all, m_broken; // HACK
        CCProjectsView m_current = null;

        // HACK TexBox Hack is awful
        bool m_bUsingTextBox = false;
        public delegate void EditText(Control[] t);
        EditText m_et;
        TextBox m_tb;
        Control[] m_controls;

        private ImageList smallImageList;
        private ImageList largeImageList;
        System.Threading.Timer m_viewUpdate;
        private bool m_bTimerRunning;

        private ToolStripMenuItem mnuForce;
        private ToolStripMenuItem mnuStart;
        private ToolStripMenuItem mnuStop;
        private ToolStripMenuItem mnuAbort;
        private ToolStripMenuItem mnuWebPage;
        private ToolStripMenuItem mnuCancelPending;
        private ToolStripMenuItem mnuCopyBuildLabel;
        private ToolStripMenuItem mnuSendToNewTab;
        private ToolStripMenuItem mnuCreateTabFromPattern;
        private ToolStripMenuItem mnuCreateTabFromCategory;
        private ToolStripMenuItem mnuFixBuild;
        public ContextMenuStrip projectContextMenu; //HACK made member public

        public ContextMenuStrip tabContextMenu; //HACK made member public
        private ToolStripMenuItem mnuEditName;
        private ToolStripMenuItem mnuDeleteTab;

        public CCProjectsView GetAllProjectsView()
        {
            return m_projectViews[0];
        }

        public TabControl TabControl
        {
            get { return m_tabControl; }
        }


        public MainForm MainForm
        {
            get { return mainForm; }
        }

        public void SetIconLists(ImageList large, ImageList small)
        {
            largeImageList = large;
            smallImageList = small;
        }

        public ImageList LargeImageList
        {
            get { return largeImageList; }
        }

        public ImageList SmallImageList
        {
            get { return smallImageList; }
        }

        public CCProjectsViewMgr(MainForm form, ImageList large, ImageList small)
            : base()
        {
            mainForm = form;
            this.SuspendLayout();
            largeImageList = large;
            smallImageList = small;


            CCProjectsViewMgrPanel = new System.Windows.Forms.Panel();
            CCProjectsViewMgrPanel.Tag = this;
            Controls.Add(CCProjectsViewMgrPanel);
            CCProjectsViewMgrPanel.Location = new System.Drawing.Point(0, 0);
            CCProjectsViewMgrPanel.Name = "CCProjectsViewMgrPanel";
            CCProjectsViewMgrPanel.BackColor = Color.AliceBlue;

            CCProjectsViewMgrPanel.MouseDoubleClick += new MouseEventHandler(CCProjectsViewMgrPanel_MouseDoubleClick);

            CCProjectsViewMgrPanel.Dock = DockStyle.Fill;

            // project views are added to the tabcontrol so it needs to be created first

            m_tabControl = new TabControl();
            m_tabControl.SuspendLayout();
            m_tabControl.Name = "tabControl";
            m_tabControl.SelectedIndex = 0;
            m_tabControl.TabIndex = 0;
            m_tabControl.MouseUp += new MouseEventHandler(TabControl_MouseUp);
            m_tabControl.MouseClick += new MouseEventHandler(CCProjectsViewMgr_MouseClick);
            m_tabControl.AllowDrop = true;
            m_tabControl.DragEnter += new DragEventHandler(TabControl_DragEnter);
            m_tabControl.DragDrop += new DragEventHandler(TabControl_DragDrop);
            m_tabControl.DragOver += new DragEventHandler(TabControl_DragOver);
            m_tabControl.DragLeave += new EventHandler(m_tabControl_DragLeave);
            m_tabControl.Dock = DockStyle.Fill;
            m_tabControl.ImageList = smallImageList;
            CCProjectsViewMgrPanel.Controls.Add(m_tabControl);

            form.DragEnter += new DragEventHandler(form_DragEnter);

            // create the container of views
            m_projectViews = new List<CCProjectsView>();

            // the 'all' view always has all the projects the user has subscribed to
            m_all = new CCProjectsView(this, "All");
            m_all.IsUserView = false;

            AddView(m_all);


            // the 'broken' view is dynamically updated with any broken projects
            m_broken = new CCProjectsView(this, "Broken");
            m_broken.IsUserView = false;
            m_broken.IsReadOnly = false;
            m_broken.TabPage.ImageIndex = ProjectState.Broken.ImageIndex;
            m_broken.TabPage.TabIndex = 1;

            // we don't add it to make  iterating over 'real' views more easy
            AddView(m_broken);

            this.ResumeLayout(false);
            m_tabControl.ResumeLayout(false);

            form.dummyDockingPanel.Controls.Add(CCProjectsViewMgrPanel);

            #region // menus

            projectContextMenu = new System.Windows.Forms.ContextMenuStrip();

            mnuForce = new System.Windows.Forms.ToolStripMenuItem();
            mnuAbort = new System.Windows.Forms.ToolStripMenuItem();
            mnuStart = new System.Windows.Forms.ToolStripMenuItem();
            mnuStop = new System.Windows.Forms.ToolStripMenuItem();
            mnuWebPage = new System.Windows.Forms.ToolStripMenuItem();
            mnuCancelPending = new System.Windows.Forms.ToolStripMenuItem();
            mnuFixBuild = new System.Windows.Forms.ToolStripMenuItem();
            mnuCopyBuildLabel = new System.Windows.Forms.ToolStripMenuItem();
            mnuSendToNewTab = new System.Windows.Forms.ToolStripMenuItem();
            mnuCreateTabFromPattern = new System.Windows.Forms.ToolStripMenuItem();
            mnuCreateTabFromCategory = new System.Windows.Forms.ToolStripMenuItem();

            projectContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripMenuItem[] 
         {
            mnuForce,
            mnuAbort,
            mnuStart,
            mnuStop,
            mnuWebPage,
            mnuCancelPending,
            mnuFixBuild,
            mnuCopyBuildLabel,
            mnuSendToNewTab,
            mnuCreateTabFromPattern,
            mnuCreateTabFromCategory
         });

            mnuForce.Text = "&Force Build";
            mnuForce.Click += new System.EventHandler(mnuForce_Click);
            mnuAbort.Text = "&Abort Build";
            mnuAbort.Click += new System.EventHandler(mnuAbort_Click);
            mnuStart.Text = "&Start Project";
            mnuStart.Click += new System.EventHandler(mnuStart_Click);
            mnuStop.Text = "&Stop Project";
            mnuStop.Click += new System.EventHandler(mnuStop_Click);
            mnuWebPage.Text = "Display &Web Page";
            mnuWebPage.Click += new System.EventHandler(mnuWebPage_Click);
            mnuCancelPending.Text = "&Cancel Pending";
            mnuCancelPending.Click += new System.EventHandler(mnuCancelPending_Click);
            mnuFixBuild.Text = "&Volunteer to Fix Build";
            mnuFixBuild.Click += new System.EventHandler(mnuFixBuild_Click);
            mnuCopyBuildLabel.Text = "Copy Build &Label";
            mnuCopyBuildLabel.Click += new System.EventHandler(mnuCopyBuildLabel_Click);

            mnuSendToNewTab.Text = "Send to New &Tab";
            mnuSendToNewTab.Click += new EventHandler(mnuSendToNewTab_Click);

            mnuCreateTabFromPattern.Text = "New Tab from &Pattern";
            mnuCreateTabFromPattern.Click += new EventHandler(mnuCreateTabFromPattern_Click);

            mnuCreateTabFromCategory.Text = "Create tabs from category";
            mnuCreateTabFromCategory.Click += new EventHandler(mnuCreateTabFromCategory_Click);

            tabContextMenu = new System.Windows.Forms.ContextMenuStrip();
            mnuEditName = new System.Windows.Forms.ToolStripMenuItem();
            mnuDeleteTab = new System.Windows.Forms.ToolStripMenuItem();
            tabContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripMenuItem[] {
                mnuEditName,
                mnuDeleteTab, 
            });

            mnuEditName.Text = "&Rename Tab";
            mnuEditName.Click += new System.EventHandler(mnuEditName_Click);
            mnuDeleteTab.Text = "&Delete Tab";
            mnuDeleteTab.Click += new System.EventHandler(mnuDeleteTab_Click);

            #endregion

            mainForm.MainFormController.BindToViewMgr(this);

            // restore users tabs
            LoadViewConfiguration();

            // colour in the icons on each tab
            UpdateAllTabPageIcons(null);


            // set up the general edit box
            // this needs a tidy up
            m_tb = new TextBox();
            m_tb.PreviewKeyDown += new PreviewKeyDownEventHandler(tb_PreviewKeyDown);
            m_tb.LostFocus += new EventHandler(tb_LostFocus);

            int timerPeriod = mainForm.Configuration.PollPeriodSeconds * 1000;
            m_bTimerRunning = false;
            m_viewUpdate = new System.Threading.Timer(new System.Threading.TimerCallback(UpdateAllTabPageIcons), this, 0, timerPeriod);

            this.Show();
        }

        void form_DragEnter(object sender, DragEventArgs e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void m_tabControl_DragLeave(object sender, EventArgs e)
        {
            Console.WriteLine("Drag leave");
        }


        // callback for when user has entered a pattern to create a new tablist from
        void doneTextEdit(Control[] t)
        {
            string pattern = t[0].Text.ToUpper();

            CCProjectsView view = new CCProjectsView(this, "");
            AddView(view);
            foreach (ListViewItem lvi in m_current.ListView.Items)
            {
                string s = lvi.SubItems[0].Text.ToUpper() + lvi.SubItems[1].Text.ToUpper();
                if (s.Contains(pattern))
                {
                    IProjectMonitor monitor = (IProjectMonitor)lvi.Tag;
                    ListViewItem item = mainForm.MainFormController.CopyBoundProject(monitor);
                    view.AddProject(item);
                }
            }

            TabControl.SelectedTab = view.TabPage;
            view.TabPage.Text = t[0].Text;
            view.Text = t[0].Text;
            m_current = view;

            Rectangle rect = TabControl.GetTabRect(TabControl.SelectedIndex);

            Point p = new Point(rect.Left, rect.Top);
            if (m_bUsingTextBox == false)
                RenameView(p, view.TabPage.Text, new Control[] { view, view.TabPage }, null);
        }

        void mnuCreateTabFromPattern_Click(object sender, EventArgs e)
        {
            Control t = new Control();
            Point p = new Point(0, 0);

            ToolStripMenuItem ts = (ToolStripMenuItem)sender;

            p = ts.GetCurrentParent().Location;
            p = TabControl.PointToClient(p);


            EditText doneEditing = new EditText(doneTextEdit);
            RenameView(p, "Enter text here", new Control[] { t }, doneEditing);
        }

        void mnuCreateTabFromCategory_Click(object sender, EventArgs e)
        {
            // clear existing user views



            for (int ccpvcounter = m_projectViews.Count - 1; ccpvcounter >= 0; ccpvcounter--)
            {
                CCProjectsView ccpv = m_projectViews[ccpvcounter];
                
                if (ccpv.IsUserView)
                {
                    ccpv.ListView.Items.Clear();
                    m_tabControl.TabPages.Remove(ccpv.TabPage);
                    ccpv.TabPage.Dispose();
                    m_projectViews.Remove(ccpv);
                }
            }



            // get all categories of the monitored projects
            System.Collections.Generic.List<string> Categories = new List<string>();

            foreach (IProjectMonitor project in this.mainForm.MainFormController.Monitors)
            {
                if (!Categories.Contains(project.Detail.Category)) Categories.Add(project.Detail.Category);
            }

            // for each category, create a tab and enter the corresponding projects to it
            foreach (string category in Categories)
            {
                CCProjectsView categoryTab = new CCProjectsView(this, category);
                categoryTab.IsUserView = true;

                AddView(categoryTab);

                // get the projects from this category
                foreach (IProjectMonitor project in this.mainForm.MainFormController.Monitors)
                {
                    if (project.Detail.Category == category)
                    {
                        ListViewItem item = mainForm.MainFormController.CopyBoundProject(project);
                        categoryTab.AddProject(item);
                    }
                }
            }
        }


        void mnuSendToNewTab_Click(object sender, EventArgs e)
        {
            CCProjectsView view = new CCProjectsView(this, "");
            AddView(view);
            foreach (ListViewItem lvi in m_current.ListView.SelectedItems)
            {
                IProjectMonitor monitor = (IProjectMonitor)lvi.Tag;
                ListViewItem item = mainForm.MainFormController.CopyBoundProject(monitor);
                view.AddProject(item);
            }

            TabControl.SelectedTab = view.TabPage;
            m_current = view;

            Rectangle rect = TabControl.GetTabRect(TabControl.SelectedIndex);

            Point p = new Point(rect.Left, rect.Top);
            if (m_bUsingTextBox == false)
                RenameView(p, view.TabPage.Text, new Control[] { view, view.TabPage }, null);
        }

        private string GetConfigFilename()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "cctray-tabs.xml");
        }

        public void LoadViewConfiguration()
        {
            string fname = GetConfigFilename();
            if (!File.Exists(fname))
                return;

            FileStream reader = new FileStream(fname, FileMode.Open, FileAccess.Read, FileShare.Read);
            System.Xml.XmlTextReader xmlReader = new XmlTextReader(reader);

            try
            {
                xmlReader.ReadStartElement("Views");
                while (xmlReader.Read())
                {
                    if (xmlReader.AttributeCount > 0)
                    {
                        string viewName = xmlReader.GetAttribute(0);

                        bool bViewAdded = false;

                        // skip "View"
                        xmlReader.Read();
                        xmlReader.Read();

                        CCProjectsView view = new CCProjectsView(this, viewName);

                        while (xmlReader.Name == "Project")
                        {

                            string ProjectName = xmlReader.GetAttribute(0);
                            xmlReader.Read();
                            xmlReader.Read();

                            string ServerName = xmlReader.GetAttribute(0);
                            xmlReader.Read();
                            xmlReader.Read();

                            CCProjectsView viewAll = m_projectViews[0];

                            ListViewItem lvi = viewAll.Contains(ProjectName);

                            if (lvi != null)
                            {
                                IProjectMonitor monitor = (IProjectMonitor)lvi.Tag;
                                ListViewItem item = mainForm.MainFormController.CopyBoundProject(monitor);

                                if (!bViewAdded)
                                {
                                    AddView(view);
                                    bViewAdded = true;
                                }

                                view.AddProject(item);

                            }
                        }
                    }
                }


                xmlReader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void SaveViewConfiguration()
        {

            FileStream writer = new FileStream(GetConfigFilename(), FileMode.Create);
            XmlTextWriter xmlWriter = new XmlTextWriter(writer, Encoding.UTF8); ;

            xmlWriter.Formatting = Formatting.Indented;

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Views");

            foreach (CCProjectsView view in m_projectViews)
            {
                view.SaveViewConfiguration(xmlWriter);
            }

            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
            writer.Close();
        }

        private void projectContextMenuFilterItems()
        {
            mnuForce.Visible = mainForm.MainFormController.IsProjectSelected && !mainForm.MainFormController.IsProjectBuilding;
            mnuAbort.Visible = mainForm.MainFormController.IsProjectSelected && mainForm.MainFormController.IsProjectBuilding;
            mnuStart.Visible = mainForm.MainFormController.IsProjectSelected && !mainForm.MainFormController.IsProjectRunning;
            mnuStop.Visible = mainForm.MainFormController.IsProjectSelected && mainForm.MainFormController.IsProjectRunning;
            mnuWebPage.Enabled = mainForm.MainFormController.IsProjectSelected;
            mnuCancelPending.Visible = mainForm.MainFormController.CanCancelPending();
            mnuFixBuild.Visible = mainForm.MainFormController.CanFixBuild();
            mnuCopyBuildLabel.Visible = mainForm.MainFormController.IsProjectSelected;
        }

        #region popup menu callbacks
        private void mnuForce_Click(object sender, EventArgs e)
        {
            mainForm.MainFormController.ForceBuild();
        }

        private void mnuAbort_Click(object sender, EventArgs e)
        {
            mainForm.MainFormController.AbortBuild();
        }

        private void mnuCopyBuildLabel_Click(object sender, EventArgs e)
        {
            mainForm.MainFormController.CopyBuildLabel();
        }
        private void mnuCancelPending_Click(object sender, EventArgs e)
        {
            mainForm.MainFormController.CancelPending();
        }

        private void mnuStart_Click(object sender, EventArgs e)
        {
            mainForm.MainFormController.StartProject();
        }

        private void mnuStop_Click(object sender, EventArgs e)
        {
            mainForm.MainFormController.StopProject();
        }

        private void mnuWebPage_Click(object sender, EventArgs e)
        {
            mainForm.MainFormController.DisplayWebPage();
        }

        public void lv_MouseDoubleClick(object sender, EventArgs e)
        {
            mainForm.MainFormController.DisplayWebPage();
        }

        private void mnuFixBuild_Click(object sender, EventArgs e)
        {
            mainForm.MainFormController.VolunteerToFixBuild();
        }
        #endregion

        private void mnuEditName_Click(object sender, EventArgs e)
        {
            Rectangle rect = TabControl.GetTabRect(TabControl.SelectedIndex);
            Point p = new Point(rect.Left, rect.Top);

            CCProjectsView view = m_current;
            if (m_bUsingTextBox == false)
                RenameView(p, m_current.TabPage.Text, new Control[] { view, view.TabPage }, null);
        }

        private void mnuDeleteTab_Click(object sender, EventArgs e)
        {
            if (m_current.IsUserView == false)
                return;

            m_current.ListView.Items.Clear();
            m_tabControl.TabPages.Remove(m_current.TabPage);
            m_current.TabPage.Dispose();
            m_projectViews.Remove(m_current);

        }


        void TabControl_DragDrop(object sender, DragEventArgs e)
        {
            Point p = m_tabControl.PointToClient(new Point(e.X, e.Y));

            // probably shouldn't need to do this 
            TabPage t = FindTabControl(p);
            if (t != null)
            {
                if (m_tabControl.SelectedTab != t)
                    Console.WriteLine("Ouch trying to drop onto a tab which isn't selected!!!");
            }

            e.Effect = DragDropEffects.Copy;

            if (e.Data.GetDataPresent(typeof(ListView.SelectedListViewItemCollection)) == false)
                Console.WriteLine("no data");

            ListView.SelectedListViewItemCollection items = (ListView.SelectedListViewItemCollection)e.Data.GetData(typeof(ListView.SelectedListViewItemCollection));
            foreach (ListViewItem lvi in items)
            {
                IProjectMonitor monitor = (IProjectMonitor)lvi.Tag;
                ListViewItem item = mainForm.MainFormController.CopyBoundProject(monitor);
                m_current.AddProject(item);
            }
        }

        void TabControl_DragOver(object sender, DragEventArgs e)
        {
            Console.WriteLine("tb dragover");
            TabControl_DragEnter(sender, e);
        }

        void TabControl_DragEnter(object sender, DragEventArgs e)
        {
            Console.WriteLine("tb dragenter");
            Point p = m_tabControl.PointToClient(new Point(e.X, e.Y));

            TabPage tp = FindTabControl(p);
            CCProjectsView v = (CCProjectsView)tp.Tag;
            if (v.IsUserView)
            {
                e.Effect = DragDropEffects.Copy;
                SelectClickedTab(p);
                UpdateCurrentView();
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        void CCProjectsViewMgrPanel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            CCProjectsViewMgr viewMgr = (CCProjectsViewMgr)((Panel)sender).Tag;
            CCProjectsView view = new CCProjectsView(viewMgr, "");
            AddView(view);
        }

        public TabPage FindTabControl(Point p)
        {
            for (int i = 0; i < m_tabControl.TabCount; i++)
            {
                // get their rectangle area and check if it contains the mouse cursor
                Rectangle r = m_tabControl.GetTabRect(i);
                r.Inflate(0, 3);

                if (r.Contains(p))
                    return m_tabControl.TabPages[i];
            }
            return null;
        }

        void TabControl_MouseUp(object sender, MouseEventArgs e)
        {
            SelectClickedTab(e.Location);
            UpdateCurrentView();
        }

        public void AddView(CCProjectsView view)
        {
            if (m_projectViews.Count == 0)
                m_current = view;


            if (m_broken != null)
                m_tabControl.Controls.Remove(m_broken.TabPage);

            m_projectViews.Add(view);
            if (view != m_broken)
            {
                view.TabPage.TabIndex = m_tabControl.Controls.Count;
                m_tabControl.Controls.Add(view.TabPage);
            }


            if (m_broken != null)
            {
                m_broken.TabPage.TabIndex = m_tabControl.Controls.Count;
                m_tabControl.Controls.Add(m_broken.TabPage);
            }

        }

        /// <summary>
        /// Edit Box
        /// </summary>

        void RenameView(Point p, string initialText, Control[] controls, EditText t)
        {
            if (m_bUsingTextBox)
                return;
            m_bUsingTextBox = true;
            m_et = t;

            m_controls = new Control[controls.GetLength(0)];
            controls.CopyTo(m_controls, 0);

            m_tb.Text = initialText;
            this.m_tabControl.Parent.Controls.Add(m_tb);
            m_tb.BringToFront();
            m_tb.Focus();
            m_tb.Location = p;
            m_tb.Show();
        }


        void CloseRenameView(bool bUpdated)
        {
            this.m_tabControl.Parent.Controls.Remove(m_tb);
            m_bUsingTextBox = false;
            if (bUpdated)
            {
                foreach (Control c in m_controls)
                {
                    c.Text = m_tb.Text;
                }

                if (m_et != null)
                    m_et(m_controls);
            }
        }

        void tb_LostFocus(object sender, EventArgs e)
        {
            CloseRenameView(false);
        }


        void tb_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CloseRenameView(true);
            }
            if (e.KeyCode == Keys.Escape)
            {
                CloseRenameView(false);
            }
        }


        //////////////////////////////////////////////////////////////////////////////////





        void UpdateCurrentView()
        {
            TabPage selected = this.m_tabControl.SelectedTab;
            foreach (CCProjectsView ccpv in m_projectViews)
            {
                if (ccpv.TabPage == selected)
                {
                    m_current = ccpv;
                    this.m_tabControl.SelectedTab = m_current.TabPage;
                }
            }
        }


        void SelectClickedTab(Point p)
        {
            TabPage t = FindTabControl(p);
            if (t != null)
            {
                m_tabControl.SelectedTab = t;
            }
        }

        void CCProjectsViewMgr_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                SelectClickedTab(e.Location);
                UpdateCurrentView();
                // Create TabControl Popup
                mnuDeleteTab.Visible = (m_current.Text != "All");
                mnuEditName.Visible = (m_current.Text != "All");
                tabContextMenu.Tag = e.Location;
                tabContextMenu.Show(m_tabControl.PointToScreen(e.Location));
            }
            else
            {
                if (e.Button == MouseButtons.Middle)
                {
                    SelectClickedTab(e.Location);
                    UpdateCurrentView();

                    mnuDeleteTab_Click(sender, e);
                }
            }
        }

        public void CCProjectViews_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_current.ListView.SelectedItems.Count == 0)
            {
                mainForm.MainFormController.SelectedProject = null;
                mainForm.tltBuildStage.RemoveAll();
            }
            else
            {
                mainForm.MainFormController.SelectedProject = (IProjectMonitor)m_current.ListView.SelectedItems[0].Tag;
                mainForm.tltBuildStage.SetToolTip(m_current.ListView, mainForm.GetBuildStage());
                System.Threading.Timer t =
                    new System.Threading.Timer(new System.Threading.TimerCallback(PollProject), mainForm.MainFormController.SelectedProject, 0,
                        System.Threading.Timeout.Infinite);


            }
        }

        private void PollProject(object obj)
        {
            IProjectMonitor projectMon = (IProjectMonitor)obj;
            projectMon.Poll();
        }

        public void UpdateAllTabPageIcons(object obj)
        {
            try
            {
                if (m_bTimerRunning && obj != null)
                    return;
                m_bTimerRunning = true;
                foreach (CCProjectsView view in m_projectViews)
                {
                    UpdateTabPageIcons(view);
                }
                m_bTimerRunning = false;
            }
            catch (Exception e)
            {
                Console.WriteLine("sdcsdfsdf");
            }
            m_bTimerRunning = false;
        }

        public void UpdateTabPageIcons(CCProjectsView view)
        {
            try
            {
                ProjectState worstStatus = ProjectState.Success;
                ListView.CheckForIllegalCrossThreadCalls = false;
                foreach (ListViewItem lvi in view.ListView.Items)
                {
                    IProjectMonitor projMon = (IProjectMonitor)lvi.Tag;

                    if (projMon.ProjectState.IsMoreImportantThan(worstStatus))
                        worstStatus = projMon.ProjectState;
                }

                view.TabPage.ImageIndex = worstStatus.ImageIndex;
            }
            catch (System.Exception e)
            {

            }

        }

        public void RemoveOrphans()
        {
            CCProjectsView allProjects = m_projectViews[0];
            int i;
            for (i = 1; i < m_projectViews.Count; i++)
            {
                CCProjectsView view = m_projectViews[i];
                int count = view.ListView.Items.Count;
                ListViewItem[] array = new ListViewItem[count];
                view.ListView.Items.CopyTo(array, 0);
                int j;
                for (j = 0; j < count; j++)
                {
                    ListViewItem item = array[j];
                    if (allProjects.Contains(item) == false)
                        view.ListView.Items.Remove(item);
                }
            }
        }


        // TODO Auto add tab on drag
        // TODO support multiple / shared server configs 
        // TODO add option to move as well as copy projects to tabs (not from 'all')
        // TODO prevent user dragging onto broken tab

        public void CreateMenuOfAvailableTabs(MouseEventArgs e)
        {
            // remove previous entries
            projectContextMenu.Items.Clear();

            projectContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripMenuItem[] {
            mnuForce,
            mnuAbort,
            mnuStart,
            mnuStop,
            mnuWebPage,
            mnuCancelPending,
            mnuFixBuild,
            mnuCopyBuildLabel});

            projectContextMenu.Items.Add(new ToolStripSeparator());
            projectContextMenu.Items.Add(mnuSendToNewTab);
            projectContextMenu.Items.Add(mnuCreateTabFromPattern);
            projectContextMenu.Items.Add(mnuCreateTabFromCategory);

            ToolStripMenuItem subMenu = new ToolStripMenuItem();
            subMenu.Text = "Send to ...";


            foreach (CCProjectsView ccpv in m_projectViews)
            {
                if (ccpv.TabPage != this.m_tabControl.SelectedTab
                    && ccpv.IsUserView == true)
                {
                    ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem();
                    item.Name = ccpv.TabPage.Name;
                    item.Text = ccpv.TabPage.Text;
                    item.Click += new System.EventHandler(ToolStripMenuItem_Click);
                    item.Tag = ccpv;

                    subMenu.DropDownItems.Add(item);
                }
            }

            if (subMenu.DropDownItems.Count > 1)
            {
                projectContextMenu.Items.Add(subMenu);
            }

            if (m_current.IsUserView)
            {
                ToolStripMenuItem del = new System.Windows.Forms.ToolStripMenuItem();
                del.Text = "Remove Project";
                del.Click += new System.EventHandler(DeleteItem_Click);
                projectContextMenu.Items.Add(del);
            }

            projectContextMenuFilterItems();
            projectContextMenu.Show(m_tabControl.PointToScreen(e.Location));
        }


        private void SaveViewConfiguration_Click(object sender, EventArgs e)
        {
            SaveViewConfiguration();
        }


        private void DeleteItem_Click(object sender, EventArgs e)
        {
            if (m_current.TabPage.Text != "All")
                foreach (ListViewItem srclvi in m_current.ListView.SelectedItems)
                    m_current.ListView.Items.Remove(srclvi);
        }


        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsi = (ToolStripMenuItem)sender;
            CCProjectsView ccpv = (CCProjectsView)tsi.Tag;

            foreach (ListViewItem lvi in m_current.ListView.SelectedItems)
            {
                IProjectMonitor monitor = (IProjectMonitor)lvi.Tag;
                ListViewItem item = mainForm.MainFormController.CopyBoundProject(monitor);
                ccpv.AddProject(item);
            }
        }
    };

    /// <summary>
    /// CCProjectsView 
    ///   wraps up a tabpage with a listview containing projects
    /// </summary>
    public partial class CCProjectsView : UserControl
    {
        CCProjectsViewMgr m_viewMgr;

        private ColumnHeader colProject;
        private ColumnHeader colServer;
        private ColumnHeader colLastBuildLabel;
        private ColumnHeader colActivity;
        private ColumnHeader colDetail;
        private ColumnHeader colLastBuildTime;
        private ColumnHeader colProjectStatus;
        private TabPage m_tabPage;
        private ListView m_listView;
        private bool bUserView;
        private bool bReadOnly;


        public CCProjectsView(CCProjectsViewMgr viewMgr, string viewName)
        {
            if (viewName == "")
            {
                viewName = "NewView" + viewMgr.TabControl.TabCount;
            }

            bUserView = true;
            bReadOnly = false;
            Text = viewName;
            m_viewMgr = viewMgr;
            m_tabPage = CreateTabPage(viewMgr, viewName);
            m_listView = CreateListView(viewMgr, m_tabPage);
            m_tabPage.Show();
            m_listView.Show();
        }

        public bool IsUserView
        {
            get { return bUserView; }
            set { bUserView = value; }
        }

        public bool IsReadOnly
        {
            get { return bReadOnly; }
            set { bReadOnly = value; }
        }

        public TabPage TabPage
        {
            get { return m_tabPage; }
        }

        public ListView ListView
        {
            get { return m_listView; }
        }


        public void SaveViewConfiguration(XmlTextWriter xmlWriter)
        {
            // don't write the 'All' or 'Broken' tab it's created automatically
            if (IsUserView == false)
                return;

            xmlWriter.WriteStartElement("View");
            xmlWriter.WriteAttributeString("Name", this.Text);
            foreach (ListViewItem lvi in ListView.Items)
            {
                xmlWriter.WriteStartElement("Project");
                xmlWriter.WriteAttributeString("Name", lvi.SubItems[0].Text);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("Server", this.Text);
                xmlWriter.WriteAttributeString("Name", lvi.SubItems[1].Text);
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
        }


        private TabPage CreateTabPage(CCProjectsViewMgr viewMgr, string name)
        {
            TabPage t = new System.Windows.Forms.TabPage();

            t.Location = new System.Drawing.Point(4, 22);
            t.Padding = new System.Windows.Forms.Padding(3);
            t.Text = name;
            t.Dock = DockStyle.Fill;
            t.UseVisualStyleBackColor = true;
            t.ImageIndex = 0;
            t.Tag = this; // this is the tabs parent (CCProjectsView)

            return t;
        }

        private ListView CreateListView(CCProjectsViewMgr viewMgr, TabPage t)
        {
            ListView lv = new ListView();

            this.colProject = new System.Windows.Forms.ColumnHeader();
            this.colServer = new System.Windows.Forms.ColumnHeader();
            this.colActivity = new System.Windows.Forms.ColumnHeader();
            this.colDetail = new System.Windows.Forms.ColumnHeader();
            this.colLastBuildLabel = new System.Windows.Forms.ColumnHeader();
            this.colLastBuildTime = new System.Windows.Forms.ColumnHeader();
            this.colProjectStatus = new System.Windows.Forms.ColumnHeader();

            lv.Tag = viewMgr;

            lv.Dock = DockStyle.Fill;

            lv.AllowDrop = true;
            lv.DragOver += new DragEventHandler(lv_DragOver);
            lv.DragEnter += new DragEventHandler(lv_DragEnter);
            lv.DragDrop += new DragEventHandler(lv_DragDrop);
            lv.ItemDrag += new ItemDragEventHandler(lv_ItemDrag);
            lv.DragLeave += new EventHandler(lv_DragLeave);
            t.Controls.Add(lv);
            lv.Location = new System.Drawing.Point(0, 0);

            lv.Size = new System.Drawing.Size(t.Width, t.Height);
            lv.TabIndex = 0;
            lv.UseCompatibleStateImageBehavior = false;
            lv.MultiSelect = true;

            lv.MouseDoubleClick += new MouseEventHandler(viewMgr.lv_MouseDoubleClick);

            lv.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colProject,
            this.colServer,
            this.colActivity,
            this.colDetail,
            this.colLastBuildLabel,
            this.colLastBuildTime,
            this.colProjectStatus
         });
            lv.ContextMenuStrip = viewMgr.projectContextMenu;
            lv.Dock = System.Windows.Forms.DockStyle.Fill;
            lv.FullRowSelect = true;
            lv.HideSelection = false;
            lv.SmallImageList = viewMgr.SmallImageList;
            lv.LargeImageList = viewMgr.LargeImageList;
            lv.TabIndex = 0;
            lv.UseCompatibleStateImageBehavior = false;
            lv.View = System.Windows.Forms.View.Details;
            lv.SelectedIndexChanged += new System.EventHandler(viewMgr.CCProjectViews_SelectedIndexChanged);
            lv.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(ColumnClick);

            lv.MouseClick += new MouseEventHandler(lv_MouseClick);


            #region
            this.colProject.Text = "Project";
            this.colProject.Width = 160;
            this.colServer.Text = "Server";
            this.colServer.Width = 100;
            this.colActivity.Text = "Activity";
            this.colActivity.Width = 132;
            this.colDetail.Text = "Detail";
            this.colDetail.Width = 282;
            this.colLastBuildLabel.Text = "Last Build Label";
            this.colLastBuildLabel.Width = 192;
            this.colLastBuildTime.Text = "Last Build Time";
            this.colLastBuildTime.Width = 112;
            this.colProjectStatus.Text = "Project Status";
            #endregion
            return lv;
        }

        void lv_DragLeave(object sender, EventArgs e)
        {
            Console.WriteLine("Drag Leave");
        }


        private void ColumnClick(object sender, ColumnClickEventArgs e)
        {

            ListViewItemComparer compare = m_listView.ListViewItemSorter as ListViewItemComparer;

            if (compare == null)
            {
                m_listView.ListViewItemSorter = new ListViewItemComparer(e.Column, true);
            }
            else
            {
                if (compare.SortColumn == e.Column)
                {
                    // Sort on same column, just the opposite direction.
                    compare.SortAscending = !compare.SortAscending;
                }
                else
                {
                    compare.SortAscending = false;
                    compare.SortColumn = e.Column;
                }
            }

            m_listView.Sort();
        }

        private class ListViewItemComparer : IComparer
        {
            private static string[] _columnSortTypes = new string[] { "string", "string", "string", "string", "int", "datetime", "string" };
            private int col;
            private bool ascendingOrder;

            public int SortColumn
            {
                get { return col; }
                set { col = value; }
            }

            public bool SortAscending
            {
                get { return ascendingOrder; }
                set { ascendingOrder = value; }
            }

            public ListViewItemComparer()
                : this(0, true)
            {
            }

            public ListViewItemComparer(int column)
                : this(column, true)
            {
            }

            public ListViewItemComparer(int column, bool ascending)
            {
                SortColumn = column;
                SortAscending = ascending;
            }

            public int Compare(object x, object y)
            {
                int compare = 0;
                switch (_columnSortTypes[col])
                {
                    case "int":
                        int xValue = 0;
                        int yValue = 0;

                        if (int.TryParse(((ListViewItem)x).SubItems[SortColumn].Text, out xValue) && int.TryParse(((ListViewItem)y).SubItems[SortColumn].Text, out yValue))
                        {
                            if (xValue < yValue)
                            {
                                compare = -1;
                            }
                            else
                            {
                                if (xValue > yValue)
                                {
                                    compare = 1;
                                }
                                else
                                {
                                    compare = 0;
                                }
                            }
                        }
                        break;
                    case "datetime":
                        DateTime xDateTime = DateTime.MinValue;
                        DateTime yDateTime = DateTime.MinValue;

                        if (DateTime.TryParse(((ListViewItem)x).SubItems[SortColumn].Text, out xDateTime) && DateTime.TryParse(((ListViewItem)y).SubItems[SortColumn].Text, out yDateTime))
                        {
                            compare = DateTime.Compare(xDateTime, yDateTime);
                        }
                        break;
                    default: // assume string
                        compare = string.Compare(((ListViewItem)x).SubItems[SortColumn].Text, ((ListViewItem)y).SubItems[SortColumn].Text);
                        break;
                }

                if (!ascendingOrder)
                {
                    compare = -compare;
                }

                return compare;
            }
        }

        void lv_DragDrop(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;

            if (e.Data.GetDataPresent(typeof(ListView.SelectedListViewItemCollection)) == false)
                return;

            ListView.SelectedListViewItemCollection items = (ListView.SelectedListViewItemCollection)e.Data.GetData(typeof(ListView.SelectedListViewItemCollection));
            foreach (ListViewItem lvi in items)
            {
                IProjectMonitor monitor = (IProjectMonitor)lvi.Tag;
                ListViewItem item = m_viewMgr.MainForm.MainFormController.CopyBoundProject(monitor);
                AddProject(item);
            }
        }

        void lv_DragEnter(object sender, DragEventArgs e)
        {
            Console.WriteLine("lv dragenter");
            e.Effect = DragDropEffects.Copy;
        }

        void lv_DragOver(object sender, DragEventArgs e)
        {
            Console.WriteLine("lv dragover");
            e.Effect = DragDropEffects.Copy;
        }

        void lv_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (this.IsReadOnly == false)
                DoDragDrop(m_listView.SelectedItems, DragDropEffects.Copy);
        }


        void lv_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                CCProjectsViewMgr viewMgr = (CCProjectsViewMgr)(((ListView)sender).Tag);
                viewMgr.CreateMenuOfAvailableTabs(e);
            }
        }

        public bool Contains(ListViewItem lvi)
        {
            foreach (ListViewItem lv in m_listView.Items)
            {
                if (lv.SubItems[0].Text == lvi.SubItems[0].Text)
                    return true;
            }
            return false;
        }

        public ListViewItem Contains(string project)
        {
            foreach (ListViewItem lv in m_listView.Items)
            {
                if (lv.SubItems[0].Text == project)
                    return lv;
            }
            return null;
        }


        public void AddProject(ListViewItem lvi)
        {
            try
            {
                if (m_listView.Items != null && m_listView.Items.Contains(lvi) == false &&
                    Contains(lvi) == false)
                {
                    m_listView.Items.Add(lvi);
                    IProjectMonitor projMon = (IProjectMonitor)lvi.Tag;
                    projMon.Polled -= projMon_Polled;
                    projMon.Polled += new MonitorPolledEventHandler(projMon_Polled);
                }
            }
            catch (System.Exception)
            {

            }
        }
        void projMon_Polled(object sender, MonitorPolledEventArgs args)
        {
            IProjectMonitor projMon = args.ProjectMonitor;
            if ((projMon.ProjectState != ProjectState.Success &&
                projMon.ProjectState != ProjectState.NotConnected &&
                projMon.ProjectState != ProjectState.Building) ||
                projMon.ProjectState == ProjectState.BrokenAndBuilding)
            {
                ListViewItem item = m_viewMgr.MainForm.MainFormController.CopyBoundProject(projMon);
                m_viewMgr.m_broken.AddProject(item);
                if (m_viewMgr.TabControl.TabPages.Contains(m_viewMgr.m_broken.TabPage) == false)
                    m_viewMgr.TabControl.TabPages.Add(m_viewMgr.m_broken.TabPage);

            }
            else
            {
                foreach (ListViewItem srclvi in m_viewMgr.m_broken.ListView.Items)
                {
                    if (srclvi != null && srclvi.Tag == projMon)
                        m_viewMgr.m_broken.ListView.Items.Remove(srclvi);
                }

                if (m_viewMgr.m_broken.ListView.Items.Count == 0)
                {
                    m_viewMgr.TabControl.TabPages.Remove(m_viewMgr.m_broken.TabPage);
                }
            }
        }
    }
};

