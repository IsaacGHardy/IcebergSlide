using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHats : MonoBehaviour
{
    [SerializeField] Penguin howToPlay;
    [SerializeField] Penguin singleplayer;
    [SerializeField] Penguin multiplayer;
    [SerializeField] Penguin settings;
    [SerializeField] Hat[] hatArray;
    [SerializeField] mainmenu mainmenu;

    private void Awake()
    {
        System.Random random = new System.Random();
        int randHTPIndex = random.Next(0, hatArray.Length);
        int randSPIndex;
        do
        {
            randSPIndex = random.Next(0, hatArray.Length);
        } while (randHTPIndex == randSPIndex);
        int randMPIndex;
        do
        {
            randMPIndex = random.Next(0, hatArray.Length);
        } while (randMPIndex == randSPIndex || randMPIndex == randHTPIndex);
        int randSIndex;
        do
        {
            randSIndex = random.Next(0, hatArray.Length);
        } while (randSIndex == randHTPIndex || randSIndex == randMPIndex || randSIndex == randSPIndex);
        howToPlay.setHat(hatArray[randHTPIndex]);
        singleplayer.setHat(hatArray[randSPIndex]);
        multiplayer.setHat(hatArray[randMPIndex]);
        settings.setHat(hatArray[randSIndex]);
    }

   

}
