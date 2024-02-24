using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuPenguinScript : MonoBehaviour
{
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {

        Play("Roll");
        Play("Spin");
        Play("Jump");

    }

    // Update is called once per frame
    void Update()
    {
        Play("Bounce");
    }

    //public void Play(string animation)
    //{
    //    if (animator.HasState(0, Animator.StringToHash(animation)))
    //    {
    //        animator.Play(animation);

    //    }
    //}
}
