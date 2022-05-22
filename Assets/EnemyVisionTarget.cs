using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemyVisionTarget : MonoBehaviour
{
    #region Singleton
    private static EnemyVisionTarget _default;
    public static EnemyVisionTarget Default => _default;
    #endregion

    public bool _isInLight;
    public int _lightCount;

    private BoxCollider2D _collider;
    private List<Light> _sceneLights;


    private void Awake() 
    {
        _default = this;
    }

    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _sceneLights = new List<Light>(FindObjectsOfType<Light>());
        _sceneLights = _sceneLights.FindAll((l) => l.type == LightType.Spot);
    }

    private void Update()
    {
        UpdateLightState();
    }

    public bool CheckIsInVision(Transform root, float distance, float angle) 
    {
        Vector2 direction = _collider.bounds.center - root.position;
        RaycastHit2D hit = Physics2D.Raycast(root.position, direction, distance, LayerMask.GetMask("Default", "PlayerRaycastTarget"));
        if (hit && hit.collider.gameObject.layer == LayerMask.NameToLayer("PlayerRaycastTarget") && _isInLight && Vector2.Angle(direction, root.forward) <= angle)
        {
            return true;
        }

        return false;
    }

    private void UpdateLightState() 
    {
        _lightCount = _sceneLights.Count;
        foreach (Light light in _sceneLights) 
        {
            Vector2 direction = _collider.bounds.center - light.transform.position;
            if (Vector2.Angle(direction, light.transform.forward) <= light.spotAngle / 2f)
            {
                RaycastHit2D hit = Physics2D.Raycast(light.transform.position, direction, 1000f, LayerMask.GetMask("Default", "PlayerRaycastTarget"));
                if (hit && hit.collider.gameObject.layer == LayerMask.NameToLayer("PlayerRaycastTarget"))
                {
                    _isInLight = true;
                    return;
                }
            }
        }

        _isInLight = false;
    }
}
