using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HotButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public Menu menu;

    public void OnPointerDown(PointerEventData pointerEventData)
    {// NU te lasa sa te razgandesti, click inseamna declansare instanta
        menu.PlayGame();
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        //te lasa sa te razgandesti
    }
}
