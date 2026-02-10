using System.Drawing;
using Sora.Entities;
using Sora.Entities.Segment;
using Sora.EventArgs.SoraEvent;
using Sora.Entities.Info;

namespace WindFrostBot.SDK
{
    public class QCommand
    {
        public long Account = 0;
        public long Group = 0;
        public int Type = 0;  //0为Sora Group;1为Sora Private;
        public Group? SourceGroup {  get; set; }
        public GroupMessageEventArgs? GroupMessageEvent { get; set; }
        public PrivateMessageEventArgs? PrivateMessageEvent { get; set; }
        public QCommand(GroupMessageEventArgs eventArgs) //type0,Sora Group;
        {
            Account = eventArgs.Sender.Id;
            Group = eventArgs.SourceGroup.Id;
            Type = 0;
            SourceGroup = eventArgs.SourceGroup;
            GroupMessageEvent = eventArgs;
        }
        public QCommand(PrivateMessageEventArgs eventArgs) //type1,Sora Private
        {
            Account = eventArgs.Sender.Id;
            //Group = eventArgs.SourceGroup.Id;
            Type = 1;
            PrivateMessageEvent = eventArgs;
        }
        //private MessageBody _messageBody = new MessageBody();
        public void SendTextMessage(string message)
        {
            MessageBody body = message;
            try
            {
                switch (Type)
                {
                    case 0:
                        //SourceGroup.SendGroupMessage(body);
                        MainSDK.service.GetApi(MainSDK.service.ServiceId).SendGroupMessage(Group , body);
                        break;
                    case 1:
                        //PrivateMessageEvent.Sender.SendPrivateMessage(body);
                        MainSDK.service.GetApi(MainSDK.service.ServiceId).SendPrivateMessage(Account, body);
                        break;
                    default:
                        break;
                }
            }
            catch(Exception ex)
            {
                if(MainSDK.service == null)
                {
                    Message.Erro("unknown servive");

                }
                else
                {
                    Message.Erro($"{MainSDK.service.ServiceId.ToString()}");

                }
                Message.Erro(ex.ToString());
                switch (Type)
                {
                    case 0:
                        Environment.Exit(0);
                        //MainSDK.OnReStartGroupMessage.ExecuteAll(new ReStartGroupMessageArgs(this, body));
                        break;
                }
            }
        }
        public QCommand ReplyMessage(string message)
        {
            int id = GroupMessageEvent.Message.MessageId;
            //_messageBody.Add()
            MessageBody body = new MessageBody(new List<SoraSegment>()
            {
                SoraSegment.Reply(id),
                SoraSegment.Text(message)
             });
            try
            {
                switch (Type)
                {
                    case 0:
                        MainSDK.service.GetApi(MainSDK.service.ServiceId).SendGroupMessage(Group, body);
                        //SourceGroup?.SendGroupMessage(body);
                        break;
                    case 1:
                        PrivateMessageEvent?.Sender.SendPrivateMessage(body);
                        break;
                    default:
                        break;
                }
            }
            catch
            {
                switch (Type)
                {
                    case 0:
                        Environment.Exit(0);
                        //MainSDK.OnReStartGroupMessage.ExecuteAll(new ReStartGroupMessageArgs(this, body));
                        break;
                }
            }
            return this;
        }
        public void UploadFile(string path, string name)
        {
            switch (Type)
            {
                case 0:
                    new Task(async () =>
                    {
                        try
                        {
                            await MainSDK.service.GetApi(MainSDK.service.ServiceId).UploadGroupFile(Group, path, name);
                        }
                        catch
                        {

                        }
                    }).Start(); 
                    break;
                case 1:
                    break;
            }
        }
        public void UploadFile(byte[] data, string path, string name,string foldname = "")
        {
            FileStream file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            file.Write(data, 0, data.Length);
            file.Close();
            switch (Type)
            {
                case 0:
                    if (!string.IsNullOrEmpty(foldname))
                    {
                        var folds = GroupMessageEvent.SourceGroup.GetGroupRootFiles().Result.groupFolders;
                        bool flag = false;
                        folds.ForEach(f =>
                        {
                            if(f.Name == foldname)
                            {
                                new Task(async () =>
                                {
                                    await MainSDK.service.GetApi(MainSDK.service.ServiceId).UploadGroupFile(Group, path, name, f.Id);
                                    File.Delete(path);
                                }).Start();
                                flag = true;
                                return;
                            }
                        });
                        if (flag)
                        {
                            return;
                        }
                    }
                    new Task(async () =>
                    {
                        await MainSDK.service.GetApi(MainSDK.service.ServiceId).UploadGroupFile(Group, path, name);
                        File.Delete(path);
                    }).Start();
                    break;
                case 1:
                    break;
            }
        }
        public void SendImage(Image img)
        {
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            Stream stream = new MemoryStream(ms.ToArray());
            MessageBody body = new MessageBody(new List<SoraSegment>()
            {
                SoraSegment.Image(stream) // 生成图片消息段
            });
            try
            {
                switch (Type)
                {
                    case 0:
                        GroupMessageEvent?.SourceGroup.SendGroupMessage(body);
                        break;
                    case 1:
                        PrivateMessageEvent?.Sender.SendPrivateMessage(body);
                        break;
                }
            }
            catch
            {
                switch (Type) 
                { 
                    case 0:
                        Environment.Exit(0);
                        //.OnReStartGroupMessage.ExecuteAll(new ReStartGroupMessageArgs(this, body));
                        break; 
                }
            }
        }
        public void SendImage(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            MessageBody body = new MessageBody(new List<SoraSegment>()
            {
                  SoraSegment.Image(stream)// 生成图片消息段
            });
            try
            {
                switch (Type)
                {
                    case 0:
                        SourceGroup?.SendGroupMessage(body);
                        break;
                    case 1:
                        PrivateMessageEvent?.Sender.SendPrivateMessage(body);
                        break;
                }
            }
            catch
            {
                switch (Type)
                {
                    case 0:
                        Environment.Exit(0);
                        //MainSDK.OnReStartGroupMessage.ExecuteAll(new ReStartGroupMessageArgs(this, body));
                        break;
                }
            }
        }
        public void SendComplex(MessageBody body)
        {
            try
            {
                switch (Type)
                {
                    case 0:
                        SourceGroup?.SendGroupMessage(body);
                        break;
                    case 1:
                        PrivateMessageEvent?.Sender.SendPrivateMessage(body);
                        break;
                }
            }
            catch
            {
                switch (Type)
                {
                    case 0:
                        Environment.Exit(0);
                        //MainSDK.OnReStartGroupMessage.ExecuteAll(new ReStartGroupMessageArgs(this, body));
                        break;
                }
            }
        }
        public List<GroupMemberInfo>? GetGroupMemembers()
        {
            try
            {
                switch (Type)
                {
                    case 0:
                        if (GroupMessageEvent != null)
                        {
                            return MainSDK.service.GetApi(GroupMessageEvent.ServiceId).GetGroupMemberList(Group).Result.groupMemberList;
                        }
                        return null;
                }
            }
            catch
            {

            }
            return null;
        }
    }
}
