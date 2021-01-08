using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarUpgradeInterface : MonoBehaviour
{
    private Engine.GameData.CarUpgradeData data
    {
        get
        {
            return Engine.meta.garage.GetCarUpgrade(type);
        }
    }
    private TextMeshProUGUI stateText
    {
        get
        {
            return transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>();
        }
    }

    public CarUpgradeType type;
    void Start()
    {
        Draw();
    }

    private void Draw()
    {
        stateText.text = "Level " + data.level;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
