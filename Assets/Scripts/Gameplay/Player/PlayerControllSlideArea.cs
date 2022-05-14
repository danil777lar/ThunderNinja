using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerControllSlideArea : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    #region Singleton
    private static PlayerControllSlideArea _default;
    public static PlayerControllSlideArea Default => _default;
    public static Action Initialized;
    #endregion

    public Action<PointerEventData> Drag; 
    public Action<PointerEventData> PointerDown; 
    public Action<PointerEventData> PointerUp;


    private void Awake()
    {
        _default = this;
        Initialized?.Invoke();
    }


    public void OnDrag(PointerEventData eventData)
    {
        Drag?.Invoke(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PointerDown?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PointerUp?.Invoke(eventData);
    }
}
