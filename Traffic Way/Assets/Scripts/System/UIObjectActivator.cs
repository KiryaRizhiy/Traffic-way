using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.Advertisements; UNCOMMENT TO IMPLEMENT UNITY ADS
using UnityEngine.UI;

public class UIObjectActivator : MonoBehaviour/*, IUnityAdsListener UNCOMMENT TO IMPLEMENT UNITY ADS */
{
    public enum ActivatorTargetEvent {
        adsVideoReady, adsInterstitialReady, adsRewardedVideoReady,
        adsVideoNotReady, adsInterstitialNotReady, adsRewardedVideoNotReady,
        adsVideoFinished, adsInterstitialFinished, adsRewardedVideoFinished,
        adsVideoFailed, adsInterstitialFailed, adsRewardedVideoFailed,
        extraRewardReceived, 
        gameWon, gameLost, gamePassed,gamePaused,gameUnpaused,
        newCarAppeareenceReceived, sceneLoaded}
    public enum ActivatorTargetConditions { none, gameInProgress, gameWon, gameLost, gamePassed, readyToGiveNewCarAppearence, newCarAppearenceReceived, notReadyToGiveNewCarAppearence, isBossFight, isNotBossFight }
    public enum ActivatorActionType { activate, deactivate }

    public List<ActivatorTargetEvent> ActivationEventList;
    public List<ActivatorTargetConditions> ActivationConditions;
    public List<ActivatorTargetEvent> DeactivationEventList;
    public List<ActivatorTargetConditions> DeactivationConditions;
    public GameObject TargetObject;
    // Start is called before the first frame update
    void Awake()
    {
        //Validation
        if (TargetObject == null)
        {
            Debug.LogError("Target object not set on " + gameObject.name + "'s activator");
        }
        if (ActivationEventList == null && DeactivationEventList == null)
        {
            Debug.LogError(TargetObject.name + "'s activator has no action events");
        }

        //Subscription
        //Advertisement.AddListener(this); UNCOMMENT TO IMPLEMENT UNITY ADS
        Engine.Events.gameSessionStateChanged += OnSessionStateChanged;
        Engine.Events.extraRewardReceived += OnExtraRewardReceived;
        Engine.Events.adLoaded += OnAdLoaded;
        Engine.Events.adFinished += OnAdFinished;
        Engine.Events.adFailed += OnAdFailed;
        Engine.Events.adNotReady += OnAdNotReady;
        Engine.Events.paused += OnPause;
        Engine.Events.unpaused += OnUnpause;
        Engine.Events.newCarAppearenceReceived += OnNewCarAppearenceReceived;
        SceneManager.activeSceneChanged += OnLevelChanged;
    }
    void OnDestroy()
    {
        //Unsubscription
        //Advertisement.RemoveListener(this); UNCOMMENT TO IMPLEMENT UNITY ADS
        Engine.Events.gameSessionStateChanged -= OnSessionStateChanged;
        Engine.Events.extraRewardReceived -= OnExtraRewardReceived;
        Engine.Events.adLoaded -= OnAdLoaded;
        Engine.Events.adFinished -= OnAdFinished;
        Engine.Events.adFailed -= OnAdFailed;
        Engine.Events.adNotReady -= OnAdNotReady;
        Engine.Events.paused -= OnPause;
        Engine.Events.unpaused -= OnUnpause;
        Engine.Events.newCarAppearenceReceived -= OnNewCarAppearenceReceived;
        SceneManager.activeSceneChanged -= OnLevelChanged;
    }

    //UNCOMMENT TO IMPLEMENT UNITY ADS
    //public void OnUnityAdsReady(string placementId)
    //{
    //    if (placementId == PlacementType.video.ToString())
    //    {
    //        if (ActivationEventList.Contains(ActivatorTargetEvent.adsVideoReady))
    //            PerformAction(ActivatorActionType.activate);
    //        if (DeactivationEventList.Contains(ActivatorTargetEvent.adsVideoReady))
    //            PerformAction(ActivatorActionType.deactivate);
    //    }
    //    if (placementId == PlacementType.rewardedVideo.ToString())
    //    {
    //        if (ActivationEventList.Contains(ActivatorTargetEvent.adsRewardedVideoReady))
    //            PerformAction(ActivatorActionType.activate);
    //        if (DeactivationEventList.Contains(ActivatorTargetEvent.adsRewardedVideoReady))
    //            PerformAction(ActivatorActionType.deactivate);
    //    }
    //}
    //public void OnUnityAdsDidError(string message)
    //{
    //}
    //public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    //{
    //}
    //public void OnUnityAdsDidStart(string placementId)
    //{
    //}


