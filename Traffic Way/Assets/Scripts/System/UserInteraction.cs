using System;
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
    private CarUpgradeType currentCarUpgradeType;
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
    private Transform Garage
    {
        get
        {
            return transform.GetChild(0).GetChild(3);
        }
    }
    private Transform TVSet
    {
        get
        {
            return Garage.GetChild(0);
        }
    }
    private Transform GarageElementUpgradePanel
    {
        get
        {
            return transform.GetChild(0).GetChild(0).GetChild(1);
        }
    }
    private Transform GarageElementUpgradeText
    {
        get
        {
            return GarageElementUpgradePanel.GetChild(1);
        }
    }
    private Transform CarUpgradeUpgradePanel
    {
        get
        {
            return transform.GetChild(0).GetChild(0).GetChild(2);
        }
    }
    private Transform CarUpgradeText
    {
        get
        {
            return CarUpgradeUpgradePanel.GetChild(1);
        }
    }
    private Transform CarUpgradeBuyButton
    {
        get
        {
            return CarUpgradeUpgradePanel.GetChild(2);
        }
    }
    private Transform CarUpgradeBuyButtonPrice
    {
        get
        {
            return CarUpgradeBuyButton.GetChild(0).GetChild(0);
        }
    }
    private Transform CarUpgradeGarageUpgradeButton
    {
        get
        {
            return CarUpgradeUpgradePanel.GetChild(3);
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
            return Garage.GetChild(6);
        }
    }
    private Transform ToolsWallPanel
    {
        get
        {
            return Garage.GetChild(3);
        }
    }
    private Transform MaterialsPedestalPanel
    {
        get
        {
            return Garage.GetChild(1);
        }
    }
    private Transform MaterialsShelfPanel
    {
        get
        {
            return Garage.GetChild(2);
        }
    }
    private Transform WorkbenchPanel
    {
        get
        {
            return Garage.GetChild(4);
        }
    }
    private Transform CarUpgradePanel
    {
        get
        {
            return transform.GetChild(0).GetChild(10);
        }
    }
    private Transform CarUpgradeBrakes
    {
        get
        {
            return CarUpgradePanel.GetChild(0).GetChild(0).GetChild(1);
        }
    }
    private Transform CarUpgradeGearbox
    {
        get
        {
            return CarUpgradePanel.GetChild(0).GetChild(0).GetChild(2);
        }
    }
    private Transform CarUpgradeEngine
    {
        get
        {
            return CarUpgradePanel.GetChild(0).GetChild(0).GetChild(3);
        }
    }
    private Transform CarUpgradeBrakesInterface
    {
        get
        {
            return CarUpgradePanel.GetChild(0).GetChild(2);
        }
    }
    private Transform CarUpgradeGearboxInterface
    {
        get
        {
            return CarUpgradePanel.GetChild(0).GetChild(3);
        }
    }
    private Transform CarUpgradeEngineInterface
    {
        get
        {
            return CarUpgradePanel.GetChild(0).GetChild(1);
        }
    }
    private Transform NitroPanel
    {
        get
        {
            return transform.GetChild(0).GetChild(0).GetChild(3);
        }
    }
    private Transform NitroPanelBuyButton
    {
        get
        {
            return NitroPanel.GetChild(2);
        }
    }
    private Transform NitroPanelBuyButtonPrice
    {
        get
        {
            return NitroPanelBuyButton.GetChild(0).GetChild(0);
        }
    }
    private Transform NitroPanelWatchButton
    {
        get
        {
            return NitroPanel.GetChild(3);
        }
    }
    private Transform NitroPanelText
    {
        get
        {
            return NitroPanel.GetChild(1);
        }
    }
    private Transform SpeedometerMask
    {
        get
        {
            return transform.GetChild(0).GetChild(1).GetChild(7).GetChild(0);
        }
    }
    private Transform SpeedometerImage
    {
        get
        {
            return SpeedometerMask.GetChild(0);
        }
    }
    private Transform GameplayNitroPurchasePanel
    {
        get
        {
            return transform.GetChild(0).GetChild(1).GetChild(9);
        }
    }
    private Transform GameplayNitroAlreadyPurchasedPanel
    {
        get
        {
            return transform.GetChild(0).GetChild(1).GetChild(10);
        }
    }
    public Transform BottomControls
    {
        get
        {
            return transform.GetChild(0).GetChild(0);
        }
    }
    public Transform TuningButton
    {
        get
        {
            return BottomControls.GetChild(0).GetChild(3);
        }
    }
    public Transform TuningButtonLockImage
    {
        get
        {
            return TuningButton.GetChild(0);
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
    private bool isGamePlayInactive
    {
        get
        {
            return transform.name != "GameplayInterface";
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
        if (!isGamePlayInactive)
        {
            if (CarDriver.CurrentCar.GetComponent<CarDriver>().mode == GameplayMode.Drive)
            {
                float gasPower = 0.012f + 0.035f * Mathf.Abs(Mathf.Sin(Time.time * 16));
                if (CarDriver.currentSpeed / Engine.maxPossibleSpeed > gasPower / 0.5f)
                    gasPower = 0.5f * (CarDriver.currentSpeed / Engine.maxPossibleSpeed);
                RectTransform _maskRect = SpeedometerMask.GetComponent<RectTransform>();
                RectTransform _imageRect = SpeedometerImage.GetComponent<RectTransform>();
                _maskRect.anchorMin = new Vector2(0.5f - gasPower, 0f);
                _maskRect.anchorMax = new Vector2(0.5f + gasPower, 1f);
                _imageRect.anchorMin = new Vector2(-(0.5f / (2 * gasPower) - 0.5f), 0f);
                _imageRect.anchorMax = new Vector2((0.5f / (2 * gasPower) + 0.5f), 1f);
            }
            else
            if (CarDriver.CurrentCar.GetComponent<CarDriver>().mode == GameplayMode.Puzzle)
            {
                SpeedometerMask.gameObject.SetActive(false);
            }
        }      
    }
    void Awake()
    {
        Engine.Events.initialized += OnInitialized;
        Engine.Events.carAppearenceChanged += ShowCurrentCar;
        Engine.Events.initialized += HideGDPRPanel;
        Engine.Events.timeEventOccured += TimeEventHandler;
        HideGDPRPanel();
    }
    void Start()
    {
        OnInitialized();
    }
    void OnDestroy()
    {
        Engine.Events.initialized -= OnInitialized;
        Engine.Events.carAppearenceChanged -= ShowCurrentCar;
        Engine.Events.initialized -= HideGDPRPanel;
        Engine.Events.timeEventOccured -= TimeEventHandler;
    }
    private void OnInitialized()
    {
        if (!isGamePlayInactive || !Engine.initialized)
            return;
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
        if (Engine.carTuningAvailable)
        {
            //Debug.LogWarning("Car tunung available");
            TuningButton.GetComponent<Button>().interactable = true;
            TuningButtonLockImage.gameObject.SetActive(false);
        }
        else
        {
            //Debug.LogWarning("Car tunung unavailable");
            TuningButton.GetComponent<Button>().interactable = false;
            TuningButtonLockImage.gameObject.SetActive(true);
        }
    }

    private void DemonstrateFirstGarageUpdate()
    {
        if (GarageFirstPurchaseDemonstration != null)
            return;
        ToolsWallPanel.GetChild(0).GetComponent<CoinMaker>().Draw();
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
        if (isGamePlayInactive)
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

    public void DemonstrateTuningUnlock()
    {
        //Debug.Log("Start tuning unlock demonstration");
        if (Engine.carTuningAvailable)
        {
            Button _bt = TuningButton.GetComponent<Button>();
            RawImage _lock = TuningButtonLockImage.GetComponent<RawImage>();
            RectTransform _lockRect = TuningButtonLockImage.GetComponent<RectTransform>();
            float _duration = 1.6f;
            _bt.image.color = _bt.colors.disabledColor;
            _bt.interactable = true;
            DOTween.Sequence()
                .AppendInterval(0.2f)
                .Append(_bt.image.DOColor(Color.white, _duration))
                .Join(_lock.DOFade(0f, _duration))
                .Join(_lockRect.DOAnchorMax(new Vector2(_lockRect.anchorMax.x, 1.3f), _duration))
                .Join(_lockRect.DOAnchorMin(new Vector2(_lockRect.anchorMin.x, 1.3f), _duration))
                .Join(TuningButtonLockImage.DOScale(2.5f, _duration))
                .AppendCallback(() => TuningButtonLockImage.gameObject.SetActive(false));
        }
        else
        {
            Debug.LogError("Invalid attempt to demonstrate tuning unlock while tuning still locked");
        }
    }

    public void CallCarUpgradeInterface()
    {
        CarUpgradePanel.gameObject.SetActive(true);
        CarUpgradeBrakes.gameObject.SetActive(false);
        CarUpgradeEngine.gameObject.SetActive(false);
        CarUpgradeGearbox.gameObject.SetActive(false);
        CarUpgradeBrakesInterface.GetComponent<CarUpgradeInterface>().Show();
        CarUpgradeEngineInterface.GetComponent<CarUpgradeInterface>().Show();
        CarUpgradeGearboxInterface.GetComponent<CarUpgradeInterface>().Show();
    }
    public void HideCarUpgradeInterface()
    {
        CarUpgradePanel.gameObject.SetActive(false);
        CarUpgradeUpgradePanel.gameObject.SetActive(false);
    }
    public void CarUpgradeUpgradeRequest(CarUpgradeType Type)
    {
        currentCarUpgradeType = Type;
        TextMeshProUGUI _txt = CarUpgradeText.GetComponent<TextMeshProUGUI>();
        Engine.GameData.CarUpgradeData data = Engine.meta.garage.GetCarUpgrade(currentCarUpgradeType);
        CarUpgradeBrakes.gameObject.SetActive(false);
        CarUpgradeEngine.gameObject.SetActive(false);
        CarUpgradeGearbox.gameObject.SetActive(false);
        switch (currentCarUpgradeType)
        {
            case CarUpgradeType.Engine:
                CarUpgradeEngine.gameObject.SetActive(true);
                break;
            case CarUpgradeType.Brakes:
                CarUpgradeBrakes.gameObject.SetActive(true);
                break;
            case CarUpgradeType.Gearbox:
                CarUpgradeGearbox.gameObject.SetActive(true);
                break;
        }
        switch (data.state)
        {
            case CarUpgradeState.Normal:
                CarUpgradeGarageUpgradeButton.gameObject.SetActive(false);
                CarUpgradeBuyButton.gameObject.SetActive(true);
                CarUpgradeBuyButtonPrice.GetComponent<TextMeshProUGUI>().text = data.updatePrice.ToString();
                if (Engine.meta.coinsCount < data.updatePrice)
                {
                    _txt.text = data.ToString()
                        + System.Environment.NewLine + "More coins required";
                    CarUpgradeBuyButton.GetComponent<Button>().interactable = false;
                }
                else
                {
                    _txt.text = data.ToString();
                    CarUpgradeBuyButton.GetComponent<Button>().interactable = true;
                }
                break;
            case CarUpgradeState.Blocked:
                _txt.text = data.ToString()
                    + System.Environment.NewLine + data.correspondentCoinMaker.ToString() + " upgrade required";
                CarUpgradeGarageUpgradeButton.gameObject.SetActive(true);
                CarUpgradeBuyButton.gameObject.SetActive(false);
                break;
            case CarUpgradeState.TopLevelReached:
                _txt.text = data.ToString()
                    + System.Environment.NewLine + "Top level reached!";
                CarUpgradeGarageUpgradeButton.gameObject.SetActive(false);
                CarUpgradeBuyButton.gameObject.SetActive(false);
                break;
        }
        CarUpgradeUpgradePanel.gameObject.SetActive(true);
    }
    public void HideCarUpgradeUpgradePanel()
    {
        CarUpgradeUpgradePanel.gameObject.SetActive(false);
        CarUpgradeBrakes.gameObject.SetActive(false);
        CarUpgradeEngine.gameObject.SetActive(false);
        CarUpgradeGearbox.gameObject.SetActive(false);
    }
    public void OpenGarageUpgrade()
    {
        HideCarUpgradeInterface();
        CoinMakerUpgradeRequest(Engine.meta.garage.GetCarUpgrade(currentCarUpgradeType).correnspondentType);
    }
    public void PurchaseCarUpgrade()
    {
        Engine.meta.garage.GetCarUpgrade(currentCarUpgradeType).Upgrade();
        HideCarUpgradeUpgradePanel();
    }

    public void CoinMakerUpgradeRequest(GarageCoinMakerType type)
    {
        currentCoinMakerType = type;
        if (Engine.meta.garage.GetCoinMaker(currentCoinMakerType).state == CoinMakerStates.MaxLevelReached)
            return;
        if (GarageFirstPurchaseDemonstration != null && type == GarageCoinMakerType.ToolsWall)
        {
            GarageFirstPurchaseDemonstration.Complete();
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
                    GarageElementUpgradeText.GetComponent<TextMeshProUGUI>().text =
                        Engine.meta.garage.GetCoinMaker(currentCoinMakerType).description
                        + System.Environment.NewLine
                        + "Not enough coins to purchase";
                    GarageElementUpgradePanel
                        .GetChild(3)
                        .GetChild(0)
                        .GetChild(0)
                        .GetComponent<TextMeshProUGUI>().color = Color.red;
                    if (Engine.meta.garage.tvSetState == TVSetState.ReadyToWatch)
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
                            .GetComponent<TextMeshProUGUI>().text = Localization.GetLocal("PlayWhenNoMoneyToBuyUpgradeButton");
                    }
                }
                else
                {
                    GarageElementUpgradeText.GetComponent<TextMeshProUGUI>().text =
                        Engine.meta.garage.GetCoinMaker(currentCoinMakerType).description;
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
                GarageElementUpgradeText.GetComponent<TextMeshProUGUI>().text = Engine.meta.garage.GetCoinMaker(currentCoinMakerType).ToString() + " is unpacking";
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
                GarageElementUpgradeText.GetComponent<TextMeshProUGUI>().text = Engine.meta.garage.GetCoinMaker(currentCoinMakerType).ToString() + " ready!";
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
            if (Engine.meta.garage.tvSetState == TVSetState.ReadyToWatch)
                TVSet.GetComponent<TVSetController>().Tap();
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
        if (Type == PlacementType.rewardedVideo && isGamePlayInactive)
        {
            CancelCoinMakerUpgrade();
            Engine.meta.garage.GetCoinMaker(currentCoinMakerType).DecreaseUnpackTimeByHour();
            Engine.Events.adFailed -= UnsuccesssfullUnpackTimeDecrease;
            Engine.Events.adFinished -= SuccessfullUnpackTimeDecrease;
            Engine.Events.adSkipped -= UnsuccesssfullUnpackTimeDecrease;
            Engine.Events.adUserLeave -= UnsuccesssfullUnpackTimeDecrease;
        }
    }
    public void UnsuccesssfullUnpackTimeDecrease(PlacementType Type)
    {
        if (Type == PlacementType.rewardedVideo && isGamePlayInactive)
        {
            CancelCoinMakerUpgrade();
            Engine.Events.adFailed -= UnsuccesssfullUnpackTimeDecrease;
            Engine.Events.adFinished -= SuccessfullUnpackTimeDecrease;
            Engine.Events.adSkipped -= UnsuccesssfullUnpackTimeDecrease;
            Engine.Events.adUserLeave -= UnsuccesssfullUnpackTimeDecrease;
        }
    }

    public void OpenNitroPanel()
    {
        switch (Engine.meta.car.nitroState)
        {
            case NitroState.Blocked:
                NitroPanel.gameObject.SetActive(false);
                break;
            case NitroState.NotPurchased:
                NitroPanel.gameObject.SetActive(true);
                if (Engine.meta.coinsCount < Settings.nitroCost)
                {
                    NitroPanelText.GetComponent<TextMeshProUGUI>().text = "Nitro boost provides extra speed"
                        + System.Environment.NewLine + "and protects a car with shield."
                        + System.Environment.NewLine + "Not enough coins";
                    NitroPanelBuyButton.GetComponent<Button>().interactable = false;
                }
                else
                {
                    NitroPanelText.GetComponent<TextMeshProUGUI>().text = "Nitro boost provides extra speed"
                        + System.Environment.NewLine + "and protects a car with shield.";
                    NitroPanelBuyButton.GetComponent<Button>().interactable = true;
                }
                NitroPanelBuyButtonPrice.GetComponent<TextMeshProUGUI>().text = Settings.nitroCost.ToString();
                NitroPanelWatchButton.gameObject.SetActive(false);
                NitroPanelBuyButton.gameObject.SetActive(true);
                break;
            case NitroState.Available:
                NitroPanel.gameObject.SetActive(true);
                NitroPanelWatchButton.gameObject.SetActive(true);
                NitroPanelBuyButton.gameObject.SetActive(false);
                NitroPanelText.GetComponent<TextMeshProUGUI>().text = "Nitro provides extra speed"
                    + System.Environment.NewLine + "and shield." + 
                    "Yoh have " + Engine.meta.car.availableNitroBottles.ToString() + (Engine.meta.car.availableNitroBottles > 1 ? " nitro baloons." : "nitro baloon");
                break;
            case NitroState.Empty:
                NitroPanel.gameObject.SetActive(true);
                NitroPanelWatchButton.gameObject.SetActive(true);
                NitroPanelBuyButton.gameObject.SetActive(false);
                NitroPanelText.GetComponent<TextMeshProUGUI>().text = "Nitro provides extra speed"
                    + System.Environment.NewLine + "and shield."
                    + System.Environment.NewLine + "Watch video to get baloon";
                break;
        }
    }
    public void CloseNitroPanel()
    {
        NitroPanel.gameObject.SetActive(false);
    }
    public void OpenGameplayNitroPanel()
    {
        Engine.SwitchPause();
        GameplayNitroPurchasePanel.gameObject.SetActive(true);
    }
    public void OpenGameplayNitroAlreadyPurchasedPanel()
    {
        GameplayNitroPurchasePanel.gameObject.SetActive(false);
        GameplayNitroAlreadyPurchasedPanel.gameObject.SetActive(true);
    }
    public void CloseGameplayNitroAplreadyPurchasedPanel()
    {
        Engine.SwitchPause();
        GameplayNitroAlreadyPurchasedPanel.gameObject.SetActive(false);
    }
    public void CloseAllGameplayNitroPanels()
    {
        Engine.SwitchPause();
        GameplayNitroPurchasePanel.gameObject.SetActive(true);
        GameplayNitroAlreadyPurchasedPanel.gameObject.SetActive(true);
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