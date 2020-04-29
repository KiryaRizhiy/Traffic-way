using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using System;

public class Initializer : MonoBehaviour
{
    public GameObject testText;
    // Start is called before the first frame update
    void Start()
    {
        //testText.GetComponent<Text>().text = "Initialization started";
        //try
        //{
        //    GameAnalytics.Initialize();
        //}
        //catch (Exception e)
        //{
        //    testText.GetComponent<Text>().text = e.Message;
        //}
        ////testText.GetComponent<Text>().text = "Analytics initilized";
        ////Advertisement.Initialize(Settings.googlePlayId, Settings.testMode);
    }
}
