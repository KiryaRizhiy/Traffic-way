using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ShowCoins : MonoBehaviour
{
    void Awake()
    {
        Engine.Events.initialized += Show;
        Engine.Events.onGarageStateChanged += GaragePurchaseHandler;
        Show();
    }
    public void Show()
    {
        if (Engine.meta != null)
            GetComponent<Text>().text = Engine.meta.coinsCount.ToString();
        else
            GetComponent<Text>().text = "0";
    }
    public void GaragePurchaseHandler(GarageCoinMakerType type)
    {
        Show();
    }
    void OnDestroy()
    {
        Engine.Events.initialized -= Show;
        Engine.Events.onGarageStateChanged -= GaragePurchaseHandler;
    }
}
