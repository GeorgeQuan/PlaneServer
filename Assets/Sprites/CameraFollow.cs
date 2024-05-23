using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject Plane;
    Vector3 Pos;
    void Start()
    {
        Pos = new Vector3(0,30,-41);
    }

    // Update is called once per frame
    void Update()
    {
        if(Plane==null)
        {
            Plane = GameObject.FindGameObjectWithTag("Player");
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position,Plane.transform.position+Pos,1);
            transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(Plane.transform.position-transform.position),1);

        }
    }
}
