using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopLine : MonoBehaviour
{
    private TrafficLight logic
    {
        get { return transform.parent.GetComponent<TrafficLight>(); }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == Tags.Car.ToString())
            logic.StopLineCrossed();
    }
}
