using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    public static string googlePlayId = "3577053";
    public static string adMobInterstitialId
    {
        get
        {
            if (testMode)
                return "ca-app-pub-3940256099942544/1033173712";
            else return "ca-app-pub-6138084763477006/9022932291";
        }
    }
    public static string adMobRewardedId
    {
        get
        {
            if (testMode)
                return "ca-app-pub-3940256099942544/5224354917";
            else return "ca-app-pub-6138084763477006/1312210652";
        }
    }
    public static string adMobBannerId
    {
        get
        {
            if (testMode)
                return "ca-app-pub-3940256099942544/6300978111";
            else return "ca-app-pub-6138084763477006/1527585657";
        }
    }
    public static string adMobApplicationId = "ca-app-pub-6138084763477006~9956003923";
    public static bool testMode = true;
    public const float carSpeedLimit = 23f;
    public const float carAcceleration = 0.4f;
    public const float carBraking = 0.8f;
    public const float carBoostMultiplyer = 1.2f;
    public const float carShieldDestroyRollBackDistance = 4f;
    public const int levelReward = 4;
    public const int extraRewardMultiplyer = 3;
    public const string privacyPolicyLink = "https://justforward.co/privacy-policy/";
    public const float tracesWidth = 0.2f;
    public const float bulletSpeed = 18f;
    public const float shootFrequency = 0.4f;
    public const float trafficLightSwitchSpeed = 1.3f;
    public const int coinMakerTickMinutes = 1;
    public const int paidTicksLimit = 24;
    public const int TVCooldownMinutes = 1;
    public const int TVWatchReward = 15;
    public const int levelCarProgress = 51;
    public const float carProgressDemoTime = 1.7f;
    public const float carProgressPauseTime = 0.12f;
    public const float carProgressBlinkTime = 2.0f;
    public const float carProgressBlingOffset = 0.7f;
    public const char LocalsSeparator = ';';
    public static Vector3 carsScale
    {
        get
        {
            return new Vector3(2.2f, 2.2f, 1f);
        }
    }
    public static string savePath 
    { get { return Application.persistentDataPath + "/trafficWaySaves/"; } }
    public static string saveFile 
    { get { return savePath + "Save.svg"; } }
}
public enum PlacementType { video, rewardedVideo, banner, interstitial }
public enum Tags { Car, Coin, NPCCar, Bullet }