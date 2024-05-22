using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageCenter<T> : Singleton<MessageCenter<T>>
{
    public Dictionary<int, Action<T>> MessageDic = new Dictionary<int, Action<T>>();
    public void AddListen(int id, Action<T> action)
    {
        if (MessageDic.ContainsKey(id))
        {
            MessageDic[id] += action;
        }
        else
        {
            MessageDic.Add(id, action);
        }
    }
    public void RemoveListen(int id, Action<T> action)
    {
        if (MessageDic.ContainsKey(id))
        {
            MessageDic[id] -= action;
            if (MessageDic[id] == null)
            {
                MessageDic.Remove(id);
            }
        }
    }
    public void BroadCast(int id, T t)
    {
        if (MessageDic.ContainsKey(id))
        {
            MessageDic[id](t);
        }
    }
}
