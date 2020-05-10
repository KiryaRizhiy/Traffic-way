using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZoneController : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == Tags.Car.ToString())
            Engine.Events.ZoneReached(gameObject);
    }
    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == Tags.Car.ToString())
            Engine.Events.ZoneLeft(gameObject);
    }
}
