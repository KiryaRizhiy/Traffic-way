using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private Transform carImg
    {
        get
        {
            return transform.GetChild(0);
        }
    }
    private Transform extraRewardButton
    {
        get
        {
            return transform.GetChild(6);
        }
    }
    private Transform newCarAppearenceButton
    {
        get
        {
            return transform.GetChild(5);
        }
    }
    private bool readyToPassNewAppearence
    {
        get
        {
            return Engine.meta.car.nextAppearenceProgress == 0 && Engine.meta.car.previousNextAppearenceProgress > 0;
        }
    }

    private void HandlePlacementReady(PlacementType type)
    {
        if (type != PlacementType.rewardedVideo)
            return;
        if (readyToPassNewAppearence)
        {//If we need to get new car appearence
            if (!Engine.newCarAppearenceReceivedInCurrentSession)
            {//New appearence not received yet
                extraRewardButton.gameObject.SetActive(false);
                newCarAppearenceButton.gameObject.SetActive(true);
                newCarAppearenceButton.GetComponent<Button>().interactable = true;
            }
            else
            {//New appearence already received
                if (!Engine.extraRewardReceivedInCurrentSession)
                {//Extra reward not received yet
                    newCarAppearenceButton.gameObject.SetActive(false);
                    extraRewardButton.gameObject.SetActive(true);
                    extraRewardButton.GetComponent<Button>().interactable = true;
                }
                else
                    return;//New appearence received and extra reward too
            }
        }
        else
        {//No need to get new car appearence
            if (!Engine.extraRewardReceivedInCurrentSession)
            {//Extra reward not received yet
                newCarAppearenceButton.gameObject.SetActive(false);
                extraRewardButton.gameObject.SetActive(true);
                extraRewardButton.GetComponent<Button>().interactable = true;
            }
            else
                return;//Extra reward received
        }
    }
    private void HandleExtraRewardReceive()
    {
        extraRewardButton.gameObject.SetActive(false);
    }
    private void HandleNewCarAppearenceReceive()
    {
        newCarAppearenceButton.gameObject.SetActive(false);
        extraRewardButton.gameObject.SetActive(true);
        if (AdMobController.isRewardedVideoReady)
            extraRewardButton.GetComponent<Button>().interactable = true;
        else
            extraRewardButton.GetComponent<Button>().interactable = false;
    }

    void Start()
    {
        Engine.Events.adLoaded += HandlePlacementReady;
        Engine.Events.extraRewardReceived += HandleExtraRewardReceive;
        Engine.Events.newCarAppearenceReceived += HandleNewCarAppearenceReceive;

        if (readyToPassNewAppearence)
        {
            extraRewardButton.gameObject.SetActive(false);
            if (AdMobController.isRewardedVideoReady)
            {
                newCarAppearenceButton.gameObject.SetActive(true);
                newCarAppearenceButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                newCarAppearenceButton.gameObject.SetActive(true);
                newCarAppearenceButton.GetComponent<Button>().interactable = false;
            }
        }
        else
        {
            newCarAppearenceButton.gameObject.SetActive(false);
            if (AdMobController.isRewardedVideoReady)
            {
                extraRewardButton.gameObject.SetActive(true);
                extraRewardButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                extraRewardButton.gameObject.SetActive(true);
                extraRewardButton.GetComponent<Button>().interactable = false;
            }
        }

        Texture2D _tex;
        if (Engine.meta.car.nextPassedAppearenceNum < 0)
        {
            if (Engine.meta.car.nextAppearenceProgress == 0)//Progress will reach to top rigt now
                _tex = Engine.CarsAppearences[Engine.CarsAppearences.Count - 1];
            else
            {//Progress reached to the top earlier, nothing to demonstrate
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(1).gameObject.SetActive(false);
                transform.GetChild(2).gameObject.SetActive(false);
                transform.GetChild(3).gameObject.SetActive(false);
                transform.GetChild(4).gameObject.SetActive(false);
                return;
            }
        }
        else//Normal progress demo
        {
            if (Engine.meta.car.nextAppearenceProgress == 0)
                _tex = Engine.CarsAppearences[Engine.meta.car.nextPassedAppearenceNum - 1];
            else
                _tex = Engine.CarsAppearences[Engine.meta.car.nextPassedAppearenceNum];
        }
        carImg.GetComponent<Image>().sprite = Sprite.Create(_tex, new Rect(0f, 0f, _tex.width, _tex.height), Vector2.one * 0.5f, 100f);
    }
    void Update()
    {
        passedTime += Time.deltaTime;
        //Check if pause
        if (passedTime < Settings.carProgressPauseTime)
        {
            progressBar.GetComponent<RectTransform>().anchorMax = new Vector2(0.15f, 0.1f + 0.8f * (Engine.meta.car.previousNextAppearenceProgress / 100f));
            return;
        }
        //Check if progress done
        if (passedTime > (Settings.carProgressPauseTime + Settings.carProgressDemoTime))
            return;
        //Check if progress is demonstrating
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
            //Debug.Log("Computing line progress fromPercent " + fromPercent + " toPercent " + toPercent +" fromAnchor " + fromAnchor + " toAnchor "+ toAnchor +" passedPercent " + passedPercent +" requiredAnchor " + requiredAnchor);
            progressBar.GetComponent<RectTransform>().anchorMax = new Vector2(0.15f, requiredAnchor);
        }
    }
    void OnDestroy()
    {
        Engine.Events.adLoaded -= HandlePlacementReady;
        Engine.Events.extraRewardReceived -= HandleExtraRewardReceive;
        Engine.Events.newCarAppearenceReceived -= HandleNewCarAppearenceReceive;
    }
}