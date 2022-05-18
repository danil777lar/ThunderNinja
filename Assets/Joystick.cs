using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Joystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    #region Singleton
    private static Joystick _default;
    public static Joystick Default => _default;
    public static Action Initialized;
    #endregion

    [SerializeField] private Transform _joystick;
    [SerializeField] private Transform _joystickArea;

    private Tween _dragEndTween;
    private CanvasGroup _group;

    public bool PointerOnJoystick { get; private set; }
    public Vector2 Direction { get; private set; }
    public Action PointerDown;
    public Action PointerUp;


    private void Awake()
    {
        _default = this;
        _group = GetComponent<CanvasGroup>();
        Initialized?.Invoke();
    }

    private void OnEnable()
    {
        Direction = Vector2.zero;
        _joystick.localPosition = Vector3.zero;
        _group.alpha = 0f;
    }

    private void OnDisable()
    {
        Direction = Vector2.zero;
        _group.alpha = 0f;
    }


    public void OnDrag(PointerEventData eventData)
    {
        _dragEndTween?.Kill();
        _joystick.position = eventData.position;
        float maxDistance = ((RectTransform)_joystickArea).sizeDelta.x / 2f;
        _joystick.localPosition = _joystick.localPosition.normalized * Mathf.Min(_joystick.localPosition.magnitude, maxDistance);
        Direction = _joystick.localPosition / maxDistance;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PointerOnJoystick = true;

        _joystickArea.position = eventData.position;
        _group.alpha = 1f;
        _dragEndTween?.Kill();
        _dragEndTween = _joystick.DOMove(eventData.position, 0.2f).SetEase(Ease.OutBack);
        PointerDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PointerOnJoystick = false;
        _dragEndTween?.Kill();
        _dragEndTween = _joystick.DOLocalMove(Vector3.zero, 0.25f).SetEase(Ease.OutBack);
        PointerUp?.Invoke();
        _group.alpha = 0f;
        Direction = Vector2.zero;
    }
}
