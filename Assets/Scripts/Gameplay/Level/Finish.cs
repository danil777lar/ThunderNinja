using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Larje.Core.Services;

public class Finish : MonoBehaviour
{
    [InjectService] private UIService _uiService;


    private void Start()
    {
        ServiceLocator.Default.InjectServicesInComponent(this);        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerJumpControll>()) 
        {
            _uiService.ShowScreen("Win");
        }
    }
}
