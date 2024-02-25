using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hat : MonoBehaviour
{
    public GameObject hat;
    private Transform head; 
    public Penguin wearer;
    public Vector3 pos; 
    public Vector3 rot; 

    public void Setup(Penguin Wearer, GameObject hair)
    {
        wearer = Wearer;
        head = hair.transform;

        if (head != null)
        {
            wear();
            hat.transform.SetParent(head); 
        }
        else
        {
            UnityEngine.Debug.LogError("Failed to find hair.01 object in Penguin.");
        }
    }

    void Update()
    {
        wear(); // Keep for real-time adjustments during development
    }
    public void remove()
    {
        Destroy(hat);
    }
    private void wear()
    {
        if (head != null)
        {
            hat.transform.localPosition = pos; 
            hat.transform.localRotation = Quaternion.Euler(rot);
        }
    }
}
