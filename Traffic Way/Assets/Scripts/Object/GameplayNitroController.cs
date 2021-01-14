using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GameplayNitroController : MonoBehaviour
{
    private Engine.GameData.CarData data
    {
        get
        {
            return Engine.meta.car;
        }
    }
    private UserInteraction UI
    {
        get
        {
            return transform.parent.parent.parent.GetComponent<UserInteraction>();
        }
    }
    private Image backgroundImage
    {
        get
        {
            return GetComponent<Image>();
        }
    }
    private Transform nitroIcon
    {
        get
        {
            return transform.GetChild(0);
        }
    }
    private Sequence nitroShake;
    void Start()
    {
        if (CarDriver.CurrentCar.GetComponent<CarDriver>().mode == GameplayMode.Puzzle)
        {
            gameObject.SetActive(false);
            return;
        }
        nitroShake = DOTween.Sequence()
            .AppendInterval(2f)
            .Append(nitroIcon.DOShakeRotation(1.2f, Vector3.forward * 90, 10, 30, true))
            //.Append(nitroIcon.DOShakeRotation(0.2f, Vector3.forward * -15, 10, 3, true))
            //.Append(nitroIcon.DOShakeRotation(0.2f, Vector3.forward * 15, 10, 3, true))
            .AppendInterval(3f)
            .SetLoops(-1);
        nitroShake.Pause();
        switch (data.nitroState)
        {
            case NitroState.Blocked:
            case NitroState.NotPurchased:
                gameObject.SetActive(false);
                break;
            case NitroState.Empty:
                backgroundImage.color = Color.yellow;
                nitroShake.Play();
                break;
            case NitroState.Available:
                backgroundImage.color = Color.green;
                nitroShake.Play();
                break;
        }
    }
    private void OnDestroy()
    {
        nitroShake.Complete();
    }

    public void Tap()
    {
        switch (data.nitroState)
        {
            case NitroState.Blocked:
            case NitroState.NotPurchased:
                break;
            case NitroState.Empty:
                UI.OpenGameplayNitroPanel();
                break;
            case NitroState.Available:
                data.BoostOn();
                Debug.Log("Boosted");
                Disable();
                break;
        }
    }
    private void Disable()
    {
        GetComponent<CanvasGroup>().alpha = 0.3f;
        GetComponent<Button>().interactable = false;
        nitroShake.Pause();
    }
    private void SuccessfullAdDemo(PlacementType Type)
    {
        if (Type == PlacementType.rewardedVideo)
        {
            Engine.meta.car.AddBaloon();
            Engine.meta.car.BoostOn(); 
            Disable();
            SwitchToNormalMode();
        }
    }
    private void UnsuccessfullAdDemo(PlacementType Type)
    {
        if (Type == PlacementType.rewardedVideo)
        {
            SwitchToNormalMode();
            UI.CloseAllGameplayNitroPanels();
        }
    }
    public void WatchForBaloon()
    {
        if (AdMobController.isRewardedVideoReady)
        {
            SwitchToRewardAwaitMode();
            UI.OpenGameplayNitroAlreadyPurchasedPanel();
            AdMobController.ShowRewardedAd();
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
    }
}
