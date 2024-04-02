using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

public class buttonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] MenuSounds menuSounds;
    UnityEngine.UI.Button button;

    private void Start()
    {
        button = GetComponent<UnityEngine.UI.Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (menuSounds != null && button != null && button.interactable) { menuSounds.playClick(); }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (menuSounds != null && button != null && button.interactable) { menuSounds.stopClick(); }
    }
}
