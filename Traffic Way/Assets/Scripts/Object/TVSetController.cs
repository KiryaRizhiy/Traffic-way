using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class TVSetController : MonoBehaviour
{
    private UserInteraction UI
    {
        get { return transform.parent.parent.parent.GetComponent<UserInteraction>(); }
    }
    void Update()
    {
        if (Engine.meta.garage.TVAbleToShow)
            GetComponent<Animator>().SetBool("isActive", true);
        else
            GetComponent<Animator>().SetBool("isActive", false);
    }
    public void Tap()
    {
        if (Engine.meta.garage.TVAbleToShow)
            UI.ShowTVAds();
    }
}