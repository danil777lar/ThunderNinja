using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class AmmoCounter : MonoBehaviour
{
    private Slider _slider;
    private PlayerControll _playerControll;


    private void Start()
    {
        _slider = GetComponent<Slider>();
        _playerControll = FindObjectOfType<PlayerControll>();

        _slider.maxValue = _playerControll.MaxAmmoCount;
    }

    private void Update()
    {
        _slider.value = Mathf.Lerp(_slider.value, (float)_playerControll.AmmoCount, Time.deltaTime * 10f);
    }
}
