using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Larje.Core.Tools.Joystick;

[RequireComponent(typeof(CharacterMover))]
public class PlayerRunControll : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;

    private Joystick _joystick;
    private CharacterMover _mover;

    public Vector2 MoveSpeed { get; private set; }


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
        MoveSpeed = Vector2.zero;
        if (!_joystick) return;

        if (_joystick.Direction.x != 0f)
        {
            MoveSpeed = _joystick.Direction * _moveSpeed;
            _mover.Move(MoveSpeed);
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
