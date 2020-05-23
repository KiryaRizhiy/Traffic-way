using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    void Update()
    {
        if(!Engine.paused) transform.Translate(Vector2.up * (Settings.bulletSpeed + CarDriver.currentSpeed)* Time.deltaTime);
    }
    void OnBecameInvisible()
    {
        //Debug.Log("Bullet became invisible");
        Destroy(gameObject);
    }
}
