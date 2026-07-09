using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using IWshRuntimeLibrary;
using File = System.IO.File;

namespace WindowsAppStaet
{
    public partial class Form1 : Form
    {
        // 存储快捷方式的字典，Key = 快捷方式名称，Value = 目标路径
        private Dictionary<string, string> shortcutDict = new Dictionary<string, string>();

        // 是否已经加载过一次系统启动项（用于首次切换到该标签页时自动加载）
        private bool startupItemsLoadedOnce = false;

        public Form1()
        {
            InitializeComponent();

            // 设置 groupBox1 允许拖放
            groupBox1.AllowDrop = true;

            // 绑定拖拽事件
            groupBox1.DragEnter += GroupBox1_DragEnter;
            groupBox1.DragDrop += GroupBox1_DragDrop;

            // 绑定 CheckedListBox 的 ItemCheck 事件实现单选
            checkedListBox1.ItemCheck += checkedListBox1_ItemCheck;
            checkedListBox1.SelectedIndexChanged += checkedListBox1_SelectedIndexChanged;

            // 设置状态栏初始文本
            textBox1.ReadOnly = true;
            textBox1.Text = "就绪，请将快捷方式拖拽到上方区域...";

            if (!StartupManager.IsAdministrator())
            {
                textBox1.Text = "提示：当前未以管理员身份运行，无法管理\"所有用户\"范围的启动项";
            }

            // 设置 numericUpDown1 默认值
            numericUpDown1.Minimum = 0;
            numericUpDown1.Maximum = 300;
            numericUpDown1.Value = 15;
        }

        // ======================================================================
        // Tab1：脚本快捷方式列表 —— 拖拽 / 增 / 删 / 改 / 备份 / 恢复
        // ======================================================================

