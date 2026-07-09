using System;
using System.Windows.Forms;

namespace WindowsAppStaet
{
    /// <summary>
    /// 通用的新增/编辑条目对话框。
    /// 简单模式（showLocationSelector = false）：仅编辑 名称 + 路径，用于脚本快捷方式列表。
    /// 完整模式（showLocationSelector = true）：编辑 名称 + 路径 + 参数 + 位置，用于系统启动项管理。
    /// </summary>
    public class ItemEditForm : Form
    {
        private TextBox txtName;
        private TextBox txtPath;
        private TextBox txtArgs;
        private ComboBox cboLocation;
        private Button btnBrowse;
        private Button btnOk;
        private Button btnCancel;

        private readonly bool showLocation;

        public string ItemName => txtName.Text.Trim();
        public string ItemPath => txtPath.Text.Trim();
        public string ItemArgs => txtArgs?.Text.Trim() ?? "";
        public StartupLocation SelectedLocation { get; private set; }

        /// <summary>简单模式构造函数：编辑快捷方式的 名称/路径</summary>
        public ItemEditForm(string title, string name, string path)
            : this(title, name, path, "", StartupLocation.HKCU_Run, false)
        {
        }

        /// <summary>完整模式构造函数：编辑系统启动项的 名称/路径/参数/位置</summary>
        public ItemEditForm(string title, string name, string path, string args, StartupLocation location, bool showLocationSelector = true)
        {
            showLocation = showLocationSelector;
            InitializeUi(title);
            txtName.Text = name ?? "";
            txtPath.Text = path ?? "";
            if (showLocation)
            {
                txtArgs.Text = args ?? "";
                cboLocation.SelectedItem = location.ToString();
            }
        }

        private void InitializeUi(string title)
        {
            this.Text = title;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.Font = new System.Drawing.Font("宋体", 10F);
            this.ClientSize = new System.Drawing.Size(460, showLocation ? 235 : 150);

            var lblName = new Label { Text = "名称：", Location = new System.Drawing.Point(15, 20), AutoSize = true };
            txtName = new TextBox { Location = new System.Drawing.Point(90, 17), Width = 350 };

            var lblPath = new Label { Text = "路径：", Location = new System.Drawing.Point(15, 55), AutoSize = true };
            txtPath = new TextBox { Location = new System.Drawing.Point(90, 52), Width = 268 };
            btnBrowse = new Button { Text = "浏览...", Location = new System.Drawing.Point(368, 51), Width = 72 };
            btnBrowse.Click += BtnBrowse_Click;

            this.Controls.Add(lblName);
            this.Controls.Add(txtName);
            this.Controls.Add(lblPath);
            this.Controls.Add(txtPath);
            this.Controls.Add(btnBrowse);

            int buttonY = 110;

            if (showLocation)
            {
                var lblArgs = new Label { Text = "参数：", Location = new System.Drawing.Point(15, 90), AutoSize = true };
                txtArgs = new TextBox { Location = new System.Drawing.Point(90, 87), Width = 350 };

                var lblLocation = new Label { Text = "位置：", Location = new System.Drawing.Point(15, 125), AutoSize = true };
                cboLocation = new ComboBox
                {
                    Location = new System.Drawing.Point(90, 122),
                    Width = 350,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                foreach (var val in Enum.GetNames(typeof(StartupLocation)))
                {
                    cboLocation.Items.Add(val);
                }
                if (cboLocation.Items.Count > 0) cboLocation.SelectedIndex = 0;

                this.Controls.Add(lblArgs);
                this.Controls.Add(txtArgs);
                this.Controls.Add(lblLocation);
                this.Controls.Add(cboLocation);

                buttonY = 175;
            }
            else
            {
                txtArgs = new TextBox(); // 占位，避免空引用
            }

            btnOk = new Button { Text = "确定", Location = new System.Drawing.Point(270, buttonY), Width = 80, DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "取消", Location = new System.Drawing.Point(360, buttonY), Width = 80, DialogResult = DialogResult.Cancel };
            btnOk.Click += BtnOk_Click;

            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "所有文件 (*.*)|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtPath.Text = dlg.FileName;
                    if (string.IsNullOrWhiteSpace(txtName.Text))
                    {
                        txtName.Text = System.IO.Path.GetFileNameWithoutExtension(dlg.FileName);
                    }
                }
            }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("名称不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPath.Text))
            {
                MessageBox.Show("路径不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }
            if (showLocation && cboLocation.SelectedItem != null)
            {
                SelectedLocation = (StartupLocation)Enum.Parse(typeof(StartupLocation), cboLocation.SelectedItem.ToString());
            }
        }
    }
}
