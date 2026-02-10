using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindFrostBot.SDK;

namespace WindFrostBot
{
    public class InitPlugin : Plugin
    {
        public override string PluginName()
        {
            return "InitPlugin";
        }

        public override string Version()
        {
            return "1.0.1";
        }

        public override string Author()
        {
            return "Cjx";
        }
        public override string Description()
        {
            return "初始插件";
        }
        public override void OnLoad()
        {
            //MainSDK.OnReStartGroupMessage.AddFunction(this, OnGroupMessageRestart);
            Admin.Init(this);//添加管理指令
            Group.Init(this);//添加群聊指令
            CommandManager.InitGroupCommand(this, Reload, "重读配置", "reload", "重读","重读配置");
            //重读事件
        }
        public void OnGroupMessageRestart(ReStartGroupMessageArgs args)
        {
            Program.ReStartSora();
            try
            {
                args.ReSend();
            }
            catch(Exception ex)
            {
                Message.Erro(ex.Message);
                Environment.Exit(0);
            }
        }
        public override string OnReload()
        {
            ConfigWriter.ReadConfig();
            return "重读配置文件成功!";
        }
        public static void Reload(CommandArgs args)
        {
            if (!args.IsOwnner())
            {
                return;
            }
            try
            {
                int number = 0;
                string reloadtext = "";
                foreach (var plugin in PluginLoader.Plugins)
                {
                    string result = plugin.OnReload();
                    number++;
                    if (!string.IsNullOrEmpty(result))
                    {
                        reloadtext += $"\n[{plugin.PluginName()}]{result}";
                    }
                }
                args.Api.SendTextMessage($"[{ConfigWriter.GetConfig().BotName}]成功执行了 {number} 个插件的重读函数!{reloadtext}");
            }
            catch (Exception ex)
            {
                args.Api.SendTextMessage($"[{ConfigWriter.GetConfig().BotName}]重读出错!");
            }
        }
    }
}
