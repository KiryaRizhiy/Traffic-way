using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using System;
using GameAnalyticsSDK;

public class Ads : MonoBehaviour, IUnityAdsListener 
{
    private const string _videoPlacement = "video";
    private const string _rewardedVideoPlacement = "rewardedVideo";
    private const string _bannerPlacement = "banner";

    public GameObject testText;

    public static bool isVideoReady
    {
        get
        {
            return Advertisement.IsReady(_videoPlacement);
        }
    }
    public static bool isRewardedVideoReady
    {
        get
        {
            return Advertisement.IsReady(_rewardedVideoPlacement);
        }
    }
    public static bool isBannerReady
    {
        get
        {
            return Advertisement.IsReady(_bannerPlacement);
        }
    }

    void Start()
    {
        testText.GetComponent<Text>().text = "Initialization started";
        try
        {
            GameAnalytics.Initialize();
        }
        catch (Exception e)
        {
            testText.GetComponent<Text>().text = e.Message + "--- trace ---" + e.StackTrace;
        }
        Advertisement.Initialize(Settings.googlePlayId, Settings.testMode);
        Advertisement.AddListener(this);
        Advertisement.Load(_videoPlacement);
        Advertisement.Load(_rewardedVideoPlacement);
        Advertisement.Load(_bannerPlacement);
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Debug.Log("Ads load initialized");
    }

    public void ShowVideo()
    {
        Advertisement.Banner.Hide();
        if (isVideoReady)
            Advertisement.Show(_videoPlacement);
        else
            Debug.Log("Video not ready");
    }
    public void ShowRewardedVideo()
    {
        Advertisement.Banner.Hide();
        if (isRewardedVideoReady)
            Advertisement.Show(_rewardedVideoPlacement);
        else
            Debug.Log("Rewarded video not ready");
    }

    public void OnUnityAdsReady(string placementId)
    {
        if (placementId == _bannerPlacement)
            Advertisement.Banner.Show(_bannerPlacement);
        Debug.Log(placementId + " ready");
    }
    public void OnUnityAdsDidError(string message)
    {
    }
    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        testText.GetComponent<Text>().text = placementId + " " + showResult.ToString();
        GAAdType adType;
        if (placementId == _videoPlacement)
            adType = GAAdType.Video;
        else
            if (placementId == _rewardedVideoPlacement)
                adType = GAAdType.RewardedVideo;
            else
                adType = GAAdType.Undefined;
        GameAnalytics.NewAdEvent(GAAdAction.Show, adType, "Unity ADS", placementId);
    }
    public void OnUnityAdsDidStart(string placementId)
    {
    }
}