        /// <summary>
        /// 拖拽进入时判断是否为文件
        /// </summary>
        private void GroupBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
                textBox1.Text = "检测到文件，松开鼠标添加...";
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// 拖放完成时解析快捷方式并添加到字典和列表
        /// </summary>
        private void GroupBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files != null && files.Length > 0)
            {
                foreach (string filePath in files)
                {
                    string extension = Path.GetExtension(filePath).ToLower();

                    if (extension == ".lnk")
                    {
                        string shortcutName = Path.GetFileNameWithoutExtension(filePath);
                        string targetPath = GetShortcutTarget(filePath);

                        if (!string.IsNullOrEmpty(targetPath))
                        {
                            AddOrUpdateShortcut(shortcutName, targetPath);
                        }
                        else
                        {
                            AddOrUpdateShortcut(shortcutName, filePath);
                        }
                    }
                    else if (extension == ".url")
                    {
                        string shortcutName = Path.GetFileNameWithoutExtension(filePath);
                        string targetUrl = GetUrlShortcutTarget(filePath);

                        if (!string.IsNullOrEmpty(targetUrl))
                        {
                            AddOrUpdateShortcut(shortcutName, targetUrl);
                        }
                        else
                        {
                            AddOrUpdateShortcut(shortcutName, filePath);
                        }
                    }
                    else
                    {
                        string fileName = Path.GetFileNameWithoutExtension(filePath);
                        AddOrUpdateShortcut(fileName, filePath);
                    }
                }
            }
        }

        /// <summary>
        /// 添加或更新快捷方式到字典和列表
        /// </summary>
        private void AddOrUpdateShortcut(string name, string path)
        {
            if (shortcutDict.ContainsKey(name))
            {
                shortcutDict[name] = path;
                textBox1.Text = $"已更新快捷方式：{name} -> {path}";
            }
            else
            {
                shortcutDict.Add(name, path);
                checkedListBox1.Items.Add(name);
                textBox1.Text = $"已添加快捷方式：{name} -> {path}";
            }
        }

        /// <summary>
        /// "手动添加" 按钮 —— 无需拖拽即可添加一个启动项到脚本列表
        /// </summary>
        private void btnAddShortcut_Click(object sender, EventArgs e)
        {
            using (var dlg = new ItemEditForm("添加快捷方式", "", ""))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    if (shortcutDict.ContainsKey(dlg.ItemName))
                    {
                        MessageBox.Show("该名称已存在，请使用\"编辑选中项\"来修改，或换一个名称。", "提示",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    AddOrUpdateShortcut(dlg.ItemName, dlg.ItemPath);
                }
            }
        }

        /// <summary>
        /// "删除选中项" 按钮
        /// </summary>
        private void btnDeleteShortcut_Click(object sender, EventArgs e)
        {
            if (checkedListBox1.SelectedItem == null)
            {
                MessageBox.Show("请先在列表中选中要删除的项目", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string name = checkedListBox1.SelectedItem.ToString();
            if (MessageBox.Show($"确定要从列表中删除\"{name}\"吗？", "确认删除",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            shortcutDict.Remove(name);
            checkedListBox1.Items.Remove(checkedListBox1.SelectedItem);
            textBox1.Text = $"已删除：{name}";
        }

        /// <summary>
        /// "编辑选中项" 按钮 —— 修改名称/路径
        /// </summary>
        private void btnEditShortcut_Click(object sender, EventArgs e)
        {
            if (checkedListBox1.SelectedItem == null)
            {
                MessageBox.Show("请先在列表中选中要编辑的项目", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string oldName = checkedListBox1.SelectedItem.ToString();
            string oldPath = shortcutDict.ContainsKey(oldName) ? shortcutDict[oldName] : "";
            int index = checkedListBox1.SelectedIndex;

            using (var dlg = new ItemEditForm("编辑快捷方式", oldName, oldPath))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    if (dlg.ItemName != oldName)
                    {
                        if (shortcutDict.ContainsKey(dlg.ItemName))
                        {
                            MessageBox.Show("新名称与已有项目重名，请更换名称。", "提示",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        shortcutDict.Remove(oldName);
                        shortcutDict.Add(dlg.ItemName, dlg.ItemPath);
                        checkedListBox1.Items[index] = dlg.ItemName;
                    }
                    else
                    {
                        shortcutDict[oldName] = dlg.ItemPath;
                    }
                    textBox1.Text = $"已更新：{dlg.ItemName} -> {dlg.ItemPath}";
                }
            }
        }

        /// <summary>
        /// "备份列表" 按钮 —— 将当前脚本快捷方式列表导出为文本文件
        /// </summary>
        private void btnBackupShortcuts_Click(object sender, EventArgs e)
        {
            if (shortcutDict.Count == 0)
            {
                MessageBox.Show("当前列表为空，无需备份。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "备份文件 (*.txt)|*.txt|所有文件 (*.*)|*.*";
                saveDialog.FileName = $"快捷方式列表备份_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine("# 脚本快捷方式列表备份，由 WindowsAppStaet 生成");
                        sb.AppendLine("# 格式：名称|路径");
                        foreach (var kv in shortcutDict)
                        {
                            sb.AppendLine($"{kv.Key}|{kv.Value}");
                        }
                        File.WriteAllText(saveDialog.FileName, sb.ToString(), Encoding.UTF8);
                        textBox1.Text = $"列表已备份到：{saveDialog.FileName}";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"备份失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// "恢复备份" 按钮 —— 从备份文件导入快捷方式列表（与当前列表合并，同名覆盖）
        /// </summary>
        private void btnRestoreShortcuts_Click(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "备份文件 (*.txt)|*.txt|所有文件 (*.*)|*.*";

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var lines = File.ReadAllLines(openDialog.FileName, Encoding.UTF8);
                        int count = 0;
                        foreach (var line in lines)
                        {
                            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#")) continue;
                            var parts = line.Split(new[] { '|' }, 2);
                            if (parts.Length < 2) continue;

                            AddOrUpdateShortcut(parts[0], parts[1]);
                            count++;
                        }
                        textBox1.Text = $"已从备份恢复 {count} 个快捷方式";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"恢复失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// CheckedListBox 勾选状态改变事件 - 实现单选逻辑
        /// </summary>
        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    if (i != e.Index)
                    {
                        checkedListBox1.SetItemChecked(i, false);
                    }
                }

                string selectedKey = checkedListBox1.Items[e.Index].ToString();
                if (shortcutDict.ContainsKey(selectedKey))
                {
                    textBox1.Text = $"已选择：{selectedKey} -> {shortcutDict[selectedKey]}";
                }
            }
            else
            {
                textBox1.Text = "未选择任何快捷方式";
            }
        }

        /// <summary>
        /// CheckedListBox 选中项改变事件
        /// </summary>
        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkedListBox1.SelectedItem != null)
            {
                string selectedKey = checkedListBox1.SelectedItem.ToString();
                if (shortcutDict.ContainsKey(selectedKey))
                {
                    // 可以在这里添加额外的视觉反馈
                }
            }
        }

        /// <summary>
        /// 使用 WSH 解析 .lnk 快捷方式的目标路径
        /// </summary>
        private string GetShortcutTarget(string shortcutPath)
        {
            try
            {
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);

                string targetPath = shortcut.TargetPath;

                if (!string.IsNullOrEmpty(targetPath) && targetPath.Contains("%"))
                {
                    targetPath = Environment.ExpandEnvironmentVariables(targetPath);
                }

                return targetPath;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 解析 .url 网址快捷方式的目标URL
        /// </summary>
        private string GetUrlShortcutTarget(string urlFilePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(urlFilePath);
                foreach (string line in lines)
                {
                    if (line.StartsWith("URL=", StringComparison.OrdinalIgnoreCase))
                    {
                        return line.Substring(4);
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取当前选中的快捷方式（唯一勾选的项）
        /// </summary>
        private KeyValuePair<string, string>? GetSelectedShortcut()
        {
            foreach (var item in checkedListBox1.CheckedItems)
            {
                string key = item.ToString();
                if (shortcutDict.ContainsKey(key))
                {
                    return new KeyValuePair<string, string>(key, shortcutDict[key]);
                }
            }
            return null;
        }

        /// <summary>
        /// 生成BAT脚本按钮点击事件
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            var selectedShortcut = GetSelectedShortcut();

            if (selectedShortcut == null)
            {
                textBox1.Text = "错误：请先在列表中勾选一个快捷方式作为第一启动项";
                MessageBox.Show("请先在列表中勾选一个快捷方式作为第一启动项", "提示",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "批处理文件 (*.bat)|*.bat|所有文件 (*.*)|*.*";
            saveDialog.DefaultExt = "bat";
            saveDialog.FileName = $"启动脚本_{DateTime.Now:yyyyMMdd_HHmmss}.bat";
            saveDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                GenerateBatScript(selectedShortcut.Value, saveDialog.FileName);
            }
        }

        /// <summary>
        /// 生成BAT脚本文件
        /// </summary>
        private void GenerateBatScript(KeyValuePair<string, string> firstItem, string savePath)
        {
            try
            {
                int delaySeconds = (int)numericUpDown1.Value;

                StringBuilder batContent = new StringBuilder();
                batContent.AppendLine("@echo off");
                batContent.AppendLine("title 自动启动脚本");
                batContent.AppendLine("echo ========================================");
                batContent.AppendLine("echo    自动启动脚本开始执行");
                batContent.AppendLine("echo ========================================");
                batContent.AppendLine("echo.");

                batContent.AppendLine($"echo 正在启动第一程序：{firstItem.Key}");
                string firstCommand = GenerateStartCommand(firstItem.Value);
                batContent.AppendLine(firstCommand);
                batContent.AppendLine($"echo 第一程序已启动，等待{delaySeconds}秒...");

                if (delaySeconds > 0)
                {
                    batContent.AppendLine($"timeout /t {delaySeconds} /nobreak >nul");
                }

                int count = 1;
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    string itemName = checkedListBox1.Items[i].ToString();

                    if (itemName == firstItem.Key || !shortcutDict.ContainsKey(itemName))
                    {
                        continue;
                    }

                    count++;
                    string targetPath = shortcutDict[itemName];
                    batContent.AppendLine($"echo 正在启动第{count}个程序：{itemName}");
                    string startCommand = GenerateStartCommand(targetPath);
                    batContent.AppendLine(startCommand);
                }

                batContent.AppendLine("echo.");
                batContent.AppendLine("echo ========================================");
                batContent.AppendLine("echo    所有程序已启动完毕");
                batContent.AppendLine("echo ========================================");
                batContent.AppendLine("timeout /t 3 >nul");
                batContent.AppendLine("exit");

                File.WriteAllText(savePath, batContent.ToString(), Encoding.GetEncoding("GB2312"));

                CreateStartupShortcut(savePath);

                textBox1.Text = $"BAT脚本已生成并添加到启动目录：{Path.GetFileName(savePath)}";
                MessageBox.Show($"脚本已成功生成！\n\n保存位置：{savePath}\n已添加快捷方式到系统启动目录",
                               "成功",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                textBox1.Text = $"生成失败：{ex.Message}";
                MessageBox.Show($"生成脚本时出错：{ex.Message}", "错误",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 根据路径类型生成对应的启动命令
        /// </summary>
        private string GenerateStartCommand(string path)
        {
            if (path.ToLower().EndsWith(".lnk"))
            {
                return $"start \"\" \"{path}\"";
            }
            else if (path.ToLower().EndsWith(".url"))
            {
                return $"start \"\" \"{path}\"";
            }
            else if (path.StartsWith("http://") || path.StartsWith("https://"))
            {
                return $"start \"\" \"{path}\"";
            }
            else
            {
                return $"start \"\" \"{path}\"";
            }
        }

        /// <summary>
        /// 创建BAT脚本的快捷方式到系统启动目录
        /// </summary>
        private void CreateStartupShortcut(string batFilePath)
        {
            try
            {
                string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

                string shortcutName = Path.GetFileNameWithoutExtension(batFilePath);
                string shortcutPath = Path.Combine(startupPath, $"{shortcutName}.lnk");

                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);

                shortcut.TargetPath = batFilePath;
                shortcut.WorkingDirectory = Path.GetDirectoryName(batFilePath);
                shortcut.Description = $"自动启动脚本 - 创建于 {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                shortcut.WindowStyle = 7;

                shortcut.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"BAT脚本已生成，但创建启动快捷方式失败：{ex.Message}\n\n" +
                               $"您可以手动将脚本复制到启动目录：\n{Environment.GetFolderPath(Environment.SpecialFolder.Startup)}",
                               "警告",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ======================================================================
        // Tab2：系统启动项管理 —— 扫描 / 增 / 删 / 改 / 备份 / 恢复
        // ======================================================================

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPageStartup && !startupItemsLoadedOnce)
            {
                startupItemsLoadedOnce = true;
                LoadStartupItems();
            }
        }

        /// <summary>
        /// 扫描系统当前所有启动项并刷新列表
        /// </summary>
        private void LoadStartupItems()
        {
            listViewStartup.BeginUpdate();
            listViewStartup.Items.Clear();

            var items = StartupManager.GetAllStartupItems();
            foreach (var item in items)
            {
                var lvi = new ListViewItem(item.Name);
                lvi.SubItems.Add(item.LocationDisplay);
                lvi.SubItems.Add(item.Command);
                lvi.Tag = item;
                listViewStartup.Items.Add(lvi);
            }

            listViewStartup.EndUpdate();
            textBox1.Text = $"已扫描到 {items.Count} 个系统启动项" +
                (StartupManager.IsAdministrator() ? "" : "（未以管理员身份运行，部分\"所有用户\"项可能无法修改）");
        }

        private void btnRefreshStartup_Click(object sender, EventArgs e)
        {
            LoadStartupItems();
        }

        /// <summary>
        /// 检查目标位置是否需要管理员权限；如需要且当前非管理员，询问是否重启程序
        /// </summary>
        /// <returns>true = 可以继续操作；false = 应中止当前操作</returns>
        private bool EnsureAdminIfNeeded(StartupLocation loc)
        {
            bool needsAdmin =
                loc == StartupLocation.HKLM_Run ||
                loc == StartupLocation.HKLM_RunOnce ||
                loc == StartupLocation.StartupFolderCommon;

            if (needsAdmin && !StartupManager.IsAdministrator())
            {
                var result = MessageBox.Show(
                    "该操作涉及\"所有用户\"范围，需要管理员权限。\n是否以管理员身份重新启动程序？",
                    "需要管理员权限", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    StartupManager.RestartAsAdministrator();
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// "添加" 按钮 —— 新增一个系统启动项
        /// </summary>
        private void btnAddStartup_Click(object sender, EventArgs e)
        {
            using (var dlg = new ItemEditForm("添加系统启动项", "", "", "", StartupLocation.HKCU_Run))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    if (!EnsureAdminIfNeeded(dlg.SelectedLocation)) return;

                    var item = new StartupItem
                    {
                        Name = dlg.ItemName,
                        Command = string.IsNullOrEmpty(dlg.ItemArgs) ? dlg.ItemPath : $"\"{dlg.ItemPath}\" {dlg.ItemArgs}",
                        Location = dlg.SelectedLocation
                    };

                    try
                    {
                        StartupManager.AddOrUpdateItem(item);
                        textBox1.Text = $"已添加启动项：{item.Name} ({item.LocationDisplay})";
                        LoadStartupItems();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"添加失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// "删除" 按钮 —— 删除选中的一个或多个系统启动项
        /// </summary>
        private void btnDeleteStartup_Click(object sender, EventArgs e)
        {
            if (listViewStartup.SelectedItems.Count == 0)
            {
                MessageBox.Show("请先在列表中选中要删除的启动项", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedItems = listViewStartup.SelectedItems.Cast<ListViewItem>()
                .Select(lvi => (StartupItem)lvi.Tag).ToList();

            string names = string.Join("、", selectedItems.Select(i => i.Name));
            if (MessageBox.Show($"确定要删除以下启动项吗？\n{names}", "确认删除",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            int successCount = 0;
            foreach (var item in selectedItems)
            {
                if (!EnsureAdminIfNeeded(item.Location)) return; // 会触发重启，直接返回

                try
                {
                    StartupManager.DeleteItem(item);
                    successCount++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"删除\"{item.Name}\"失败：{ex.Message}", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            textBox1.Text = $"已删除 {successCount} 个启动项";
            LoadStartupItems();
        }

        /// <summary>
        /// "修改" 按钮 —— 编辑选中的系统启动项（名称/路径/参数/位置）
        /// </summary>
        private void btnEditStartup_Click(object sender, EventArgs e)
        {
            if (listViewStartup.SelectedItems.Count != 1)
            {
                MessageBox.Show("请选中且仅选中一个要修改的启动项", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var oldItem = (StartupItem)listViewStartup.SelectedItems[0].Tag;

            using (var dlg = new ItemEditForm("修改系统启动项", oldItem.Name, oldItem.Command, "", oldItem.Location))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    // 新旧位置都需要检查管理员权限
                    if (!EnsureAdminIfNeeded(oldItem.Location)) return;
                    if (!EnsureAdminIfNeeded(dlg.SelectedLocation)) return;

                    var newItem = new StartupItem
                    {
                        Name = dlg.ItemName,
                        Command = string.IsNullOrEmpty(dlg.ItemArgs) ? dlg.ItemPath : $"\"{dlg.ItemPath}\" {dlg.ItemArgs}",
                        Location = dlg.SelectedLocation
                    };

                    try
                    {
                        if (oldItem.Name != newItem.Name || oldItem.Location != newItem.Location)
                        {
                            StartupManager.DeleteItem(oldItem);
                        }
                        StartupManager.AddOrUpdateItem(newItem);
                        textBox1.Text = $"已更新启动项：{newItem.Name} ({newItem.LocationDisplay})";
                        LoadStartupItems();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"修改失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// "备份" 按钮 —— 导出当前所有系统启动项到文本文件
        /// </summary>
        private void btnBackupStartup_Click(object sender, EventArgs e)
        {
            var items = StartupManager.GetAllStartupItems();
            if (items.Count == 0)
            {
                MessageBox.Show("未扫描到任何启动项。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "备份文件 (*.txt)|*.txt|所有文件 (*.*)|*.*";
                saveDialog.FileName = $"系统启动项备份_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        StartupManager.BackupItems(items, saveDialog.FileName);
                        textBox1.Text = $"已备份 {items.Count} 个启动项到：{saveDialog.FileName}";
                        MessageBox.Show("备份完成！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"备份失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// "恢复备份" 按钮 —— 从备份文件读取并重新写回系统启动项
        /// </summary>
        private void btnRestoreStartup_Click(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "备份文件 (*.txt)|*.txt|所有文件 (*.*)|*.*";

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    List<StartupItem> backupItems;
                    try
                    {
                        backupItems = StartupManager.LoadBackup(openDialog.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"读取备份文件失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (backupItems.Count == 0)
                    {
                        MessageBox.Show("备份文件中没有可恢复的启动项。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    if (MessageBox.Show($"备份文件中共有 {backupItems.Count} 个启动项，是否全部恢复到系统？\n（同名项将被覆盖）",
                        "确认恢复", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        return;
                    }

                    int successCount = 0, skippedCount = 0;
                    foreach (var item in backupItems)
                    {
                        if (item.RequiresAdmin && !StartupManager.IsAdministrator())
                        {
                            skippedCount++;
                            continue;
                        }
                        try
                        {
                            StartupManager.AddOrUpdateItem(item);
                            successCount++;
                        }
                        catch
                        {
                            skippedCount++;
                        }
                    }

                    textBox1.Text = $"恢复完成：成功 {successCount} 个，跳过 {skippedCount} 个";
                    if (skippedCount > 0)
                    {
                        MessageBox.Show(
                            $"成功恢复 {successCount} 个启动项。\n{skippedCount} 个需要管理员权限的项目未恢复，请以管理员身份重新运行本程序后再次恢复。",
                            "部分完成", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    LoadStartupItems();
                }
            }
        }
    }
}
