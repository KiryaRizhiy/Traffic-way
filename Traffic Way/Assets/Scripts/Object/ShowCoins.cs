using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ShowCoins : MonoBehaviour
{
    void Awake()
    {
        Engine.Events.initialized += Show;
        Show();
    }
    public void Show()
    {
        if (Engine.meta != null)
            GetComponent<Text>().text = Engine.meta.coinsCount.ToString();
        else
            GetComponent<Text>().text = "0";
    }
    void OnDestroy()
    {
        Engine.Events.initialized -= Show;
    }
}
