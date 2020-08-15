using UnityEngine;
//using UnityEngine.UI;
using TMPro;

//[RequireComponent(typeof(Text))]
[RequireComponent(typeof(TextMeshProUGUI))]
public class ShowReward : MonoBehaviour
{
    public enum DisplayableRewardType {levelFinishReward, TVWatchReward}
    public string prefix;
    public DisplayableRewardType rewardType;

    void Update()
    {
        switch (rewardType)
        {
            case DisplayableRewardType.levelFinishReward:
                GetComponent<TextMeshProUGUI>().text = Localization.GetLocal(prefix) + " " + Engine.rewardAmount;
                break;
            case DisplayableRewardType.TVWatchReward:
                GetComponent<TextMeshProUGUI>().text = Localization.GetLocal(prefix) + " " + Settings.TVWatchReward;
                break;
            default:
                Debug.LogError("Unknown type of reward to display " + rewardType.ToString());
                GetComponent<TextMeshProUGUI>().text = prefix + " undefined"; 
                break;
        }
    }
}