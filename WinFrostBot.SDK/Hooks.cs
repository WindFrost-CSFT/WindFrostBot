using Sora.Entities;
using System;

namespace WindFrostBot.SDK
{
    public class GroupAtArgs : EventArgs
    {
        public long Account;
        public QCommand Api;
        public string Message;
        public GroupAtArgs(QCommand qcmd, string message)
        {
            Account = qcmd.Account;
            Api = qcmd;
            Message = message;
        }
    }
    public class GroupMessgaeArgs : EventArgs
    {
        public long Account;
        public QCommand Api;
        public string Message;
        public bool Handled = false;
        public GroupMessgaeArgs(QCommand qcmd, string message)
        {
            Account = qcmd.Account;
            Api = qcmd;
            Message = message;
        }
    }
    public class ReStartGroupMessageArgs : EventArgs
    {
        public long Account;
        public QCommand Api;
        public MessageBody Message;
        public ReStartGroupMessageArgs(QCommand qcmd, MessageBody message)
        {
            Account = qcmd.Account;
            Api = qcmd;
            Message = message;
        }
        public void ReSend()
        {
            MainSDK.service.GetApi(MainSDK.service.ServiceId).SendGroupMessage(Api.Group, Message);
        }
    }
}
