using UnityEngine.EventSystems;
using UnityEngine;

public class Rotate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Penguin penguin;
    private Quaternion resetRotation;
    public float rotationSpeed;
    private bool pointerEnter = false;

    void Start()
    {
        resetRotation = penguin.gameObject.transform.rotation;
    }

    void Update()
    {
        if (pointerEnter)
        {
            penguin.gameObject.transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
        }
    }

    public void setPointerEnter()
    {
        pointerEnter = true;
    }

    public void setPointerExit()
    {
        pointerEnter = false;
        penguin.gameObject.transform.rotation = resetRotation;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerEnter = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerEnter = false;
        penguin.gameObject.transform.rotation = resetRotation;
    }
}
