using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// �ͻ�������
/// </summary>
public class Client
{
    public Socket Sock;//��֮ͨ�ŵ��׽���
    public byte[] Data = new byte[1024];
    public string Name;
}
public class MsgData
{
    public int Id;
    public Client Client;
    public byte[] Data = new byte[1024];
}