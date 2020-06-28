using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;
using UnityEngine.Advertisements;
using GoogleMobileAds.Api;
using System;

public class Initializer : MonoBehaviour
{
    void Start()
    {
        if (Engine.initialized)
            return;
        //Logger.UpdateContent(UILogDataType.Init,"Game analytics initialization");
        GameAnalytics.Initialize();
        GameAnalytics.NewDesignEvent("Technical:Info:Version_" + Application.version);
        try
        {
            //Advertisement.Initialize(Settings.googlePlayId, Settings.testMode); UNCOMMENT TO IMPLEMENT UNITY ADS
            MobileAds.Initialize(initStatus => { });
            Engine.Initialize();
            AdMobController adController = new GameObject().AddComponent<AdMobController>();
            adController.gameObject.name = "AdMobController";
        }
        catch (Exception e)
        {
            GameAnalytics.NewErrorEvent(GAErrorSeverity.Critical, e.Message + Environment.NewLine + "-------Trace------" + Environment.NewLine + e.StackTrace);
        }
        Engine.Events.Initialized();
    }
}