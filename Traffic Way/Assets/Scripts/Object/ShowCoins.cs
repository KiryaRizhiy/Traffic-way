using System.Collections;
using System.Collections.Generic;
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
        GetComponent<Text>().text = Engine.meta.coinsCount.ToString();
    }
    void OnDestroy()
    {
        Engine.Events.initialized -= Show;
    }
}
