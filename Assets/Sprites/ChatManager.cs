using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatManager : Singleton<ChatManager>   
{
   public List<Client> AllClient=new List<Client>();//�洢���������ϵĿͻ���
    /// <summary>
    /// ��ʼ�� �����ﶨ���¼��ļ���,�ɻ������в���Ҫ�����ͻ�����Ϣ
    /// </summary>
    public void InitChat()
    {

    }
    /// <summary>
    /// ��ȥ�������ɻ����ݷ��͸����еĿͻ���
    /// </summary>
    public void SendPos(MsgData data)
    {
        for (int i = 0; i < AllClient.Count; i++)
        {
            NetManager.Instance.OnSendCall(data.Id, data.Data, AllClient[i]);
        }
    }
}
