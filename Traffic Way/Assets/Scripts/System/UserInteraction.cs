using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class UserInteraction : MonoBehaviour
{
    public static bool gas
    {
        get;
        private set;
    }
    private GarageCoinMakerType currentCoinMakerType;
    private Transform TVRewardPanel
    {
        get { return transform.GetChild(0).GetChild(6); }
    }
    private Transform TVAdsDemonstrationConfirmationPanel
    {
        get { return transform.GetChild(0).GetChild(6); }
    }
    private Transform NitroAdsDemonstrationConfirmationPanel
    {
        get { return transform.GetChild(0).GetChild(7); }
    }
    private Transform NitroRewardPanel
    {
        get { return transform.GetChild(0).GetChild(8); }
    }
    private Transform CarSelectPanel
    {
        get
        {
            return transform.GetChild(0).GetChild(9);
        }
    }
    private Transform VictoryPanel
    {
        get
        {
            return transform.GetChild(0).GetChild(2);
        }
    }
    private Transform VictoryPanelMainDataPanel
    {
        get
        {
            return VictoryPanel.GetChild(1);
        }
    }
    private Transform VictoryPanelTopControls
    {
        get
        {
            return VictoryPanel.GetChild(2);
        }
    }
    private Transform VictoryPanelBackground
    {
        get
        {
            return VictoryPanel.GetChild(0);
        }
    }
    private Transform GarageElementUpgradePanel
    {
        get
        {
            return transform.GetChild(0).GetChild(0).GetChild(1);
        }
    }
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
            return transform.GetChild(0).GetChild(0).GetChild(0).GetChild(2);
        }
    }
    private Transform GDPRPanel
    {
        get
        {
            return transform.GetChild(1);
        }
    }
    private Transform Gate
    {
        get
        {
            return transform.GetChild(0).GetChild(3).GetChild(6);
        }
    }
    private Transform ToolsWallPanel
    {
        get
        {
            return transform.GetChild(0).GetChild(3).GetChild(3);
        }
    }
    private Transform MaterialsPedestalPanel
    {
        get
        {
            return transform.GetChild(0).GetChild(3).GetChild(1);
        }
    }
    private Transform MaterialsShelfPanel
    {
        get
        {
            return transform.GetChild(0).GetChild(3).GetChild(2);
        }
    }
    private Transform WorkbenchPanel
    {
        get
        {
            return transform.GetChild(0).GetChild(3).GetChild(4);
        }
    }
    public Transform BottomControls
    {
        get
        {
            return transform.GetChild(0).GetChild(0);
        }
    }
    public Transform TopPanelCoinIcon
    {
        get
        {
            return transform.GetChild(0).GetChild(4).GetChild(0).GetChild(1);
        }
    }

    private Sequence GarageFirstPurchaseDemonstration;


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
        Engine.Events.timeEventOccured += TimeEventHandler;
        HideGDPRPanel();
    }
    void Start()
    {
        ShowCurrentCar();
        if (Engine.meta.GarageOpened)
        {
            RectTransform _rt = Gate.GetComponent<RectTransform>();
            _rt.DOAnchorMax(new Vector2(_rt.anchorMax.x, 2), 0.45f).SetDelay(0.05f);
            _rt.DOAnchorMin(new Vector2(_rt.anchorMin.x, 1), 0.45f).SetDelay(0.05f);
        }
        if (Engine.meta.GarageOpened && Engine.meta.FirstUpgradeNotPurchased)
        {
            DemonstrateFirstGarageUpdate();
        }
    }
    void OnDestroy()
    {
        Engine.Events.initialized -= ShowCurrentCar;
        Engine.Events.carAppearenceChanged -= ShowCurrentCar;
        Engine.Events.initialized -= HideGDPRPanel;
        Engine.Events.timeEventOccured -= TimeEventHandler;
    }

    private void DemonstrateFirstGarageUpdate()
    {
        ToolsWallPanel.GetChild(0).GetComponent<CoinMaker>().isUnderAnimation = true;
        ToolsWallPanel.GetComponent<CanvasGroup>().ignoreParentGroups = true;
        GarageFirstPurchaseDemonstration = DOTween.Sequence();
        GarageFirstPurchaseDemonstration
            .AppendInterval(0.6f)
            .Append(GetComponent<CanvasGroup>().DOFade(0.6f, 0.55f))
            .Join(ToolsWallPanel.GetChild(0).GetComponent<Image>().DOColor(Color.white, 0.55f))
            .Join(ToolsWallPanel.DOScale(1.1f, 0.3f))
            .Insert(0.9f,ToolsWallPanel.DOScale(1.05f, 0.25f))
            .Append(
                DOTween.Sequence()
                .Append(
                    DOTween.Sequence()
                        .Append(ToolsWallPanel.DOScale(1.1f, 0.2f))
                        .Append(ToolsWallPanel.DOScale(1.05f, 0.2f))
                        .SetLoops(4))
                .AppendInterval(2f)
                .SetLoops(3))
            .Append(ToolsWallPanel.DOScale(1f, 1.3f))
            .Join(GetComponent<CanvasGroup>().DOFade(1f, 1.3f))
            .Join(ToolsWallPanel.GetChild(0).GetComponent<Image>().DOColor(CoinMaker.fadeColor, 1.3f))
            .AppendCallback(() => ToolsWallPanel.GetChild(0).GetComponent<CoinMaker>().isUnderAnimation = false)
            .AppendCallback(() => ToolsWallPanel.GetComponent<CanvasGroup>().ignoreParentGroups = false);
    }
    public void TimeEventHandler(string EventName)
    {
        //Reloading coin maker upgrade panel on unpacking finish
        if (EventName == currentCoinMakerType.ToString() + "_unpackFinish" && GarageElementUpgradePanel.gameObject.activeInHierarchy)
            CoinMakerUpgradeRequest(currentCoinMakerType);
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
        if (!Engine.isBossFight)
            Engine.LevelDone();
        else
        {
            VictoryPanelMainDataPanel.GetComponent<CanvasGroup>().DOFade(0f, 0.5f);
            VictoryPanelTopControls.GetComponent<CanvasGroup>().DOFade(0f, 0.5f);
            VictoryPanelBackground.GetComponent<Image>().DOColor(Color.white, 0.5f).OnComplete(Engine.LevelDone);
        }
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
        if (Engine.meta.garage.GetCoinMaker(currentCoinMakerType).state == CoinMakerStates.MaxLevelReached)
            return;
        if (GarageFirstPurchaseDemonstration != null && type == GarageCoinMakerType.ToolsWall)
        {
            GarageFirstPurchaseDemonstration.Complete(true);
            ToolsWallPanel.GetChild(0).GetComponent<CoinMaker>().isUnderAnimation = true;
            ToolsWallPanel.GetComponent<CanvasGroup>().ignoreParentGroups = true;
            DOTween.Sequence()
                .Append(GetComponent<CanvasGroup>().DOFade(1f, 0.15f))
                .Join(ToolsWallPanel.GetChild(0).GetComponent<Image>().DOColor(CoinMaker.fadeColor, 0.15f))
                .Join(ToolsWallPanel.DOScale(1f, 0.15f))
                .AppendCallback(() => ToolsWallPanel.GetChild(0).GetComponent<CoinMaker>().isUnderAnimation = false)
                .AppendCallback(() => ToolsWallPanel.GetComponent<CanvasGroup>().ignoreParentGroups = false);
        }
        switch (Engine.meta.garage.GetCoinMaker(currentCoinMakerType).state)
        {
            case CoinMakerStates.Normal:
            case CoinMakerStates.NotPurchased:
                //Turn on purchase button
                GarageElementUpgradePanel
                    .GetChild(3)
                    .gameObject.SetActive(true);
                //Turn off upgrade button
                GarageElementUpgradePanel
                    .GetChild(2)
                    .gameObject.SetActive(false);
                //Turn off unpack button
                GarageElementUpgradePanel
                    .GetChild(4)
                    .gameObject.SetActive(false);
                //Set price
                GarageElementUpgradePanel
                    .GetChild(3)
                    .GetChild(0)
                    .GetChild(0) 
                    .GetComponent<TextMeshProUGUI>().text = Engine.meta.garage.GetCoinMaker(currentCoinMakerType).updatePrice.ToString();
                if (Engine.meta.coinsCount < Engine.meta.garage.GetCoinMaker(currentCoinMakerType).updatePrice)
                {
                    GarageElementUpgradePanel
                        .GetChild(3)
                        .GetChild(0)
                        .GetChild(0)
                        .GetComponent<TextMeshProUGUI>().color = Color.red;
                    if (Engine.meta.garage.TVAbleToShow)
                    {
                        GarageElementUpgradePanel
                            .GetChild(3)
                            .GetChild(0)
                            .GetComponent<TextMeshProUGUI>().text = Localization.GetLocal("WatchVideoForCoinsWhenNoMoneyToBuyUpgrade");
                    }
                    else
                    {
                        GarageElementUpgradePanel
                            .GetChild(3)
                            .GetChild(0)
                            .GetComponent<TextMeshProUGUI>().text = Localization.GetLocal("PlayWhenNoMoneyToBuyUpgrade");
                    }
                }
                else
                {
                    GarageElementUpgradePanel
                        .GetChild(3)
                        .GetChild(0)
                        .GetChild(0)
                        .GetComponent<TextMeshProUGUI>().color = Color.white;
                    GarageElementUpgradePanel
                        .GetChild(3)
                        .GetChild(0)
                        .GetComponent<TextMeshProUGUI>().text = Localization.GetLocal("UpgradeGarageElementButton");
                }
                break;
            case CoinMakerStates.Unpacking:
                //Turn off purchase button
                GarageElementUpgradePanel
                    .GetChild(3)
                    .gameObject.SetActive(false);
                //Turn on upgrade button
                GarageElementUpgradePanel
                    .GetChild(2)
                    .gameObject.SetActive(true);
                //Turn off unpack button
                GarageElementUpgradePanel
                    .GetChild(4)
                    .gameObject.SetActive(false);
                break;
            case CoinMakerStates.UnpackingFinished:
                //Turn off purchase button
                GarageElementUpgradePanel
                    .GetChild(3)
                    .gameObject.SetActive(false);
                //Turn off upgrade button
                GarageElementUpgradePanel
                    .GetChild(2)
                    .gameObject.SetActive(false);
                //Turn on unpack button
                GarageElementUpgradePanel
                    .GetChild(4)
                    .gameObject.SetActive(true);
                break;
        }
        GarageElementUpgradePanel.gameObject.SetActive(true);
    }
    public void CancelCoinMakerUpgrade()
    {
        GarageElementUpgradePanel.gameObject.SetActive(false);
    }
    public void PurchaseCoinMakerUpgrade()
    {
        GarageElementUpgradePanel.gameObject.SetActive(false);
        if (Engine.meta.coinsCount < Engine.meta.garage.GetCoinMaker(currentCoinMakerType).updatePrice)
        {
            if (Engine.meta.garage.TVAbleToShow)
                ShowTVAds();
            else
                Play();
        }
        else
            Engine.meta.garage.GetCoinMaker(currentCoinMakerType).StartUnpacking();
    }
    public void CollectCoinMakerUnpack()
    {
        GarageElementUpgradePanel.gameObject.SetActive(false);
        //Processing tools wall first upgrade
        if (currentCoinMakerType == GarageCoinMakerType.ToolsWall && Engine.meta.garage.GetCoinMaker(currentCoinMakerType).level == 0)
        {
            CoinMaker MaterialsPedestalCoinMaker = MaterialsPedestalPanel.GetChild(0).GetComponent<CoinMaker>();
            CoinMaker MaterialsShelfCoinMaker = MaterialsShelfPanel.GetChild(0).GetComponent<CoinMaker>();
            CoinMaker WorkbencCoinMaker = WorkbenchPanel.GetChild(0).GetComponent<CoinMaker>();
            float lockFadeDuration = 0.9f;
            float stateAppearInterval = 0.25f;
            float stateAppearDiration = 1.2f;

            MaterialsPedestalCoinMaker.isUnderAnimation = true;
            MaterialsShelfCoinMaker.isUnderAnimation = true;
            WorkbencCoinMaker.isUnderAnimation = true;

            MaterialsPedestalCoinMaker.statePanel.gameObject.SetActive(true);
            MaterialsShelfCoinMaker.statePanel.gameObject.SetActive(true);
            WorkbencCoinMaker.statePanel.gameObject.SetActive(true);

            MaterialsPedestalCoinMaker.statePanel.GetComponent<CanvasGroup>().alpha = 0;
            MaterialsShelfCoinMaker.statePanel.GetComponent<CanvasGroup>().alpha = 0;
            WorkbencCoinMaker.statePanel.GetComponent<CanvasGroup>().alpha = 0;

            DOTween.Sequence()
                .AppendInterval(0.05f)
                .Append(MaterialsPedestalPanel.GetChild(1).DOScale(2f, lockFadeDuration))
                .Join(MaterialsPedestalPanel.GetChild(1).GetComponent<CanvasGroup>().DOFade(0, lockFadeDuration))
                .Insert(stateAppearInterval, MaterialsPedestalCoinMaker.statePanel.GetComponent<CanvasGroup>().DOFade(1f, stateAppearDiration));
            DOTween.Sequence()
                .Append(MaterialsShelfPanel.GetChild(1).DOScale(2f, lockFadeDuration))
                .Join(MaterialsShelfPanel.GetChild(1).GetComponent<CanvasGroup>().DOFade(0, lockFadeDuration))
                .Insert(stateAppearInterval, MaterialsShelfCoinMaker.statePanel.GetComponent<CanvasGroup>().DOFade(1f, stateAppearDiration));
            DOTween.Sequence()
                .AppendInterval(0.1f)
                .Append(WorkbenchPanel.GetChild(1).DOScale(2f, lockFadeDuration))
                .Join(WorkbenchPanel.GetChild(1).GetComponent<CanvasGroup>().DOFade(0, lockFadeDuration))
                .Insert(stateAppearInterval, WorkbencCoinMaker.statePanel.GetComponent<CanvasGroup>().DOFade(1f, stateAppearDiration))
                .AppendCallback(Engine.meta.garage.GetCoinMaker(currentCoinMakerType).Unpack)
                .AppendCallback(CoinMakersUnlockDemonstrated);
        }
        else
        {
            Engine.meta.garage.GetCoinMaker(currentCoinMakerType).Unpack();
        }
    }
    public void CoinMakersUnlockDemonstrated()
    {
        MaterialsPedestalPanel.GetChild(0).GetComponent<CoinMaker>().isUnderAnimation = false;
        MaterialsShelfPanel.GetChild(0).GetComponent<CoinMaker>().isUnderAnimation = false;
        WorkbenchPanel.GetChild(0).GetComponent<CoinMaker>().isUnderAnimation = false;
    }
    public void ShowAdsToDecreaseUnpackTime()
    {
        if (AdMobController.isRewardedVideoReady)
        {
            SubscribeAdEvents();
            AdMobController.ShowRewardedAd();
        }
    }
    public void SubscribeAdEvents()
    {
        Engine.Events.adFailed += UnsuccesssfullUnpackTimeDecrease;
        Engine.Events.adFinished += SuccessfullUnpackTimeDecrease;
        Engine.Events.adSkipped += UnsuccesssfullUnpackTimeDecrease;
        Engine.Events.adUserLeave += UnsuccesssfullUnpackTimeDecrease;
    }
    public void SuccessfullUnpackTimeDecrease(PlacementType Type)
    {
        CancelCoinMakerUpgrade();
        Engine.meta.garage.GetCoinMaker(currentCoinMakerType).DecreaseUnpackTimeByHour();
        Engine.Events.adFailed -= UnsuccesssfullUnpackTimeDecrease;
        Engine.Events.adFinished -= SuccessfullUnpackTimeDecrease;
        Engine.Events.adSkipped -= UnsuccesssfullUnpackTimeDecrease;
        Engine.Events.adUserLeave -= UnsuccesssfullUnpackTimeDecrease;
    }
    public void UnsuccesssfullUnpackTimeDecrease(PlacementType Type)
    {
        CancelCoinMakerUpgrade();
        Engine.Events.adFailed -= UnsuccesssfullUnpackTimeDecrease;
        Engine.Events.adFinished -= SuccessfullUnpackTimeDecrease;
        Engine.Events.adSkipped -= UnsuccesssfullUnpackTimeDecrease;
        Engine.Events.adUserLeave -= UnsuccesssfullUnpackTimeDecrease;
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