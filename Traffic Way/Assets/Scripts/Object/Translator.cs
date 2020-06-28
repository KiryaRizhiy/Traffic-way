using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Translator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (Engine.initialized)
            Translate();
        else
            Engine.Events.initialized += Translate;
    }

    void OnDestroy()
    {
        Engine.Events.initialized -= Translate;
    }

    void Translate()
    {
        if (GetComponent<Text>() != null)
        {
            GetComponent<Text>().text = Localization.GetLocal(GetComponent<Text>().text);
        }
    }
}
