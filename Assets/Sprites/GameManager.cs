using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    
    void Start()
    {
       
        ChatManager.Instance.InitChat();
        NetManager.Instance.InitSocket();
        Debug.Log("Íõ†´Ð¦ÎûÎûs");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
