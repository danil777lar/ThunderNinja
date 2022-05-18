using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraField : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _camera;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            _camera.Priority = 100 + transform.GetSiblingIndex();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _camera.Priority = 0;
        }
    }
}
