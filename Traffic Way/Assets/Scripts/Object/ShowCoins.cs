using UnityEngine;
//using UnityEngine.UI;
using TMPro;

//[RequireComponent(typeof(Text))]
[RequireComponent(typeof(TextMeshProUGUI))]
public class ShowCoins : MonoBehaviour
{
    void Awake()
    {
        Engine.Events.initialized += Show;
        Engine.Events.onGarageStateChanged += GaragePurchaseHandler;
        Engine.Events.onCarUpgradeStateChanged += CarUpgradeStateChanged;
        Engine.Events.adFailed += HandleAdEvent;
        Engine.Events.adFinished += HandleAdEvent;
        Engine.Events.adLoaded += HandleAdEvent;
        Engine.Events.adNotReady += HandleAdEvent;
        Engine.Events.adSkipped += HandleAdEvent;
        Show();
    }
    public void Show()
    {
        if (Engine.initialized)
            GetComponent<TextMeshProUGUI>().text = Engine.meta.coinsCount.ToString();
        else
            GetComponent<TextMeshProUGUI>().text = "0";
    }
    public void GaragePurchaseHandler(GarageCoinMakerType type)
    {
        Show();
    }
    public void CarUpgradeStateChanged(CarUpgradeType Type)
    {
        Show();
    }
    private void HandleAdEvent(PlacementType type)
    {
        Show();
    }
    void OnDestroy()
    {
        Engine.Events.initialized -= Show;
        Engine.Events.onGarageStateChanged -= GaragePurchaseHandler;
        Engine.Events.onCarUpgradeStateChanged -= CarUpgradeStateChanged;
        Engine.Events.adFailed -= HandleAdEvent;
        Engine.Events.adFinished -= HandleAdEvent;
        Engine.Events.adLoaded -= HandleAdEvent;
        Engine.Events.adNotReady -= HandleAdEvent;
        Engine.Events.adSkipped -= HandleAdEvent;
    }
}