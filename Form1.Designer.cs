namespace WindowsAppStaet
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPageScript = new System.Windows.Forms.TabPage();
			this.btnRestoreShortcuts = new System.Windows.Forms.Button();
			this.btnBackupShortcuts = new System.Windows.Forms.Button();
			this.btnEditShortcut = new System.Windows.Forms.Button();
			this.btnDeleteShortcut = new System.Windows.Forms.Button();
			this.btnAddShortcut = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
			this.button1 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.tabPageStartup = new System.Windows.Forms.TabPage();
			this.btnRestoreStartup = new System.Windows.Forms.Button();
			this.btnBackupStartup = new System.Windows.Forms.Button();
			this.btnEditStartup = new System.Windows.Forms.Button();
			this.btnDeleteStartup = new System.Windows.Forms.Button();
			this.btnAddStartup = new System.Windows.Forms.Button();
			this.btnRefreshStartup = new System.Windows.Forms.Button();
			this.listViewStartup = new System.Windows.Forms.ListView();
			this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeaderLocation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeaderCommand = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.tabControl1.SuspendLayout();
			this.tabPageScript.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			this.tabPageStartup.SuspendLayout();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.BackColor = System.Drawing.Color.RosyBrown;
			this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.textBox1.Font = new System.Drawing.Font("宋体", 12F);
			this.textBox1.HideSelection = false;
			this.textBox1.Location = new System.Drawing.Point(0, 460);
			this.textBox1.Margin = new System.Windows.Forms.Padding(4);
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(884, 19);
			this.textBox1.TabIndex = 0;
			this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPageScript);
			this.tabControl1.Controls.Add(this.tabPageStartup);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Font = new System.Drawing.Font("宋体", 12F);
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(884, 460);
			this.tabControl1.TabIndex = 10;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
			// 
			// tabPageScript
			// 
			this.tabPageScript.Controls.Add(this.btnRestoreShortcuts);
			this.tabPageScript.Controls.Add(this.btnBackupShortcuts);
			this.tabPageScript.Controls.Add(this.btnEditShortcut);
			this.tabPageScript.Controls.Add(this.btnDeleteShortcut);
			this.tabPageScript.Controls.Add(this.btnAddShortcut);
			this.tabPageScript.Controls.Add(this.label2);
			this.tabPageScript.Controls.Add(this.numericUpDown1);
			this.tabPageScript.Controls.Add(this.checkedListBox1);
			this.tabPageScript.Controls.Add(this.button1);
			this.tabPageScript.Controls.Add(this.label1);
			this.tabPageScript.Controls.Add(this.groupBox1);
			this.tabPageScript.Location = new System.Drawing.Point(4, 26);
			this.tabPageScript.Name = "tabPageScript";
			this.tabPageScript.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageScript.Size = new System.Drawing.Size(876, 430);
			this.tabPageScript.TabIndex = 0;
			this.tabPageScript.Text = "生成启动脚本";
			this.tabPageScript.UseVisualStyleBackColor = true;
			// 
			// btnRestoreShortcuts
			// 
			this.btnRestoreShortcuts.Font = new System.Drawing.Font("宋体", 12F);
			this.btnRestoreShortcuts.Location = new System.Drawing.Point(362, 290);
			this.btnRestoreShortcuts.Name = "btnRestoreShortcuts";
			this.btnRestoreShortcuts.Size = new System.Drawing.Size(120, 31);
			this.btnRestoreShortcuts.TabIndex = 15;
			this.btnRestoreShortcuts.Text = "恢复备份";
			this.btnRestoreShortcuts.UseVisualStyleBackColor = true;
			this.btnRestoreShortcuts.Click += new System.EventHandler(this.btnRestoreShortcuts_Click);
			// 
			// btnBackupShortcuts
			// 
			this.btnBackupShortcuts.Font = new System.Drawing.Font("宋体", 12F);
			this.btnBackupShortcuts.Location = new System.Drawing.Point(362, 250);
			this.btnBackupShortcuts.Name = "btnBackupShortcuts";
			this.btnBackupShortcuts.Size = new System.Drawing.Size(120, 31);
			this.btnBackupShortcuts.TabIndex = 14;
			this.btnBackupShortcuts.Text = "备份列表";
			this.btnBackupShortcuts.UseVisualStyleBackColor = true;
			this.btnBackupShortcuts.Click += new System.EventHandler(this.btnBackupShortcuts_Click);
			// 
			// btnEditShortcut
			// 
			this.btnEditShortcut.Font = new System.Drawing.Font("宋体", 12F);
			this.btnEditShortcut.Location = new System.Drawing.Point(362, 210);
			this.btnEditShortcut.Name = "btnEditShortcut";
			this.btnEditShortcut.Size = new System.Drawing.Size(120, 31);
			this.btnEditShortcut.TabIndex = 13;
			this.btnEditShortcut.Text = "编辑选中项";
			this.btnEditShortcut.UseVisualStyleBackColor = true;
			this.btnEditShortcut.Click += new System.EventHandler(this.btnEditShortcut_Click);
			// 
			// btnDeleteShortcut
			// 
			this.btnDeleteShortcut.Font = new System.Drawing.Font("宋体", 12F);
			this.btnDeleteShortcut.Location = new System.Drawing.Point(362, 170);
			this.btnDeleteShortcut.Name = "btnDeleteShortcut";
			this.btnDeleteShortcut.Size = new System.Drawing.Size(120, 31);
			this.btnDeleteShortcut.TabIndex = 12;
			this.btnDeleteShortcut.Text = "删除选中项";
			this.btnDeleteShortcut.UseVisualStyleBackColor = true;
			this.btnDeleteShortcut.Click += new System.EventHandler(this.btnDeleteShortcut_Click);
			// 
			// btnAddShortcut
			// 
			this.btnAddShortcut.Font = new System.Drawing.Font("宋体", 12F);
			this.btnAddShortcut.Location = new System.Drawing.Point(362, 130);
			this.btnAddShortcut.Name = "btnAddShortcut";
			this.btnAddShortcut.Size = new System.Drawing.Size(120, 31);
			this.btnAddShortcut.TabIndex = 11;
			this.btnAddShortcut.Text = "手动添加";
			this.btnAddShortcut.UseVisualStyleBackColor = true;
			this.btnAddShortcut.Click += new System.EventHandler(this.btnAddShortcut_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("宋体", 12F);
			this.label2.Location = new System.Drawing.Point(458, 49);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(23, 16);
			this.label2.TabIndex = 9;
			this.label2.Text = "秒";
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.Font = new System.Drawing.Font("宋体", 12F);
			this.numericUpDown1.Location = new System.Drawing.Point(365, 44);
			this.numericUpDown1.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(85, 26);
			this.numericUpDown1.TabIndex = 8;
			this.numericUpDown1.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
			// 
			// checkedListBox1
			// 
			this.checkedListBox1.Dock = System.Windows.Forms.DockStyle.Right;
			this.checkedListBox1.Font = new System.Drawing.Font("宋体", 12F);
			this.checkedListBox1.FormattingEnabled = true;
			this.checkedListBox1.Location = new System.Drawing.Point(516, 3);
			this.checkedListBox1.Name = "checkedListBox1";
			this.checkedListBox1.ScrollAlwaysVisible = true;
			this.checkedListBox1.Size = new System.Drawing.Size(357, 424);
			this.checkedListBox1.TabIndex = 7;
			// 
			// button1
			// 
			this.button1.Font = new System.Drawing.Font("宋体", 12F);
			this.button1.Location = new System.Drawing.Point(362, 80);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(120, 31);
			this.button1.TabIndex = 5;
			this.button1.Text = "生成脚本";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("宋体", 12F);
			this.label1.Location = new System.Drawing.Point(363, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(87, 16);
			this.label1.TabIndex = 4;
			this.label1.Text = "延迟时间：";
			// 
			// groupBox1
			// 
			this.groupBox1.AllowDrop = true;
			this.groupBox1.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
			this.groupBox1.Font = new System.Drawing.Font("宋体", 12F);
			this.groupBox1.Location = new System.Drawing.Point(3, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
			this.groupBox1.Size = new System.Drawing.Size(352, 424);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "拖拽快捷方式到这里";
			// 
			// tabPageStartup
			// 
			this.tabPageStartup.Controls.Add(this.btnRestoreStartup);
			this.tabPageStartup.Controls.Add(this.btnBackupStartup);
			this.tabPageStartup.Controls.Add(this.btnEditStartup);
			this.tabPageStartup.Controls.Add(this.btnDeleteStartup);
			this.tabPageStartup.Controls.Add(this.btnAddStartup);
			this.tabPageStartup.Controls.Add(this.btnRefreshStartup);
			this.tabPageStartup.Controls.Add(this.listViewStartup);
			this.tabPageStartup.Location = new System.Drawing.Point(4, 26);
			this.tabPageStartup.Name = "tabPageStartup";
			this.tabPageStartup.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageStartup.Size = new System.Drawing.Size(992, 430);
			this.tabPageStartup.TabIndex = 1;
			this.tabPageStartup.Text = "系统启动项管理";
			this.tabPageStartup.UseVisualStyleBackColor = true;
			// 
			// btnRestoreStartup
			// 
			this.btnRestoreStartup.Font = new System.Drawing.Font("宋体", 12F);
			this.btnRestoreStartup.Location = new System.Drawing.Point(845, 235);
			this.btnRestoreStartup.Name = "btnRestoreStartup";
			this.btnRestoreStartup.Size = new System.Drawing.Size(120, 31);
			this.btnRestoreStartup.TabIndex = 6;
			this.btnRestoreStartup.Text = "恢复备份";
			this.btnRestoreStartup.UseVisualStyleBackColor = true;
			this.btnRestoreStartup.Click += new System.EventHandler(this.btnRestoreStartup_Click);
			// 
			// btnBackupStartup
			// 
			this.btnBackupStartup.Font = new System.Drawing.Font("宋体", 12F);
			this.btnBackupStartup.Location = new System.Drawing.Point(845, 190);
			this.btnBackupStartup.Name = "btnBackupStartup";
			this.btnBackupStartup.Size = new System.Drawing.Size(120, 31);
			this.btnBackupStartup.TabIndex = 5;
			this.btnBackupStartup.Text = "备份";
			this.btnBackupStartup.UseVisualStyleBackColor = true;
			this.btnBackupStartup.Click += new System.EventHandler(this.btnBackupStartup_Click);
			// 
			// btnEditStartup
			// 
			this.btnEditStartup.Font = new System.Drawing.Font("宋体", 12F);
			this.btnEditStartup.Location = new System.Drawing.Point(845, 145);
			this.btnEditStartup.Name = "btnEditStartup";
			this.btnEditStartup.Size = new System.Drawing.Size(120, 31);
			this.btnEditStartup.TabIndex = 4;
			this.btnEditStartup.Text = "修改";
			this.btnEditStartup.UseVisualStyleBackColor = true;
			this.btnEditStartup.Click += new System.EventHandler(this.btnEditStartup_Click);
			// 
			// btnDeleteStartup
			// 
			this.btnDeleteStartup.Font = new System.Drawing.Font("宋体", 12F);
			this.btnDeleteStartup.Location = new System.Drawing.Point(845, 100);
			this.btnDeleteStartup.Name = "btnDeleteStartup";
			this.btnDeleteStartup.Size = new System.Drawing.Size(120, 31);
			this.btnDeleteStartup.TabIndex = 3;
			this.btnDeleteStartup.Text = "删除";
			this.btnDeleteStartup.UseVisualStyleBackColor = true;
			this.btnDeleteStartup.Click += new System.EventHandler(this.btnDeleteStartup_Click);
			// 
			// btnAddStartup
			// 
			this.btnAddStartup.Font = new System.Drawing.Font("宋体", 12F);
			this.btnAddStartup.Location = new System.Drawing.Point(845, 55);
			this.btnAddStartup.Name = "btnAddStartup";
			this.btnAddStartup.Size = new System.Drawing.Size(120, 31);
			this.btnAddStartup.TabIndex = 2;
			this.btnAddStartup.Text = "添加";
			this.btnAddStartup.UseVisualStyleBackColor = true;
			this.btnAddStartup.Click += new System.EventHandler(this.btnAddStartup_Click);
			// 
			// btnRefreshStartup
			// 
			this.btnRefreshStartup.Font = new System.Drawing.Font("宋体", 12F);
			this.btnRefreshStartup.Location = new System.Drawing.Point(845, 10);
			this.btnRefreshStartup.Name = "btnRefreshStartup";
			this.btnRefreshStartup.Size = new System.Drawing.Size(120, 31);
			this.btnRefreshStartup.TabIndex = 1;
			this.btnRefreshStartup.Text = "刷新";
			this.btnRefreshStartup.UseVisualStyleBackColor = true;
			this.btnRefreshStartup.Click += new System.EventHandler(this.btnRefreshStartup_Click);
			// 
			// listViewStartup
			// 
			this.listViewStartup.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderLocation,
            this.columnHeaderCommand});
			this.listViewStartup.Dock = System.Windows.Forms.DockStyle.Left;
			this.listViewStartup.Font = new System.Drawing.Font("宋体", 10F);
			this.listViewStartup.FullRowSelect = true;
			this.listViewStartup.GridLines = true;
			this.listViewStartup.HideSelection = false;
			this.listViewStartup.Location = new System.Drawing.Point(3, 3);
			this.listViewStartup.Name = "listViewStartup";
			this.listViewStartup.Size = new System.Drawing.Size(830, 424);
			this.listViewStartup.TabIndex = 0;
			this.listViewStartup.UseCompatibleStateImageBehavior = false;
			this.listViewStartup.View = System.Windows.Forms.View.Details;
			// 
			// columnHeaderName
			// 
			this.columnHeaderName.Text = "名称";
			this.columnHeaderName.Width = 160;
			// 
			// columnHeaderLocation
			// 
			this.columnHeaderLocation.Text = "位置";
			this.columnHeaderLocation.Width = 180;
			// 
			// columnHeaderCommand
			// 
			this.columnHeaderCommand.Text = "命令/路径";
			this.columnHeaderCommand.Width = 460;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(884, 479);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.textBox1);
			this.Font = new System.Drawing.Font("宋体", 12F);
			this.MinimumSize = new System.Drawing.Size(900, 500);
			this.Name = "Form1";
			this.Text = "快速生成优先级和延时的启动脚本 / 系统启动项管理";
			this.tabControl1.ResumeLayout(false);
			this.tabPageScript.ResumeLayout(false);
			this.tabPageScript.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			this.tabPageStartup.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageScript;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnAddShortcut;
        private System.Windows.Forms.Button btnDeleteShortcut;
        private System.Windows.Forms.Button btnEditShortcut;
        private System.Windows.Forms.Button btnBackupShortcuts;
        private System.Windows.Forms.Button btnRestoreShortcuts;
        private System.Windows.Forms.TabPage tabPageStartup;
        private System.Windows.Forms.ListView listViewStartup;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderLocation;
        private System.Windows.Forms.ColumnHeader columnHeaderCommand;
        private System.Windows.Forms.Button btnRefreshStartup;
        private System.Windows.Forms.Button btnAddStartup;
        private System.Windows.Forms.Button btnDeleteStartup;
        private System.Windows.Forms.Button btnEditStartup;
        private System.Windows.Forms.Button btnBackupStartup;
        private System.Windows.Forms.Button btnRestoreStartup;
    }
}
