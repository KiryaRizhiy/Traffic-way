using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdMobController : MonoBehaviour
{
    public static bool isRegularVideoReady
    {
        get
        {
            if (interstitial != null)
                return interstitial.IsLoaded();
            else
                return false;
        }
    }
    public static bool isRewardedVideoReady
    {
        get
        {
            if (rewarded != null)
                return rewarded.IsLoaded();
            else
                return false;
        }
    }

    private static BannerView banner;
    private static InterstitialAd interstitial;
    private static RewardBasedVideoAd rewarded;
    private static AdRequest request;

    public static void ShowRegularAd()
    {
        interstitial.Show();
    }
    public static void ShowRewardedAd()
    {
        rewarded.Show();
    }

    void Start()
    {
        Logger.UpdateContent(UILogDataType.Monetization, "Ads initialization start");
        request = new AdRequest.Builder().Build();
        loadBanner();
        loadInterstitial();
        loadRewarded();
        Logger.AddContent(UILogDataType.Monetization, "Ads initialized");
    }
    
    # region Loading ads methods

    private void loadBanner()
    {
        banner = new BannerView(Settings.adMobBannerId, AdSize.Banner, AdPosition.Bottom);
        banner.OnAdLoaded += bannerHandleOnAdLoaded;
        banner.OnAdFailedToLoad += bannerHandleOnAdFailedToLoad;
        banner.OnAdOpening += bannerHandleOnAdOpened;
        banner.OnAdClosed += bannerHandleOnAdClosed;
        banner.OnAdLeavingApplication += bannerHandleOnAdLeavingApplication;
        banner.LoadAd(request);
    }
    private void loadInterstitial()
    {
        interstitial = new InterstitialAd(Settings.adMobInterstitialId);
        interstitial.OnAdLoaded += interstitialHandleOnAdLoaded;
        interstitial.OnAdFailedToLoad += interstitialHandleOnAdFailedToLoad;
        interstitial.OnAdLeavingApplication += interstitialHandleOnAdLeavingApplication;
        interstitial.OnAdClosed += interstitialHandleOnAdClosed;
        interstitial.OnAdOpening += interstitialHandleOnAdOpened;
        interstitial.LoadAd(request);
    }
    private void loadRewarded()
    {
        rewarded = RewardBasedVideoAd.Instance;
        //rewarded.OnAdClosed no handler
        rewarded.OnAdFailedToLoad += rewardedHandleOnAdFailedToLoad;
        rewarded.OnAdLeavingApplication += rewardedHandleOnAdLeavingApplication;
        rewarded.OnAdLoaded += rewardedHandleOnAdLoaded;
        rewarded.OnAdOpening += rewardedHandleOnAdOpened;
        rewarded.OnAdRewarded += rewardedRewardBasedVideoRewarded;
        //rewarded.OnAdStarted no handler
        rewarded.LoadAd(request, Settings.adMobRewardedId);
    }
    //private void reloadRewarded()
    //{
    //    request = new AdRequest.Builder().Build();
    //    rewarded.LoadAd(request, Settings.adMobRewardedId);
    //}

    #endregion

    void OnDestroy()
    {
        banner.Hide();
        banner.OnAdLoaded -= this.bannerHandleOnAdLoaded;
        banner.OnAdFailedToLoad -= this.bannerHandleOnAdFailedToLoad;
        banner.OnAdOpening -= this.bannerHandleOnAdOpened;
        banner.OnAdClosed -= this.bannerHandleOnAdClosed;
        banner.OnAdLeavingApplication -= this.bannerHandleOnAdLeavingApplication;
        banner.Destroy();
        interstitial.OnAdLoaded -= interstitialHandleOnAdLoaded;
        interstitial.OnAdFailedToLoad -= interstitialHandleOnAdFailedToLoad;
        interstitial.OnAdLeavingApplication -= interstitialHandleOnAdLeavingApplication;
        interstitial.OnAdClosed -= interstitialHandleOnAdClosed;
        interstitial.OnAdOpening -= interstitialHandleOnAdOpened;
        interstitial.Destroy();
        rewarded.OnAdFailedToLoad -= rewardedHandleOnAdFailedToLoad;
        rewarded.OnAdLeavingApplication -= rewardedHandleOnAdLeavingApplication;
        rewarded.OnAdLoaded -= rewardedHandleOnAdLoaded;
        rewarded.OnAdOpening -= rewardedHandleOnAdOpened;
        rewarded.OnAdRewarded -= rewardedRewardBasedVideoRewarded;
    }

    # region Banner handling

    public void bannerHandleOnAdLoaded(object sender, EventArgs args)
    {
        Engine.Events.AdLoaded(PlacementType.banner);
    }
    public void bannerHandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("HandleFailedToReceiveAd event received with message: "
                            + args.Message);
        Logger.AddContent(UILogDataType.Monetization, "Load fail event handling");
        Engine.Events.AdFailed(PlacementType.banner);
    }
    public void bannerHandleOnAdOpened(object sender, EventArgs args)
    {
        Engine.Events.AdOpened(PlacementType.banner);
    }
    public void bannerHandleOnAdClosed(object sender, EventArgs args)
    {
        Engine.Events.AdSkipped(PlacementType.banner);
    }
    public void bannerHandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        Engine.Events.AdUserLeave(PlacementType.banner);
    }
    
    #endregion

    #region Interstitial handling

    public void interstitialHandleOnAdLoaded(object sender, EventArgs args)
    {
        Engine.Events.AdLoaded(PlacementType.interstitial);
    }
    public void interstitialHandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("HandleFailedToReceiveAd event received with message: "
                            + args.Message);
        Engine.Events.AdFailed(PlacementType.interstitial);
    }
    public void interstitialHandleOnAdOpened(object sender, EventArgs args)
    {
        Engine.Events.AdOpened(PlacementType.interstitial);
    }
    public void interstitialHandleOnAdClosed(object sender, EventArgs args)
    {
        Engine.Events.AdFinished(PlacementType.interstitial);
    }
    public void interstitialHandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        Engine.Events.AdUserLeave(PlacementType.interstitial);
    }

    #endregion

    #region Rewarded handling

    public void rewardedHandleOnAdLoaded(object sender, EventArgs args)
    {
        Engine.Events.AdLoaded(PlacementType.rewardedVideo);
    }
    public void rewardedHandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("HandleFailedToReceiveAd event received with message: "
                            + args.Message);
        Engine.Events.AdFailed(PlacementType.rewardedVideo);
    }
    public void rewardedHandleOnAdOpened(object sender, EventArgs args)
    {
        Engine.Events.AdOpened(PlacementType.rewardedVideo);
    }
    public void rewardedHandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        Engine.Events.AdUserLeave(PlacementType.rewardedVideo);
    }
    public void rewardedRewardBasedVideoRewarded(object sender, Reward args)
    {
        Engine.Events.AdFinished(PlacementType.rewardedVideo);
    }

    #endregion
}