using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using IWshRuntimeLibrary;
using File = System.IO.File;

namespace WindowsAppStaet
{
    /// <summary>
    /// 负责枚举、增加、删除、备份/恢复 Windows 开机启动项
    /// （注册表 Run / RunOnce 以及 启动文件夹）
    /// </summary>
    public static class StartupManager
    {
        private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string RunOnceKeyPath = @"Software\Microsoft\Windows\CurrentVersion\RunOnce";

        /// <summary>当前进程是否以管理员身份运行</summary>
        public static bool IsAdministrator()
        {
            try
            {
                using (var identity = WindowsIdentity.GetCurrent())
                {
                    var principal = new WindowsPrincipal(identity);
                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>以管理员身份重新启动当前程序（会退出当前进程）</summary>
        public static void RestartAsAdministrator()
        {
            try
            {
                var psi = new ProcessStartInfo(Application.ExecutablePath)
                {
                    UseShellExecute = true,
                    Verb = "runas"
                };
                Process.Start(psi);
                Application.Exit();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                // 用户在 UAC 提示中点击了"取消"，不做处理
            }
        }

        /// <summary>枚举所有已知位置的启动项</summary>
        public static List<StartupItem> GetAllStartupItems()
        {
            var list = new List<StartupItem>();
            list.AddRange(ReadRegistryItems(Registry.CurrentUser, RunKeyPath, StartupLocation.HKCU_Run));
            list.AddRange(ReadRegistryItems(Registry.LocalMachine, RunKeyPath, StartupLocation.HKLM_Run));
            list.AddRange(ReadRegistryItems(Registry.CurrentUser, RunOnceKeyPath, StartupLocation.HKCU_RunOnce));
            list.AddRange(ReadRegistryItems(Registry.LocalMachine, RunOnceKeyPath, StartupLocation.HKLM_RunOnce));
            list.AddRange(ReadFolderItems(Environment.GetFolderPath(Environment.SpecialFolder.Startup), StartupLocation.StartupFolderUser));
            list.AddRange(ReadFolderItems(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup), StartupLocation.StartupFolderCommon));
            return list;
        }

        private static List<StartupItem> ReadRegistryItems(RegistryKey root, string path, StartupLocation loc)
        {
            var result = new List<StartupItem>();
            try
            {
                using (var key = root.OpenSubKey(path, false))
                {
                    if (key == null) return result;
                    foreach (var name in key.GetValueNames())
                    {
                        if (string.IsNullOrEmpty(name)) continue;
                        var value = key.GetValue(name) as string;
                        result.Add(new StartupItem { Name = name, Command = value ?? "", Location = loc });
                    }
                }
            }
            catch
            {
                // 权限不足或键不存在时忽略
            }
            return result;
        }

        private static List<StartupItem> ReadFolderItems(string folder, StartupLocation loc)
        {
            var result = new List<StartupItem>();
            try
            {
                if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder)) return result;

                foreach (var file in Directory.GetFiles(folder))
                {
                    string ext = Path.GetExtension(file).ToLower();
                    string name = Path.GetFileNameWithoutExtension(file);
                    if (name.Equals("desktop", StringComparison.OrdinalIgnoreCase)) continue;

                    string command = file;
                    if (ext == ".lnk")
                    {
                        try
                        {
                            WshShell shell = new WshShell();
                            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(file);
                            if (!string.IsNullOrEmpty(shortcut.TargetPath))
                            {
                                command = shortcut.TargetPath;
                                if (!string.IsNullOrEmpty(shortcut.Arguments))
                                {
                                    command += " " + shortcut.Arguments;
                                }
                            }
                        }
                        catch
                        {
                            // 无法解析时退回使用快捷方式本身的路径
                        }
                    }

                    result.Add(new StartupItem { Name = name, Command = command, Location = loc });
                }
            }
            catch
            {
                // 权限不足时忽略
            }
            return result;
        }

        /// <summary>新增或更新一个启动项（按名称+位置覆盖写入）</summary>
        public static void AddOrUpdateItem(StartupItem item)
        {
            switch (item.Location)
            {
                case StartupLocation.HKCU_Run:
                    WriteRegistryValue(Registry.CurrentUser, RunKeyPath, item.Name, item.Command);
                    break;
                case StartupLocation.HKLM_Run:
                    WriteRegistryValue(Registry.LocalMachine, RunKeyPath, item.Name, item.Command);
                    break;
                case StartupLocation.HKCU_RunOnce:
                    WriteRegistryValue(Registry.CurrentUser, RunOnceKeyPath, item.Name, item.Command);
                    break;
                case StartupLocation.HKLM_RunOnce:
                    WriteRegistryValue(Registry.LocalMachine, RunOnceKeyPath, item.Name, item.Command);
                    break;
                case StartupLocation.StartupFolderUser:
                    CreateFolderShortcut(Environment.GetFolderPath(Environment.SpecialFolder.Startup), item);
                    break;
                case StartupLocation.StartupFolderCommon:
                    CreateFolderShortcut(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup), item);
                    break;
            }
        }

