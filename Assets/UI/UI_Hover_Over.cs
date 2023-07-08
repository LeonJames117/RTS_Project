using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class UI_Hover_Over : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject Hover_Display;
    // Start is called before the first frame update
    public void OnPointerEnter(PointerEventData eventData)
    {
        Hover_Display.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Hover_Display.SetActive(false);
    }
}
