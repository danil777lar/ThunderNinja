using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMover))]
public class PlayerRunControll : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;

    private Joystick _joystick;
    private CharacterMover _mover;


    private void Start()
    {
        _mover = GetComponent<CharacterMover>();
        StartCoroutine(LinkJoystickCoroutine());
    }

    private void Update()
    {
        TryComputeWalk();
    }

    private void OnDestroy()
    {
        _joystick.PointerDown -= OnPointerDown;
        _joystick.PointerUp -= OnPointerUp;
    }


    private void TryComputeWalk() 
    {
        if (!_joystick) return;

        if (_joystick.Direction.x != 0f)
        {
            _mover.Move(_joystick.Direction * _moveSpeed);
        }
    }

    private void OnPointerDown() 
    {

    }

    private void OnPointerUp() 
    {

    }


    private IEnumerator LinkJoystickCoroutine()
    {
        yield return StartCoroutine(Joystick.WaitJoystickInit(Joystick.JoystickPlacement.Right));
        _joystick = Joystick.GetJoystick(Joystick.JoystickPlacement.Right);

        _joystick.PointerDown += OnPointerDown;
        _joystick.PointerUp += OnPointerUp;
    }
}
