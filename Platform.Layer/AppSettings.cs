using System;
using System.Configuration;
using System.IO;
using Logger;

namespace Platform.Layer
{
    public class AppSettings
    {
        // 简化了的读取配置文件的方法, 读取失败返回空字符串
        public static string Get(string keyName)
        {
            try
            {
                return ConfigurationManager.AppSettings[keyName];
            }
            catch (Exception ex)
            {
                Log.AppendErrorInfo(ex.Message, ex);
                return "";
            }
        }

        // 添加
        public static bool Add(string keyName, string value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            try
            {
                config.AppSettings.Settings.Add(keyName, value);
                config.Save(ConfigurationSaveMode.Modified); //save
                ConfigurationManager.RefreshSection("appSettings"); //重新加载新的配置文件
            }
            catch (Exception ex)
            {
                Log.AppendErrorInfo(ex.Message, ex);
                return false;
            }
            return true;
        }

        // 删除
        public static bool Remove(string keyName)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            try
            {
                config.AppSettings.Settings.Remove(keyName);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings"); //重新加载新的配置文件   
            }
            catch (Exception ex)
            {
                Log.AppendErrorInfo(ex.Message, ex);
                return false;
            }
            return true;
        }

        // 修改
        public static bool Set(string keyName, string value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            try
            {
                if (config.AppSettings.Settings[keyName] == null)
                {
                    config.AppSettings.Settings.Add(keyName, value);
                }
                else
                {
                    config.AppSettings.Settings[keyName].Value = value;
                }
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings"); //重新加载新的配置文件   
            }
            catch (Exception ex)
            {
                Log.AppendErrorInfo(ex.Message, ex);
                return false;
            }
            return true;
        }

        /// <summary>
        ///     获取指定的配置文件的节点的值
        /// </summary>
        /// <param name="sConfigPath">指定的配置文件的物理路径</param>
        /// <param name="sAppSettingsKey">配置文件内的AppSettings内的节点名称</param>
        /// <returns>节点值</returns>
        public static string GetAppointConfig(string sConfigPath, string sAppSettingsKey)
        {
            try
            {
                if (!File.Exists(sConfigPath)) return "";
                var configFileMap = new ExeConfigurationFileMap();
                configFileMap.ExeConfigFilename = sConfigPath;
                var config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
                return config.AppSettings.Settings[sAppSettingsKey].Value.Trim();
            }
            catch (Exception ex)
            {
                Log.AppendErrorInfo(ex.Message, ex);
                return "";
            }
        }

        /// <summary>
        ///     更新指定的配置文件的节点的值
        /// </summary>
        /// <param name="sConfigPath">指定的配置文件的物理路径</param>
        /// <param name="sAppSettingKey">配置文件内的AppSettings内的节点名称</param>
        /// <param name="value">要更新到的值</param>
        /// <returns>更新是否成功</returns>
        public static void SetAppointConfig(string sConfigPath, string sAppSettingKey, string value)
        {
            try
            {
                if (!File.Exists(sConfigPath)) return;
                var configFileMap = new ExeConfigurationFileMap();
                configFileMap.ExeConfigFilename = sConfigPath;
                var config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
                config.AppSettings.Settings[sAppSettingKey].Value = value;
                //save
                config.Save(ConfigurationSaveMode.Modified);
                //reload
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception ex)
            {
                Log.AppendErrorInfo(ex.Message, ex);
            }
        }
    }
}
