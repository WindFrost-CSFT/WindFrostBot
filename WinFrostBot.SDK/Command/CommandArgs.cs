using Sora.EventArgs.SoraEvent;
using System;

namespace WindFrostBot.SDK
{
    public delegate void ComDelegate(CommandArgs args);
    public class CommandArgs : EventArgs
    {
        public string Message { get; private set; }
        public List<string> Parameters { get; private set; }
        public long Account = 0;
        public long Group = 0;
        public bool Handled = false;
        //public GroupMessageEventArgs EventArgs { get; private set; }

        public bool IsOwnner()
        {
            if (MainSDK.BotConfig.Owners.Contains(Account))
            {
                return true;
            }
            return false;
        }
        public bool IsAdmin()
        {
            if (MainSDK.BotConfig.Owners.Contains(Account) || MainSDK.BotConfig.Admins.Contains(Account))
            {
                return true;
            }
            return false;
        }
        public QCommand Api { get; private set; }
        public CommandArgs(string msg,List<string> args, QCommand cmd)
        {
            Parameters = args;
            Message = msg;
            Account = cmd.Account;
            Group = cmd.Group;
            Api = cmd;
            //EventArgs = eventarg;
        }
    }
    public class CommandManager
    {
        public static List<Command> Coms = new List<Command>();
        public static void InitCommandToSora()
        {
            //群聊部分
            MainSDK.service.Event.OnGroupMessage += (sender, eventArgs) =>
            {
                if (!Utils.Utils.CanSend(eventArgs.SourceGroup.Id))
                {
                    return ValueTask.CompletedTask;
                }
                string text = eventArgs.Message.ToString();//接收的所有消息
                string msg = text.Split(" ")[0].ToLower();//指令消息
                List<string> arg = text.Split(" ").ToList();
                arg.Remove(text.Split(" ")[0]);//除去指令消息的其他段消息
                //var cmd =  Coms.Find(c => c.Names.Contains(msg));
                if (eventArgs.Message.GetAllAtList().Contains(eventArgs.LoginUid) && !eventArgs.Message.ToString().Contains("mp4") && !eventArgs.Message.ToString().Contains("amr"))
                {
                    var handler = new GroupAtArgs(new QCommand(eventArgs), eventArgs.Message.ToString());
                    MainSDK.OnGroupAt.ExecuteAll(handler);
                }
                var ongroupmessage = new GroupMessgaeArgs(new QCommand(eventArgs), eventArgs.Message.ToString());
                MainSDK.OnGroupMessgae.ExecuteAll(ongroupmessage);
                if (!ongroupmessage.Handled)
                {
                    foreach (var cmd in Coms)
                    {
                        if (cmd != null && cmd.Names.Contains(msg) && cmd.Type == 0)
                        {
                            try
                            {
                                var handler = new CommandArgs(msg, arg, new QCommand(eventArgs));
                                MainSDK.OnCommand.ExecuteAll(handler);
                                if (!handler.Handled)
                                {
                                    cmd.Run(msg, arg, new QCommand(eventArgs));
                                }
                            }
                            catch (Exception ex)
                            {
                                if (ex.Message.Contains("System.NullReferenceException"))
                                {
                                    Environment.Exit(0);
                                }
                            }
                        }
                    }
                }
                return ValueTask.CompletedTask;
            };
            //私聊部分
            MainSDK.service.Event.OnPrivateMessage += (sender, eventArgs) =>
            {
                //if (!Utils.CanSend(eventArgs.SourceGroup.Id))
                //{
                    //return ValueTask.CompletedTask;
                //}
                string text = eventArgs.Message.ToString();//接收的所有消息
                string msg = text.Split(" ")[0].ToLower();//指令消息
                List<string> arg = text.Split(" ").ToList();
                arg.Remove(text.Split(" ")[0]);//除去指令消息的其他段消息
                var cmd = Coms.Find(c => c.Names.Contains(msg));
                if (cmd != null)
                {
                    if (cmd.Type == 1)
                    {
                        try
                        {
                            cmd.Run(msg, arg, new QCommand(eventArgs));
                        }
                        catch (Exception ex)
                        {
                            Message.LogErro(ex.Message);
                            if (ex.Message.Contains("System.NullReferenceException"))
                            {
                                Environment.Exit(0);
                            }
                        }
                    }
                }
                return ValueTask.CompletedTask;
            };
            MainSDK.service.Event.OnGroupRequest += (sender, eventArgs) =>
            {
                if (Utils.Utils.IsOwner(eventArgs.Sender.Id))
                {
                    eventArgs.Accept();
                }
                return ValueTask.CompletedTask;
            };
            //MainSDK.service.Event. += (sender, eventArgs) =>
            //{
                //eventArgs.EventName
                //return ValueTask.CompletedTask;
            //};
        }
        public static void InitGroupCommand(Plugin plugin,ComDelegate cmd,string cmdinfo,params string[] cmdnames)
        {
            Command com = new Command(cmd, cmdinfo, 0, cmdnames);
            plugin.Commands.Add(com);
            Coms.Add(com);
        }
        public static void InitPrivateCommand(Plugin plugin, ComDelegate cmd, string cmdinfo, params string[] cmdnames)
        {
            Command com = new Command(cmd, cmdinfo, 1, cmdnames);
            plugin.Commands.Add(com);
            Coms.Add(com);
        }
    }
    public class Command
    {
        private ComDelegate cd;
        public int Type;
        public ComDelegate Cd
        {
            get
            {
                return cd;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                cd = value;
            }
        }
        public Command(ComDelegate cmd, int type,params string[] names)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException("cmd");
            }

            if (names == null || names.Length < 1)
            {
                throw new ArgumentException("names");
            }
            Names = new List<string>(names);
            cd = cmd;
            HelpText = "此指令没有帮助.";
            Type = type;
        }
        public Command(ComDelegate cmd, string help, int type ,params string[] names)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException("cmd");
            }

            if (names == null || names.Length < 1)
            {
                throw new ArgumentException("names");
            }
            Names = new List<string>(names);
            cd = cmd;
            HelpText = help;
            Type = type;
        }
        public List<string> Names = new List<string>();
        public string HelpText = "";
        public bool Run(string msg,List<string> parms,QCommand cmd)
        {
            try
            {
                cd(new CommandArgs(msg, parms, cmd));
            }
            catch(Exception ex)
            {
                Message.Erro("指令出错!:" + ex.ToString());
                if (ex.ToString().ToLower().Contains("null"))
                {
                    Environment.Exit(0);
                }
            }
            return true;
        }
    }
}
