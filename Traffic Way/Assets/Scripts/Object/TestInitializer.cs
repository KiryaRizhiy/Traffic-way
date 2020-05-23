using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInitializer : MonoBehaviour
{
    void Awake()
    {
        if (Settings.testMode)
        {
            Engine.InitializeTest();
            Debug.Log("Test initialization done");
        }
    }
}
