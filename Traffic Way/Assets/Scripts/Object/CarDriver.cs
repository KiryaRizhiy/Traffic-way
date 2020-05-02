using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDriver : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Engine.paused)
            return;
        if (UserInteraction.gas)
            transform.Translate(Vector2.up * Settings.carSpeed);
    }
}
