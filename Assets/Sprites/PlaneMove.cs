using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlaneMove : MonoBehaviour
{

    public Button ZiDong;
    public Button ShouDong;


    [SerializeField]
    List<GameObject> AllPoint = new List<GameObject>();//所有的点

    Queue<GameObject> Points = new Queue<GameObject>();//存储一次行程的目标点
    [SerializeField]
    GameObject _nowPoint;

    int index = 1;
    [SerializeField]
    float speed = 40;

    float timer = 0;
    Coroutine coroutine;

    bool _isMove = false;

    float Zhongx = 0;

    void Start()
    {
        ZiDong.onClick.AddListener(() =>
        {
            _isMove = false;

        });
        ShouDong.onClick.AddListener(() =>
        {
            _isMove = true;
        });


    }
    //IEnumerator PlaneMovePoint(GameObject Point)
    //{
    //    float t = 0;
    //    while (Vector3.Distance(transform.position, Point.transform.position) > 0.1f)
    //    {
    //        t += Time.deltaTime * speed;
    //        transform.position = Vector3.Lerp(transform.position, Point.transform.position, t);
    //        yield return null;
    //    }


    //}

    // Update is called once per frame
    void Update()
    {


        //timer += Time.deltaTime * (speed * 0.1f);
        //transform.position = Vector3.Lerp(transform.position, AllPoint[index].transform.position, timer);
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(AllPoint[index].transform.position - transform.position), timer);


        if (!_isMove)
        {

            if (index == -1)
            {
                float n = Vector3.Distance(transform.position, AllPoint[0].transform.position);
                index = 0;
                for (int i = 0; i < AllPoint.Count; i++)
                {
                    if (Vector3.Distance(transform.position, AllPoint[i].transform.position) < n)
                    {
                        index = i;
                        n = Vector3.Distance(transform.position, AllPoint[i].transform.position);
                    }

                }

            }
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
            Debug.Log(Vector3.Distance(transform.position, AllPoint[index].transform.position));
            if (coroutine == null)
            {
                transform.LookAt(AllPoint[index].transform.position);
            }
            if (Vector3.Distance(transform.position, AllPoint[index].transform.position) < 1f)
            {

                index++;
                if (index > AllPoint.Count - 1)
                {
                    index = 0;
                }
                coroutine = StartCoroutine(Rotate(AllPoint[index]));

            }
        }
        else
        {
            if (index != -1)
            {
                index = -1;
                coroutine = null;
                speed = 40;
            }
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector3.forward * Time.deltaTime * speed);
                if (Input.GetKey(KeyCode.A))
                {
                    transform.Rotate(0, -Time.deltaTime * speed, 0);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    transform.Rotate(0, Time.deltaTime * speed, 0);
                }
               
                if (Input.GetKey(KeyCode.M))
                {
                    speed += Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.N))
                {
                    speed -= Time.deltaTime;

                }

            }
            if (Input.GetKey(KeyCode.U))
            {

                if (transform.eulerAngles.x > 30 && transform.eulerAngles.x < 50)
                {
                    return;

                }
                transform.Rotate(Time.deltaTime * speed, 0, 0);
                Debug.Log("trans"+transform.eulerAngles.x);
               
            }
            if (Input.GetKey(KeyCode.I))
            {
                if (transform.eulerAngles.x < 330 && transform.eulerAngles.x > 270)
                {
                    return;
                 
                }
                transform.Rotate(-Time.deltaTime * speed, 0, 0);
                Debug.Log(transform.eulerAngles.x);
              
            }
            if (Input.GetKey(KeyCode.J))
            {
                if (transform.eulerAngles.z > 30 && transform.eulerAngles.z < 50)
                {
                    return;
                }
                transform.Rotate(0, 0, Time.deltaTime * speed);
                Debug.Log(transform.eulerAngles.z);
               
            }
            if (Input.GetKey(KeyCode.K))
            {
                if (transform.eulerAngles.z < 330 && transform.eulerAngles.z > 270)
                {
                    return;
                }
                transform.Rotate(0, 0, -Time.deltaTime * speed);
                Debug.Log(transform.eulerAngles.z);
                
            }


        }
        Plane plane = new Plane();
        plane.X = transform.position.x;
        plane.Y = transform.position.y;
        plane.Z = transform.position.z;
        plane.Rx = transform.eulerAngles.x;
        plane.Ry = transform.eulerAngles.y;
        plane.Rz = transform.eulerAngles.z;
        MsgData data = new MsgData();
        data.Id = MessageNumber.RefreshPlanePos;
        data.Data = plane.ToByteArray();
        ChatManager.Instance.SendPos(data);






    }
    IEnumerator Rotate(GameObject point)
    {
        float t = 0;
        // Quaternion rotate = Quaternion.LookRotation(point.transform.position - transform.position);
        while (Quaternion.Angle(transform.rotation, Quaternion.LookRotation(point.transform.position - transform.position)) > 0f)
        {
            // Debug.Log(Quaternion.Angle(transform.rotation, Quaternion.LookRotation(point.transform.position - transform.position)));
            t += (Time.deltaTime * speed) * 0.01f;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(point.transform.position - transform.position), t);
            yield return null;
        }
        transform.LookAt(point.transform.position);
        //  Debug.Log("End");
    }
}
