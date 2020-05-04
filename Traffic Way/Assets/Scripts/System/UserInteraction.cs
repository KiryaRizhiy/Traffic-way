using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.EventSystems;

public class UserInteraction : MonoBehaviour
{
    public static bool gas
    {
        get;
        private set;
    }

    void Update()
    {

        gas = false;
        if (Engine.paused)
            return;
        int pointerId;
        if (Input.touchCount > 0)
            pointerId = Input.touches[0].fingerId;
        else
            pointerId = -1;
        Logger.UpdateContent(UILogDataType.Controls, "Pointer " + EventSystem.current.IsPointerOverGameObject(pointerId));
        if (EventSystem.current.IsPointerOverGameObject(pointerId) && !GasButton.pressed)
            return;
        if (Input.GetMouseButton(0) || Input.touchCount == 1 || GasButton.pressed)
            gas = true;
        else
            gas = false;
    }
    public void NextLevel()
    {
        Engine.LevelDone();
    }
    public void Restart()
    {
        Engine.LevelFailed();
    }
    public void SwitchPause()
    {
        Engine.SwitchPause();
    }
    public void ShowPrivacyPolicy()
    {
        Application.OpenURL(Settings.privacyPolicyLink);
    }
    public void ShowRewardedVideo()
    {
        Engine.ShowRewardedVideo();
    }
    public void RewardedWathced()
    {
        Engine.Events.AdFinished(PlacementType.rewardedVideo);
    }
    public void InterstitialWatched()
    {
        Engine.Events.AdFinished(PlacementType.interstitial);
    }
    public void FinishLineReached()
    {
        Engine.Events.FinishLineReached();
    }
    public void CrashHappened()
    {
        Engine.Events.CrashHappened();
    }
    public void Play()
    {
        Engine.Play();
    }
    public void Quit()
    {
        Engine.Quit();
    }
    public void ClearSaveFile()
    {
        Engine.ClearSaveFile();
    }
}