using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hat : MonoBehaviour
{
    public GameObject hat;
    private Transform head; // Changed to Transform since we're only interested in the Transform component
    public Penguin wearer;
    public Vector3 pos; // Using a Vector3 for cleaner code
    public Vector3 rot; // Using a Vector3 for cleaner code
    public bool hideHair;

    public void Start(){

    }
    public void Setup(Penguin Wearer, GameObject hair)
    {
        wearer = Wearer;
        head = hair.transform;

        if (head != null)
        {
            wear();
            hat.transform.SetParent(head); // Make the hat a child of the head for automatic relative positioning
        }
        else
        {
            UnityEngine.Debug.LogError("Failed to find hair.01 object in Penguin.");
        }
    }

    void Update()
    {
        wear();
    }

    private void wear()
    {
        if (head != null)
        {
            hat.transform.localPosition = pos; // Assumes hat is a child of head
            // Rotation is automatically inherited from the parent, so no need to set it explicitly
        }
    }
}
