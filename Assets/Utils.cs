using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Utils
{
    public class Utils : MonoBehaviour
    {
        public static Vector3 GetVectorFromAngle(float angle)
        {
            return new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        }

        public static float GetAngleFromVector(Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360f;
            
            return n;
        }
    }
}