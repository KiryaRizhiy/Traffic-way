using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("Finish line reached with " + collider.name);
        Engine.Events.FinishLineReached();
    }
}
