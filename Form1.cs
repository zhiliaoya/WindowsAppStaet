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

            // 设置 numericUpDown1 默认值
            numericUpDown1.Minimum = 0;
            numericUpDown1.Maximum = 300;
            numericUpDown1.Value = 15;
        }

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
                            // 成功解析，使用目标路径
                            AddOrUpdateShortcut(shortcutName, targetPath);
                        }
                        else
                        {
                            // 无法解析，直接使用快捷方式文件路径
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
                        // 如果不是快捷方式，也允许添加
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
                // 更新已存在的项
                shortcutDict[name] = path;
                textBox1.Text = $"已更新快捷方式：{name} -> {path}";
            }
            else
            {
                // 添加新项
                shortcutDict.Add(name, path);
                checkedListBox1.Items.Add(name);
                textBox1.Text = $"已添加快捷方式：{name} -> {path}";
            }
        }

        /// <summary>
        /// CheckedListBox 勾选状态改变事件 - 实现单选逻辑
        /// </summary>
        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // 只在用户勾选新项时处理（取消勾选不处理）
            if (e.NewValue == CheckState.Checked)
            {
                // 遍历所有项，取消其他项的勾选
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    if (i != e.Index)
                    {
                        checkedListBox1.SetItemChecked(i, false);
                    }
                }

                // 更新状态栏显示当前选中的快捷方式
                string selectedKey = checkedListBox1.Items[e.Index].ToString();
                if (shortcutDict.ContainsKey(selectedKey))
                {
                    textBox1.Text = $"已选择：{selectedKey} -> {shortcutDict[selectedKey]}";
                }
            }
            else
            {
                // 如果用户取消当前项的勾选
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

                // 如果目标路径包含环境变量，尝试展开
                if (!string.IsNullOrEmpty(targetPath) && targetPath.Contains("%"))
                {
                    targetPath = Environment.ExpandEnvironmentVariables(targetPath);
                }

                return targetPath;
            }
            catch
            {
                // 解析失败返回null，调用方会使用原始快捷方式路径
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

            // 使用保存对话框让用户选择保存位置
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

                // 第一个启动项
                batContent.AppendLine($"echo 正在启动第一程序：{firstItem.Key}");
                string firstCommand = GenerateStartCommand(firstItem.Value);
                batContent.AppendLine(firstCommand);
                batContent.AppendLine($"echo 第一程序已启动，等待{delaySeconds}秒...");

                if (delaySeconds > 0)
                {
                    batContent.AppendLine($"timeout /t {delaySeconds} /nobreak >nul");
                }

                // 其他启动项
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

                // 创建快捷方式到启动目录
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
            // 判断是否为.lnk快捷方式文件
            if (path.ToLower().EndsWith(".lnk"))
            {
                return $"start \"\" \"{path}\"";
            }
            // 判断是否为.url快捷方式文件
            else if (path.ToLower().EndsWith(".url"))
            {
                return $"start \"\" \"{path}\"";
            }
            // 判断是否为网址
            else if (path.StartsWith("http://") || path.StartsWith("https://"))
            {
                return $"start \"\" \"{path}\"";
            }
            // 其他可执行文件或文档
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
                // 获取系统启动目录路径
                string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

                // 快捷方式名称（去掉.bat扩展名）
                string shortcutName = Path.GetFileNameWithoutExtension(batFilePath);
                string shortcutPath = Path.Combine(startupPath, $"{shortcutName}.lnk");

                // 使用WSH创建快捷方式
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);

                // 设置快捷方式属性
                shortcut.TargetPath = batFilePath;
                shortcut.WorkingDirectory = Path.GetDirectoryName(batFilePath);
                shortcut.Description = $"自动启动脚本 - 创建于 {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                shortcut.WindowStyle = 7; // 最小化运行窗口

                // 保存快捷方式
                shortcut.Save();
            }
            catch (Exception ex)
            {
                // 如果创建快捷方式失败，不影响主流程，但给出警告
                MessageBox.Show($"BAT脚本已生成，但创建启动快捷方式失败：{ex.Message}\n\n" +
                               $"您可以手动将脚本复制到启动目录：\n{Environment.GetFolderPath(Environment.SpecialFolder.Startup)}",
                               "警告",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}