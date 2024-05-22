using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// 客户端数据
/// </summary>
public class Client
{
    public Socket Sock;//与之通信的套接字
    public byte[] Data = new byte[1024];
    public string Name;
}
public class MsgData
{
    public int Id;
    public Client Client;
    public byte[] Data = new byte[1024];
}