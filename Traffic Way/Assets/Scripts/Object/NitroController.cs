using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class NitroController : MonoBehaviour
{
    private Engine.GameData.CarData data
    {
        get
        {
            return Engine.meta.car;
        }
    }
    private Transform statePanel
    {
        get
        {
            return transform.GetChild(0);
        }
    }
    private TextMeshProUGUI stateText
    {
        get
        {
            return statePanel.GetChild(1).GetComponent<TextMeshProUGUI>();
        }
    }
    private Transform lockPanel
    {
        get
        {
            return transform.GetChild(1);
        }
    }
    private Transform baloonsCountPanel
    {
        get
        {
            return transform.GetChild(3);
        }
    }
    public ParticleSystem sparkles
    {
        get
        {
            return transform.GetChild(2).GetComponent<ParticleSystem>();
        }
    }
    private TextMeshProUGUI baloonsCountText
    {
        get
        {
            return baloonsCountPanel.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        }
    }
    private UserInteraction UI
    {
        get { return transform.parent.parent.parent.GetComponent<UserInteraction>(); }
    }
    private Image baloonImage
    {
        get
        {
            return GetComponent<Image>();
        }
    }
    private Sequence baloonsCountZoom;
    private const float baloonsCountZoomDuration = 0.25f;

    private bool isUnderAnimation;
    // Start is called before the first frame update
    void Awake()
    {
        isUnderAnimation = false;
        Engine.Events.initialized += Draw;
        Engine.Events.onGarageStateChanged += GaragePurchaseHandler;
        if (Engine.initialized)
            Draw();
        baloonsCountZoom = DOTween.Sequence()
            .Append(baloonsCountPanel.DOScale(1.05f, baloonsCountZoomDuration))
            .Append(baloonsCountPanel.DOScale(1f, baloonsCountZoomDuration))
            .SetLoops(-1);
        baloonsCountZoom.Pause();
    }
    void OnDestroy()
    {
        Engine.Events.initialized -= Draw;
        Engine.Events.onGarageStateChanged -= GaragePurchaseHandler;
    }
    private void Draw()
    {
        if (isUnderAnimation)
            return;
        switch (data.nitroState)
        {
            case NitroState.Blocked:
                lockPanel.gameObject.SetActive(true);
                statePanel.gameObject.SetActive(false);
                baloonsCountPanel.gameObject.SetActive(false);
                baloonImage.color = CoinMaker.fadeColor;
                break;
            case NitroState.NotPurchased:
                lockPanel.gameObject.SetActive(false);
                statePanel.gameObject.SetActive(true);
                baloonsCountPanel.gameObject.SetActive(false);
                stateText.text = "BUY NITRO";
                baloonImage.color = CoinMaker.fadeColor;
                break;
            case NitroState.Available:
                lockPanel.gameObject.SetActive(false);
                statePanel.gameObject.SetActive(false);
                baloonsCountPanel.gameObject.SetActive(true);
                baloonImage.color = Color.white;
                baloonsCountZoom.Pause();
                baloonsCountText.text = data.availableNitroBottles.ToString();
                break;
            case NitroState.Empty:
                lockPanel.gameObject.SetActive(false);
                statePanel.gameObject.SetActive(false);
                baloonsCountPanel.gameObject.SetActive(true);
                baloonImage.color = Color.white;
                baloonsCountZoom.Play();
                baloonsCountText.text = data.availableNitroBottles.ToString();
                break;
        }
    }
    private void SwitchToRewardAwaitMode()
    {
        Engine.Events.adFailed += UnsuccessfullAdDemo;
        Engine.Events.adFinished += SuccessfullAdDemo;
        Engine.Events.adSkipped += UnsuccessfullAdDemo;
    }
    private void SwitchToNormalMode()
    {
        Engine.Events.adFailed -= UnsuccessfullAdDemo;
        Engine.Events.adFinished -= SuccessfullAdDemo;
        Engine.Events.adSkipped -= UnsuccessfullAdDemo;
        Draw();
    }
    private void SuccessfullAdDemo(PlacementType Type)
    {
        if (Type == PlacementType.rewardedVideo)
        {
            Engine.meta.car.AddBaloon();
            SwitchToNormalMode();
        }
    }
    private void UnsuccessfullAdDemo(PlacementType Type)
    {
        if (Type == PlacementType.rewardedVideo)
        {
            SwitchToNormalMode();
        }
    }

    public void BuyNitro()
    {
        if (data.nitroState == NitroState.NotPurchased)
        {
            Engine.meta.car.PurchaseNitro();
            Draw();
            UI.OpenNitroPanel();
        }
    }
    public void WatchForBaloon()
    {
        if (AdMobController.isRewardedVideoReady)
        {
            SwitchToRewardAwaitMode();
            AdMobController.ShowRewardedAd();
        }
    }
    public void GaragePurchaseHandler(GarageCoinMakerType Type)
    {
        Draw();
    }
}
