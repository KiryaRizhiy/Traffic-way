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
    private Transform TVRewardPanel
    {
        get { return transform.GetChild(0).GetChild(6); }
    }
    private Transform TVAdsDemonstrationConfirmationPanel
    {
        get { return transform.GetChild(0).GetChild(7); }
    }
    private Transform NitroAdsDemonstrationConfirmationPanel
    {
        get { return transform.GetChild(0).GetChild(8); }
    }
    private Transform NitroRewardPanel
    {
        get { return transform.GetChild(0).GetChild(9); }
    }
    private Transform CarSelectPanel
    {
        get
        {
            return transform.GetChild(0).GetChild(10);
        }
    }
    //private Transform CarUnlockConfirmationPanel
    //{
    //    get
    //    {
    //        return CarSelectPanel.GetChild(2);
    //    }
    //}
    private Transform CarUnlockedPanel
    {
        get
        {
            return CarSelectPanel.GetChild(2);
        }
    }
    private Transform CurrentCarImage
    {
        get
        {
            return transform.GetChild(0).GetChild(1).GetChild(2);
        }
    }
    private Transform GDPRPanel
    {
        get
        {
            return transform.GetChild(1);
        }
    }
    public Transform BottomControls
    {
        get
        {
            return transform.GetChild(0).GetChild(1);
        }
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
    void Awake()
    {
        Engine.Events.initialized += ShowCurrentCar;
        Engine.Events.carAppearenceChanged += ShowCurrentCar;
        Engine.Events.initialized += HideGDPRPanel;
        HideGDPRPanel();
    }
    void Start()
    {
        ShowCurrentCar();
    }
    void OnDestroy()
    {
        Engine.Events.initialized -= ShowCurrentCar;
        Engine.Events.carAppearenceChanged -= ShowCurrentCar;
        Engine.Events.initialized -= HideGDPRPanel;
    }

    public void ShowCurrentCar()
    {
        if (gameObject.name == "Interface" && Engine.initialized)
            CurrentCarImage.GetComponent<Image>().sprite = Sprite.Create(
                Engine.CarsAppearences[Engine.meta.car.currentAppearenceNum],
                new Rect(0f, 0f, Engine.CarsAppearences[Engine.meta.car.currentAppearenceNum].width, Engine.CarsAppearences[Engine.meta.car.currentAppearenceNum].height),
                    Vector2.one * 0.5f);
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
    public void ToMainMenu()
    {
        Engine.ToMainMenu();
    }
    public void ShowCarSelectPanel()
    {
        CarSelectPanel.gameObject.SetActive(true);
        CarSelectPanel.GetComponent<CarSelectInterface>().Open();
        BottomControls.gameObject.SetActive(false);
    }
    public void ClearSaveFile()
    {
        Engine.ClearSaveFile();
    }
    public void CoinMakerUpgradeRequest(GarageCoinMakerType type)
    {
        currentCoinMakerType = type;
        if (!Engine.meta.garage.GetCoinMaker(type).updatable)
            return;
        GarageUpgradePurchasePanel.GetChild(0).GetComponent<Image>().sprite =
            Sprite.Create(Engine.meta.garage.GetCoinMaker(type).futureTexture
            , new Rect(0.0f, 0.0f, Engine.meta.garage.GetCoinMaker(type).futureTexture.width, Engine.meta.garage.GetCoinMaker(type).futureTexture.height)
            , new Vector2(0.5f, 0.5f)
            , 100f);
        GarageUpgradePurchasePanel.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = Engine.meta.garage.GetCoinMaker(type).updatePrice.ToString();
        if (Engine.meta.coinsCount < Engine.meta.garage.GetCoinMaker(type).updatePrice)
        {
            GarageUpgradePurchasePanel.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().color = Color.red;
            if (Engine.meta.garage.TVAbleToShow)
            {
                GarageUpgradePurchasePanel.GetChild(1).GetChild(0).GetComponent<Text>().text = Localization.GetLocal("WatchVideoForCoinsWhenNoMoneyToBuyUpgrade");
            }
            else
            {
                GarageUpgradePurchasePanel.GetChild(1).GetChild(0).GetComponent<Text>().text = Localization.GetLocal("PlayWhenNoMoneyToBuyUpgrade");
            }
        }
        else
        {
            GarageUpgradePurchasePanel.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().color = Color.white;
            GarageUpgradePurchasePanel.GetChild(1).GetChild(0).GetComponent<Text>().text = Localization.GetLocal("UpgradeGarageElementButton");
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
        if (Engine.meta.coinsCount < Engine.meta.garage.GetCoinMaker(currentCoinMakerType).updatePrice)
        {
            if (Engine.meta.garage.TVAbleToShow)
                ShowTVAds();
            else
                Play();
        }
        else
            Engine.meta.garage.UpgradeCoinMaker(currentCoinMakerType);
    }

    public void RequestConfirmNitroAdsDemonstration()
    {
        if (!Engine.meta.car.isBoosted /*&& AdMobController.isRewardedVideoReady*/ && AdMobController.isRewardedVideoReady)
            NitroAdsDemonstrationConfirmationPanel.gameObject.SetActive(true);
    }
    public void DeclineNitroAdsDemonstration()
    {
        NitroAdsDemonstrationConfirmationPanel.gameObject.SetActive(false);
    }
    public void ShowNitroAds()
    {
        NitroAdsDemonstrationConfirmationPanel.gameObject.SetActive(false);
        AdMobController.ShowRewardedAd();
        UIObjectActivator _act = gameObject.AddComponent<UIObjectActivator>();
        _act.TargetObject = NitroRewardPanel.gameObject;
        _act.DeactivationEventList = new List<UIObjectActivator.ActivatorTargetEvent>();
        _act.ActivationEventList = new List<UIObjectActivator.ActivatorTargetEvent>() { UIObjectActivator.ActivatorTargetEvent.adsRewardedVideoFinished };
        //NitroRewardPanel.gameObject.SetActive(true);
    }
    public void CollectNitroWatchReward()
    {
        Engine.meta.car.BoostOn();
        NitroRewardPanel.gameObject.SetActive(false);
        UIObjectActivator[] _aarr = GetComponents<UIObjectActivator>();
        foreach (UIObjectActivator _a in _aarr)
        {
            if (_a.TargetObject.name == NitroRewardPanel.name)
                Destroy(_a);
            else
                Debug.Log("This component has target " + _a.TargetObject.name + " , not " + NitroRewardPanel.name);
        }
    }

    public void DeclineCarAdsDemonstration()
    {
        //CarUnlockConfirmationPanel.gameObject.SetActive(false);
        CarSelectPanel.gameObject.SetActive(true);
        CarSelectPanel.GetComponent<CarSelectInterface>().Refresh();
    }
    public void ShowCarAds()
    {
        //CarUnlockConfirmationPanel.gameObject.SetActive(false);
        AdMobController.ShowRewardedAd();
        UIObjectActivator _act = gameObject.AddComponent<UIObjectActivator>();
        _act.TargetObject = CarUnlockedPanel.gameObject;
        _act.DeactivationEventList = new List<UIObjectActivator.ActivatorTargetEvent>();
        _act.ActivationEventList = new List<UIObjectActivator.ActivatorTargetEvent>() { UIObjectActivator.ActivatorTargetEvent.adsRewardedVideoFinished };
        _act.threadSwitchRequired = true;
        //CarUnlockedPanel.gameObject.SetActive(true);
    }
    public void CollectCarAdsWatchReward()
    {
        CarSelectInterface.UnlockAppearence();
        CarSelectPanel.GetComponent<CarSelectInterface>().Refresh();
        CarUnlockedPanel.gameObject.SetActive(false);
        UIObjectActivator[] _aarr = GetComponents<UIObjectActivator>();
        foreach (UIObjectActivator _a in _aarr)
        {
            if (_a.TargetObject.name == CarUnlockedPanel.name)
                Destroy(_a);
            else
                Debug.Log("This component has target " + _a.TargetObject.name + " , not " + CarUnlockedPanel.name);
        }
        CarSelectPanel.gameObject.SetActive(true);
        CarSelectPanel.GetComponent<CarSelectInterface>().Refresh();
    }

    public void RequestConfirmTVAdsDemonstration()
    {
        if (AdMobController.isRewardedVideoReady)
            TVAdsDemonstrationConfirmationPanel.gameObject.SetActive(true);
    }
    public void DeclineTVAdsDemonstration()
    {
        TVAdsDemonstrationConfirmationPanel.gameObject.SetActive(false);
    }
    public void ShowTVAds()
    {
        TVAdsDemonstrationConfirmationPanel.gameObject.SetActive(false);
        AdMobController.ShowRewardedAd();
        UIObjectActivator _act = gameObject.AddComponent<UIObjectActivator>();
        _act.TargetObject = TVRewardPanel.gameObject;
        _act.DeactivationEventList = new List<UIObjectActivator.ActivatorTargetEvent>();
        _act.ActivationEventList = new List<UIObjectActivator.ActivatorTargetEvent>() { UIObjectActivator.ActivatorTargetEvent.adsRewardedVideoFinished };
        //TVRewardPanel.gameObject.SetActive(true);
    }
    public void CollectTVWatchReward()
    {
        Engine.meta.garage.TVWatched();
        UIObjectActivator[] _aarr = GetComponents<UIObjectActivator>();
        foreach (UIObjectActivator _a in _aarr)
        {
            if (_a.TargetObject.name == TVRewardPanel.name)
                Destroy(_a);
            else
                Debug.Log("This component has target " + _a.TargetObject.name + " , not " + TVRewardPanel.name);
        }
        TVRewardPanel.gameObject.SetActive(false);
    }

    public void AcceptGDPR()
    {
        Engine.AcceptGDPR();
        HideGDPRPanel();
    }
    public void HideGDPRPanel()
    {
        if (Engine.initialized && GDPRPanel.name == "GDPRAccept")
            GDPRPanel.gameObject.SetActive(!Engine.meta.GDPRAccepted);
    }
}