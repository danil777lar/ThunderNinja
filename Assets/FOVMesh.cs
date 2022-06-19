using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Larje.Core.Utils;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class FOVMesh : MonoBehaviour
{
    [SerializeField, Min(1)] private int _castCount;
    [SerializeField, Range(1f, 360f)] private float _angleRange;
    [SerializeField] private float _maxDistance;
    [SerializeField] private LayerMask _contactLayers;

    private Mesh _mesh;
    
    private void Start()
    {
        _mesh = new Mesh();
        _mesh.name = "FOV";
        GetComponent<MeshFilter>().mesh = _mesh;
    }

    private void Update()
    {
        Vector3[] verticles = new Vector3[_castCount + 2];
        Vector2[] uv = new Vector2[verticles.Length];
        int[] triangles = new int[_castCount * 3];

        verticles[0] = Vector3.zero;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= _castCount; i++)
        {
            float angle = (_angleRange / 2f) - (_angleRange / (float) _castCount) * i;
            Vector3 vertex;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.TransformDirection(Utils.GetVectorFromAngle(angle)), _maxDistance, _contactLayers);
            if (hit.collider == null)
            {
                vertex = Utils.GetVectorFromAngle(angle) * _maxDistance;
            }
            else
            {
                vertex = transform.InverseTransformPoint(hit.point);
                vertex.z = 0f;
            }
            verticles[vertexIndex] = vertex;
            

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
        }

        _mesh.vertices = verticles;
        _mesh.uv = uv;
        _mesh.triangles = triangles;
    }
}
