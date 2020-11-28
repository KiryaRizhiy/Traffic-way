using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GasButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler 
{
    public static GameObject staticGameObject
    {
        get;
        private set;
    }
    public static bool pressed
    {
        get;
        private set;
    }
    public void Awake()
    {
        pressed = false;
        staticGameObject = gameObject;
    }
    public void OnPointerDown (PointerEventData eventData)
    {
        pressed = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
    }
    public void OnDestroy()
    {
        staticGameObject = null;
    }
}