using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityEngine.EventSystems;

public class UserInteraction : MonoBehaviour
{
    public static bool gas
    {
        get;
        private set;
    }
    private GarageCoinMakerType currentCoinMakerType;
    private Transform GarageUpgradePurchasePanel
    {
        get { return transform.GetChild(0).GetChild(5); }
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
        Engine.RestartLevel();
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
    public void CoinMakerUpgradeRequest(GarageCoinMakerType type)
    {
        currentCoinMakerType = type;
        GarageUpgradePurchasePanel.GetChild(0).GetComponent<Image>().sprite =
            Sprite.Create(Engine.meta.Garage.GetCoinMaker(type).futureTexture
            , new Rect(0.0f, 0.0f, Engine.meta.Garage.GetCoinMaker(type).futureTexture.width, Engine.meta.Garage.GetCoinMaker(type).futureTexture.height)
            , new Vector2(0.5f, 0.5f)
            , 100f);
        GarageUpgradePurchasePanel.GetChild(1).GetChild(0).GetComponent<Text>().text = Engine.meta.Garage.GetCoinMaker(type).updatePrice.ToString();
        if (Engine.meta.coinsCount < Engine.meta.Garage.GetCoinMaker(type).updatePrice)
        {
            GarageUpgradePurchasePanel.GetChild(1).GetChild(0).GetComponent<Text>().color = Color.red;
            GarageUpgradePurchasePanel.GetChild(2).GetChild(0).GetComponent<Text>().text = "Play to buy!";
        }
        else
        {
            GarageUpgradePurchasePanel.GetChild(1).GetChild(0).GetComponent<Text>().color = Color.white;
            GarageUpgradePurchasePanel.GetChild(2).GetChild(0).GetComponent<Text>().text = "Upgrade";
        }
        GarageUpgradePurchasePanel.gameObject.SetActive(true);
    }
    public void CancelCoinMakerUpgrade()
    {
        GarageUpgradePurchasePanel.gameObject.SetActive(false);
    }
    public void PurchaseCoinMakerUpgrade()
    {
        GarageUpgradePurchasePanel.gameObject.SetActive(false);
        if (Engine.meta.coinsCount < Engine.meta.Garage.GetCoinMaker(currentCoinMakerType).updatePrice)
            Play();
        else
            Engine.meta.Garage.UpgradeCoinMaker(currentCoinMakerType);
    }
}