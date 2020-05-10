using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ShowReward : MonoBehaviour
{
    public string prefix;

    void Update()
    {
        GetComponent<Text>().text = prefix + " " + Engine.rewardAmount;
    }
}
