using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public GameObject SwitchStartZone;

    private static Texture2D TLYellow;
    private static Texture2D TLGreen;
    private bool isGreen = false;
    private SpriteRenderer Light
    {
        get { return transform.GetChild(1).GetComponent<SpriteRenderer>(); }
    }

    public static void LoadResources()
    {
        TLYellow = Resources.Load<Texture2D>("TrafficWay/Textures/Objects/TrafficLight/TLYellow");
        TLGreen = Resources.Load<Texture2D>("TrafficWay/Textures/Objects/TrafficLight/TLGreen");
    }
    public void StopLineCrossed()
    {
        if (!isGreen) Engine.Events.CrashHappened();
    }

    void Start()
    {
        if (SwitchStartZone == null)
            Debug.LogError("Switch start zone not set");
        Engine.Events.zoneReached += StartSwitching;
    }
    void OnDestroy()
    {
        Engine.Events.zoneReached -= StartSwitching;
    }
    private void StartSwitching(GameObject zone)
    {
        if (zone == SwitchStartZone)
            StartCoroutine(SwitchToYellow());
    }
    private IEnumerator SwitchToYellow()
    {
        yield return new WaitForSeconds(Settings.trafficLightSwitchSpeed);
        Light.sprite = Sprite.Create(TLYellow, new Rect(0.0f, 0.0f, TLYellow.width, TLYellow.height), Vector2.one * 0.5f);
        StartCoroutine(SwitchToGreen());
    }
    private IEnumerator SwitchToGreen()
    {
        yield return new WaitForSeconds(Settings.trafficLightSwitchSpeed);
        Light.sprite = Sprite.Create(TLGreen, new Rect(0.0f, 0.0f, TLGreen.width, TLGreen.height), Vector2.one * 0.5f);
        isGreen = true;
    }
}
