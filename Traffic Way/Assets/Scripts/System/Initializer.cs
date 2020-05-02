using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;
using UnityEngine.Advertisements;

public class Initializer : MonoBehaviour
{
    void Start()
    {
        GameAnalytics.Initialize();
        Advertisement.Initialize(Settings.googlePlayId, Settings.testMode);
        Engine.Load();
        Engine.Events.Initialized();
    }
}