using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampEffectController : MonoBehaviour
{
    [SerializeField] private ParticleSystem _parts;
    [SerializeField] private Light _light;

    private void Start()
    {
        ParticleSystem.MainModule main = _parts.main;
        main.startColor = _light.color;
        ParticleSystem.ShapeModule shape = _parts.shape;
        shape.angle = _light.spotAngle / 2f;
    }
}
