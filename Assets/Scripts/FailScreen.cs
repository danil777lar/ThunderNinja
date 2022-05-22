using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Larje.Core.Services;

public class FailScreen : MonoBehaviour
{
    //DEPENDENCIES
    [InjectService] private LevelManagerService _levelManager;
    [InjectService] private UIService _uiService;

    //OPTIONS
    [SerializeField] private Button _homeButton;
    [SerializeField] private Button _restartButton;


    private void Start()
    {
        ServiceLocator.Default.InjectServicesInComponent(this);

        _homeButton.onClick.AddListener(OnHomeButtonClicked);
        _restartButton.onClick.AddListener(OnRestartButtonClicked);
    }


    private void OnHomeButtonClicked()
    {

    }

    private void OnRestartButtonClicked()
    {
        _levelManager.RestartLevel();
        _uiService.ShowScreen("Process");
    }
}
