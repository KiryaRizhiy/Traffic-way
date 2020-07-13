using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Translator : MonoBehaviour
{
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
        TranslateObject(gameObject);
    }
    void TranslateObject(GameObject obj)
    {
        if (obj.GetComponent<Text>() != null)
        {
            obj.GetComponent<Text>().text = Localization.GetLocal(obj.GetComponent<Text>().text);
        }
        if (obj.transform.childCount > 0)
        {
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                TranslateObject(obj.transform.GetChild(i).gameObject);
            }
        }
    }
}