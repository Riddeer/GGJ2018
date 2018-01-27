using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIEventListener : MonoBehaviour,
    IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{

    public delegate void UIEventProxy(GameObject gb);

    public event UIEventProxy OnClick;

    public event UIEventProxy OnEnter;

    public event UIEventProxy OnExit;

    public event UIEventProxy OnDown;

    public event UIEventProxy OnUp;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClick != null)
            OnClick(this.gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (OnEnter != null)
            OnEnter(this.gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (OnExit != null)
            OnExit(this.gameObject);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
		if (OnDown != null)
            OnDown(this.gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
		if (OnUp != null)
            OnUp(this.gameObject);
    }

}