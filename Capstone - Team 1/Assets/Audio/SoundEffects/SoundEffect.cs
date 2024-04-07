using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    [SerializeField] private AudioSource[] audioSources;
    //0 is bloop3
    //1 is poke
    //2 is pitter
    //3 is bloop 2
     
    public void playBloop3()
    {
        audioSources[0].Play();
    }

    public void stopBloop3()
    {
        audioSources[0].Stop();
    }

    public void playBloop2()
    {
        audioSources[3].Play();
    }

    public void stopBloop2()
    {
        audioSources[3].Stop();
    }

    public void playPoke()
    {
        audioSources[1].Play();
    }

    public void stopPoke()
    {
        audioSources[1].Stop();
    }

    public void playPitter()
    {
        audioSources[2].Play();
    }

    public void stopPitter()
    {
        audioSources[2].Stop();
    }

    public void stopAllsounds()
    {
        foreach(AudioSource source in  audioSources) {
            source.Stop();
        }

    }

}
