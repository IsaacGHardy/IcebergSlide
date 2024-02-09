using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverAnimation : MonoBehaviour
{

    public Animator animator;

    private void OnMouseEnter()
    {
        animator.SetBool("Hover", true);
    }

    private void OnMouseExit()
    {
        animator.SetBool("Hover", false);
    }
}
