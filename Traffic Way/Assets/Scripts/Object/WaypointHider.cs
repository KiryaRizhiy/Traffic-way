using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointHider : MonoBehaviour
{
    void Awake()
    {
        DestroyImmediate(GetComponent<SpriteRenderer>());
    }
}
