using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class UserInteraction : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextLevel()
    {
        Engine.LevelDone();
    }
    public void Restart()
    {
        Engine.LevelFailed();
    }
    public void ShowRewardedVideo()
    {
        if (Engine.isRewardedVideoReady)
            Advertisement.Show(PlacementType.rewardedVideo.ToString());
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
