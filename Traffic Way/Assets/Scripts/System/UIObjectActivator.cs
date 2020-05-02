using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class UIObjectActivator : MonoBehaviour, IUnityAdsListener
{
    public enum ActivatorTargetEvent {adsVideoReady,adsRewardedVideoReady,extraRewardReceived,gameWon,gameLost,gamePassed}
    public enum ActivatorActionType { activate, deactivate }

    public List<ActivatorTargetEvent> ActivationEventList;
    public List<ActivatorTargetEvent> DeactivationEventList;
    public GameObject TargetObject;
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(string.Format("{0}'s activator, linked to {1} initializes", TargetObject.name, gameObject.name));
        if (TargetObject == null)
        {
            Debug.LogError("Target object not set on " + gameObject.name + "'s activator");
        }
        if (ActivationEventList == null && DeactivationEventList == null)
        {
            Debug.LogError(TargetObject.name + "'s activator has no action events");
        }
        Advertisement.AddListener(this);
        Engine.Events.gameSessionStateChanged += OnSessionStateChanged;
        Engine.Events.extraRewardReceived += OnExtraRewardReceived;
    }
    void OnDestroy()
    {
        Advertisement.RemoveListener(this);
        Engine.Events.gameSessionStateChanged -= OnSessionStateChanged;
        Engine.Events.extraRewardReceived -= OnExtraRewardReceived;
    }

    public void OnUnityAdsReady(string placementId)
    {
        if (placementId == PlacementType.video.ToString())
        {
            if (ActivationEventList.Contains(ActivatorTargetEvent.adsVideoReady))
                PerformAction(ActivatorActionType.activate);
            if (DeactivationEventList.Contains(ActivatorTargetEvent.adsVideoReady))
                PerformAction(ActivatorActionType.deactivate);
        }
        if (placementId == PlacementType.rewardedVideo.ToString())
        {
            if (ActivationEventList.Contains(ActivatorTargetEvent.adsRewardedVideoReady))
                PerformAction(ActivatorActionType.activate);
            if (DeactivationEventList.Contains(ActivatorTargetEvent.adsRewardedVideoReady))
                PerformAction(ActivatorActionType.deactivate);
        }
    }
    public void OnUnityAdsDidError(string message)
    {
    }
    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
    }
    public void OnUnityAdsDidStart(string placementId)
    {
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

    private void PerformAction(ActivatorActionType action)
    {
        TargetObject.SetActive(action == ActivatorActionType.activate);
        if (TargetObject.GetComponent<Button>() != null)
            TargetObject.GetComponent<Button>().interactable = (action == ActivatorActionType.activate);
    }
}
