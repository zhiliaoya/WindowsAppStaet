using System;

namespace WindowsAppStaet
{
    /// <summary>
    /// 启动项来源位置
    /// </summary>
    public enum StartupLocation
    {
        HKCU_Run,            // 当前用户 注册表 Run
        HKLM_Run,            // 所有用户 注册表 Run（需管理员权限）
        HKCU_RunOnce,        // 当前用户 注册表 RunOnce
        HKLM_RunOnce,        // 所有用户 注册表 RunOnce（需管理员权限）
        StartupFolderUser,   // 当前用户 启动文件夹
        StartupFolderCommon  // 所有用户 启动文件夹（需管理员权限）
    }

    /// <summary>
    /// 表示一个系统开机启动项
    /// </summary>
    public class StartupItem
    {
        /// <summary>启动项名称（注册表键值名 / 快捷方式文件名）</summary>
        public string Name { get; set; }

        /// <summary>启动命令或目标路径（可能包含参数）</summary>
        public string Command { get; set; }

        /// <summary>来源位置</summary>
        public StartupLocation Location { get; set; }

        /// <summary>用于界面展示的中文位置描述</summary>
        public string LocationDisplay
        {
            get
            {
                switch (Location)
                {
                    case StartupLocation.HKCU_Run: return "注册表-当前用户(Run)";
                    case StartupLocation.HKLM_Run: return "注册表-所有用户(Run)";
                    case StartupLocation.HKCU_RunOnce: return "注册表-当前用户(RunOnce)";
                    case StartupLocation.HKLM_RunOnce: return "注册表-所有用户(RunOnce)";
                    case StartupLocation.StartupFolderUser: return "启动文件夹-当前用户";
                    case StartupLocation.StartupFolderCommon: return "启动文件夹-所有用户";
                    default: return "未知";
                }
            }
        }

        /// <summary>该位置的增删改是否需要管理员权限</summary>
        public bool RequiresAdmin =>
            Location == StartupLocation.HKLM_Run ||
            Location == StartupLocation.HKLM_RunOnce ||
            Location == StartupLocation.StartupFolderCommon;
    }
}
