using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuPenguinScript : MonoBehaviour
{
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        
        Play("Idle_A");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play(string animation){
        if(animator.HasState(0, Animator.StringToHash(animation))){
            animator.Play(animation);
            
        }
    }
}
