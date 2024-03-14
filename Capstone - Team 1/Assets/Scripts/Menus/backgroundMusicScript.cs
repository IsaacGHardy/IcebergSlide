using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class backgroundMusicScript : MonoBehaviour
{

    // Code gotten from the following video: https://www.youtube.com/watch?v=63BEZMjcegE


    public static backgroundMusicScript instance;

    private void Awake()
    {
        if(instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }
}
