using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Penguin penguin;
    [SerializeField] public SoundEffect soundEffect;

    public void Play(string animation)
    {
        Animator animator = this.GetComponent<Animator>();
        if (animator.HasState(0, Animator.StringToHash(animation)))
        {
            animator.Play(animation);
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        Play("Bounce");
        soundEffect.playBloop3();

    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        Play("Idle_A");
        soundEffect.stopAllsounds();
    }

    public void OnPointerClicked(PointerEventData pointerEventData)
    {
        soundEffect.playPoke();
    }

}
