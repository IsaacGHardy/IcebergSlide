using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private GameObject checkmark;
    [SerializeField] Slider volumeSlider; 
    [SerializeField] QuixoClass quixoClass;
    public static bool isQuick = false;

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
    }

    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
        Save();
    }

    private void Load()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
        if (checkmark != null) { checkmark.SetActive(isQuick); }
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
    }

    public void toggleQuick()
    {
        checkmark.SetActive(!checkmark.activeSelf);
        isQuick = !isQuick;
        if(quixoClass != null) { quixoClass.toggleQuick(); }
    }
}
