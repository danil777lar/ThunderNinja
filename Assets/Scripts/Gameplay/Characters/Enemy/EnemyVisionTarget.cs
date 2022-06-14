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

    [SerializeField] private BoxCollider2D _collider;

    private bool _isInLight;
    private List<Light> _sceneLights;


    private void Awake() 
    {
        _default = this;
    }

    private void Start()
    {
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
        foreach (Light light in _sceneLights) 
        {
            List<Vector2> directions = new List<Vector2>();
            directions.Add(Vector3.MoveTowards(_collider.bounds.max, _collider.bounds.center, 0.05f) - light.transform.position);
            directions.Add(Vector3.MoveTowards((_collider.bounds.max - Vector3.right * _collider.size.x), _collider.bounds.center, 0.05f) - light.transform.position);
            directions.Add(Vector3.MoveTowards(_collider.bounds.min, _collider.bounds.center, 0.05f) - light.transform.position);
            directions.Add(Vector3.MoveTowards((_collider.bounds.min + Vector3.right * _collider.size.x), _collider.bounds.center, 0.05f) - light.transform.position);

            foreach (Vector2 direction in directions)
            {
                if (Vector2.Angle(direction, light.transform.forward) <= light.spotAngle / 2f)
                {
                    RaycastHit2D hit = Physics2D.Raycast(light.transform.position, direction, 1000f, LayerMask.GetMask("Default", "PlayerRaycastTarget"));
                    if (hit && hit.collider.gameObject.layer != LayerMask.NameToLayer("PlayerRaycastTarget")) 
                        Debug.DrawLine(light.transform.position, hit.point, Color.red);
                    if (hit && hit.collider.gameObject.layer == LayerMask.NameToLayer("PlayerRaycastTarget"))
                    {
                        Debug.DrawLine(light.transform.position, hit.point, Color.green);
                        _isInLight = true;
                        return;
                    }
                }
            }
        }

        _isInLight = false;
    }
}
