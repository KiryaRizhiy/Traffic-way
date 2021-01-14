using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TVSetController : MonoBehaviour
{
    private UserInteraction UI
    {
        get { return transform.parent.parent.parent.GetComponent<UserInteraction>(); }
    }
    private Transform loader
    {
        get
        {
            return transform.GetChild(0);
        }
    }
    private Transform playIcon
    {
        get
        {
            return transform.GetChild(1);
        }
    }
    private Transform coinIcon
    {
        get
        {
            return transform.GetChild(2);
        }
    }
    private Sequence loaderSequence;
    private Sequence playIconSequence;
    private const float animationHalfDuration = 0.7f;
    //0.4-0.8
    void Awake()
    {
        Engine.Events.initialized += Draw;
        Engine.Events.adFailed += HandleAdEvent;
        Engine.Events.adFinished += HandleAdEvent;
        Engine.Events.adLoaded += HandleAdEvent;
        Engine.Events.adNotReady += HandleAdEvent;
        Engine.Events.adSkipped += HandleAdEvent;
        Engine.Events.timeEventOccured += HandleTimeEvent;
        if (Engine.initialized)
            Draw();
        loaderSequence = DOTween.Sequence()
            .Append(loader.GetComponent<RectTransform>().DORotate(Vector3.forward * -180, animationHalfDuration))
            .Append(loader.GetComponent<RectTransform>().DORotate(Vector3.forward * -360, animationHalfDuration))
            .SetLoops(-1);
        playIconSequence = DOTween.Sequence()
            .Append(playIcon.GetComponent<RawImage>().DOFade(0.5f, 0.2f))
            .Join(playIcon.DOScale(0.95f, 0.2f))
            .Append(playIcon.GetComponent<RawImage>().DOFade(0.7f, 0.2f))
            .Join(playIcon.DOScale(1.05f, 0.2f))
            .SetLoops(-1);
    }
    void OnDestroy()
    {
        Engine.Events.initialized -= Draw;
        Engine.Events.adFailed -= HandleAdEvent;
        Engine.Events.adFinished -= HandleAdEvent;
        Engine.Events.adLoaded -= HandleAdEvent;
        Engine.Events.adNotReady -= HandleAdEvent;
        Engine.Events.adSkipped -= HandleAdEvent;
        Engine.Events.timeEventOccured -= HandleTimeEvent;
        loaderSequence.Complete();
        playIconSequence.Complete();
    }

    public void Tap()
    {
        if (Engine.meta.garage.tvSetState == TVSetState.ReadyToWatch)
        {
            AdMobController.ShowRewardedAd();
            SwitchToRewardAwaitMode();
        }
    }
    private void HandleTimeEvent(string EventName)
    {
        if (EventName == "tvSetReady")
            Draw();
    }
    private void HandleAdEvent(PlacementType type)
    {
        Draw();
    }
    private void SuccessfullAdDemo(PlacementType Type)
    {
        if (Type == PlacementType.rewardedVideo)
        {
            Engine.meta.garage.TVWatched();
            DOTween.Sequence()
                .Append(
                DOTween.Sequence()
                    .AppendCallback(SendACoin)
                    .AppendInterval(0.05f)
                    .SetLoops(Settings.TVWatchReward))
                .AppendCallback(SwitchToNormalMode);
        }
    }
    private void UnsuccessfullAdDemo(PlacementType Type)
    {
        if (Type == PlacementType.rewardedVideo)
        {
            SwitchToNormalMode();
        }
    }
    private void SwitchToRewardAwaitMode()
    {
        Engine.Events.adFailed -= HandleAdEvent;
        Engine.Events.adFinished -= HandleAdEvent;
        Engine.Events.adSkipped -= HandleAdEvent;


        Engine.Events.adFailed += UnsuccessfullAdDemo;
        Engine.Events.adFinished += SuccessfullAdDemo;
        Engine.Events.adSkipped += UnsuccessfullAdDemo;
    }
    private void SwitchToNormalMode()
    {
        Engine.Events.adFailed -= UnsuccessfullAdDemo;
        Engine.Events.adFinished -= SuccessfullAdDemo;
        Engine.Events.adSkipped -= UnsuccessfullAdDemo;

        Engine.Events.adFailed += HandleAdEvent;
        Engine.Events.adFinished += HandleAdEvent;
        Engine.Events.adSkipped += HandleAdEvent;

        Draw();
    }
    private void Draw()
    {
        switch (Engine.meta.garage.tvSetState)
        {
            case TVSetState.VideoNotReady:
                gameObject.GetComponent<Button>().interactable = false;
                loader.gameObject.SetActive(true);
                playIcon.gameObject.SetActive(false);
                break;
            case TVSetState.ReadyToWatch:
                gameObject.GetComponent<Button>().interactable = true;
                loader.gameObject.SetActive(false);
                playIcon.gameObject.SetActive(true);
                break;
            case TVSetState.RecentlyWatched:
                gameObject.GetComponent<Button>().interactable = false;
                loader.gameObject.SetActive(false);
                playIcon.gameObject.SetActive(false);
                break;
        }
    }
    private void SendACoin()
    {
        float moveDuratoin = 0.4f + Random.Range(0.0f, 0.1f);
        float fadeTime = 0.05f;
        GameObject _coin;
        _coin = Instantiate(coinIcon.gameObject, UI.TopPanelCoinIcon, true);
        _coin.SetActive(true);
        DOTween.Sequence()
            .Append(_coin.transform.DOMove(UI.TopPanelCoinIcon.position, moveDuratoin))
            .Append(_coin.transform.DOScale(3, fadeTime))
            .Join(_coin.GetComponent<Image>().DOFade(0f, fadeTime))
            .AppendCallback(Engine.AddCoin)
            .AppendCallback(() => Destroy(_coin));
    }
}