        private static void WriteRegistryValue(RegistryKey root, string path, string name, string value)
        {
            using (var key = root.CreateSubKey(path))
            {
                key.SetValue(name, value ?? "", RegistryValueKind.String);
            }
        }

        private static void CreateFolderShortcut(string folder, StartupItem item)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string lnkPath = Path.Combine(folder, item.Name + ".lnk");
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(lnkPath);

            string target = item.Command ?? "";
            string args = "";

            // 尝试从命令字符串中拆分出 目标路径 和 参数
            if (target.StartsWith("\""))
            {
                int endQuote = target.IndexOf('"', 1);
                if (endQuote > 0)
                {
                    args = target.Substring(endQuote + 1).Trim();
                    target = target.Substring(1, endQuote - 1);
                }
            }
            else
            {
                int spaceIdx = target.IndexOf(' ');
                if (spaceIdx > 0 && File.Exists(target.Substring(0, spaceIdx)))
                {
                    args = target.Substring(spaceIdx + 1).Trim();
                    target = target.Substring(0, spaceIdx);
                }
            }

            shortcut.TargetPath = target;
            shortcut.Arguments = args;
            shortcut.Save();
        }

        /// <summary>删除一个启动项</summary>
        public static void DeleteItem(StartupItem item)
        {
            switch (item.Location)
            {
                case StartupLocation.HKCU_Run:
                    DeleteRegistryValue(Registry.CurrentUser, RunKeyPath, item.Name);
                    break;
                case StartupLocation.HKLM_Run:
                    DeleteRegistryValue(Registry.LocalMachine, RunKeyPath, item.Name);
                    break;
                case StartupLocation.HKCU_RunOnce:
                    DeleteRegistryValue(Registry.CurrentUser, RunOnceKeyPath, item.Name);
                    break;
                case StartupLocation.HKLM_RunOnce:
                    DeleteRegistryValue(Registry.LocalMachine, RunOnceKeyPath, item.Name);
                    break;
                case StartupLocation.StartupFolderUser:
                    DeleteFolderShortcut(Environment.GetFolderPath(Environment.SpecialFolder.Startup), item.Name);
                    break;
                case StartupLocation.StartupFolderCommon:
                    DeleteFolderShortcut(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup), item.Name);
                    break;
            }
        }

        private static void DeleteRegistryValue(RegistryKey root, string path, string name)
        {
            using (var key = root.OpenSubKey(path, true))
            {
                if (key != null && Array.IndexOf(key.GetValueNames(), name) >= 0)
                {
                    key.DeleteValue(name, false);
                }
            }
        }

        private static void DeleteFolderShortcut(string folder, string name)
        {
            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder)) return;

            string lnkPath = Path.Combine(folder, name + ".lnk");
            if (File.Exists(lnkPath))
            {
                File.Delete(lnkPath);
                return;
            }

            foreach (var f in Directory.GetFiles(folder))
            {
                if (Path.GetFileNameWithoutExtension(f).Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    File.Delete(f);
                    break;
                }
            }
        }

        /// <summary>
        /// 备份启动项列表到文本文件，每行格式：名称|命令|位置
        /// </summary>
        public static void BackupItems(IEnumerable<StartupItem> items, string filePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# 系统启动项备份文件，由 WindowsAppStaet 生成");
            sb.AppendLine("# 格式：名称|命令|位置");
            foreach (var item in items)
            {
                string safeCommand = (item.Command ?? "").Replace("|", "¦"); // 避免命令中出现分隔符
                sb.AppendLine($"{item.Name}|{safeCommand}|{item.Location}");
            }
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// 从备份文件加载启动项列表（不会自动写回系统，需要调用方逐条调用 AddOrUpdateItem）
        /// </summary>
        public static List<StartupItem> LoadBackup(string filePath)
        {
            var result = new List<StartupItem>();
            var lines = File.ReadAllLines(filePath, Encoding.UTF8);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#")) continue;
                var parts = line.Split('|');
                if (parts.Length < 3) continue;

                if (Enum.TryParse(parts[2].Trim(), out StartupLocation loc))
                {
                    result.Add(new StartupItem
                    {
                        Name = parts[0],
                        Command = parts[1].Replace("¦", "|"),
                        Location = loc
                    });
                }
            }
            return result;
        }
    }
}
