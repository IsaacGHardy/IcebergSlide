using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hat : MonoBehaviour
{
    public GameObject hat;
    public GameObject head; 
    public Penguin wearer;
    public float relx;
    public float rely;
    public float relz;

    void Start()
    {
        head = wearer.penguin.transform.Find("hair").gameObject;
        wear();
    }

    void Update()
    {
        wear();
    }

    public void wear()
    {
        hat.transform.position = new Vector3(head.transform.position.x + relx, head.transform.position.y + rely, head.transform.position.z + relz);
        hat.transform.rotation = head.transform.rotation; 
    }
}
