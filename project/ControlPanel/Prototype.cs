using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;

namespace ThoughtWorks.CruiseControl.ControlPanel
{
	public class Prototype : System.Windows.Forms.Form
	{
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem fileMenuItem;
		private System.Windows.Forms.MenuItem openConfigurationMenuItem;

// THESE ITEMS ARE NOT REFERENCED AND CAUSING A COMPILER WARNING
//		private System.Windows.Forms.MenuItem closeConfigurationMenuItem;
//		private System.Windows.Forms.MenuItem addProjectMenuItem;
//		private System.Windows.Forms.MenuItem removeProjectMenuItem;
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TreeView treeView;
		private System.Windows.Forms.Panel bodyPanel;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textBox5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textBox6;
		private System.Windows.Forms.Panel panel5;
		private System.Windows.Forms.Panel panel6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textBox7;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.ComboBox comboBox2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textBox4;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox textBox8;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Panel panel7;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Panel panel8;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Panel panel9;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox textBox9;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox textBox10;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox textBox11;
		private System.Windows.Forms.Panel panel10;
		private System.Windows.Forms.Panel panel11;
		private System.Windows.Forms.ComboBox comboBox3;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.Button button3;

		public Prototype()
		{
			InitializeComponent();
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.fileMenuItem = new System.Windows.Forms.MenuItem();
			this.openConfigurationMenuItem = new System.Windows.Forms.MenuItem();
			this.treeView = new System.Windows.Forms.TreeView();
			this.bodyPanel = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.panel7 = new System.Windows.Forms.Panel();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label4 = new System.Windows.Forms.Label();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.textBox4 = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.textBox8 = new System.Windows.Forms.TextBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.button2 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.panel8 = new System.Windows.Forms.Panel();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.panel9 = new System.Windows.Forms.Panel();
			this.label11 = new System.Windows.Forms.Label();
			this.textBox9 = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.textBox10 = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.textBox11 = new System.Windows.Forms.TextBox();
			this.panel10 = new System.Windows.Forms.Panel();
			this.panel11 = new System.Windows.Forms.Panel();
			this.comboBox3 = new System.Windows.Forms.ComboBox();
			this.label14 = new System.Windows.Forms.Label();
			this.button3 = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.panel4 = new System.Windows.Forms.Panel();
			this.label5 = new System.Windows.Forms.Label();
			this.textBox5 = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textBox6 = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textBox7 = new System.Windows.Forms.TextBox();
			this.panel5 = new System.Windows.Forms.Panel();
			this.panel6 = new System.Windows.Forms.Panel();
			this.label9 = new System.Windows.Forms.Label();
			this.comboBox2 = new System.Windows.Forms.ComboBox();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.label15 = new System.Windows.Forms.Label();
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.bodyPanel.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.panel7.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panel8.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.panel9.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.panel4.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.fileMenuItem});
			// 
			// fileMenuItem
			// 
			this.fileMenuItem.Index = 0;
			this.fileMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.openConfigurationMenuItem});
			this.fileMenuItem.Text = "&File";
			// 
			// openConfigurationMenuItem
			// 
			this.openConfigurationMenuItem.Index = 0;
			this.openConfigurationMenuItem.Text = "&Open";
			// 
			// treeView
			// 
			this.treeView.Dock = System.Windows.Forms.DockStyle.Left;
			this.treeView.ImageIndex = -1;
			this.treeView.Location = new System.Drawing.Point(0, 0);
			this.treeView.Name = "treeView";
			this.treeView.SelectedImageIndex = -1;
			this.treeView.Size = new System.Drawing.Size(232, 825);
			this.treeView.TabIndex = 0;
			// 
			// bodyPanel
			// 
			this.bodyPanel.AutoScroll = true;
			this.bodyPanel.Controls.Add(this.listView1);
			this.bodyPanel.Controls.Add(this.label1);
			this.bodyPanel.Controls.Add(this.textBox1);
			this.bodyPanel.Controls.Add(this.groupBox1);
			this.bodyPanel.Controls.Add(this.label2);
			this.bodyPanel.Controls.Add(this.textBox2);
			this.bodyPanel.Controls.Add(this.groupBox2);
			this.bodyPanel.Controls.Add(this.label9);
			this.bodyPanel.Controls.Add(this.comboBox2);
			this.bodyPanel.Controls.Add(this.label15);
			this.bodyPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.bodyPanel.Location = new System.Drawing.Point(232, 0);
			this.bodyPanel.Name = "bodyPanel";
			this.bodyPanel.Size = new System.Drawing.Size(784, 825);
			this.bodyPanel.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(-64, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(256, 20);
			this.label1.TabIndex = 2;
			this.label1.Text = "Project Name : ";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(200, 16);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(256, 20);
			this.textBox1.TabIndex = 1;
			this.textBox1.Text = "textBox1";
			// 
			// groupBox1
			// 
			this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
			this.groupBox1.Controls.Add(this.panel7);
			this.groupBox1.Controls.Add(this.button1);
			this.groupBox1.Controls.Add(this.panel8);
			this.groupBox1.Location = new System.Drawing.Point(16, 216);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(728, 408);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Publishers";
			// 
			// panel7
			// 
			this.panel7.BackColor = System.Drawing.SystemColors.Control;
			this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel7.Controls.Add(this.groupBox3);
			this.panel7.Controls.Add(this.comboBox1);
			this.panel7.Controls.Add(this.label3);
			this.panel7.Controls.Add(this.button2);
			this.panel7.Location = new System.Drawing.Point(16, 48);
			this.panel7.Name = "panel7";
			this.panel7.Size = new System.Drawing.Size(696, 168);
			this.panel7.TabIndex = 4;
			// 
			// groupBox3
			// 
			this.groupBox3.BackColor = System.Drawing.SystemColors.Control;
			this.groupBox3.Controls.Add(this.panel1);
			this.groupBox3.Controls.Add(this.panel2);
			this.groupBox3.Controls.Add(this.panel3);
			this.groupBox3.Location = new System.Drawing.Point(168, 48);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(472, 112);
			this.groupBox3.TabIndex = 0;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "XML";
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.Control;
			this.panel1.Controls.Add(this.label4);
			this.panel1.Controls.Add(this.textBox3);
			this.panel1.Controls.Add(this.label8);
			this.panel1.Controls.Add(this.textBox4);
			this.panel1.Controls.Add(this.label10);
			this.panel1.Controls.Add(this.textBox8);
			this.panel1.Location = new System.Drawing.Point(16, 16);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(440, 88);
			this.panel1.TabIndex = 0;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 8);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(152, 20);
			this.label4.TabIndex = 2;
			this.label4.Text = "Project Name : ";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBox3
			// 
			this.textBox3.Location = new System.Drawing.Point(160, 8);
			this.textBox3.Name = "textBox3";
			this.textBox3.Size = new System.Drawing.Size(256, 20);
			this.textBox3.TabIndex = 1;
			this.textBox3.Text = "textBox1";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(8, 56);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(152, 20);
			this.label8.TabIndex = 2;
			this.label8.Text = "Web Url : ";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBox4
			// 
			this.textBox4.Location = new System.Drawing.Point(160, 56);
			this.textBox4.Name = "textBox4";
			this.textBox4.Size = new System.Drawing.Size(256, 20);
			this.textBox4.TabIndex = 1;
			this.textBox4.Text = "textBox1";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(8, 32);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(152, 20);
			this.label10.TabIndex = 2;
			this.label10.Text = "Web Url : ";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBox8
			// 
			this.textBox8.Location = new System.Drawing.Point(160, 32);
			this.textBox8.Name = "textBox8";
			this.textBox8.Size = new System.Drawing.Size(256, 20);
			this.textBox8.TabIndex = 1;
			this.textBox8.Text = "textBox1";
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.SystemColors.Control;
			this.panel2.Location = new System.Drawing.Point(16, 176);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(688, 104);
			this.panel2.TabIndex = 0;
			// 
			// panel3
			// 
			this.panel3.BackColor = System.Drawing.SystemColors.Control;
			this.panel3.Location = new System.Drawing.Point(16, 296);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(688, 104);
			this.panel3.TabIndex = 0;
			// 
			// comboBox1
			// 
			this.comboBox1.Location = new System.Drawing.Point(168, 16);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(304, 21);
			this.comboBox1.TabIndex = 3;
			this.comboBox1.Text = "XML";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(-96, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(256, 20);
			this.label3.TabIndex = 2;
			this.label3.Text = "Publisher : ";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// button2
			// 
			this.button2.BackColor = System.Drawing.SystemColors.Control;
			this.button2.Location = new System.Drawing.Point(608, 8);
			this.button2.Name = "button2";
			this.button2.TabIndex = 1;
			this.button2.Text = "Remove";
			// 
			// button1
			// 
			this.button1.BackColor = System.Drawing.SystemColors.Control;
			this.button1.Location = new System.Drawing.Point(624, 16);
			this.button1.Name = "button1";
			this.button1.TabIndex = 1;
			this.button1.Text = "Add";
			// 
			// panel8
			// 
			this.panel8.BackColor = System.Drawing.SystemColors.Control;
			this.panel8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel8.Controls.Add(this.groupBox4);
			this.panel8.Controls.Add(this.comboBox3);
			this.panel8.Controls.Add(this.label14);
			this.panel8.Controls.Add(this.button3);
			this.panel8.Location = new System.Drawing.Point(16, 224);
			this.panel8.Name = "panel8";
			this.panel8.Size = new System.Drawing.Size(696, 168);
			this.panel8.TabIndex = 4;
			// 
			// groupBox4
			// 
			this.groupBox4.BackColor = System.Drawing.SystemColors.Control;
			this.groupBox4.Controls.Add(this.panel9);
			this.groupBox4.Controls.Add(this.panel10);
			this.groupBox4.Controls.Add(this.panel11);
			this.groupBox4.Location = new System.Drawing.Point(168, 48);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(472, 112);
			this.groupBox4.TabIndex = 0;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Log";
			// 
			// panel9
			// 
			this.panel9.BackColor = System.Drawing.SystemColors.Control;
			this.panel9.Controls.Add(this.label11);
			this.panel9.Controls.Add(this.textBox9);
			this.panel9.Controls.Add(this.label12);
			this.panel9.Controls.Add(this.textBox10);
			this.panel9.Controls.Add(this.label13);
			this.panel9.Controls.Add(this.textBox11);
			this.panel9.Location = new System.Drawing.Point(16, 16);
			this.panel9.Name = "panel9";
			this.panel9.Size = new System.Drawing.Size(440, 88);
			this.panel9.TabIndex = 0;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(8, 8);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(152, 20);
			this.label11.TabIndex = 2;
			this.label11.Text = "Project Name : ";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBox9
			// 
			this.textBox9.Location = new System.Drawing.Point(160, 8);
			this.textBox9.Name = "textBox9";
			this.textBox9.Size = new System.Drawing.Size(256, 20);
			this.textBox9.TabIndex = 1;
			this.textBox9.Text = "textBox1";
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(8, 56);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(152, 20);
			this.label12.TabIndex = 2;
			this.label12.Text = "Web Url : ";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBox10
			// 
			this.textBox10.Location = new System.Drawing.Point(160, 56);
			this.textBox10.Name = "textBox10";
			this.textBox10.Size = new System.Drawing.Size(256, 20);
			this.textBox10.TabIndex = 1;
			this.textBox10.Text = "textBox1";
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(8, 32);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(152, 20);
			this.label13.TabIndex = 2;
			this.label13.Text = "Web Url : ";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBox11
			// 
			this.textBox11.Location = new System.Drawing.Point(160, 32);
			this.textBox11.Name = "textBox11";
			this.textBox11.Size = new System.Drawing.Size(256, 20);
			this.textBox11.TabIndex = 1;
			this.textBox11.Text = "textBox1";
			// 
			// panel10
			// 
			this.panel10.BackColor = System.Drawing.SystemColors.Control;
			this.panel10.Location = new System.Drawing.Point(16, 176);
			this.panel10.Name = "panel10";
			this.panel10.Size = new System.Drawing.Size(688, 104);
			this.panel10.TabIndex = 0;
			// 
			// panel11
			// 
			this.panel11.BackColor = System.Drawing.SystemColors.Control;
			this.panel11.Location = new System.Drawing.Point(16, 296);
			this.panel11.Name = "panel11";
			this.panel11.Size = new System.Drawing.Size(688, 104);
			this.panel11.TabIndex = 0;
			// 
			// comboBox3
			// 
			this.comboBox3.Location = new System.Drawing.Point(168, 16);
			this.comboBox3.Name = "comboBox3";
			this.comboBox3.Size = new System.Drawing.Size(304, 21);
			this.comboBox3.TabIndex = 3;
			this.comboBox3.Text = "Log";
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(-96, 16);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(256, 20);
			this.label14.TabIndex = 2;
			this.label14.Text = "Publisher : ";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// button3
			// 
			this.button3.BackColor = System.Drawing.SystemColors.Control;
			this.button3.Location = new System.Drawing.Point(608, 8);
			this.button3.Name = "button3";
			this.button3.TabIndex = 1;
			this.button3.Text = "Remove";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(-64, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(256, 20);
			this.label2.TabIndex = 2;
			this.label2.Text = "Web Url : ";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(200, 40);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(256, 20);
			this.textBox2.TabIndex = 1;
			this.textBox2.Text = "textBox1";
			// 
			// groupBox2
			// 
			this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
			this.groupBox2.Controls.Add(this.panel4);
			this.groupBox2.Controls.Add(this.panel5);
			this.groupBox2.Controls.Add(this.panel6);
			this.groupBox2.Location = new System.Drawing.Point(200, 96);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(472, 112);
			this.groupBox2.TabIndex = 0;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "CVS";
			// 
			// panel4
			// 
			this.panel4.BackColor = System.Drawing.SystemColors.Control;
			this.panel4.Controls.Add(this.label5);
			this.panel4.Controls.Add(this.textBox5);
			this.panel4.Controls.Add(this.label6);
			this.panel4.Controls.Add(this.textBox6);
			this.panel4.Controls.Add(this.label7);
			this.panel4.Controls.Add(this.textBox7);
			this.panel4.Location = new System.Drawing.Point(16, 16);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(440, 88);
			this.panel4.TabIndex = 0;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(8, 8);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(152, 20);
			this.label5.TabIndex = 2;
			this.label5.Text = "Project Name : ";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBox5
			// 
			this.textBox5.Location = new System.Drawing.Point(160, 8);
			this.textBox5.Name = "textBox5";
			this.textBox5.Size = new System.Drawing.Size(256, 20);
			this.textBox5.TabIndex = 1;
			this.textBox5.Text = "textBox1";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(8, 56);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(152, 20);
			this.label6.TabIndex = 2;
			this.label6.Text = "Web Url : ";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBox6
			// 
			this.textBox6.Location = new System.Drawing.Point(160, 56);
			this.textBox6.Name = "textBox6";
			this.textBox6.Size = new System.Drawing.Size(256, 20);
			this.textBox6.TabIndex = 1;
			this.textBox6.Text = "textBox1";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(8, 32);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(152, 20);
			this.label7.TabIndex = 2;
			this.label7.Text = "Web Url : ";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBox7
			// 
			this.textBox7.Location = new System.Drawing.Point(160, 32);
			this.textBox7.Name = "textBox7";
			this.textBox7.Size = new System.Drawing.Size(256, 20);
			this.textBox7.TabIndex = 1;
			this.textBox7.Text = "textBox1";
			// 
			// panel5
			// 
			this.panel5.BackColor = System.Drawing.SystemColors.Control;
			this.panel5.Location = new System.Drawing.Point(16, 176);
			this.panel5.Name = "panel5";
			this.panel5.Size = new System.Drawing.Size(688, 104);
			this.panel5.TabIndex = 0;
			// 
			// panel6
			// 
			this.panel6.BackColor = System.Drawing.SystemColors.Control;
			this.panel6.Location = new System.Drawing.Point(16, 296);
			this.panel6.Name = "panel6";
			this.panel6.Size = new System.Drawing.Size(688, 104);
			this.panel6.TabIndex = 0;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(-64, 64);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(256, 20);
			this.label9.TabIndex = 2;
			this.label9.Text = "Build : ";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboBox2
			// 
			this.comboBox2.Location = new System.Drawing.Point(200, 64);
			this.comboBox2.Name = "comboBox2";
			this.comboBox2.Size = new System.Drawing.Size(304, 21);
			this.comboBox2.TabIndex = 3;
			this.comboBox2.Text = "CVS";
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(232, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 825);
			this.splitter1.TabIndex = 3;
			this.splitter1.TabStop = false;
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(-56, 640);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(256, 20);
			this.label15.TabIndex = 2;
			this.label15.Text = "Publisher : ";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listView1
			// 
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.columnHeader1,
																						this.columnHeader2,
																						this.columnHeader3});
			this.listView1.Location = new System.Drawing.Point(200, 640);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(312, 56);
			this.listView1.TabIndex = 4;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Name";
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "PublisherType";
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Publisher";
			// 
			// Prototype
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(1016, 825);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.bodyPanel);
			this.Controls.Add(this.treeView);
			this.Menu = this.mainMenu1;
			this.Name = "Prototype";
			this.Text = "Cruise Control - Control Panel";
			this.bodyPanel.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.panel7.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel8.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.panel9.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.panel4.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
