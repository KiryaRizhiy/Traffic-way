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
        if (Engine.initialized)
            return;
        //Logger.UpdateContent(UILogDataType.Init,"Game analytics initialization");
        GameAnalytics.Initialize();
        //Logger.AddContent(UILogDataType.Init, "Game analytics initialized");
        //Advertisement.Initialize(Settings.googlePlayId, Settings.testMode); UNCOMMENT TO IMPLEMENT UNITY ADS
        //Logger.AddContent(UILogDataType.Init, "AdMob initialization");
        MobileAds.Initialize(initStatus => { });
        //Logger.AddContent(UILogDataType.Init, "AdMob initialized");
        Logger.AddContent(UILogDataType.Init, "Engine initialization");
        Engine.Initialize();
        Logger.AddContent(UILogDataType.Init, "Engine initialized");
        AdMobController adController = new GameObject().AddComponent<AdMobController>();
        adController.gameObject.name = "AdMobController";
        Engine.Events.Initialized();
    }
}