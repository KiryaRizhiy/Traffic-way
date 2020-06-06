using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CoinMaker : MonoBehaviour
{
    public GarageCoinMakerType type;
    private UserInteraction UI
    {
        get { return transform.parent.parent.parent.GetComponent<UserInteraction>(); }
    }
    private DateTime lastInfoUpdate;

    void Awake()
    {
        lastInfoUpdate = DateTime.UtcNow;
        Engine.Events.initialized += Draw;
        Engine.Events.onGarageStateChanged += GaragePurchaseHandler;
    }
    void OnDestroy()
    {
        Engine.Events.initialized -= Draw;
        Engine.Events.onGarageStateChanged -= GaragePurchaseHandler;
    }
    public void Draw()
    {
        try
        {
            GetComponent<Image>().sprite = Sprite.Create(Engine.meta.garage.GetCoinMaker(type).currentTexture
                , new Rect(0.0f, 0.0f, Engine.meta.garage.GetCoinMaker(type).currentTexture.width, Engine.meta.garage.GetCoinMaker(type).currentTexture.height)
                , new Vector2(0.5f, 0.5f)
                , 100.0f);
            //Engine.meta.Garage.GetCoinMaker(type).currentTexture;
            if (Engine.meta.garage.GetCoinMaker(type).currentProfit == 0)
            {
                transform.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                transform.GetChild(1).gameObject.SetActive(true);
                transform.GetChild(1).GetChild(0).GetComponent<Text>().text = Engine.meta.garage.GetCoinMaker(type).currentProfit.ToString();
            }
        }
        catch (System.Exception e)
        {
            Logger.AddContent(UILogDataType.Init,"Coin maker " + type.ToString() + " exception " + e.Message + Environment.NewLine + "trace: " + e.StackTrace);
        }
    }
    public void GaragePurchaseHandler(GarageCoinMakerType Type)
    {
        if (Type == type)
            Draw();
    }
    public void Tap()
    {
        if (Engine.meta.garage.GetCoinMaker(type).currentProfit == 0)
            UI.CoinMakerUpgradeRequest(type);
        else
            Engine.meta.garage.CollectProfit(type);
    }

    void Update()
    {
        if (Engine.meta.garage.GetCoinMaker(type).level == 0)
            return;
        if (Engine.meta.garage.GetCoinMaker(type).currentProfit / Engine.meta.garage.GetCoinMaker(type).profitRate == Settings.paidTicksLimit)
            return;
        if (DateTime.UtcNow <= new DateTime(Engine.meta.garage.GetCoinMaker(type).lastCoinCollect)
        + new TimeSpan(0, ((Engine.meta.garage.GetCoinMaker(type).currentProfit / Engine.meta.garage.GetCoinMaker(type).profitRate) + 1) * Settings.coinMakerTickMinutes, 0)) ;
        Draw();
    }
}