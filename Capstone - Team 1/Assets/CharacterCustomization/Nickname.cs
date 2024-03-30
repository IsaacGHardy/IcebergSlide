using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Nickname : MonoBehaviour
{
    [SerializeField] TMP_InputField input;
    [SerializeField] Button button;

    private void Start()
    {
        button.onClick.AddListener(changeNickname);
    }

    private void changeNickname()
    {
        input.Select();
        input.ActivateInputField();
    }
}
