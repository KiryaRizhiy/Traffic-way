using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPicker : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == Tags.Car.ToString())
        {
            Engine.AddCoins(1);
            Destroy(gameObject, 0.3f);
        }
    }
}
