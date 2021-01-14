using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CoinMaker : MonoBehaviour
{
    public static Texture2D wrench;
    public static Texture2D clock;
    public static Texture2D upArrow;
    public static Color fadeColor
    {
        get
        {
            return new Color(67f / 255f, 67f / 255f, 67f / 255f, 67f / 255f);
        }
    }
    [SerializeField]
    public GarageCoinMakerType type;
    public bool isUnderAnimation;
    private float stateIconFadeDuration = 0.7f;
    private UserInteraction UI
    {
        get { return transform.parent.parent.parent.parent.GetComponent<UserInteraction>(); }
    }
    private Engine.GameData.CoinMakerData data
    {
        get
        {
            return Engine.meta.garage.GetCoinMaker(type);
        }
    }
    private CoinMakerStates currentState
    {
        get
        { return Engine.meta.garage.GetCoinMaker(type).state; }
    }
    private CoinMakerStates savedState;
    private int savedLevel;
    private RawImage stateIcon
    {
        get
        {
            return statePanel.GetChild(0).GetComponent<RawImage>();
        }
    }
    private TextMeshProUGUI stateText
    {
        get
        {
            return statePanel.GetChild(1).GetComponent<TextMeshProUGUI>();
        }
    }
    public Transform statePanel
    {
        get
        {
            return transform.GetChild(2);
        }
    }
    public Transform lockPanel
    {
        get
        {
            return transform.parent.GetChild(1);
        }
    }
    public Transform coinIcon
    {
        get
        {
            return transform.GetChild(1);
        }
    }
    public ParticleSystem sparkles
    {
        get
        {
            return transform.parent.GetChild(2).GetComponent<ParticleSystem>();
        }
    }
    public ParticleSystem miniSparkles
    {
        get
        {
            return statePanel.GetChild(1).GetChild(0).GetComponent<ParticleSystem>();
        }
    }
    public static void LoadResources()
    {
        wrench = Resources.Load<Texture2D>("TrafficWay/Textures/Interface/T_14_wrench_");
        clock = Resources.Load<Texture2D>("TrafficWay/Textures/Interface/T_26_clock_");
        upArrow = Resources.Load<Texture2D>("TrafficWay/Textures/Interface/T_9_arrow_rounded_up_");
    }
    void Awake()
    {
        Engine.Events.initialized += Stabilize;
        Engine.Events.onGarageStateChanged += GaragePurchaseHandler;
        Engine.Events.timeEventOccured += TimeEventHandler;
        if (Engine.initialized)
        {
            savedState = currentState;
            savedLevel = data.level;
            Draw();
        }
    }
    void OnDestroy()
    {
        Engine.Events.initialized -= Draw;
        Engine.Events.onGarageStateChanged -= GaragePurchaseHandler;
        Engine.Events.timeEventOccured -= TimeEventHandler;
    }
    private void ShowRestOfUnpackingTime()
    {
        stateText.text = String.Format("{0}:{1}:{2}",
            (int)Engine.meta.garage.GetCoinMaker(type).unpackTimeLeft.TotalHours,
            Engine.meta.garage.GetCoinMaker(type).unpackTimeLeft.Minutes,
            Engine.meta.garage.GetCoinMaker(type).unpackTimeLeft.Seconds);
    }
    public void Stabilize()
    {
        savedState = currentState;
        savedLevel = data.level;
        Draw();
    }
    public void Draw()
    {
        try
        {
            if (isUnderAnimation)
                return;
            switch (currentState)
            {
                case CoinMakerStates.UnpackingFinished:
                    stateIcon.texture = upArrow;
                    stateText.text = "OPEN";
                    break;
                case CoinMakerStates.Unpacking:
                    ShowRestOfUnpackingTime();
                    stateIcon.texture = clock;
                    break;
                case CoinMakerStates.MaxLevelReached:
                    statePanel.gameObject.SetActive(false);
                    break;
                default:
                    stateIcon.texture = wrench;
                    stateText.text = "UP to lvl " + (Engine.meta.garage.GetCoinMaker(type).level + 1);
                    break;
            }
            if (currentState == CoinMakerStates.Blocked)
            {
                lockPanel.gameObject.SetActive(true);
                statePanel.gameObject.SetActive(false);
            }
            else
            {
                lockPanel.gameObject.SetActive(false);
                statePanel.gameObject.SetActive(true);
            }
            if (Engine.meta.garage.GetCoinMaker(type).level == 0)
            {
                GetComponent<Image>().color = fadeColor;
            }
            else
            {
                GetComponent<Image>().color = Color.white;
            }            
            if (Engine.meta.garage.GetCoinMaker(type).currentProfit == 0)
            {
                transform.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                transform.GetChild(1).gameObject.SetActive(true);
            }
        }
        catch (System.Exception e)
        {
            Logger.AddContent(UILogDataType.Init,"Coin maker " + type.ToString() + " exception " + e.Message + Environment.NewLine + "trace: " + e.StackTrace);
            Debug.LogError(e);
        }
    }
    public void TimeEventHandler(string EventName)
    {
        if (EventName == type.ToString() + "_unpackFinish")
        {
            Debug.Log("Processing time event");
            GaragePurchaseHandler(type);
        }
    }
    public void GaragePurchaseHandler(GarageCoinMakerType Type)
    {
        if (Type == type)
        {
            //Start unpacking
            if (currentState == CoinMakerStates.Unpacking && savedState != CoinMakerStates.Unpacking)
            {
                UpdateStatePanel(clock);
                return;
            }
            //Unpacking finished
            if (currentState == CoinMakerStates.UnpackingFinished && savedState != CoinMakerStates.UnpackingFinished)
            {
                UpdateStatePanel(upArrow);
                return;
            }
            //Player unpacked
            if(currentState == CoinMakerStates.Normal && savedState != CoinMakerStates.Normal)
            {                
                isUnderAnimation = true;
                UpdateStatePanel(wrench);
                Sequence _sq = DOTween.Sequence()
                    .Append(GetComponent<RawImage>().DOColor(Color.white, 0.4f))
                    .AppendCallback(() =>
                    { if (data.level == 1) sparkles.Play(); else miniSparkles.Play(); })
                    .AppendCallback(()=>isUnderAnimation = false)
                    .AppendCallback(Stabilize);
                if (Engine.firstGarageUpgradeUnpacked)
                    _sq.AppendCallback(UI.DemonstrateTuningUnlock);
                //else
                //    Debug.Log("No reason to demonstrate tuning unlock");
                return;
            }
            Draw();
        }
    }
    public void CoinMakerTap()
    {
        if (data.currentProfit == 0)
        {
            CallUpdateInterface();
        }
        else
        {
            isUnderAnimation = true;
            coinIcon.GetComponent<InterfaceAnimation>().currentAnimation.Complete();
            coinIcon.GetComponent<InterfaceAnimation>().enabled = false;
            coinIcon.gameObject.SetActive(false);
            DOTween.Sequence()
                .AppendCallback(SendACoin)
                .AppendInterval(0.05f)
                .SetLoops(data.currentProfit);
        }
    }
    public void CallUpdateInterface()
    {
        if (currentState != CoinMakerStates.Blocked && currentState != CoinMakerStates.MaxLevelReached)
            UI.CoinMakerUpgradeRequest(type);
        else
            Debug.Log("Cant upgrade " + type.ToString() + ". reason: " + currentState.ToString());
    }
    private void UpdateStatePanel(Texture2D NewTexture)
    {
        isUnderAnimation = true;
        GameObject newImg = Instantiate(statePanel.GetChild(0).gameObject, statePanel);
        newImg.transform.GetComponent<RawImage>().texture = NewTexture;
        stateIcon.transform.SetAsLastSibling();
        newImg.transform.SetAsFirstSibling();
        Sequence _sq =
        DOTween.Sequence()
            .Append(statePanel.GetChild(2).GetComponent<RawImage>().DOFade(0f, stateIconFadeDuration))
            .Join(statePanel.GetChild(2).DOScale(2f, stateIconFadeDuration))
            .AppendCallback(()=>Destroy(statePanel.GetChild(2).gameObject))
            .AppendCallback(()=> isUnderAnimation = false)
            .AppendCallback(Stabilize);
    }
    private void SendACoin()
    {
        float moveDuratoin = 0.4f + UnityEngine.Random.Range(0.0f, 0.1f);
        float fadeTime = 0.05f;
        GameObject _coin;
        _coin = Instantiate(coinIcon.gameObject, UI.TopPanelCoinIcon, true);
        _coin.SetActive(true);
        DOTween.Sequence()
            .Append(_coin.transform.DOMove(UI.TopPanelCoinIcon.position, moveDuratoin))
            .Append(_coin.transform.DOScale(3, fadeTime))
            .Join(_coin.GetComponent<Image>().DOFade(0f, fadeTime))
            .AppendCallback(data.CollectACoin)
            .AppendCallback(() => Destroy(_coin))
            .AppendCallback(() => 
                { if (data.currentProfit == 0) 
                    isUnderAnimation = false; });
    }
    void Update()
    {
        if (Engine.meta.garage.GetCoinMaker(type).state == CoinMakerStates.UnpackingFinished)
            Draw();
        if (Engine.meta.garage.GetCoinMaker(type).state == CoinMakerStates.Unpacking)
            ShowRestOfUnpackingTime();
        if (Engine.meta.garage.GetCoinMaker(type).level == 0)
            return;
        if (Engine.meta.garage.GetCoinMaker(type).currentProfit / Engine.meta.garage.GetCoinMaker(type).profitRate == Settings.paidTicksLimit)
            return;
        if (DateTime.UtcNow <=
            new DateTime(Engine.meta.garage.GetCoinMaker(type).lastCoinCollect)
            + new TimeSpan(0, ((Engine.meta.garage.GetCoinMaker(type).currentProfit / Engine.meta.garage.GetCoinMaker(type).profitRate) + 1) * Settings.coinMakerTickMinutes, 0))
            Draw();
    }
}