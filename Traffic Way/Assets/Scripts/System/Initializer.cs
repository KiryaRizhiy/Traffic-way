using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;
using UnityEngine.Advertisements;
using GoogleMobileAds.Api;

public class Initializer : MonoBehaviour
{
    void Start()
    {
        GameAnalytics.Initialize();
        //Advertisement.Initialize(Settings.googlePlayId, Settings.testMode); UNCOMMENT TO IMPLEMENT UNITY ADS
        MobileAds.Initialize(initStatus => { });
        Engine.Load();
        Engine.Events.Initialized();
    }
}