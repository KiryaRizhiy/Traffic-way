using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressPanelDemostrator : MonoBehaviour
{
    private float passedTime = 0;

    private float passedTimeWithoutPause
    {
        get { return passedTime - Settings.carProgressPauseTime; }
    }
    private Transform progressBar
    {
        get { return transform.GetChild(2); }
    }

    void Update()
    {
        //There is nothing to show
        if (Engine.meta.car.nextPassedAppearenceNum == -1)
        {
            Destroy(gameObject);
            return;
        }
        passedTime += Time.deltaTime;
        //Check if progress move required
        if (passedTime < Settings.carProgressPauseTime)
        {
            progressBar.GetComponent<RectTransform>().anchorMax = new Vector2(0.15f, 0.1f + 0.8f * (Engine.meta.car.previousNextAppearenceProgress / 100f));
            return;
        }
        if (passedTime > (Settings.carProgressPauseTime + Settings.carProgressDemoTime))
            return;
        if (passedTime >= Settings.carProgressPauseTime && passedTime <= (Settings.carProgressPauseTime + Settings.carProgressDemoTime))
        {
            int fromPercent = Engine.meta.car.previousNextAppearenceProgress;
            int toPercent;
            if (Engine.meta.car.nextAppearenceProgress == 0)
                toPercent = 100;
            else
                toPercent = Engine.meta.car.nextAppearenceProgress;
            float fromAnchor = 0.1f + 0.8f * (fromPercent/100f);
            float toAnchor = fromAnchor + 0.8f * ( toPercent/100f);
            if (toAnchor > 0.9f)
                toAnchor = 0.9f;
            float passedPercent = passedTimeWithoutPause / Settings.carProgressDemoTime;
            float requiredAnchor = fromAnchor + (toAnchor - fromAnchor) * passedPercent;
            if (requiredAnchor > toAnchor)
                requiredAnchor = toAnchor;
            Debug.Log("Computing line progress fromPercent " + fromPercent + " toPercent " + toPercent +" fromAnchor " + fromAnchor + " toAnchor "+ toAnchor +" passedPercent " + passedPercent +" requiredAnchor " + requiredAnchor);
            progressBar.GetComponent<RectTransform>().anchorMax = new Vector2(0.15f, requiredAnchor);
        }
    }
}