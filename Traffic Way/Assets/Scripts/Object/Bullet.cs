using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    void Update()
    {
        transform.Translate(Vector2.up * Settings.bulletSpeed * Time.deltaTime);
    }
    void OnBecameInvisible()
    {
        Debug.Log("Bullet became invisible");
        Destroy(gameObject);
    }
}
