using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class NetManager : Singleton<NetManager>
{
    Socket _mainSocket;
    public void InitSocket()
    {
        _mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _mainSocket.Bind(new IPEndPoint(IPAddress.Any, 2109));
        _mainSocket.Listen(100);
        Debug.Log("�������ѿ���");
        _mainSocket.BeginAccept(OnEndAccept, null);//��ʼ���տͻ�������
    }
    /// <summary>
    /// ���ӻص�
    /// </summary>
    /// <param name="ar"></param>
    private void OnEndAccept(IAsyncResult ar)
    {
        Socket sock = _mainSocket.EndAccept(ar);//��ȡ��ͻ������ӵ��׽���
        IPEndPoint ip = sock.RemoteEndPoint as IPEndPoint;//��ȡ�ͻ�����Ϣ
        Debug.Log($"{ip.Port}���ӳɹ�");
        Client cli = new Client();//�����ͻ�����
        cli.Sock = sock;
        cli.Sock.BeginReceive(cli.Data, 0, cli.Data.Length, SocketFlags.None, OnEndReceive, cli);

        ChatManager.Instance.AllClient.Add(cli);//�洢�������ͨѶ�Ŀͻ���,

        _mainSocket.BeginAccept(OnEndAccept, null);//���¿�ʼ�����ͻ�������
    }
    /// <summary>
    /// ���տͻ������ݻص�
    /// </summary>
    /// <param name="ar"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnEndReceive(IAsyncResult ar)
    {
        Client cli = ar.AsyncState as Client;
        int len = cli.Sock.EndReceive(ar);//��ȡ���յ������ݵĳ���
        try
        {
            if (len > 0)
            {
                byte[] data = new byte[len];//��������յ�����һ�����ȵ�����
                Buffer.BlockCopy(cli.Data, 0, data, 0, len);//�ѽ��յ������ݸ��ƽ��µ�����
                while (data.Length >= 8)
                {
                    int bodylen = BitConverter.ToInt32(data, 0);//�Զ���0��ʼ��ȡ4���ֽ�ת����int����,ǰ�ĸ��ֽڿ������ǰ��ĳ���
                    byte[] data2 = new byte[bodylen];//��������ȴ�С�����������洢����
                    Buffer.BlockCopy(data, 4, data2, 0, bodylen);//����ȥ�����ĳ��ȵ�����
                    int num = BitConverter.ToInt32(data2, 0);//�ٴ�ǰ����4���ֽ�,�����Ƿ��͵����ݵ���Ϣ��
                    byte[] data3 = new byte[data2.Length - 4];//���ﶨ��洢�������ݵ�����
                    Buffer.BlockCopy(data2, 4, data3, 0, data2.Length - 4);//��������
                    MsgData msg = new MsgData();//������Ϣ������
                    msg.Data = data3;//��������
                    msg.Id = num;//��Ϣ��
                    msg.Client = cli;//��֮ͨѶ�Ŀͻ�������
                    MessageCenter<MsgData>.Instance.BroadCast(num, msg);//�㲥��Ϣ

                    int EndBodyLen = data.Length - 4 - bodylen;//���յ������ݼ�ȥ���δ����������
                    byte[] NewBody = new byte[EndBodyLen];
                    Buffer.BlockCopy(data, bodylen + 4, NewBody, 0, EndBodyLen);//�����δ��������ݾ͸��ƽ��´���������
                    data = NewBody;//��δ��������ݸ��Ƹ���������  
                    //�����Ƕ�ճ�����еĴ���
                }
                cli.Sock.BeginReceive(cli.Data, 0, cli.Data.Length, SocketFlags.None, OnEndReceive, cli);
            }
            else
            {
                //û�з��͹�������,�пͻ��˶Ͽ����ӵĿ���
            }
        }
        catch (Exception ex)
        {
            //�����ȥ�߼��������ͻ��˶Ͽ�������
            Debug.Log(ex.ToString());//��ӡ������Ϣ 
        }
    }
    public void OnSendCall(int id, byte[] data, Client cli)
    {
        byte[] info = new byte[0];
        info = info.Concat(BitConverter.GetBytes(data.Length + 4)).Concat(BitConverter.GetBytes(id)).Concat(data).ToArray();//���峤��+��Ϣ��+��������ƴ�ӳ�һ����
        cli.Sock.BeginSend(info, 0, info.Length, SocketFlags.None, OnEndSend, cli);//��������
    }

    private void OnEndSend(IAsyncResult ar)
    {
        Client client = ar.AsyncState as Client;
        int len = client.Sock.EndSend(ar);
        Debug.Log($"������������{len}����");
    }
}
