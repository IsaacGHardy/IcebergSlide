using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private GameObject checkmark;
    [SerializeField] Slider volumeSlider; 
    [SerializeField] QuixoClass quixoClass;

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
            Load();
        }

        else
        {
            Load();
        }

        if (!PlayerPrefs.HasKey("quick"))
        {
            PlayerPrefs.SetInt("quick", 1);
            Load();
        }

        else
        {
            Load();
        }
    }

    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
        Save();
    }

    private void Load()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
        if (checkmark != null) { checkmark.SetActive((PlayerPrefs.GetInt("quick") == 1)); }
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
    }

    public void toggleQuick()
    {
        checkmark.SetActive(!checkmark.activeSelf);
        PlayerPrefs.SetInt("quick", (checkmark.activeSelf ? 1 : 0));
        if(quixoClass != null) { quixoClass.toggleQuick(); }
    }
}
