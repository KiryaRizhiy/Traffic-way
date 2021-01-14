using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RewardedLoader : MonoBehaviour
{
    public GameObject loader;
    public GameObject activeOnLoadedObject;

    private Sequence _rotationSequence;
    private const float rotationHalfDuration = 0.7f;
    void Awake()
    {
        Engine.Events.adFailed += AnyAdsEventHandler;
        Engine.Events.adFinished += AnyAdsEventHandler;
        Engine.Events.adLoaded += AnyAdsEventHandler;
        Engine.Events.adNotReady += AnyAdsEventHandler;
        Engine.Events.adOpened += AnyAdsEventHandler;
        Engine.Events.adSkipped += AnyAdsEventHandler;
        Engine.Events.adUserLeave += AnyAdsEventHandler;
        Engine.Events.initialized += () => AnyAdsEventHandler(PlacementType.rewardedVideo);
        if (Engine.initialized)
            AnyAdsEventHandler(PlacementType.rewardedVideo);
        _rotationSequence = DOTween.Sequence()
            .Append(loader.GetComponent<RectTransform>().DORotate(Vector3.forward * -180, rotationHalfDuration))
            .Append(loader.GetComponent<RectTransform>().DORotate(Vector3.forward * -360, rotationHalfDuration))
            .SetLoops(-1);
    }
    private void OnDestroy()
    {
        Engine.Events.adFailed -= AnyAdsEventHandler;
        Engine.Events.adFinished -= AnyAdsEventHandler;
        Engine.Events.adLoaded -= AnyAdsEventHandler;
        Engine.Events.adNotReady -= AnyAdsEventHandler;
        Engine.Events.adOpened -= AnyAdsEventHandler;
        Engine.Events.adSkipped -= AnyAdsEventHandler;
        Engine.Events.adUserLeave -= AnyAdsEventHandler;
        Engine.Events.initialized -= () => AnyAdsEventHandler(PlacementType.rewardedVideo);
        _rotationSequence.Complete();
    }

    private void AnyAdsEventHandler(PlacementType Type)
    {
        if(Type == PlacementType.rewardedVideo)
        {
            if(AdMobController.isRewardedVideoReady)
            {
                loader.SetActive(false);
                if (activeOnLoadedObject != null)
                    activeOnLoadedObject.SetActive(true);
                if (GetComponent<Button>() != null)
                    GetComponent<Button>().interactable = true;
            }
            else
            {
                loader.SetActive(true);
                if (activeOnLoadedObject != null)
                    activeOnLoadedObject.SetActive(false);
                if (GetComponent<Button>() != null)
                    GetComponent<Button>().interactable = false;
            }
        }
    }
}