    public void OnAdLoaded(PlacementType type)
    {
        if (type == PlacementType.video)
        {
            if (ActivationEventList.Contains(ActivatorTargetEvent.adsVideoReady))
                PerformAction(ActivatorActionType.activate);
            if (DeactivationEventList.Contains(ActivatorTargetEvent.adsVideoReady))
                PerformAction(ActivatorActionType.deactivate);
        }
        if (type == PlacementType.rewardedVideo)
        {
            if (ActivationEventList.Contains(ActivatorTargetEvent.adsRewardedVideoReady))
                PerformAction(ActivatorActionType.activate);
            if (DeactivationEventList.Contains(ActivatorTargetEvent.adsRewardedVideoReady))
                PerformAction(ActivatorActionType.deactivate);
        }
        if (type == PlacementType.interstitial)
        {
            if (ActivationEventList.Contains(ActivatorTargetEvent.adsInterstitialReady))
                PerformAction(ActivatorActionType.activate);
            if (DeactivationEventList.Contains(ActivatorTargetEvent.adsInterstitialReady))
                PerformAction(ActivatorActionType.deactivate);
        }
    }
    public void OnAdNotReady(PlacementType type)
    {
        if (type == PlacementType.video)
        {
            if (ActivationEventList.Contains(ActivatorTargetEvent.adsVideoNotReady))
                PerformAction(ActivatorActionType.activate);
            if (DeactivationEventList.Contains(ActivatorTargetEvent.adsVideoNotReady))
                PerformAction(ActivatorActionType.deactivate);
        }
        if (type == PlacementType.rewardedVideo)
        {
            if (ActivationEventList.Contains(ActivatorTargetEvent.adsRewardedVideoNotReady))
                PerformAction(ActivatorActionType.activate);
            if (DeactivationEventList.Contains(ActivatorTargetEvent.adsRewardedVideoNotReady))
                PerformAction(ActivatorActionType.deactivate);
        }
        if (type == PlacementType.interstitial)
        {
            if (ActivationEventList.Contains(ActivatorTargetEvent.adsInterstitialNotReady))
                PerformAction(ActivatorActionType.activate);
            if (DeactivationEventList.Contains(ActivatorTargetEvent.adsInterstitialNotReady))
                PerformAction(ActivatorActionType.deactivate);
        }
    }
    public void OnAdFinished(PlacementType type)
    {
        if (type == PlacementType.video)
        {
            if (ActivationEventList.Contains(ActivatorTargetEvent.adsVideoFinished))
                PerformAction(ActivatorActionType.activate);
            if (DeactivationEventList.Contains(ActivatorTargetEvent.adsVideoFinished))
                PerformAction(ActivatorActionType.deactivate);
        }
        if (type == PlacementType.rewardedVideo)
        {
            if (ActivationEventList.Contains(ActivatorTargetEvent.adsRewardedVideoFinished))
                PerformAction(ActivatorActionType.activate);
            if (DeactivationEventList.Contains(ActivatorTargetEvent.adsRewardedVideoFinished))
                PerformAction(ActivatorActionType.deactivate);
        }
        if (type == PlacementType.interstitial)
        {
            if (ActivationEventList.Contains(ActivatorTargetEvent.adsInterstitialFinished))
                PerformAction(ActivatorActionType.activate);
            if (DeactivationEventList.Contains(ActivatorTargetEvent.adsInterstitialFinished))
                PerformAction(ActivatorActionType.deactivate);
        }
    }
    public void OnAdFailed(PlacementType type)
    {
        if (type == PlacementType.video)
        {
            if (ActivationEventList.Contains(ActivatorTargetEvent.adsVideoFailed))
                PerformAction(ActivatorActionType.activate);
            if (DeactivationEventList.Contains(ActivatorTargetEvent.adsVideoFailed))
                PerformAction(ActivatorActionType.deactivate);
        }
        if (type == PlacementType.rewardedVideo)
        {
            if (ActivationEventList.Contains(ActivatorTargetEvent.adsRewardedVideoFailed))
                PerformAction(ActivatorActionType.activate);
            if (DeactivationEventList.Contains(ActivatorTargetEvent.adsRewardedVideoFailed))
                PerformAction(ActivatorActionType.deactivate);
        }
        if (type == PlacementType.interstitial)
        {
            if (ActivationEventList.Contains(ActivatorTargetEvent.adsInterstitialFailed))
                PerformAction(ActivatorActionType.activate);
            if (DeactivationEventList.Contains(ActivatorTargetEvent.adsInterstitialFailed))
                PerformAction(ActivatorActionType.deactivate);
        }
    }
    //    public static void AdSkipped(PlacementType type)
    //    {
    //        Debug.Log(type + " placement skipped");
    //        if (adSkipped != null)
    //            adSkipped(type);
    //    }
    //    public static void AdOpened(PlacementType type)
    //    {
    //        Debug.Log(type + " placement clicked");
    //        if (adOpened != null)
    //            adOpened(type);
    //    }
    //    public static void AdUserLeave(PlacementType type)
    //    {
    //        Debug.Log("User left the application watching" + type );
    //        if (adUserLeave != null)
    //            adUserLeave(type);
    //    }

