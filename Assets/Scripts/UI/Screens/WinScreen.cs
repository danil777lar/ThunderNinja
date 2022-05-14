using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Larje.Core.Services;

public class WinScreen : MonoBehaviour
{
    //DEPENDENCIES
    [InjectService] private LevelManagerService _levelManager;
    [InjectService] private UIService _uiService;

    //OPTIONS
    [SerializeField] private Button _homeButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _nextButton;


    private void Start()
    {
        ServiceLocator.Default.InjectServicesInComponent(this);

        _homeButton.onClick.AddListener(OnHomeButtonClicked);
        _restartButton.onClick.AddListener(OnRestartButtonClicked);
        _nextButton.onClick.AddListener(OnNextButtonClicked);
    }


    private void OnHomeButtonClicked() 
    {

    }

    private void OnRestartButtonClicked() 
    {

    }

    private void OnNextButtonClicked() 
    {
        _levelManager.NextLevel();
        _uiService.ShowScreen("Process");
    }
}
