using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskHolder : MonoBehaviour
{
    [ContextMenu("Show Mask")]
    private void ShowMask() 
    {
        foreach (SpriteRenderer rend in GetComponentsInChildren<SpriteRenderer>()) 
        {
            rend.color = new Color(1f, 1f, 1f, 0.1f);
        }
    }

    [ContextMenu("Hide Mask")]
    private void HideMask()
    {
        foreach (SpriteRenderer rend in GetComponentsInChildren<SpriteRenderer>())
        {
            rend.color = new Color(1f, 1f, 1f, 0f);
        }
    }
}
