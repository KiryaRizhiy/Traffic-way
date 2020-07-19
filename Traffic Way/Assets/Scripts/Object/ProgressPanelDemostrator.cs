using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressPanelDemostrator : MonoBehaviour
{
    private float _passedTime = 0;
    private Texture2D _carAppearence;
    private Texture2D _carAppearenceShadow;
    private Color _transparent = new Color(1, 1, 1, 0);
    private bool carOpenDemonstrated = false;

    private float passedTimeWithoutPause
    {
        get { return _passedTime - Settings.carProgressPauseTime; }
    }
    private float blinkPassedTime
    {
        get
        {
            return _passedTime - (Settings.carProgressPauseTime + Settings.carProgressDemoTime - Settings.carProgressBlingOffset);
        }
    }
    private Transform carShadowImg
    {
        get
        {
            return transform.GetChild(0).GetChild(0);
        }
    }
    private Transform carImg
    {
        get
        {
            return transform.GetChild(0).GetChild(1);
        }
    }
    private Transform extraRewardButton
    {
        get
        {
            return transform.GetChild(2);
        }
    }
    private Transform newCarAppearenceButton
    {
        get
        {
            return transform.GetChild(1);
        }
    }
    private bool readyToPassNewAppearence
    {
        get
        {
            return Engine.meta.car.nextAppearenceProgress == 0 && Engine.meta.car.previousNextAppearenceProgress > 0;
        }
    }
    private int fromPercent
    {
        get
        { return Engine.meta.car.previousNextAppearenceProgress; }
    }
    private int toPercent
    {
        get
        {
            if (Engine.meta.car.nextAppearenceProgress == 0)
                return 100;
            else
                return Engine.meta.car.nextAppearenceProgress;
        }
    }
    private float currentPercent
    {
        get
        {
            if (passedTimeWithoutPause <= 0)
                return fromPercent;
            if (passedTimeWithoutPause < Settings.carProgressDemoTime)
                return fromPercent + (toPercent - fromPercent) * (passedTimeWithoutPause / Settings.carProgressDemoTime);
            else 
                return toPercent;
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
        if (Engine.meta.car.nextPassedAppearenceNum < 0)
        {
            if (Engine.meta.car.nextAppearenceProgress == 0)//Progress will reach to top rigt now
            {
                _carAppearenceShadow = Engine.CarAppearencesShadows[Engine.CarAppearencesShadows.Count - 1];
                _carAppearence = Engine.CarsAppearences[Engine.CarsAppearences.Count - 1];
            }
            else
            {//Progress reached to the top earlier, nothing to demonstrate
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(1).gameObject.SetActive(false);
                //transform.GetChild(2).gameObject.SetActive(false);
                //transform.GetChild(3).gameObject.SetActive(false);
                //transform.GetChild(4).gameObject.SetActive(false);
                //transform.GetChild(7).gameObject.SetActive(false);
                return;
            }
        }
        else//Normal progress demo
        {
            if (Engine.meta.car.nextAppearenceProgress == 0)
            {
                _carAppearenceShadow = Engine.CarAppearencesShadows[Engine.meta.car.nextPassedAppearenceNum - 1];
                _carAppearence = Engine.CarsAppearences[Engine.meta.car.nextPassedAppearenceNum - 1];
            }
            else
            {
                _carAppearenceShadow = Engine.CarAppearencesShadows[Engine.meta.car.nextPassedAppearenceNum];
                _carAppearence = Engine.CarsAppearences[Engine.meta.car.nextPassedAppearenceNum];
            }
        }
        carShadowImg.GetComponent<Image>().sprite = Sprite.Create(_carAppearenceShadow, new Rect(0f, 0f, _carAppearenceShadow.width, _carAppearenceShadow.height), Vector2.one * 0.5f, 100f);
        Texture2D _t = Functions.CutAndLerpTexture(_carAppearence, Mathf.RoundToInt(fromPercent), 0, Color.white);
        carImg.GetComponent<Image>().sprite = Sprite.Create(_t,
            new Rect(0f, 0f, _t.width, _t.height),
            Vector2.one * 0.5f);
    }
    void Update()
    {
        _passedTime += Time.deltaTime;
        //Check if pause
        if (_carAppearence == null)
            return;
        if (_passedTime < Settings.carProgressPauseTime)
        {
            //Texture2D _t = GetTextureAccordingToPercent(Engine.meta.car.previousNextAppearenceProgress);
            //carImg.GetComponent<Image>().sprite = Sprite.Create(_t,
            //    new Rect(0f, 0f, _t.width, _t.height),
            //    Vector2.one * 0.5f);
            return;
        }
        //Check if progress done
        if (_passedTime > (Settings.carProgressPauseTime + Settings.carProgressDemoTime))
            return;
        //Check if progress is demonstrating
        if (_passedTime >= Settings.carProgressPauseTime && _passedTime <= (Settings.carProgressPauseTime + Settings.carProgressDemoTime))
        {
            //Debug.Log("Passed time: " + _passedTime);
            //float fromAnchor = 0.1f + 0.8f * (fromPercent/100f);
            //float toAnchor = fromAnchor + 0.8f * ( toPercent/100f);
            //if (toAnchor > 0.9f)
            //    toAnchor = 0.9f;
            //float passedPercent = passedTimeWithoutPause / Settings.carProgressDemoTime;
            //float currentPercent = fromPercent + (toPercent - fromPercent) * (passedTimeWithoutPause / Settings.carProgressDemoTime);
            //if (currentPercent > toPercent)
            //    currentPercent = toPercent;
            //float requiredAnchor = fromAnchor + (toAnchor - fromAnchor) * passedPercent;
            //if (requiredAnchor > toAnchor)
            //    requiredAnchor = toAnchor;
            ////Debug.Log("Computing line progress fromPercent " + fromPercent + " toPercent " + toPercent +" fromAnchor " + fromAnchor + " toAnchor "+ toAnchor +" passedPercent " + passedPercent +" requiredAnchor " + requiredAnchor);
            //progressBar.GetComponent<RectTransform>().anchorMax = new Vector2(0.15f, requiredAnchor);
            if (toPercent == 100 && _passedTime >= (Settings.carProgressPauseTime + Settings.carProgressDemoTime - Settings.carProgressBlingOffset))
            {
                if (!carOpenDemonstrated)
                    StartCoroutine(Blink());
            }
            else
            {
                Texture2D _t = Functions.CutAndLerpTexture(_carAppearence, Mathf.RoundToInt(currentPercent), 0, Color.white);
                carImg.GetComponent<Image>().sprite = Sprite.Create(_t,
                    new Rect(0f, 0f, _t.width, _t.height),
                    Vector2.one * 0.5f);
            }
        }
    }
    void OnDestroy()
    {
        Engine.Events.adLoaded -= HandlePlacementReady;
        Engine.Events.extraRewardReceived -= HandleExtraRewardReceive;
        Engine.Events.newCarAppearenceReceived -= HandleNewCarAppearenceReceive;
        StopAllCoroutines();
    }

    //private Texture2D GetTextureAccordingToPercent(int percent)
    //{
    //    //Debug.Log("Drawing " + percent + "%");
    //    if (percent > 100)
    //    {
    //        percent = 100;
    //        Debug.LogWarning("Unable to use drawing percent" + percent + " used 100 instead");
    //    }
    //    Texture2D _txt = new Texture2D(_carAppearence.width, _carAppearence.height,_carAppearence.format,false);
    //    RenderTexture currentRT = RenderTexture.active;
    //    RenderTexture renderTexture = new RenderTexture(_carAppearence.width, _carAppearence.height, 3, RenderTextureFormat.ARGB32);
    //    Graphics.Blit(_carAppearence, renderTexture);
    //    _txt.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);
    //    _txt.Apply();
    //    for (int x = 0; x < _txt.width; x++)
    //        for (int y = _txt.height; y > _txt.height * (percent / 100f); y--)
    //                _txt.SetPixel(x, y, _transparent);
    //    _txt.Apply();
    //    RenderTexture.active = currentRT;
    //    renderTexture.Release();
    //    return _txt;
    //}
    //private Texture2D LerpTextureWithColor(Texture2D source, float lighteningCoeffitient, Color color,bool useTextureAlpha = true)
    //{
    //    if (lighteningCoeffitient > 1f)
    //    {
    //        lighteningCoeffitient = 1f;
    //        Debug.LogWarning("Unable to use lightening coeffitient " + lighteningCoeffitient + " used 1 instead");
    //    }
    //    Texture2D _txt = new Texture2D(source.width, source.height, source.format, false);
    //    Color currentColor = color;
    //    RenderTexture currentRT = RenderTexture.active;
    //    RenderTexture renderTexture = new RenderTexture(source.width, source.height, 3, RenderTextureFormat.ARGB32);
    //    Graphics.Blit(source, renderTexture);
    //    _txt.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);
    //    _txt.Apply();
    //    for (int x = 0; x < _txt.width - 1; x++)
    //        for (int y = 0; y < _txt.height - 1; y++)
    //        {
    //            if (useTextureAlpha)
    //                currentColor.a = _txt.GetPixel(x, y).a;
    //            //Debug.Log("Coeffitient:_txt.GetPixel(x, y) " + lighteningCoeffitient + " old color " + _txt.GetPixel(x, y).ToString() + " new color: " + Color.Lerp(_txt.GetPixel(x, y), color, lighteningCoeffitient).ToString());
    //            _txt.SetPixel(x, y, Color.Lerp(_txt.GetPixel(x, y), currentColor, lighteningCoeffitient));
    //        }
    //    _txt.Apply();
    //    RenderTexture.active = currentRT;
    //    renderTexture.Release();
    //    return _txt;
    //}
    private IEnumerator Blink()
    {
        carOpenDemonstrated = true;
        Debug.Log("Blinking start");
        Texture2D _carTxt;
        Texture2D _carShadowTxt;
        while (blinkPassedTime < Settings.carProgressBlinkTime/2)
        {
            _carTxt = Functions.CutAndLerpTexture(_carAppearence, Mathf.RoundToInt(currentPercent), (blinkPassedTime / Settings.carProgressBlinkTime) * 2, Color.white);
            _carShadowTxt = Functions.CutAndLerpTexture(_carAppearenceShadow, 100, (blinkPassedTime / Settings.carProgressBlinkTime) * 2, Color.white); 
            carImg.GetComponent<Image>().sprite = Sprite.Create(_carTxt,
                new Rect(0f, 0f, _carTxt.width, _carTxt.height),
                Vector2.one * 0.5f);
            carShadowImg.GetComponent<Image>().sprite = Sprite.Create(_carShadowTxt,
                new Rect(0f, 0f, _carShadowTxt.width, _carShadowTxt.height),
                Vector2.one * 0.5f);
            Debug.Log("Blink stage " + (blinkPassedTime / Settings.carProgressBlinkTime));
            yield return new WaitForEndOfFrame();
        }
        while (blinkPassedTime >= Settings.carProgressBlinkTime / 2 && blinkPassedTime < Settings.carProgressBlinkTime)
        {
            _carTxt = Functions.CutAndLerpTexture(_carAppearence, Mathf.RoundToInt(currentPercent), (1 - blinkPassedTime / Settings.carProgressBlinkTime) * 2, Color.white);
            _carShadowTxt = Functions.CutAndLerpTexture(_carAppearenceShadow, 100, (blinkPassedTime / Settings.carProgressBlinkTime) * 2, _transparent, false);
            carImg.GetComponent<Image>().sprite = Sprite.Create(_carTxt,
                new Rect(0f, 0f, _carTxt.width, _carTxt.height),
                Vector2.one * 0.5f);
            carShadowImg.GetComponent<Image>().sprite = Sprite.Create(_carShadowTxt,
                new Rect(0f, 0f, _carShadowTxt.width, _carShadowTxt.height),
                Vector2.one * 0.5f);
            Debug.Log("Blink stage " + (blinkPassedTime / Settings.carProgressBlinkTime));
            yield return new WaitForEndOfFrame();
        }
        _carTxt = _carAppearence;
        _carShadowTxt = Functions.CutAndLerpTexture(_carAppearenceShadow, 100, 1, _transparent, false);
        carImg.GetComponent<Image>().sprite = Sprite.Create(_carTxt,
            new Rect(0f, 0f, _carTxt.width, _carTxt.height),
            Vector2.one * 0.5f);
        carShadowImg.GetComponent<Image>().sprite = Sprite.Create(_carShadowTxt,
            new Rect(0f, 0f, _carShadowTxt.width, _carShadowTxt.height),
            Vector2.one * 0.5f);
    }
}