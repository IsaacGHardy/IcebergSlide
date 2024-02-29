using UnityEngine.EventSystems;
using UnityEngine;
using Photon.Pun;

public class OnlineRotate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Penguin penguin;
    private Quaternion resetRotation;
    public float rotationSpeed;
    private bool pointerEnter = false;
    [SerializeField] private PhotonView photonView;

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
    [PunRPC]
    public void setPointerEnter()
    {
        pointerEnter = true;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        photonView.RPC("setPointerEnter", RpcTarget.All);
    }

    [PunRPC]
    public void setPointerExit()
    {
        pointerEnter = false;
        penguin.gameObject.transform.rotation = resetRotation;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        photonView.RPC("setPointerExit", RpcTarget.All);
    }
}