    public void OnPause()
    {
        if (ActivationEventList.Contains(ActivatorTargetEvent.gamePaused))
            PerformAction(ActivatorActionType.activate);
        if (DeactivationEventList.Contains(ActivatorTargetEvent.gamePaused))
            PerformAction(ActivatorActionType.deactivate);
    }
    public void OnUnpause()
    {
        if (ActivationEventList.Contains(ActivatorTargetEvent.gameUnpaused))
            PerformAction(ActivatorActionType.activate);
        if (DeactivationEventList.Contains(ActivatorTargetEvent.gameUnpaused))
            PerformAction(ActivatorActionType.deactivate);
    }
    public void OnSessionStateChanged(GameSessionState state)
    {
        if (state == GameSessionState.Won)
        {
            if (ActivationEventList.Contains(ActivatorTargetEvent.gameWon))
                PerformAction(ActivatorActionType.activate);
            if (DeactivationEventList.Contains(ActivatorTargetEvent.gameWon))
                PerformAction(ActivatorActionType.deactivate);
        }
        if (state == GameSessionState.Lost)
        {
            if (ActivationEventList.Contains(ActivatorTargetEvent.gameLost))
                PerformAction(ActivatorActionType.activate);
            if (DeactivationEventList.Contains(ActivatorTargetEvent.gameLost))
                PerformAction(ActivatorActionType.deactivate);
        }
        if (state == GameSessionState.Passed)
        {
            if (ActivationEventList.Contains(ActivatorTargetEvent.gamePassed))
                PerformAction(ActivatorActionType.activate);
            if (DeactivationEventList.Contains(ActivatorTargetEvent.gamePassed))
                PerformAction(ActivatorActionType.deactivate);
        }
    }
    public void OnExtraRewardReceived()
    {
            if (ActivationEventList.Contains(ActivatorTargetEvent.extraRewardReceived))
                PerformAction(ActivatorActionType.activate);
            if (DeactivationEventList.Contains(ActivatorTargetEvent.extraRewardReceived))
                PerformAction(ActivatorActionType.deactivate);
    }
    public void OnNewCarAppearenceReceived()
    {
        if (ActivationEventList.Contains(ActivatorTargetEvent.newCarAppeareenceReceived))
            PerformAction(ActivatorActionType.activate);
        if (DeactivationEventList.Contains(ActivatorTargetEvent.newCarAppeareenceReceived))
            PerformAction(ActivatorActionType.deactivate);
    }
    public void OnLevelChanged(Scene current, Scene next)
    {
        if (ActivationEventList.Contains(ActivatorTargetEvent.sceneLoaded))
            PerformAction(ActivatorActionType.activate);
        if (DeactivationEventList.Contains(ActivatorTargetEvent.sceneLoaded))
            PerformAction(ActivatorActionType.deactivate);
    }
    private void PerformAction(ActivatorActionType action)
    {
        if (
            (
                action == ActivatorActionType.activate &&
                (
                    ActivationConditions.Count == 0 ||
                    ActivationConditions.Contains(ActivatorTargetConditions.none) ||
                    (ActivationConditions.Contains(ActivatorTargetConditions.gameInProgress) && Engine.sessionState == GameSessionState.InProgress) ||
                    (ActivationConditions.Contains(ActivatorTargetConditions.gameWon) && Engine.sessionState == GameSessionState.Won) ||
                    (ActivationConditions.Contains(ActivatorTargetConditions.gameLost) && Engine.sessionState == GameSessionState.Lost) ||
                    (ActivationConditions.Contains(ActivatorTargetConditions.gamePassed) && Engine.sessionState == GameSessionState.Passed) ||
                    (ActivationConditions.Contains(ActivatorTargetConditions.readyToGiveNewCarAppearence) && (Engine.meta.car.nextAppearenceProgress == 0 && Engine.meta.car.previousNextAppearenceProgress >= 0)) ||
                    (ActivationConditions.Contains(ActivatorTargetConditions.notReadyToGiveNewCarAppearence) && !(Engine.meta.car.nextAppearenceProgress == 0 && Engine.meta.car.previousNextAppearenceProgress >= 0)) ||
                    (ActivationConditions.Contains(ActivatorTargetConditions.newCarAppearenceReceived) && Engine.newCarAppearenceReceivedInCurrentSession) ||
                    (ActivationConditions.Contains(ActivatorTargetConditions.isBossFight) && Engine.isBossFight) ||
                    (ActivationConditions.Contains(ActivatorTargetConditions.isNotBossFight) && !Engine.isBossFight)
                )
            ) ||
            (
                action == ActivatorActionType.deactivate &&
                (
                    DeactivationConditions.Count == 0 ||
                    DeactivationConditions.Contains(ActivatorTargetConditions.none) ||
                    (DeactivationConditions.Contains(ActivatorTargetConditions.gameInProgress) && Engine.sessionState == GameSessionState.InProgress) ||
                    (DeactivationConditions.Contains(ActivatorTargetConditions.gameWon) && Engine.sessionState == GameSessionState.Won) ||
                    (DeactivationConditions.Contains(ActivatorTargetConditions.gameLost) && Engine.sessionState == GameSessionState.Lost) ||
                    (DeactivationConditions.Contains(ActivatorTargetConditions.gamePassed) && Engine.sessionState == GameSessionState.Passed) ||
                    (DeactivationConditions.Contains(ActivatorTargetConditions.readyToGiveNewCarAppearence) && (Engine.meta.car.nextAppearenceProgress == 0 && Engine.meta.car.previousNextAppearenceProgress >= 0)) ||
                    (DeactivationConditions.Contains(ActivatorTargetConditions.notReadyToGiveNewCarAppearence) && !(Engine.meta.car.nextAppearenceProgress == 0 && Engine.meta.car.previousNextAppearenceProgress >= 0)) ||
                    (DeactivationConditions.Contains(ActivatorTargetConditions.newCarAppearenceReceived) && Engine.newCarAppearenceReceivedInCurrentSession) ||
                    (DeactivationConditions.Contains(ActivatorTargetConditions.isBossFight) && Engine.isBossFight) ||
                    (DeactivationConditions.Contains(ActivatorTargetConditions.isNotBossFight) && !Engine.isBossFight)
                )
            )
          )
        {
            Debug.Log(action.ToString() + " " + TargetObject.name);
            TargetObject.SetActive(action == ActivatorActionType.activate);
            if (TargetObject.GetComponent<Button>() != null)
                TargetObject.GetComponent<Button>().interactable = (action == ActivatorActionType.activate);
        }
    }
}
