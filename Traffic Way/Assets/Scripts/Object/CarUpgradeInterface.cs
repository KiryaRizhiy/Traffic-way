using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class CarUpgradeInterface : MonoBehaviour
{

    public static Texture2D wrench;
    public static Texture2D lockIcon;
    public static Texture2D topLevelIcon;

    private Engine.GameData.CarUpgradeData data
    {
        get
        {
            return Engine.meta.garage.GetCarUpgrade(type);
        }
    }
    private TextMeshProUGUI stateText
    {
        get
        {
            return statePanel.GetChild(1).GetComponent<TextMeshProUGUI>();
        }
    }
    private UserInteraction UI
    {
        get
        {
            return transform.parent.parent.parent.parent.GetComponent<UserInteraction>();
        }
    }
    private RawImage stateImage
    {
        get
        {
            return statePanel.GetChild(0).GetComponent<RawImage>();
        }
    }
    private Transform statePanel
    {
        get
        {
            return transform.GetChild(0).GetChild(2);
        }
    }
    private float stateIconFadeDuration = 0.7f;

    public static void LoadResources()
    {
        wrench = Resources.Load<Texture2D>("TrafficWay/Textures/Interface/T_14_wrench_");
        lockIcon = Resources.Load<Texture2D>("TrafficWay/Textures/Interface/T_21_doorlock_close_");
        topLevelIcon = Resources.Load<Texture2D>("TrafficWay/Textures/Interface/T_12_ok_");
    }

    public CarUpgradeType type;
    void Awake()
    {
        Engine.Events.initialized += Draw;
        Engine.Events.onGarageStateChanged += GaragePurchaseHandler;
        Engine.Events.onCarUpgradeStateChanged += CarUpgradeUpgradeHandler;
    }
    void Start()
    {
        Draw();
    }
    void OnDestroy()
    {
        Engine.Events.initialized -= Draw;
        Engine.Events.onGarageStateChanged -= GaragePurchaseHandler;
        Engine.Events.onCarUpgradeStateChanged -= CarUpgradeUpgradeHandler;
    }

    public void Show()
    {
        if (data.state == CarUpgradeState.Normal && !data.unlockDemonstrated)
        {
            stateImage.texture = lockIcon;
            UpdateStatePanel(wrench);
            data.UnlockDemonstrated();
        }
    }
    private void Draw()
    {
        stateText.text = "Level " + data.level;
        switch (data.state)
        {
            case CarUpgradeState.Normal:
                stateImage.texture = wrench;
                break;
            case CarUpgradeState.Blocked:
                stateImage.texture = lockIcon;
                break;
            case CarUpgradeState.TopLevelReached:
                stateImage.texture = topLevelIcon;
                break;
        }
    }
    public void CallUpdateInterface()
    {
        UI.CarUpgradeUpgradeRequest(type);
    }
    public void GaragePurchaseHandler(GarageCoinMakerType Type)
    {
        if (data.correspondentCoinMaker.type == Type)
            Draw();
    }
    public void CarUpgradeUpgradeHandler(CarUpgradeType Type)
    {
        if (Type == type)
            switch (data.state)
            {
                case CarUpgradeState.Normal:
                    UpdateStatePanel(wrench);
                    break;
                case CarUpgradeState.Blocked:
                    UpdateStatePanel(lockIcon);
                    break;
                case CarUpgradeState.TopLevelReached:
                    UpdateStatePanel(topLevelIcon);
                    break;
            }
    }
    private void UpdateStatePanel(Texture2D NewTexture)
    {
        GameObject newImg = Instantiate(stateImage.gameObject, statePanel);
        newImg.transform.GetComponent<RawImage>().texture = NewTexture;
        stateImage.transform.SetAsLastSibling();
        newImg.transform.SetAsFirstSibling();
        Sequence _sq =
        DOTween.Sequence()
            .Append(statePanel.GetChild(2).GetComponent<RawImage>().DOFade(0f, stateIconFadeDuration))
            .Join(statePanel.GetChild(2).DOScale(2f, stateIconFadeDuration))
            .AppendCallback(() => Destroy(statePanel.GetChild(2).gameObject))
            .AppendCallback(Draw);
    }
}
