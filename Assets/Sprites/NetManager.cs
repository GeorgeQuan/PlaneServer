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
        Debug.Log("服务器已开启");
        _mainSocket.BeginAccept(OnEndAccept, null);//开始接收客户端连接
    }
    /// <summary>
    /// 连接回调
    /// </summary>
    /// <param name="ar"></param>
    private void OnEndAccept(IAsyncResult ar)
    {
        Socket sock = _mainSocket.EndAccept(ar);//获取与客户端连接的套接字
        IPEndPoint ip = sock.RemoteEndPoint as IPEndPoint;//获取客户端信息
        Debug.Log($"{ip.Port}连接成功");
        Client cli = new Client();//创建客户端类
        cli.Sock = sock;
        cli.Sock.BeginReceive(cli.Data, 0, cli.Data.Length, SocketFlags.None, OnEndReceive, cli);

        ChatManager.Instance.AllClient.Add(cli);//存储与服务器通讯的客户端,

        _mainSocket.BeginAccept(OnEndAccept, null);//重新开始监听客户端连接
    }
    /// <summary>
    /// 接收客户端数据回调
    /// </summary>
    /// <param name="ar"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnEndReceive(IAsyncResult ar)
    {
        Client cli = ar.AsyncState as Client;
        int len = cli.Sock.EndReceive(ar);//获取接收到的数据的长度
        try
        {
            if (len > 0)
            {
                byte[] data = new byte[len];//定义与接收到数据一样长度的容器
                Buffer.BlockCopy(cli.Data, 0, data, 0, len);//把接收到的数据复制进新的容器
                while (data.Length >= 8)
                {
                    int bodylen = BitConverter.ToInt32(data, 0);//自动从0开始获取4个字节转换成int类型,前四个字节看起来是包的长度
                    byte[] data2 = new byte[bodylen];//定义包长度大小的容器用来存储数据
                    Buffer.BlockCopy(data, 4, data2, 0, bodylen);//复制去除包的长度的数据
                    int num = BitConverter.ToInt32(data2, 0);//再从前面拿4个字节,这里是发送的数据的消息号
                    byte[] data3 = new byte[data2.Length - 4];//这里定义存储包的数据的容器
                    Buffer.BlockCopy(data2, 4, data3, 0, data2.Length - 4);//复制数据
                    MsgData msg = new MsgData();//定义消息数据类
                    msg.Data = data3;//包体数据
                    msg.Id = num;//消息号
                    msg.Client = cli;//与之通讯的客户端数据
                    MessageCenter<MsgData>.Instance.BroadCast(num, msg);//广播消息

                    int EndBodyLen = data.Length - 4 - bodylen;//接收到的数据减去本次处理过的数据
                    byte[] NewBody = new byte[EndBodyLen];
                    Buffer.BlockCopy(data, bodylen + 4, NewBody, 0, EndBodyLen);//如果有未处理的数据就复制进新创建的容器
                    data = NewBody;//把未处理的数据复制给数据容器  
                    //这里是对粘包进行的处理
                }
                cli.Sock.BeginReceive(cli.Data, 0, cli.Data.Length, SocketFlags.None, OnEndReceive, cli);
            }
            else
            {
                //没有发送过来数据,有客户端断开连接的可能
            }
        }
        catch (Exception ex)
        {
            //报错除去逻辑错误就算客户端断开了连接
            Debug.Log(ex.ToString());//打印报错信息 
        }
    }
    public void OnSendCall(int id, byte[] data, Client cli)
    {
        byte[] info = new byte[0];
        info = info.Concat(BitConverter.GetBytes(data.Length + 4)).Concat(BitConverter.GetBytes(id)).Concat(data).ToArray();//包体长度+消息号+包体数据拼接成一个包
        cli.Sock.BeginSend(info, 0, info.Length, SocketFlags.None, OnEndSend, cli);//发送数据
    }

    private void OnEndSend(IAsyncResult ar)
    {
        Client client = ar.AsyncState as Client;
        int len = client.Sock.EndSend(ar);
        Debug.Log($"服务器共发送{len}长度");
    }
}
