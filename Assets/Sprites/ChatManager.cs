using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatManager : Singleton<ChatManager>   
{
   public List<Client> AllClient=new List<Client>();//存储所有连接上的客户端
    /// <summary>
    /// 初始化 在这里定义事件的监听,飞机案例中不需要监听客户端消息
    /// </summary>
    public void InitChat()
    {

    }
    /// <summary>
    /// 拿去服务器飞机数据发送给所有的客户端
    /// </summary>
    public void SendPos(MsgData data)
    {
        for (int i = 0; i < AllClient.Count; i++)
        {
            NetManager.Instance.OnSendCall(data.Id, data.Data, AllClient[i]);
        }
    }
}
