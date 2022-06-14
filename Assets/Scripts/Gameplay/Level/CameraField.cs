using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraField : MonoBehaviour
{
    private static Action ClearPriority;

    [SerializeField] private CinemachineVirtualCamera _camera;


    private void Start()
    {
        ClearPriority += OnClearPriority;
    }

    private void OnDestroy()
    {
        ClearPriority -= OnClearPriority;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            ClearPriority?.Invoke();
            _camera.Priority = 1;
        }
    }


    private void OnClearPriority() 
    {
        _camera.Priority = 0;
    }
}
