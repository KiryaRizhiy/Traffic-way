using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPicker : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == Tags.Car.ToString())
        {
            Engine.CoinCollected();
            Destroy(gameObject, 0.05f);
        }
    }
}
