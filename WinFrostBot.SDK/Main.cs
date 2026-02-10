using System.Data;
using Sora.EventArgs.SoraEvent;
using Sora.Interfaces;

namespace WindFrostBot.SDK
{
    public class MainSDK
    {
        public static Config BotConfig { get; set; }
        public static ISoraService service { get; set; }
        public static IDbConnection Db { get; set; }
        //public static FunctionManager<ReStartGroupMessageArgs> OnReStartGroupMessage = new FunctionManager<ReStartGroupMessageArgs>();
        public static FunctionManager<GroupAtArgs> OnGroupAt = new FunctionManager<GroupAtArgs>();
        public static FunctionManager<CommandArgs> OnCommand = new FunctionManager<CommandArgs>();
        public static FunctionManager<GroupMessgaeArgs> OnGroupMessgae = new FunctionManager<GroupMessgaeArgs>();
        public static FunctionManager<GroupMemberChangeEventArgs> OnGroupMemberChange = new FunctionManager<GroupMemberChangeEventArgs>();
        public static FunctionManager<AddGroupRequestEventArgs> OnGroupRequest = new FunctionManager<AddGroupRequestEventArgs>();
    }
    public abstract class Plugin
    {
        public string ?PluginPath { get; set; }
        public List<Command> Commands = new List<Command>();
        public abstract string PluginName();
        public abstract string Version();
        public abstract string Author();
        public abstract string Description();
        public abstract void OnLoad();
        public virtual string OnReload()
        {
            return "";
        }
        public virtual void OnDispose()
        {
            var copylist = new List<Command>(Commands);
            foreach(var command in copylist)
            {
                CommandManager.Coms.Remove(command);
            }
            if (MainSDK.OnCommand.functions.ContainsKey(PluginName()))
            {
                MainSDK.OnCommand.functions.Remove(PluginName());
            }
            if (MainSDK.OnGroupMemberChange.functions.ContainsKey(PluginName()))
            {
                MainSDK.OnGroupMemberChange.functions.Remove(PluginName());
            }
            if (MainSDK.OnGroupRequest.functions.ContainsKey(PluginName()))
            {
                MainSDK.OnGroupRequest.functions.Remove(PluginName());
            }
            if (MainSDK.OnGroupAt.functions.ContainsKey(PluginName()))
            {
                MainSDK.OnGroupAt.functions.Remove(PluginName());
            }
            if (MainSDK.OnGroupMessgae.functions.ContainsKey(PluginName()))
            {
                MainSDK.OnGroupMessgae.functions.Remove(PluginName());
            }
        }
    }
    public class FunctionManager<T>
    {
        public Dictionary<string, Action<T>> functions = new Dictionary<string, Action<T>>();
        public void AddFunction(Plugin plugin ,Action<T> func)
        {
            if (!functions.ContainsKey(plugin.PluginName()))
            {
                functions.Add(plugin.PluginName(), func);
            }
        }
        public void ExecuteAll(T args)
        {
            foreach (var func in functions)
            {
                func.Value(args);
            }
        }
    }
}