using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class TVSetController : MonoBehaviour
{
    private UserInteraction UI
    {
        get { return transform.parent.parent.parent.GetComponent<UserInteraction>(); }
    }

    void Awake()
    {
        GetComponent<Animator>().SetBool("isActive", false);
        Engine.Events.initialized += HandleInitialization;
        Engine.Events.adFailed += HandleAdEvent;
        Engine.Events.adFinished += HandleAdEvent;
        Engine.Events.adLoaded += HandleAdEvent;
        Engine.Events.adNotReady += HandleAdEvent;
        Engine.Events.adSkipped += HandleAdEvent;
        if (Engine.initialized)
            Demonstrate();
    }
    void OnDestroy()
    {
        Engine.Events.initialized -= HandleInitialization;
        Engine.Events.adFailed -= HandleAdEvent;
        Engine.Events.adFinished -= HandleAdEvent;
        Engine.Events.adLoaded -= HandleAdEvent;
        Engine.Events.adNotReady -= HandleAdEvent;
        Engine.Events.adSkipped -= HandleAdEvent;
    }


    //void Update()
    //{
    //    //if (
    //    //    new DateTime(DateTime.UtcNow.Ticks - Mathf.FloorToInt(Time.deltaTime * 10000000f)) <= new DateTime(Engine.meta.garage.lastTVWatched) + new TimeSpan(0,Settings.TVCooldownMinutes,0)
    //    //    &&
    //    //    new DateTime(DateTime.UtcNow.Ticks + Mathf.CeilToInt(Time.deltaTime * 10000000f)) >= new DateTime(Engine.meta.garage.lastTVWatched) + new TimeSpan(0,Settings.TVCooldownMinutes,0)
    //    //    )
    //    //{
    //    //    Demonstrate();
    //    //    //Debug.Log("Now is the time to show ");
    //    //    //Debug.Log(
    //    //    //    "Garage last watched: " + new DateTime(Engine.meta.garage.lastTVWatched).ToString() + " next watch time " + (new DateTime(Engine.meta.garage.lastTVWatched) + new TimeSpan(0,Settings.TVCooldownMinutes,0)).ToString()  + Environment.NewLine +
    //    //    //    "Delta time in ticks lower : " + Mathf.FloorToInt(Time.deltaTime * 10000000f).ToString() + " higher " + Mathf.CeilToInt(Time.deltaTime * 10000000f).ToString() + Environment.NewLine +
    //    //    //    "Garage last watched lower frame border: " + new DateTime(DateTime.UtcNow.Ticks - Mathf.FloorToInt(Time.deltaTime * 10000000f)).ToString() + Environment.NewLine +
    //    //    //    "Current datetime: " + DateTime.UtcNow.ToString() + Environment.NewLine +
    //    //    //    "Garage last watched higher frame border: " + new DateTime(DateTime.UtcNow.Ticks + Mathf.CeilToInt(Time.deltaTime * 10000000f)).ToString() + Environment.NewLine
    //    //    //    );
    //    //}
    //    //if (DateTime.UtcNow >= new DateTime(Engine.meta.garage.lastTVWatched) + new TimeSpan(0, Settings.TVCooldownMinutes, 0))
    //    //    Debug.Log(
    //    //        "Garage last watched: " + new DateTime(Engine.meta.garage.lastTVWatched).ToString() + Environment.NewLine +
    //    //        "Delta time in ticks lower : " + Mathf.FloorToInt(Time.deltaTime * 10000000f).ToString() + " higher " + Mathf.CeilToInt(Time.deltaTime * 10000000f).ToString() + Environment.NewLine +
    //    //        "Garage last watched lower frame border: " + new DateTime(DateTime.UtcNow.Ticks - Mathf.FloorToInt(Time.deltaTime * 10000000f)).ToString() + Environment.NewLine +
    //    //        "Current datetime: " + DateTime.UtcNow.ToString() + Environment.NewLine +
    //    //        "Garage last watched higher frame border: " + new DateTime(DateTime.UtcNow.Ticks + Mathf.CeilToInt(Time.deltaTime * 10000000f)).ToString() + Environment.NewLine
    //    //        );
    //}

    public void Tap()
    {
        if (Engine.meta.garage.TVAbleToShow)
            UI.ShowTVAds();
    }

    private void HandleAdEvent(PlacementType type)
    {
        ShowCountDown();
    }
    private void HandleInitialization()
    {
        ShowCountDown();
    }
    private void Demonstrate()
    {
        Debug.Log("Demonstrating TV");
        if (Engine.meta.garage.TVAbleToShow)
            GetComponent<Animator>().SetBool("isActive", true);
        else
            GetComponent<Animator>().SetBool("isActive", false);
    }
    private IEnumerator ShowCountDown()
    {
        Debug.Log("Start TV countdown");
        long _offset = new TimeSpan(0, Settings.TVCooldownMinutes, 0).Ticks;
        yield return new WaitUntil(() => DateTime.UtcNow.Ticks > Engine.meta.garage.lastTVWatched + _offset);
        Debug.Log("TV counted down");
        Demonstrate();
    }
}