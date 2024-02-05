using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAnimation : MonoBehaviour
{
    public Animator animator;

    private void OnMouseDown()
    {
        animator.SetBool("Clicked", true);
    }

    public void OnMouseUp()
    {
        animator.SetBool("Clicked", false);
    }
}
